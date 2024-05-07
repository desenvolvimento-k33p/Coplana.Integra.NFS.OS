using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Models
{
    public class InvoiceDTOReturn
    {
        [JsonIgnore]
        public string DocNumPedTransf { get; set; }

        [JsonIgnore]
        public string DocNumTransf { get; set; }
        [JsonIgnore]
        public string DocEntryTransf { get; set; }

       
        public int DocEntry { get; set; }

        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string Comments { get; set; }
        //public int Series { get; set; }
        public string TaxDate { get; set; }
        public int BPL_IDAssignedToInvoice { get; set; }
        public int SequenceCode { get; set; }

        public string U_NumTransf { get; set; }

        public List<DocumentLineReturn> DocumentLines { get; set; }

        public ErrorInvoiceDtoReturn error { get; set; }

        public TaxExtensionReturn TaxExtension { get; set; }
    }

    public class DocumentLineReturn
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string WarehouseCode { get; set; }

        public string TaxCode { get; set; }
        public string CFOPCode { get; set; }
        public int Usage { get; set; }

        public List<BatchNumbersReturn> BatchNumbers { get; set; }

        // public string CostingCode { get; set; }
        //  public decimal UnitPrice { get; set; }
        //  public string COGSCostingCode { get; set; }
        //   public string COGSAccountCode { get; set; }
        //   public string CostingCode2 { get; set; }
        //   public string CostingCode3 { get; set; }
        //    public string CostingCode4 { get; set; }
        //    public string CostingCode5 { get; set; }

    }

    public class BatchNumbersReturn
    {
        public string ItemCode { get; set; }
        public string BatchNumber { get; set; }
        public decimal Quantity { get; set; }

        public int SystemSerialNumber { get; set; }
    }

    public class TaxExtensionReturn
    {
        //public string TaxId0 { get; set; }
        //public string TaxId1 { get; set; }

        public string Incoterms { get; set; }

    }

    public class ErrorInvoiceDtoReturn
    {
        public int code { get; set; }
        public MessageInvoiceDtoReturn message { get; set; }
    }

    public class MessageInvoiceDtoReturn
    {
        public string lang { get; set; }
        public string value { get; set; }
    }
}
