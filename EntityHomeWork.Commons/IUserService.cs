namespace EntityHomeWork.Commons
{
    public interface IUserService
    {
        Task<(object? user, string token)> Login(string login, string password);

        Task<(object? user, string token)> Register(object user, string password);
    }
}
