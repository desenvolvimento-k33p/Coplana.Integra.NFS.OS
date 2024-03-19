using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Models
{
    public class InsereItensOS
    {
        public int DocEntry {  get; set; }
        public int LineId { get; set; }
        public int VisOrder { get; set; }

        public string U_Tipo { get; set; }
        public DateTime U_DtAplicaca { get; set; }
        public DateTime U_DtLiberaca { get; set; }
        public string U_CodItem { get; set; }
        public string U_DscItem { get; set; }
        public string U_CodigoFabricante { get; set; }
        public decimal U_Quantidade { get; set; }
        public decimal U_ValorUnitario { get; set; }
        public string U_CodDeposit { get; set; }
        public string U_NomDeposit { get; set; }
        public int U_NumeroNota { get; set; }
        public decimal U_Custo { get; set; }
    }
}
