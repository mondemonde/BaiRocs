
namespace BaiRocs.Models
{
    /// <summary>
    /// Describes the structure of the result returned by the OCR processor
    /// </summary>
    public class BaiRocResult
    {
        public string ItemId { get; set; }
        public string Text { get; set; }
        public string StatusCode { get; set; }
        public string ErrorText { get; set; }
    }
}
