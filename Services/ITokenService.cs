using HotByteAPI.Models;

namespace HotByteAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
