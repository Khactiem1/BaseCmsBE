using System;

namespace Cms.Model
{
    /// <summary>
    /// Enum FormatType định dạng dữ liệu
    /// </summary>
    public enum FormatType : int
    {
        /// <summary>
        /// Loại mà không xử lý gì cả
        /// </summary>
        None = 0,

        /// <summary>
        /// Định dạng cho tiền hạch toán (đơn giá)
        /// </summary>
        UnitPrice = 1,

        /// <summary>
        /// Thành tiền quy đổi
        /// </summary>
        Amount = 2,

        /// <summary>
        /// Hệ số, Tỷ lệ
        /// </summary>
        Rate = 3,

        /// <summary>
        /// Tỷ giá
        /// </summary>
        ExchangeRate = 4,

        /// <summary>
        /// Định dạng cho tiền ngoai tệ
        /// </summary>
        UnitPriceOC = 5,

        /// <summary>
        /// Thành tiền nguyên tệ
        /// </summary>
        AmountOC = 6,

        /// <summary>
        /// Hệ số, tỷ lệ phân bổ
        /// </summary>
        Allocation = 7,

        /// <summary>
        /// Số lượng chuyển đổi
        /// </summary>
        QuantityConvert = 8,

        /// <summary>
        /// Số lượng
        /// </summary>
        Quantity = 11,

        /// <summary>
        /// Kiểu chữ
        /// </summary>
        Text = 12,

        /// <summary>
        /// Kiểu tích chọn
        /// </summary>
        Checkbox = 13,

        /// <summary>
        /// Kiểu ngày tháng
        /// </summary>
        Date = 14,

        /// <summary>
        /// Kiểu danh sách
        /// </summary>
        List = 15,

        /// <summary>
        /// Kiểu thời gian
        /// </summary>
        Time = 16,

        /// <summary>
        /// Join mảng theo dấu ;
        /// </summary>
        Join = 17,

        /// <summary>
        /// Group Item
        /// </summary>
        GroupItem = 18,

        /// <summary>
        /// Kiểu ngày/tháng/năm có giờ
        /// </summary>
        DateTime = 19,

        /// <summary>
        /// Kiểu số không có định dạng sau dấu phẩy
        /// </summary>
        Number = 20,

        /// <summary>
        /// Kiểu text nhưng trên grid khi click vào sẽ drilldown
        /// </summary>
        Drilldown = 21,

        /// <summary>
        /// Dung lượng file
        /// </summary>
        FileSize = 24,

        /// <summary>
        /// Text nhiều dòng
        /// </summary>
        TextArea = 26,
        
        /// <summary>
        /// Định dạng dữ liệu html
        /// </summary>
        Html = 99,

        /// <summary>
        /// Kiểu enum
        /// </summary>
        Enum = 100,

        /// <summary>
        /// Kiểu trạng thái
        /// </summary>
        State = 101,

        /// <summary>
        /// Kiểu phần trăm
        /// </summary>
        Percentage = 102,

        /// <summary>
        /// Kiểu tỷ lệ
        /// </summary>
        Ratio = 27,

        /// <summary>
        /// Kiểu tiền quy đổi
        /// </summary>
        CurrencyExchange = 28,

        /// <summary>
        /// Kiểu đơn giá quy đổi
        /// </summary>
        UnitPriceConversion = 29,

        /// <summary>
        /// Kiểu dạng hình ảnh
        /// </summary>
        Image = 104,

        /// <summary>
        /// Kiểu cột dòng số
        /// </summary>
        RowIndex = 105,

        /// <summary>
        /// Mã quy cách
        /// </summary>
        Serial = 106,

        /// <summary>
        /// Kiểu biểu đồ
        /// </summary>
        Chart = 107,

        /// <summary>
        /// Kiểu năm
        /// </summary>
        Year = 108
    }
}
