using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Models
{
   
    public class AGRMOSOACollectionFull
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public int VisOrder { get; set; }
        public string Object { get; set; }
        public string LogInst { get; set; }
        public string U_CodSisVeic { get; set; }
        public string U_DscSisVeic { get; set; }
        public string U_CodOperMan { get; set; }
        public string U_DscOperMan { get; set; }
        public string U_DtAplicaca { get; set; }
        public string U_CodigoFabricante { get; set; }
        public string U_CodItem { get; set; }
        public string U_DscItem { get; set; }
        public string U_CodDeposit { get; set; }
        public string U_NomDeposit { get; set; }
        public double U_Quantidade { get; set; }
        public string U_NumeroNota { get; set; }
        public double U_ValorNota { get; set; }
        public string U_DtLiberaca { get; set; }
        public double U_QuantidadeDisponiv { get; set; }
        public double U_Custo { get; set; }
        public string U_Observacao { get; set; }
        public double U_ValorUnitario { get; set; }
        public string U_CodSolicitacaoCompra { get; set; }
        public string U_LinhaEncerrada { get; set; }
        public string U_StatusOperacao { get; set; }
        public string U_CodComponente { get; set; }
        public string U_DscComponente { get; set; }
        public string U_Tipo { get; set; }
    }

    public class AGRMOSOMCollectionFull
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public int VisOrder { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public string U_CodMecanico { get; set; }
        public string U_NomeMecanico { get; set; }
        public DateTime U_Data { get; set; }
        public string U_HoraInicio { get; set; }
        public string U_HoraFim { get; set; }
        public string U_Observacoes { get; set; }
        public double U_Custo { get; set; }
    }

    public class Full_AGRM_UDO_OSOF
    {
        //[JsonProperty("odata.metadata")]
        //public string odatametadata { get; set; }
        public int DocNum { get; set; }
        public int Period { get; set; }
        public int Instance { get; set; }
        public int Series { get; set; }
        public string Handwrtten { get; set; }
        public string Status { get; set; }
        public string RequestStatus { get; set; }
        public string Creator { get; set; }
        public string Remark { get; set; }
        public int DocEntry { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public int UserSign { get; set; }
        public string Transfered { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateTime { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string DataSource { get; set; }
        public string U_Numero { get; set; }
        public string U_Origem { get; set; }
        public string U_Status { get; set; }
        public string U_CodEquipam { get; set; }
        public string U_CodMotivo { get; set; }
        public string U_DscMotivo { get; set; }
        public string U_CodClassMa { get; set; }
        public string U_DscClassMa { get; set; }
        public DateTime U_DtEntrada { get; set; }
        public DateTime U_DtEncerram { get; set; }
        public double U_HorimOdome { get; set; }
        public string U_ServiExect { get; set; }
        public int U_BaixouMate { get; set; }
        public string U_CodMecanico { get; set; }
        public string U_NomeMecanico { get; set; }
        public string U_CodOperador { get; set; }
        public string U_NomeOperador { get; set; }
        public string U_CodigoCalendario { get; set; }
        public double U_CustoAplicacoes { get; set; }
        public double U_CustoMecanicos { get; set; }
        public string U_HoraEntrada { get; set; }
        public string U_HoraEncerramento { get; set; }
        public object U_NotaSaidaItem { get; set; }
        public string U_Avanco { get; set; }
        public List<AGRMOSOACollectionFull> AGRM_OSOACollection { get; set; }
        public List<AGRMOSOMCollectionFull> AGRM_OSOMCollection { get; set; }
        //public List<object> AGRM_OSO1Collection { get; set; }
    }



}
