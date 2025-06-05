using Common.SearchParams;
using Dal.DbModels;

namespace Dal.Interfaces
{
    public interface IXuysDal : IBaseDal<DefaultDbContext, Xuy, Entities.Xuy, int, XuysSearchParams, object>
    {
    }
}
