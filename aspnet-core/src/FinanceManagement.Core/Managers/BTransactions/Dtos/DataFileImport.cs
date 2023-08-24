using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class DataFileImport
    {
        public int Row { get; set; }
        public string ValueDate { get; set; }
        public string Message { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string TransactionFee { get; set; }
        public string TransactionVAT { get; set; }
    }
    public class DataTransformFromFile
    {
        public int Row { get; set; }
        public DateTime ValueDate { get; set; }
        public string Message { get; set; }
        public double Value { get; set; }
        public double TransactionFee { get; set; }
        public double TransactionVAT { get; set; }
    }
    public class ResultTransformDataFromFile
    {
        public List<DataTransformFromFile> Data { get; set; } = new List<DataTransformFromFile>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
