using Common.SearchParams;
using Dal.DbModels;

namespace Dal.Interfaces
{
    public interface ITokensDal : IBaseDal<DefaultDbContext, DbModels.Token, Entities.Token, int, TokensSearchParams, object>
    {
    }
}
