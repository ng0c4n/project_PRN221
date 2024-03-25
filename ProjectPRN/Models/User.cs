using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(50)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }
        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        [Required]
        public string Email { get; set; }
        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        public string Password { get; set; }
        public int RoleID { get; set; }


        public virtual ICollection<Order>? Orders { get; set; }
        public virtual UserRole? UserRole { get; set; }


    }
}
