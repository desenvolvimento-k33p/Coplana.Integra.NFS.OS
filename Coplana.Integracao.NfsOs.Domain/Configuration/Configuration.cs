using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Domain.Configuration
{
    public class Configuration
    {
        public ServiceLayer ServiceLayer { get; set; }
        public HanaDbConnection HanaDbConnection { get; set; }
        public CoplanaHttp CoplanaHttp { get; set; }
        public CoplanaBusiness CoplanaBusiness { get; set; }
    }

    public class CoplanaHttp
    {
        public string Uri { get; set; } = String.Empty;
        public string Key { get; set; } = String.Empty;
    }

    public class CoplanaBusiness
    {

        public string TaxCodeNFS { get; set; }
        public string CFOPNFS { get; set; }

        public int SeriesNFS { get; set; }
        public int SequenceCodeNFS { get; set; }
      
    }

    public class HanaDbConnection
    {
        public string Server { get; set; } = String.Empty;
        public string UserID { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Database { get; set; } = String.Empty;
    }

    public class ServiceLayer
    {
        public string SessionId { get; set; } = String.Empty;
        public string Uri { get; set; } = String.Empty;
        public string CompanyDB { get; set; } = String.Empty;
        public string UsernameManager { get; set; } = String.Empty;
        public string PasswordManager { get; set; } = String.Empty;
        public string Username { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string UrlFront { get; set; } = String.Empty;
        public int Language { get; set; }
        public string ApprovalUsername { get; set; } = String.Empty;
        public string ApprovalPassword { get; set; } = String.Empty;
    }
}
