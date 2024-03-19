﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Models
{
 

    public class AGRM_OSOACollectionDTOReturn
    {

        public List<AGRMOSOACollectionReturn> AGRM_OSOACollection { get; set; }

        public Error error { get; set; }
    }

    public class AGRMOSOACollectionReturn
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public int VisOrder { get; set; }
        public string U_Tipo { get; set; }
        public string U_DtAplicaca { get; set; }
        public string U_DtLiberaca { get; set; }
        public string U_CodigoFabricante { get; set; }
        public string U_CodItem { get; set; }
        public string U_DscItem { get; set; }
        public string U_CodDeposit { get; set; }
        public string U_NomDeposit { get; set; }
        public decimal U_Quantidade { get; set; }
        public decimal U_ValorUnitario { get; set; }
        public decimal U_Custo { get; set; }
        public int U_NumeroNota { get; set; }

    }

    public class Error
    {
        public int code { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public string value { get; set; }
    }
}
