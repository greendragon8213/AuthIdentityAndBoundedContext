using System.Threading.Tasks;
using DAL.Interfaces;
using DL.Identity;

namespace DAL.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityContext _db;

        public UnitOfWork(IdentityContext db)
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
