using AutoMapper;
using Coplana.Integracao.NfsOs.Core;
using Coplana.Integracao.NfsOs.Core.Interfaces;
using Coplana.Integracao.NfsOs.Domain.Configuration;
using Coplana.Integracao.NfsOs.Domain.Logger;
using Coplana.Integracao.NfsOs.Infra.Interfaces;
using Coplana.Integracao.NfsOs.Services.Interfaces;
using Coplana.Integracao.NfsOs.Services.Models;
using Integracao.Magis5.Services.SQLs;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Services
{

    public class InsertNFSaidaService : IInsertNFSaidaService
    {
        private readonly IHanaAdapter _hanaAdapter;
        private readonly IHttpAdapter _httpAdapter;
        private readonly ILoggerRepository _logger;
        private readonly IServiceLayerAdapter _serviceLayerAdapter;
        private readonly IMapper _mapper;
        private readonly CoplanaHttp _coplanaHttp;
        private readonly CoplanaBusiness _coplanaBusiness;
        private readonly ServiceLayer _serviceLayerHttp;
        private readonly IHanaAdapter _hana;
        private readonly string HANA_DB;
        private readonly IOptions<Configuration> _configuration;
        private string nomeServico = "InsertNFSaidaService";

        public InsertNFSaidaService(IHanaAdapter hanaAdapter,
                            IOptions<Configuration> configurations,
                            IHttpAdapter httpAdapter,
                            IServiceLayerAdapter serviceLayerAdapter,
                            IMapper mapper,
                            ILoggerRepository logger,
                            IHanaAdapter hana,
                            IOptions<Configuration> configuration)
        {
            _hanaAdapter = hanaAdapter;
            _httpAdapter = httpAdapter;
            _logger = logger;
            _mapper = mapper;
            _serviceLayerAdapter = serviceLayerAdapter;
            _configuration = configuration;
            _hana = hana;

            HANA_DB = Criptografia.Instancia.Descriptografar(_configuration.Value.HanaDbConnection.Database);

            _coplanaHttp = configurations.Value.CoplanaHttp;
            _coplanaBusiness = configurations.Value.CoplanaBusiness;
            _serviceLayerHttp = configurations.Value.ServiceLayer;

        }
        public async Task<bool> ProcessAsync()
        {
            try
            {
                var itens = await _getItensPendent();

                await _processItems(itens);

                return true;
            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Houve um erro na função _processItems - Erro {ex.Message}",
                    Owner = nomeServico,
                    Method = "_processItems"
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<InvoiceDTO>> _getItensPendent()
        {

            List<InvoiceDTO> itensResult = new List<InvoiceDTO>();

            try
            {

                var query = SQLSupport.GetConsultas("GeiItensToInsertNFS");
                //query = String.Format(query, "", "");
                var allItens = await _hanaAdapter.Query<InvoiceDTO>(query);
                itensResult = allItens.ToList();

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Houve um erro na função _processItems - Erro {ex.Message}",
                    Owner = nomeServico,
                    Method = "_processItems"
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }

            return itensResult;
        }

        private async Task _processItems(List<InvoiceDTO> itens)
        {

            int cont = 0;
            List<InvoiceDTO> newLista;

            try
            {

                //agrupamento COM PEDIDO
                var groupedList = itens.Where(c => c.DocNumPedTransf != "")
               .GroupBy(x => x.DocNumPedTransf)//, x.DocNumTransf))
               .Select(grp => grp.ToList())
               .ToList();



                //if (groupedList.Count() == 0)
                //    return false;

                foreach (var item in groupedList)
                {

                    await _processUnitItem(item, "ComPedido");

                }

                //agrupamento SEM PEDIDO
                var groupedList2 = itens.Where(c => c.DocNumPedTransf == "")
               .GroupBy(x => x.DocNumTransf)//, x.DocNumTransf))
               .Select(grp => grp.ToList())
               .ToList();

                //if (groupedList2.Count() == 0)
                //    return false;

                foreach (var item in groupedList2)
                {

                    await _processUnitItem(item, "SemPedido");

                }

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Houve um erro na função _processItems - Erro {ex.Message}",
                    Owner = nomeServico,
                    Method = "_processItems"
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<dynamic> _processUnitItem(List<InvoiceDTO> item, string tipo)
        {
            try
            {
                string numTransf = "";

                foreach (var lista in item)
                {
                    if (numTransf != lista.DocNumPedTransf || String.IsNullOrEmpty(lista.DocNumPedTransf))
                    {
                        var response = await _createItemNFS(lista, tipo);

                    }
                    numTransf = lista.DocNumPedTransf;
                }

                return null;
            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Houve um erro na função _processUnitItem - Erro {ex.Message}",
                    Owner = nomeServico,
                    Method = "_processUnitItem"
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<bool> _createItemNFS(dynamic item, string tipo)
        {
            var nfsSAP = await _populateSOACollection(item, tipo);

            InvoiceDTOReturn responseOrder = await _serviceLayerAdapter.Call<InvoiceDTOReturn>(
                    $"Invoices", HttpMethod.Post, nfsSAP, Criptografia.Instancia.Descriptografar(_serviceLayerHttp.Uri));

            await _logger.Logger(new LogIntegration
            {
                LogTypeCode = 1,
                Message = $"Resposta da Service Layer",
                Owner = nomeServico,
                Method = "_createItemNFS",
                Key = (object)responseOrder != null ? responseOrder.DocEntry.ToString() : "",
                Key2 = "",
                RequestObject = JsonSerializer.Serialize(nfsSAP),
                ResponseObject = JsonSerializer.Serialize(responseOrder)
            });



            if (responseOrder != null)
            {
                if (!String.IsNullOrEmpty(responseOrder.DocEntry.ToString()) && responseOrder.DocEntry.ToString() != "0")
                {
                    //atualiza flag
                    if (tipo == "SemPedido")
                    {
                        var query = SQLSupport.GetConsultas("AtualizaFlagNFS");
                        query = string.Format(query, nfsSAP.DocNumTransf);
                        var result = await _hanaAdapter.Execute(query);
                    }
                    else
                    {
                        var query = SQLSupport.GetConsultas("AtualizaFlagNFSPed");
                        query = string.Format(query, nfsSAP.DocNumPedTransf);
                        var result = await _hanaAdapter.Execute(query);
                    }
                }

            }

            return false;
        }

        private async Task<InvoiceDTO> _populateSOACollection(InvoiceDTO item, string tipo)
        {

            try
            {
                List<BatchNumbers> lotes = new List<BatchNumbers>();
                List<DocumentLine> linhas = new List<DocumentLine>();
                InvoiceDTO obj = item;
                string query = "";
                string consultaLote = "";

                if (tipo == "SemPedido")
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFS1");
                    query = String.Format(query, obj.DocNumTransf);
                    consultaLote = "GetLotes";
                }
                else
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFS1_Ped");
                    query = String.Format(query, obj.DocNumPedTransf);
                    consultaLote = "GetLotesPed";
                }

                var itensResult = await _hanaAdapter.QueryList<DocumentLine>(query);               
            

                foreach (var lines in itensResult)
                {
                    lotes = new List<BatchNumbers>();
                    DocumentLine l = new DocumentLine();
                    

                    l.Price = lines.Price;              
                    l.Quantity = lines.Quantity;
                    l.Usage = lines.Usage;
                    l.ItemCode = lines.ItemCode;
                    l.WarehouseCode = lines.WarehouseCode;
                    l.BaseType = -1;
                    l.BaseLine = null;
                    l.BaseEntry = null;

                    ///////////////////////////lotes//////////////////////////
                    BatchNumbers b = new BatchNumbers();
                    query = SQLSupport.GetConsultas(consultaLote);
                    query = String.Format(query, (tipo == "SemPedido" ? obj.DocEntryTransf : obj.DocNumPedTransf), lines.ItemCode);

                    List<BatchNumbers> retlotes = await _hanaAdapter.QueryList<BatchNumbers>(query);

                    foreach (var lote in retlotes)
                    {
                        BatchNumbers batch = new BatchNumbers();
                        batch.SystemSerialNumber = lote.SystemSerialNumber;
                        batch.BatchNumber = lote.BatchNumber;
                        batch.Quantity = lote.Quantity;
                        batch.ItemCode = lines.ItemCode;
                        lotes.Add(batch);
                    }

                    l.BatchNumbers = lotes;

                    linhas.Add(l);

                   
                }

             
                // obj.DocumentLines.BatchNumbers = lotes;
                obj.DocumentLines = linhas;
                //obj.Series = _configuration.Value.CoplanaBusiness.SeriesNFS;//ássa config
                obj.SequenceCode = item.SequenceCode;

                TaxExtension tax = new TaxExtension();
                tax.Incoterms = "9";
                obj.TaxExtension = tax;

                obj.U_StSentWMS = "0";

                if (tipo == "SemPedido")
                    obj.U_NumTransf = item.DocNumTransf;
                if (tipo == "ComPedido")
                    obj.U_NumPedTr = item.DocNumPedTransf;

                return obj;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Houve um erro na função _populateSOACollection - Erro {ex.Message}",
                    Owner = nomeServico,
                    Method = "_populateSOACollection"
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }





    }
}
