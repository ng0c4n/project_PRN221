using System.ComponentModel.DataAnnotations;

namespace ProjectPRN.ViewModels
{
    public class Cart
    {
        public int? OrderID {  get; set; }
        public int? ProductID { get; set; }
        public string? UserName { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Image {  get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
    }
}
