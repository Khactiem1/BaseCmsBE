using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Cms.Core.Common.Extension;
using Cms.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Cms.BL
{
    public static class ExcelUtil
    {
        /// <summary>
        /// Hàm lấy ra column name theo số cột muốn lấy
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// CreatedBy:ntkiem (6/10/2022)
        public static string GetColumnName(int index)
        {
            string columnName = "";
            while (index > 0)
            {
                int remainder = (index - 1) % 26;
                columnName = (char)(65 + remainder) + columnName;
                index = (index - 1) / 26;
            }
            return columnName;
        }

        /// <summary>
        /// Hàm xử lý css file excel
        /// </summary>
        /// <param name="sheet">excel</param>
        /// <param name="column">tên cột excel cuối cùng</param>
        /// <param name="Header">tên cột đầu tiên</param>
        /// <param name="count">số lượng bản ghi xuất</param>
        /// <param name="headerColumns">các properties khi xuất</param>
        /// CreatedBy:ntkiem (6/10/2022)
        public static void SetUpExportHeaderData(ref ExcelWorksheet sheet, string column, string Header, int count, List<HeaderColumn> headerColumns)
        {
            // style header
            sheet.Cells[$"A1:{column}1"].Merge = true;
            sheet.Cells[$"A1:{column}1"].Value = Header;
            sheet.Cells[$"A1:{column}1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A1:{column}1"].Style.Font.Bold = true;
            sheet.Cells[$"A1:{column}1"].Style.Font.Size = 16;
            sheet.Cells[$"A1:{column}1"].Style.Font.Name = "Arial";
            sheet.Cells[$"A2:{column}2"].Merge = true;
            sheet.Row(3).Style.Font.Name = "Arial";
            sheet.Row(3).Style.Font.Bold = true;
            sheet.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Row(3).Style.Font.Size = 10;
            sheet.Cells[$"A3:{column}3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[$"A3:{column}3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D8D8D8"));
            sheet.Cells[3, 1].Value = "STT";
            sheet.Column(1).Width = 10;
            sheet.Cells[$"A3:{column}{count + 3}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A3:{column}{count + 3}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A3:{column}{count + 3}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A3:{column}{count + 3}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A3:{column}{count + 3}"].Style.Font.Name = "Times New Roman";
            sheet.Cells[$"A3:{column}{count + 3}"].Style.Font.Size = 11;

            // customize tên header của file excel
            int indexHeader = 2;
            // Duyệt từng header
            foreach (var header in headerColumns)
            {
                // add vào header của file excel
                sheet.Column(indexHeader).Width = ((header.Width - 5) / 7);
                sheet.Cells[3, indexHeader].Value = header.Header;
                indexHeader++;
            }
        }

        /// <summary>
        /// Set dữ liệu cho cell
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="indexRow"></param>
        /// <param name="indexBody"></param>
        /// <param name="recordItem"></param>
        /// <param name="header"></param>
        public static void SetCellValue(ref ExcelWorksheet sheet, int indexRow, int indexBody, object recordItem, HeaderColumn header)
        {
            var value = recordItem.GetValue(header.DataField);
            if(value == null)
            {
                sheet.Cells[indexRow + 4, indexBody].Value = "";
                return;
            }
            switch (header.FormatType)
            {
                case FormatType.Text:
                case FormatType.None:
                    sheet.Cells[indexRow + 4, indexBody].Value = value;
                    break;
                case FormatType.Date:
                case FormatType.DateTime:
                    string convertDate = "M/d/yyyy hh:mm:ss tt";
                    try
                    {
                        sheet.Cells[indexRow + 4, indexBody].Value = DateTime.ParseExact(value.ToString(), convertDate, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        if (convertDate == "M/d/yyyy hh:mm:ss tt")
                        {
                            convertDate = "d/M/yyyy hh:mm:ss tt";
                        }
                        else
                        {
                            convertDate = "M/d/yyyy hh:mm:ss tt";
                        }
                        sheet.Cells[indexRow + 4, indexBody].Value = DateTime.ParseExact(value.ToString(), convertDate, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    sheet.Cells[indexRow + 4, indexBody].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    break;
                case FormatType.Number:
                case FormatType.Quantity:
                    decimal valueNumber = 0;
                    if (decimal.TryParse(value.ToString(), out valueNumber))
                    {
                        sheet.Cells[indexRow + 4, indexBody].Value = string.Format("{0:0,0.0}", valueNumber);
                        sheet.Cells[indexRow + 4, indexBody].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    break;
                case FormatType.Enum:
                    if (header.EnumResources != null && header.EnumResources.Count > 0)
                    {
                        foreach (var item in header.EnumResources)
                        {
                            if (item.enumValue.ToString() == value.ToString())
                            {
                                sheet.Cells[indexRow + 4, indexBody].Value = item.enumText;
                                break;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
