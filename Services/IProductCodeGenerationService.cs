using HMProductInfoAPI.Data;
using HMProductInfoAPI.Models;

namespace HMProductInfoAPI.Services
{
    public interface IProductCodeGenerationService
    {
        void GenerateProductCode(Product product, out string productCode);
    }
}
