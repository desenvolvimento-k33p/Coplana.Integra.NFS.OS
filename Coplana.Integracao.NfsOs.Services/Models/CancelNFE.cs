using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Models
{
    public class CancelNFE
    {
        public string DocEntryNFE { get; set; }        
        public string DocNumNFE { get; set; }
        public string SerialSaída { get; set; }
        public string SerialEntrada { get; set; }
        public string DataSaida { get; set; }
        public string FilialOrigem { get; set; }
        public string FilialDestino { get; set; }
    }
}
