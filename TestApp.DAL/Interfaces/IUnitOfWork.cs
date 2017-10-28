using System.Threading.Tasks;

namespace TestApp.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitChangesToDatabaseAsync();
    }
}
