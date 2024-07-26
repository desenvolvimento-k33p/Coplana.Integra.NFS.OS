using Coplana.Integracao.NfsOs.Core;
using Coplana.Integracao.NfsOs.Core.Interfaces;
using Coplana.Integracao.NfsOs.Domain.Configuration;
using Coplana.Integracao.NfsOs.Domain.Logger;
using Coplana.Integracao.NfsOs.Infra.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Coplana.Integracao.NfsOs.Infra.Repositories
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly IOptions<Configuration> _configuration;
        private readonly IHanaAdapter _hana;
        private readonly string HANA_DB;

        public LoggerRepository(IOptions<Configuration> configuration, IHanaAdapter hana)
        {
            _configuration = configuration;
            _hana = hana;

            HANA_DB = Criptografia.Instancia.Descriptografar(_configuration.Value.HanaDbConnection.Database);
        }

        public async Task Logger(LogIntegration logData)
        {
            //try
            //{
            string msg = Regex.Replace(String.IsNullOrEmpty(logData.Message) ? "" : logData.Message, "[^a-zA-Z0-9. ãêç/-:. ]+", "");
            var response = (object)logData.ResponseObject == null ? "" : logData.ResponseObject.ToString();
            response = System.Text.RegularExpressions.Regex.Unescape(response).Replace("'", "");

            await DeleteLogs();

            var sql = $@"INSERT INTO ""KEEPLOGS"".""KEEP_LOG_NFS_OS"" ( 
                ""LOGDATE"", 
                ""LOGHOUR"",               
                ""COMPANY"",               
                ""MESSAGE"",
                ""KEY_PARC"",
                ""KEY_SAP"",
                ""REQUESTOBJECT"",
                ""RESPONSEOBJECT"",
                ""OWNER"",
                ""METHOD"")
                VALUES (
                    CURRENT_DATE,
                    CURRENT_TIME,                   
                    '{HANA_DB}',                    
                    '{msg}',
                    '{logData.Key}',
                    '{logData.Key2}',
                    '{logData.RequestObject}',
                    '{response}',
                    '{logData.Owner}',
                    '{logData.Method}'
                )";

            var result = await _hana.Execute(sql);

            //}
            //catch (Exception e)
            //{
            //    throw new Exception($"Erro ao incluir Log {e.Message}", e);
            //}
        }

        private async Task DeleteLogs()
        {
            try
            {
                string sql = $@"DELETE FROM ""KEEPLOGS"".""KEEP_LOG_NFS_OS"" WHERE (OWNER = 'InsertNFEntradaService' OR OWNER = 'InsertNFSaidaService' ) AND LOGDATE < ADD_DAYS(CURRENT_DATE, -4) AND (IFNULL(KEY_PARC,'0') = '0' OR  IFNULL(KEY_PARC,'') = '')";
                await _hana.Execute(sql);

                
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao excluir Log {e.Message}", e);
            }
        }
    }
}
