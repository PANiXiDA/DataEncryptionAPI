using Common.SearchParams;
using Dal.DbModels;

namespace Dal.Interfaces
{
    public interface IUsersDal : IBaseDal<DefaultDbContext, DbModels.User, Entities.User, int, UsersSearchParams, object>
    {
        Task<bool> ExistsAsync(string login);
        Task<Entities.User?> GetAsync(string login);
    }
}
