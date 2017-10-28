using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using DAL.Interfaces;

namespace BLL.Implementation
{
    public abstract class BaseService : ValidationService, IRunOnEachRequest, IBaseService
    {
        protected readonly IMapper Mapper;
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        
        public async Task SaveChangesAsync()
        {
            await UnitOfWork.CommitChangesToDatabaseAsync();
        }
    }
}
