using HMProductInfoAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMProductInfoAPI.Models
{
    /*principal entity*/
    [Table("Products")]
    public class Product : Entity
    {
        [Key]
        [Column("Id", Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; }


        [Required]
        [Column("Code", Order = 1)]
        public string ProductCode { get; set; } = "Invalid";
        [MaxLength(100, ErrorMessage ="Max length : 100 exceeded")] //send 406 http status code upon violation


        [Column("Name", Order =2)]
        
        public String ProductName { get; set; } = String.Empty;


        [Column("Year", Order =3)]
        public int ProductYear { get; set; }


        [Column("ChannelId", Order = 4)]

        public int ChannelId { get; set; } = 3;


        [Column("SizeScale", Order = 5)]
        [GuidValidation(ErrorMessage = "Invalid SizeScaleId")]
        public Guid SizeScaleId { get; set; }

        //navigation property
        //[Column("Article", Order = 6)]
        public ICollection<Article> Articles { get; set; } = new List<Article>();

        [Timestamp]
        [Column("RowVersion", Order = 6)]
        public byte[] RowVersion { get; set; } =  new byte[8];

    }

    public enum ChannelId
    {
        Store, Online, All
    }
}
