using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labour
{
    class ExcelConverter : ICellValueToColumnTypeConverter
    {
        public bool SkipErrorValues { get; set; }
        public CellValue EmptyCellValue { get; set; }

        public ConversionResult Convert(Cell readOnlyCell, CellValue cellValue, Type dataColumnType, out object result)
        {
            result = DBNull.Value;
            ConversionResult converted = ConversionResult.Success;
            if (cellValue.IsEmpty)
            {
                result = EmptyCellValue;
                return converted;
            }
            if (cellValue.IsError)
            {
                result = "N/A";
                return ConversionResult.Success;
            }

            result = String.Format("{0:MMMM-yyyy}", cellValue.DateTimeValue);
            return converted;

        }
    }
}
