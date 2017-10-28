using System.Threading.Tasks;
using TestApp.DAL.Interfaces;
using TestApp.DL.Application;

namespace TestApp.DAL.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _db;

        public UnitOfWork(ApplicationContext db)
        {
            _db = db;
#if DEBUG
            _db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
        }

        public async Task CommitChangesToDatabaseAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
