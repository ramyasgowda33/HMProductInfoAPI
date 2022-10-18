using HMProductInfoAPI.Data;
using HMProductInfoAPI.Models;
using System.Text;

namespace HMProductInfoAPI.Services
{
    public class ProductCodeGenerationService : IProductCodeGenerationService
    {
        private readonly HMProductDbContext _dbContext;

        public ProductCodeGenerationService(HMProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private static int channel1Code = 001;
        private static long channel3Code = 10000000;
        private static Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public void GenerateProductCode(Product product, out string code)
        {
            code = ComputeCode(product);

            while (CheckIfUnique(code))
            {
                code = ComputeCode(product);
            }
        }
       
        private string ComputeCode(Product product)
        {
            string code;

            switch (product.ChannelId)
            {
                //productYear + three digit integer code(2022001)
                case 1:
                    code = $"{product.ProductYear}00{channel1Code}";
                    channel1Code++;
                    break;
                // 6 digit unique alphanumeric code 
                case 2:
                    code = AlphanumericGenerator(6);
                    break;
                //integer which increases sequencially
                case 3:
                    code = $"{channel3Code}";
                    channel3Code++;
                    break;

                default: code = "Invalid Code"; break;
            }
            return code;
        }
        private bool CheckIfUnique(string code)
        {
            return _dbContext.Products.Any(x => x.ProductCode == code);
        }
        private string AlphanumericGenerator(int length)
        {
            var result = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }
    }
}
