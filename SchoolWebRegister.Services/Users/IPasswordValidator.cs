namespace SchoolWebRegister.Services.Users
{
    public interface IPasswordValidator
    {
        bool IsValid(string password);
    }

    public sealed class PasswordValidator : IPasswordValidator
    {
        public bool IsValid(string password)
        {
            return true;
        }
    }
}
