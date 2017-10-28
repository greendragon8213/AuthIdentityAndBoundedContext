using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IBaseService : IValidationService
    {
        Task SaveChangesAsync();
    }
}
