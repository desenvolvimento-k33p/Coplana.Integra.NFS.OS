using AutoMapper;
using Coplana.Integracao.NfsOs.Core.Interfaces;
using Coplana.Integracao.NfsOs.Domain.Configuration;
using Coplana.Integracao.NfsOs.Domain.Logger;
using Coplana.Integracao.NfsOs.Infra.Interfaces;
using Coplana.Integracao.NfsOs.Services.Interfaces;
using Coplana.Integracao.NfsOs.Services.Models;
using Integracao.Magis5.Services.SQLs;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Services
{

    public class InsertNFEntradaService : IInsertNFEntradaService
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
        private string nomeServico = "InsertNFEntradaService";

        public InsertNFEntradaService(IHanaAdapter hanaAdapter,
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

            HANA_DB = _configuration.Value.HanaDbConnection.Database;

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

        private async Task<List<InvoiceDTO2>> _getItensPendent()
        {

            List<InvoiceDTO2> itensResult = new List<InvoiceDTO2>();

            try
            {

                var query = SQLSupport.GetConsultas("GeiItensToInsertNFE");
                //query = String.Format(query, "", "");
                var allItens = await _hanaAdapter.Query<InvoiceDTO2>(query);
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

        private async Task _processItems(List<InvoiceDTO2> itens)
        {

            int cont = 0;
            List<InvoiceDTO2> newLista;

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

        private async Task<dynamic> _processUnitItem(List<InvoiceDTO2> item,string tipo)
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

        private async Task<bool> _createItemNFS(dynamic item,string tipo)
        {
            var nfsSAP = await _populateSOACollection(item,tipo);

            InvoiceDTOReturn responseOrder = await _serviceLayerAdapter.Call<InvoiceDTOReturn>(
                    $"PurchaseInvoices", HttpMethod.Post, nfsSAP, _serviceLayerHttp.Uri);

            await _logger.Logger(new LogIntegration
            {
                LogTypeCode = 1,
                Message = $"Resposta da Service Layer",
                Owner = nomeServico,
                Method = "_createItemNFS",
                Key = (object)responseOrder != null ? responseOrder.DocEntry.ToString() : "",//chave NFS
                Key2 ="",
                RequestObject = JsonSerializer.Serialize(nfsSAP),
                ResponseObject = JsonSerializer.Serialize(responseOrder)
            });



            if (responseOrder != null)
            {
                if (!String.IsNullOrEmpty(responseOrder.DocEntry.ToString()) && responseOrder.DocEntry.ToString() != "0")
                {
                    ////atualiza flag
                    //var query = SQLSupport.GetConsultas("AtualizaFlagNFE");
                    //query = string.Format(query, nfsSAP.DocNumTransf);
                    //var result = await _hanaAdapter.Execute(query);
                    //atualiza flag
                    if (tipo == "SemPedido")
                    {
                        var query = SQLSupport.GetConsultas("AtualizaFlagNFE");
                        query = string.Format(query, nfsSAP.DocNumTransf);
                        var result = await _hanaAdapter.Execute(query);
                    }
                    else
                    {
                        var query = SQLSupport.GetConsultas("AtualizaFlagNFEPed");
                        query = string.Format(query, nfsSAP.DocNumPedTransf);
                        var result = await _hanaAdapter.Execute(query);
                    }
                }

            }

            return false;
        }

        private async Task<InvoiceDTO2> _populateSOACollection(InvoiceDTO2 item,string tipo)
        {


            try
            {
                List<BatchNumbers2> lotes = new List<BatchNumbers2>();
                List<DocumentLine2> linhas = new List<DocumentLine2>();
                InvoiceDTO2 obj = item;
                string query = "";

                if (tipo == "SemPedido")
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1");
                    query = String.Format(query, obj.DocNumTransf);
                }
                else
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1_Ped");
                    query = String.Format(query, obj.DocNumPedTransf);
                }

                var itensResult = await _hanaAdapter.QueryList<DocumentLine>(query);
                //var itensResult = retLinhas.ToList();
                //itensResult.GroupBy(c => c.ItemCode);NÃO AGRUPA E SOMA POIS PODEM HAVER LINHAS DAS TRANSF COM DEPOSITOS DIFERENTES,

                foreach (var lines in itensResult)
                {
                    lotes = new List<BatchNumbers2>();
                    DocumentLine2 l = new DocumentLine2();
                    l.Price = lines.Price;

                    //l.TaxCode = _configuration.Value.CoplanaBusiness.TaxCodeNFS;
                   // l.CFOPCode = _configuration.Value.CoplanaBusiness.CFOPNFS;
                    l.Quantity = lines.Quantity;
                    l.Usage = lines.Usage;
                    l.ItemCode = lines.ItemCode;
                    l.WarehouseCode = lines.WarehouseCode;


                    //lotes//////////////////////////
                    BatchNumbers b = new BatchNumbers();


                    query = SQLSupport.GetConsultas("GetLotes");
                    query = String.Format(query, obj.DocEntryTransf, lines.ItemCode);//itensLote
                    BatchNumbers2 retlotes = await _hanaAdapter.QueryFirst<BatchNumbers2>(query);
                    retlotes.Quantity = lines.Quantity;
                    retlotes.ItemCode = lines.ItemCode;
                    lotes.Add(retlotes);

                    l.BatchNumbers = lotes;

                    linhas.Add(l);

                    //itensLote+= "'" + l.ItemCode +"'" + ","; 
                }

                //foreach (var lines in itensResult)
                //{
                //    BatchNumbers b = new BatchNumbers();
                //    //itensLote = itensLote.Remove(itensLote.LastIndexOf(","));

                //    query = SQLSupport.GetConsultas("GetLotes");
                //    query = String.Format(query, lines.ItemCode);//itensLote
                //    BatchNumbers retlotes = await _hanaAdapter.QueryFirst<BatchNumbers>(query);
                //    retlotes.Quantity = lines.Quantity;
                //    retlotes.ItemCode = lines.ItemCode;
                //    lotes.Add(retlotes);
                //    //lotes = retlotes.ToList();
                //}

                // obj.DocumentLines.BatchNumbers = lotes;
                obj.DocumentLines = linhas;
                //obj.Series = _configuration.Value.CoplanaBusiness.SeriesNFS;//ássa config
                obj.SequenceCode = item.SequenceCode;

                TaxExtension2 tax = new TaxExtension2();
                tax.Incoterms = "9";
                obj.TaxExtension = tax;

                if (tipo == "SemPedido")
                    obj.U_NumTransf = item.DocNumTransf;
                if (tipo == "ComPedido")
                    obj.U_NumPedTr = item.DocNumPedTransf;

                obj.U_StSentWMS = "0";


                obj.U_ChaveAcesso =  item.U_ChaveAcesso;
                obj.SequenceSerial = item.SequenceSerial;
                obj.SequenceModel = "39";

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
