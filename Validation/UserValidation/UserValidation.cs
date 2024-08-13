
namespace EasyWheelsApi.Validation.UserValidation
{
    public static class UserValidation
    {
        public static bool IsValidAddress(this string value)
        {
            return string.IsNullOrEmpty(value) || value == "string";
        }
    }
}