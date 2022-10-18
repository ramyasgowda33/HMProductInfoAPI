using HMProductInfoAPI.Models;

namespace HMProductInfoAPI.DTO
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public String ProductName { get; set; }
        public string ProductCode { get; set; }
        public int ProductYear { get; set; }
        public int ChannelId { get; set; }
        public Guid SizeScaleId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public ICollection<ArticleDto> Article { get; set; }

        public ICollection<Size> Size { get; set; }
    }
}
