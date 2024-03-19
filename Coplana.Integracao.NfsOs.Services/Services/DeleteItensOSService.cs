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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Services
{
    public class DeleteItensOSService : IDeleteItensOSService
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
        private string nomeServico = "DeleteItensOSService";

        public DeleteItensOSService(IHanaAdapter hanaAdapter,
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

        private async Task<List<InsereItensOS>> _getItensPendent()
        {

            List<InsereItensOS> itensResult = new List<InsereItensOS>();

            try
            {

                var query = SQLSupport.GetConsultas("GeiItensToDeleteOS");
                //query = String.Format(query, "", "");
                var allItens = await _hanaAdapter.Query<InsereItensOS>(query);
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

        private async Task _processItems(List<InsereItensOS> itens)
        {

            int cont = 0;

            try
            {
                foreach (var item in itens)
                {
                    cont++;
                    //var query = SQLSupport.GetConsultas("GetOrderById");                    
                    //var orderResult = await _hanaAdapter.QueryFirst<int>(query);

                    //if (order.ExternalId == "Lojas Americanas-298345431201")
                    //   continue;

                    //if (orderResult == 0)
                    //    await _processUnitOrder(order);
                    //else
                    //    await SendToReleaseOrders(order);

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

        private async Task<dynamic> _processUnitItem(InsereItensOS item)
        {
            try
            {


                //await _logger.Logger(new LogIntegration
                //{
                //    LogTypeCode = 1,
                //    Message = $"Inicio do envio dos Adiantanentos para o SAP",
                //    Owner = "DownPaymentsService",
                //    Method = "processUnitOrder"
                //});

                var response = await _deleteItemOS(item);

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

        private async Task<bool> _deleteItemOS(InsereItensOS itemLiberali)
        {
            var itemSAP = await _populateSOACollection(itemLiberali);

            if (itemSAP != null)
            {
                var responseOrder = await _serviceLayerAdapter.Call<Full_AGRM_UDO_OSOF>(
                        $"AGRM_UDO_OSOF({itemLiberali.DocEntry})", HttpMethod.Put, itemSAP, _serviceLayerHttp.Uri);

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Resposta da Service Layer",
                    Owner = nomeServico,
                    Method = "_createItemOS",
                    Key = itemLiberali.DocEntry.ToString(),
                    Key2 = (object)responseOrder != null ? responseOrder.AGRM_OSOACollection[0].DocEntry.ToString() : "",
                    RequestObject = JsonSerializer.Serialize(itemSAP),
                    ResponseObject = JsonSerializer.Serialize(responseOrder)
                });

            }

           

            return false;
        }

        private async Task<Full_AGRM_UDO_OSOF> _populateSOACollection(InsereItensOS itemLiberali)
        {

            try
            {
                await _hanaAdapter.Execute($@"update ""@AGRM_OSOA"" set ""U_ValorNota"" = 0 WHERE IFNULL(""U_ValorNota"",0) = 0 and ""DocEntry"" = {itemLiberali.DocEntry}");
                await _hanaAdapter.Execute($@"update ""@AGRM_OSOA"" set ""U_QuantidadeDisponiv"" = 0 WHERE IFNULL(""U_QuantidadeDisponiv"",0) = 0 and ""DocEntry"" = {itemLiberali.DocEntry}");


                var responseOrder = await _serviceLayerAdapter.Call<Full_AGRM_UDO_OSOF>(
                    $"AGRM_UDO_OSOF({itemLiberali.DocEntry})", HttpMethod.Get, null, _serviceLayerHttp.Uri);


                int index = responseOrder.AGRM_OSOACollection.FindLastIndex(g => g.LineId == itemLiberali.LineId);
                responseOrder.AGRM_OSOACollection.RemoveAt(index);
                //AGRM_OSOACollectionDTO ret = new AGRM_OSOACollectionDTO();
                //List<AGRMOSOACollection> lista = new List<AGRMOSOACollection>();

                //var obj = new AGRMOSOACollection();

                //string data1 = Convert.ToDateTime(itemLiberali.U_DtAplicaca).ToString("yyyy-MM-dd");

                //obj.DocEntry = itemLiberali.DocEntry;
                //obj.LineId = itemLiberali.LineId;
                //obj.VisOrder = itemLiberali.VisOrder;
                //obj.U_Tipo = itemLiberali.U_Tipo;
                //obj.U_DtAplicaca = data1;
                //obj.U_DtLiberaca = Convert.ToDateTime(itemLiberali.U_DtLiberaca).ToString("yyyy-MM-dd");
                //obj.U_CodigoFabricante = itemLiberali.U_CodigoFabricante;
                //obj.U_CodItem = itemLiberali.U_CodItem;
                //obj.U_DscItem = itemLiberali.U_DscItem;
                //obj.U_CodDeposit = itemLiberali.U_CodDeposit;
                //obj.U_NomDeposit = itemLiberali.U_NomDeposit;
                //obj.U_Quantidade = itemLiberali.U_Quantidade;
                //obj.U_ValorUnitario = itemLiberali.U_ValorUnitario;
                //obj.U_Custo = itemLiberali.U_Custo;
                //obj.U_NumeroNota = itemLiberali.U_NumeroNota;

                //lista.Add(obj);


                //ret.AGRM_OSOACollection = lista;

                return responseOrder;

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
                //throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
                return null;
            }
        }




    }
}
