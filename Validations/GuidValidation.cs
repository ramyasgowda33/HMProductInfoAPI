using System.ComponentModel.DataAnnotations;

namespace HMProductInfoAPI.Validations
{
    public class GuidValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return Guid.TryParse(value.ToString(), out _);

        }
    }
}
