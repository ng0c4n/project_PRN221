using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UserID { get; set; } 
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }   
        public DateTime UpdatedDate { get; set;}

        public virtual User User { get; set; }
        public virtual Status Status { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }


    }
}
