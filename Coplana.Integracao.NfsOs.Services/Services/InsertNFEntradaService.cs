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
                //DESCOMENTAR DEPOIS DIA 12
                //agrupamento COM PEDIDO
                var groupedList = itens.Where(c => c.DocNumPedTransf != "").Where(c => c.Tipo == "Com Pedido")
               .GroupBy(x => x.DocNumPedTransf)//, x.DocNumTransf))
               .Select(grp => grp.ToList())
               .ToList();

                foreach (var item in groupedList)
                {

                    await _processUnitItem(item, "ComPedido");

                }

                // //agrupamento SEM PEDIDO
                var groupedList2 = itens.Where(c => c.DocNumPedTransf == "").Where(c => c.Tipo == "Sem Pedido")
               .GroupBy(x => x.DocNumTransf)//, x.DocNumTransf))
               .Select(grp => grp.ToList())
               .ToList();

                foreach (var item in groupedList2)
                {

                    await _processUnitItem(item, "SemPedido");

                }
                //DESCOMENTAR DEPOIS DIA 12


                //transf NFS
                var groupedList3 = itens.Where(c => c.Tipo == "Transf.")
               .GroupBy(x => x.SequenceSerial)//, x.DocNumTransf))
               .Select(grp => grp.ToList())
               .ToList();



                foreach (var item in groupedList3)
                {

                    await _processUnitItem(item, "Transf.");

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

        private async Task<dynamic> _processUnitItem(List<InvoiceDTO2> item, string tipo)
        {
            try
            {
                string numTransf = "";

                //DESCOMENTAR DEPOIS DIA 12
                if (tipo != "Transf.")
                    foreach (var lista in item)
                    {
                        if (numTransf != lista.DocNumPedTransf || String.IsNullOrEmpty(lista.DocNumPedTransf))
                        {
                            var response = await _createItemNFS(lista, tipo, lista.GerarEsboco);

                        }
                        numTransf = lista.DocNumPedTransf;
                    }
                //DESCOMENTAR DEPOIS DIA 12

                if (tipo == "Transf.")
                    foreach (var lista in item)
                    {
                        if (numTransf != lista.SequenceSerial.ToString())//|| String.IsNullOrEmpty(lista.SequenceSerial.ToString()))
                        {
                            var response = await _createItemNFS(lista, tipo, lista.GerarEsboco);

                        }
                        numTransf = lista.SequenceSerial.ToString();
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

        private async Task<bool> _createItemNFS(dynamic item, string tipo, string geraEsboco)
        {
            dynamic nfsSAP = null;
            dynamic nfsSAPDraft = null;

            if (geraEsboco == "N")
                 nfsSAP = await _populateSOACollection(item, tipo);
            else
                 nfsSAPDraft = await _populateSOACollectionDraft(item, tipo);

            string endpoint = geraEsboco == "N" ? "PurchaseInvoices" : "Drafts";

            InvoiceDTOReturn responseOrder = await _serviceLayerAdapter.Call<InvoiceDTOReturn>(
                    $"{endpoint}", HttpMethod.Post, geraEsboco == "N" ? nfsSAP : nfsSAPDraft, Criptografia.Instancia.Descriptografar(_serviceLayerHttp.Uri));

            await _logger.Logger(new LogIntegration
            {
                LogTypeCode = 1,
                Message = $"Resposta da Service Layer",
                Owner = nomeServico,
                Method = "_createItemNFS",
                Key = (object)responseOrder != null ? responseOrder.DocEntry.ToString() : "",//chave NFS
                Key2 = "",
                RequestObject = JsonSerializer.Serialize(geraEsboco == "N" ? nfsSAP : nfsSAPDraft),
                ResponseObject = JsonSerializer.Serialize(responseOrder)
            });



            if (responseOrder != null)
            {
                if (!String.IsNullOrEmpty(responseOrder.DocEntry.ToString()) && responseOrder.DocEntry.ToString() != "0")
                {
                    
                    //atualiza flag
                    if (tipo == "SemPedido")
                    {
                        var query = SQLSupport.GetConsultas("AtualizaFlagNFE");
                        query = string.Format(query, geraEsboco == "N" ? nfsSAP.DocNumTransf : nfsSAPDraft.DocNumTransf);
                        var result = await _hanaAdapter.Execute(query);
                    }
                    else if (tipo == "ComPedido")
                    {
                        var query = SQLSupport.GetConsultas("AtualizaFlagNFEPed");
                        query = string.Format(query, geraEsboco == "N" ? nfsSAP.DocNumPedTransf : nfsSAPDraft.DocNumPedTransf);
                        var result = await _hanaAdapter.Execute(query);
                    }
                   
                }

            }

            return false;
        }

        private async Task<InvoiceDTO2> _populateSOACollection(InvoiceDTO2 item, string tipo)
        {


            try
            {
                List<DocumentLinesBinAllocations2> location = new List<DocumentLinesBinAllocations2>();
                List<BatchNumbers2> lotes = new List<BatchNumbers2>();
                List<DocumentLine2> linhas = new List<DocumentLine2>();
                InvoiceDTO2 obj = item;
                string query = "";
                string consultaLote = "";

                //DESCOMENTAR DEPOIS DIA 12
                if (tipo == "SemPedido")
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1");
                    query = String.Format(query, obj.DocNumTransf);
                    consultaLote = "GetLotes";
                }
                else if (tipo == "ComPedido")
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1_Ped");
                    query = String.Format(query, obj.DocNumPedTransf);
                    consultaLote = "GetLotesPed";
                }
                //DESCOMENTAR DEPOIS DIA 12

                if (tipo == "Transf.")//elseif
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1_Transf");
                    query = String.Format(query, obj.DocNumTransf);
                    consultaLote = "GetLotesTransf";
                }

                var itensResult = await _hanaAdapter.QueryList<DocumentLine>(query);

                int x = 0;
                foreach (var lines in itensResult)
                {
                    location = new List<DocumentLinesBinAllocations2>(); 
                    lotes = new List<BatchNumbers2>();
                    DocumentLine2 l = new DocumentLine2();
                    l.Price = lines.Price;
                    l.UnitPrice = lines.Price;

                    l.LineNum = lines.LineNum;
                    l.Quantity = lines.Quantity;
                    l.Usage = lines.Usage;
                    l.ItemCode = lines.ItemCode;
                    l.WarehouseCode = lines.Destino;
                    l.BaseEntry = lines.BaseEntry == -1 ? null : lines.BaseEntry;
                    l.BaseLine = lines.BaseLine == -1 ? null : lines.BaseLine;
                    l.BaseType = lines.BaseEntry == -1 ? -1 : lines.BaseType ;
                    l.U_K_CustoDespesaAtivo = lines.U_K_CustoDespesaAtivo;
                    l.CostingCode = lines.CostingCode;

                    ///////////////////////////lotes//////////////////////////
                    string varLotes = "";
                    switch (tipo)
                    {
                        case "SemPedido":
                            varLotes = obj.DocEntryTransf;
                            break;
                        case "ComPedido":
                            varLotes = obj.DocNumPedTransf;
                            break;
                        case "Transf.":
                            varLotes = obj.DocNumPedTransf.ToString();
                            break;
                    }

                    //lotes
                    BatchNumbers b = new BatchNumbers();
                    query = SQLSupport.GetConsultas(consultaLote);
                    query = String.Format(query, varLotes, lines.LineNum);// lines.ItemCode);

                    List<BatchNumbers2> retlotes = await _hanaAdapter.QueryList<BatchNumbers2>(query);

                    //agrupa os lotes por BatchNumber**
                    var groupedList = retlotes.GroupBy(x => x.BatchNumber).Select(grp => new BatchNumbers2
                    {
                        ItemCode = grp.First().ItemCode,
                        BatchNumber = grp.First().BatchNumber,
                        SystemSerialNumber = grp.First().SystemSerialNumber,
                        Quantity = grp.Sum(c => c.Quantity)
                    }).ToList();

                    foreach (var lote in groupedList)//***
                    {
                        BatchNumbers2 batch = new BatchNumbers2();
                        batch.SystemSerialNumber = lote.SystemSerialNumber;
                        batch.BatchNumber = lote.BatchNumber;
                        batch.Quantity = lote.Quantity;
                        batch.ItemCode = lines.ItemCode;
                        batch.BaseLineNumber = lines.LineNum;
                        lotes.Add(batch);
                    }

                    l.BatchNumbers = lotes;



                    //localizacao
                    int y = 0;//**
                    foreach (var lote in groupedList)//**
                    {
                        if (!String.IsNullOrEmpty(lines.BinAbsEntry.ToString()) && lines.BinAbsEntry != 0)
                        {
                            DocumentLinesBinAllocations2 loc = new DocumentLinesBinAllocations2();
                            loc.BinAbsEntry = lines.BinAbsEntry;
                            loc.Quantity = lote.Quantity;
                            loc.BaseLineNumber = lines.LineNum;// x;
                            loc.SerialAndBatchNumbersBaseLine = y;//**
                            location.Add(loc);

                            

                        }
                        y++;//**
                    }//**
                    l.DocumentLinesBinAllocations = location;//**

                    linhas.Add(l);
                    x++;

                }



                // obj.DocumentLines.BatchNumbers = lotes;
                obj.DocumentLines = linhas;
                //obj.Series = _configuration.Value.CoplanaBusiness.SeriesNFS;//ássa config
                obj.SequenceCode = item.SequenceCode;
                obj.SeriesString = item.SeriesString;
                obj.U_ChaveAcesso = item.U_ChaveAcesso;
                obj.SequenceSerial = item.SequenceSerial;
                obj.SequenceModel = "39";

                TaxExtension2 tax = new TaxExtension2();
                tax.Incoterms = "9";
                obj.TaxExtension = tax;

                //DESCOMENTAR DEPOIS DIA 12
                if (tipo == "SemPedido")
                    obj.U_NumTransf = item.DocNumTransf;
                if (tipo == "ComPedido")
                    obj.U_NumPedTr = item.DocNumPedTransf;
                if (tipo == "Transf.")
                    obj.U_NumPedTr = "TR " + item.SequenceSerial.ToString();

                obj.U_StSentWMS = "0";

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



        private async Task<Draft> _populateSOACollectionDraft(InvoiceDTO2 item, string tipo)
        {


            try
            {
                List<DocumentLinesBinAllocationsDraft> location = new List<DocumentLinesBinAllocationsDraft>();
                List<BatchNumbersDraft> lotes = new List<BatchNumbersDraft>();
                List<DocumentLineDraft> linhas = new List<DocumentLineDraft>();
                Draft obj = new Draft();

                obj.CardCode = item.CardCode;
                obj.U_ChaveAcesso = item.U_ChaveAcesso;              
                obj.Comments = item.Comments;
                obj.DocDate=item.DocDate;
                obj.DocDueDate = item.DocDueDate;
                obj.TaxDate = item.TaxDate;
                obj.BPL_IDAssignedToInvoice = item.BPL_IDAssignedToInvoice;
                obj.DocObjectCode="18";
                obj.DocNumPedTransf = item.DocNumPedTransf;
                obj.DocNumTransf = item.DocNumTransf;   

                string query = "";
                string consultaLote = "";

                //DESCOMENTAR DEPOIS DIA 12
                if (tipo == "SemPedido")
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1");
                    query = String.Format(query, obj.DocNumTransf);
                    consultaLote = "GetLotes";
                }
                else if (tipo == "ComPedido")
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1_Ped");
                    query = String.Format(query, obj.DocNumPedTransf);
                    consultaLote = "GetLotesPed";
                }
                if (tipo == "Transf.")//else if
                {
                    query = SQLSupport.GetConsultas("GeiItensToInsertNFE1_Transf");
                    query = String.Format(query, obj.DocNumTransf);
                    consultaLote = "GetLotesTransf";
                }

                var itensResult = await _hanaAdapter.QueryList<DocumentLine>(query);

                int x = 0;
                foreach (var lines in itensResult)
                {
                    lotes = new List<BatchNumbersDraft>();
                    DocumentLineDraft l = new DocumentLineDraft();
                    l.Price = lines.Price;
                    l.UnitPrice = lines.Price;

                    l.Quantity = lines.Quantity;
                    l.Usage = lines.Usage;
                    l.ItemCode = lines.ItemCode;
                    l.WarehouseCode = lines.Destino;
                    l.BaseEntry = lines.BaseEntry == -1 ? null : lines.BaseEntry;
                    l.BaseLine = lines.BaseLine == -1 ? null : lines.BaseLine;
                    l.BaseType = lines.BaseEntry == -1 ? null : lines.BaseType;
                    l.U_K_CustoDespesaAtivo = item.CustoDesp;
                    l.CostingCode = lines.CostingCode;
                    ///////////////////////////lotes//////////////////////////
                    string varLotes = "";
                    switch (tipo)
                    {
                        case "SemPedido":
                            varLotes = obj.DocEntryTransf;
                            break;
                        case "ComPedido":
                            varLotes = obj.DocEntryTransf;
                            break;
                        case "Transf.":
                            varLotes = obj.DocNumPedTransf;
                            break;
                    }

                    //lotes
                    BatchNumbersDraft b = new BatchNumbersDraft();
                    query = SQLSupport.GetConsultas(consultaLote);
                    query = String.Format(query, varLotes, lines.LineNum);

                    List<BatchNumbersDraft> retlotes = await _hanaAdapter.QueryList<BatchNumbersDraft>(query);

                    //agrupa os lotes por BatchNumber**
                    var groupedList = retlotes.GroupBy(x => x.BatchNumber).Select(grp => new BatchNumbers2
                    {
                        ItemCode = grp.First().ItemCode,
                        BatchNumber = grp.First().BatchNumber,
                        SystemSerialNumber = grp.First().SystemSerialNumber,
                        Quantity = grp.Sum(c => c.Quantity)
                    }).ToList();

                    foreach (var lote in groupedList)//***
                    {
                        BatchNumbersDraft batch = new BatchNumbersDraft();
                        batch.SystemSerialNumber = lote.SystemSerialNumber;
                        batch.BatchNumber = lote.BatchNumber;
                        batch.Quantity = lote.Quantity;
                        batch.ItemCode = lines.ItemCode;
                        lotes.Add(batch);
                    }

                    l.BatchNumbers = lotes;

                    //List<BatchNumbersDraft> retlotes = await _hanaAdapter.QueryList<BatchNumbersDraft>(query);

                    //foreach (var lote in retlotes)
                    //{
                    //    BatchNumbersDraft batch = new BatchNumbersDraft();
                    //    batch.SystemSerialNumber = lote.SystemSerialNumber;
                    //    batch.BatchNumber = lote.BatchNumber;
                    //    batch.Quantity = lote.Quantity;
                    //    batch.ItemCode = lines.ItemCode;
                    //    lotes.Add(batch);
                    //}

                    //l.BatchNumbers = lotes;


                    //localizacao
                    if (!String.IsNullOrEmpty(lines.BinAbsEntry.ToString()) && lines.BinAbsEntry != 0)
                    {

                        DocumentLinesBinAllocationsDraft loc = new DocumentLinesBinAllocationsDraft();
                        loc.BinAbsEntry = lines.BinAbsEntry;
                        loc.Quantity = lines.Quantity;
                        loc.BaseLineNumber = x;
                        location.Add(loc);

                        l.DocumentLinesBinAllocations = location;
                    }

                    linhas.Add(l);
                    x++;

                    


                }



                // obj.DocumentLines.BatchNumbers = lotes;
                obj.DocumentLines = linhas;
                //obj.Series = _configuration.Value.CoplanaBusiness.SeriesNFS;//ássa config
                obj.SequenceCode = item.SequenceCode;
                obj.SeriesString = item.SeriesString;
                obj.U_ChaveAcesso = item.U_ChaveAcesso;
                obj.SequenceSerial = item.SequenceSerial;
                obj.SequenceModel = "39";
               

                TaxExtensionDraft tax = new TaxExtensionDraft();
                tax.Incoterms = "9";
                obj.TaxExtension = tax;

                //DESCOMENTAR DEPOIS DIA 12
                if (tipo == "SemPedido")
                    obj.U_NumTransf = item.DocNumTransf;
                if (tipo == "ComPedido")
                    obj.U_NumPedTr = item.DocNumPedTransf;
                if (tipo == "Transf.")
                    obj.U_NumPedTr = "TR " + item.SequenceSerial.ToString();

                obj.U_StSentWMS = "0";

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
