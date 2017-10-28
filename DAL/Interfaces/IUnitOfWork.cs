using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitChangesToDatabaseAsync();
    }
}
