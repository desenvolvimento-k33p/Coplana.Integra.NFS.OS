using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Models
{
    public class InvoiceDTO2
    {

        [JsonIgnore]
        public string CustoDesp { get; set; }

        [JsonIgnore]
        public string Tipo { get; set; }

        [JsonIgnore]
        public string DocNumPedTransf { get; set; }

        [JsonIgnore]
        public string GerarEsboco { get; set; }

        [JsonIgnore]
        public string DocNumTransf { get; set; }
        [JsonIgnore]
        public string DocEntryTransf { get; set; }

        [JsonIgnore]
        public string DocEntry { get; set; }

        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string Comments { get; set; }
        //public int Series { get; set; }
        public string TaxDate { get; set; }
        public int BPL_IDAssignedToInvoice { get; set; }
        public int SequenceCode { get; set; }

        public string U_NumTransf { get; set; }

        public string U_NumPedTr { get; set; }

        public string U_StSentWMS { get; set; }

        public string U_ChaveAcesso { get; set; }

        public int SequenceSerial { get; set; }

        public string SequenceModel { get; set; }

        public string SeriesString { get; set; }

        public List<DocumentLine2> DocumentLines { get; set; }

        public ErrorInvoiceDto2 error { get; set; }

        public TaxExtension2 TaxExtension { get; set; }
    }

    public class DocumentLine2
    {

        public string U_K_CustoDespesaAtivo { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
        public string WarehouseCode { get; set; }

       // public string TaxCode { get; set; }
       // public string CFOPCode { get; set; }
        public int Usage { get; set; }       

        public int? BaseEntry { get; set; }

        public int? BaseType { get; set; }

        public int? BaseLine { get; set; }

        public List<BatchNumbers2> BatchNumbers { get; set; }

        public List<DocumentLinesBinAllocations2> DocumentLinesBinAllocations { get; set; }

        public string CostingCode { get; set; }
        //  public decimal UnitPrice { get; set; }
        //  public string COGSCostingCode { get; set; }
        //   public string COGSAccountCode { get; set; }
        //   public string CostingCode2 { get; set; }
        //   public string CostingCode3 { get; set; }
        //    public string CostingCode4 { get; set; }
        //    public string CostingCode5 { get; set; }

        public int LineNum { get; set; }

    }

    public class DocumentLinesBinAllocations2
    {
         public int BinAbsEntry { get; set; }
        public decimal Quantity { get; set; }

        public string AllowNegativeQuantity { get; set; } = "tNO";

        public int SerialAndBatchNumbersBaseLine { get; set; } = 0;
        
        public int BaseLineNumber { get; set; }
    }

    public class BatchNumbers2
    {
        public string ItemCode { get; set; }
        public string BatchNumber { get; set; }
        public decimal Quantity { get; set; }

        public int SystemSerialNumber { get; set; }

        public int BaseLineNumber { get; set; }
    }

    public class TaxExtension2
    {
        //public string TaxId0 { get; set; }
        //public string TaxId1 { get; set; }

        public string Incoterms { get; set; }

    }

    public class ErrorInvoiceDto2
    {
        public int code { get; set; }
        public MessageInvoiceDto2 message { get; set; }
    }

    public class MessageInvoiceDto2
    {
        public string lang { get; set; }
        public string value { get; set; }
    }
}
