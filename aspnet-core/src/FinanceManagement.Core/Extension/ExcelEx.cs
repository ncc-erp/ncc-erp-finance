using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Extension
{
    public static class ExcelEx
    {
        public static void Merge(this ExcelWorksheet worksheet, int startRow, int startCol, int endRow, int endCol)
        {
            worksheet.Cells[startRow, startCol, endRow, endCol].Merge = true;
        }
    }
}
