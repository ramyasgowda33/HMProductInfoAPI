using HMProductInfoAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMProductInfoAPI.Models
{
    /*dependent entity*/
    [Table("Articles")]
    public class Article : Entity
    {
        [Key]
        [Column("Id", Order =0)]
        public Guid ArticleId { get; set; }


        [Column("Name", Order = 1)]
        public string ArticleName { get; set; } = string.Empty;

        
        [Column("ProductId", Order = 2)]
        [ForeignKey("ProductRefId")]
        public Guid ProductId { get; set; }

        [Column("ColourId", Order = 3)]
        [GuidValidation(ErrorMessage = "Invalid ColourId")]
        public Guid ColourId { get; set; }

    }
}