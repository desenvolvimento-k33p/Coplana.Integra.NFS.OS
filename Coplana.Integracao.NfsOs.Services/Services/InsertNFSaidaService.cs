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

                //agrupa por DocNum
                var groupedList = itens
               .GroupBy(u => u.DocNumPedTransf)
               .Select(grp => grp.ToList())
               .ToList();



                //if (groupedList.Count() == 0)
                //    return false;

                foreach (var item in groupedList)
                {


                    await _processUnitItem(item);

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

        private async Task<dynamic> _processUnitItem(List<InvoiceDTO> item)
        {
            try
            {
                foreach (var lista in item)
                {

                    var response = await _createItemNFS(lista);

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

        private async Task<bool> _createItemNFS(dynamic item)
        {
            var nfsSAP = await _populateSOACollection(item);

            var responseOrder = await _serviceLayerAdapter.Call<InvoiceDTOReturn>(
                    $"Invoices", HttpMethod.Post, nfsSAP, _serviceLayerHttp.Uri);

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



            if ((object)responseOrder != null)
            {


            }

            return false;
        }

        private async Task<InvoiceDTO> _populateSOACollection(InvoiceDTO item)
        {

            try
            {
                List<BatchNumbers> lotes = new List<BatchNumbers>();
                List<DocumentLine> linhas = new List<DocumentLine>();
                InvoiceDTO obj = item;
                //string itensLote = "";

                var query = SQLSupport.GetConsultas("GeiItensToInsertNFS1");
                query = String.Format(query, obj.DocNumTransf);
                var retLinhas = await _hanaAdapter.Query<DocumentLine>(query);
                var itensResult = retLinhas.ToList();

                foreach (var lines in itensResult)
                {
                    lotes = new List<BatchNumbers>();
                    DocumentLine l = new DocumentLine();
                    l.Price = lines.Price;

                    l.TaxCode = _configuration.Value.CoplanaBusiness.TaxCodeNFS;
                    l.CFOPCode = _configuration.Value.CoplanaBusiness.CFOPNFS;
                    l.Quantity = lines.Quantity;
                    l.Usage = lines.Usage;
                    l.ItemCode = lines.ItemCode;
                    l.WarehouseCode = lines.WarehouseCode;

                    //lotes//////////////////////////
                    BatchNumbers b = new BatchNumbers();
                  

                    query = SQLSupport.GetConsultas("GetLotes");
                    query = String.Format(query,obj.DocEntryTransf, lines.ItemCode);//itensLote
                    BatchNumbers retlotes = await _hanaAdapter.QueryFirst<BatchNumbers>(query);
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

                TaxExtension tax = new TaxExtension();
                tax.Incoterms ="9";
                obj.TaxExtension = tax;

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
