﻿using AutoMapper;
using Coplana.Integracao.NfsOs.Core.Interfaces;
using Coplana.Integracao.NfsOs.Core;
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
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Text.Json;

namespace Coplana.Integracao.NfsOs.Services.Services
{

    public class CancelNFEService : ICancelNFEService
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
        private string nomeServico = "CancelNFEService";

        public CancelNFEService(IHanaAdapter hanaAdapter,
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

        private async Task<List<CancelNFE>> _getItensPendent()
        {
            List<CancelNFE> itensResult = new List<CancelNFE>();

            try
            {

                var query = SQLSupport.GetConsultas("GeiItensToCancelNFE");               
                var allItens = await _hanaAdapter.Query<CancelNFE>(query);
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

        private async Task _processItems(List<CancelNFE> itens)
        {

            try
            {

                foreach (var item in itens)
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

        private async Task<dynamic> _processUnitItem(CancelNFE item)
        {
            try
            {

                var response = await _cancelNFE(item);


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

        private async Task<bool> _cancelNFE(CancelNFE item)
        {


            string endpoint = "PurchaseInvoices(" + item.DocEntryNFE + ")/Cancel";

            InvoiceDTOReturn responseOrder = await _serviceLayerAdapter.Call<InvoiceDTOReturn>(
                    $"{endpoint}", HttpMethod.Post, null, Criptografia.Instancia.Descriptografar(_serviceLayerHttp.Uri));

            await _logger.Logger(new LogIntegration
            {
                LogTypeCode = 1,
                Message = $"Resposta da Service Layer",
                Owner = nomeServico,
                Method = "_cancelNFE",
                Key = item.DocEntryNFE,
                Key2 = item.DocEntryNFE,
                RequestObject = JsonSerializer.Serialize(item),
                ResponseObject = JsonSerializer.Serialize(responseOrder)
            });


            if (responseOrder != null)
            {

                //atualiza flag
                //if (tipo == "SemPedido")
                //{
                //    var query = SQLSupport.GetConsultas("AtualizaFlagNFE");
                //    query = string.Format(query, geraEsboco == "N" ? nfsSAP.DocNumTransf : nfsSAPDraft.DocNumTransf);
                //    var result = await _hanaAdapter.Execute(query);
                //}        

            }

            return false;
        }
      
    }
}
