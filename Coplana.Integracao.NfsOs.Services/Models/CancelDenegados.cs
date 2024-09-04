using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Models
{
    public class CancelDenegados
    {
        public string Entity { get; set; }
        public string CardCode { get; set; }

        public string CardName { get; set; }
        public string DocEntry { get; set; }
        public string KeyNfe { get; set; }
        public string Description { get; set; }


    }
}
