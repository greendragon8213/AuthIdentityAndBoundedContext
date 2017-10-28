using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;

namespace BLL.Interfaces
{
    public interface IRefreshTokenService : IBaseService
    {
        Task CreateRefreshTokenAsync(AuthenticationTokenCreateContext context);
        Task ReceiveRefreshTokenAsync(AuthenticationTokenReceiveContext context);
    }
}
