using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        public string Email { get; set; }
        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        public string Password { get; set; }
        public int Role { get; set; }


        public virtual ICollection<Order>? Orders { get; set; }

    }
}
