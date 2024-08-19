using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Dal.DbModels;
using Dal.Interfaces;
using Common.SearchParams;

namespace Dal.SQL
{
    public class TokensDal : BaseDal<DefaultDbContext, Token, Entities.Token, int, TokensSearchParams, object>, ITokensDal
    {
        protected override bool RequiresUpdatesAfterObjectSaving => false;

        public TokensDal(DefaultDbContext context) : base(context) { }

        protected override Task UpdateBeforeSavingAsync(DefaultDbContext context, Entities.Token entity, Token dbObject, bool exists)
        {
            dbObject.AccessToken = entity.AccessToken;
            dbObject.RefreshToken = entity.RefreshToken;
            dbObject.UserId = entity.User.Id;
            dbObject.IsActive = entity.IsActive;
            dbObject.CreatedAt = entity.CreatedAt;
            dbObject.AccessTokenExpiresAt = entity.AccessTokenExpiresAt;
            dbObject.RefreshTokenExpiresAt = entity.RefreshTokenExpiresAt;
            dbObject.IpAddress = entity.IpAddress;
            dbObject.UserAgent = entity.UserAgent;

            return Task.CompletedTask;
        }

        protected override IQueryable<Token> BuildDbQuery(DefaultDbContext context, IQueryable<Token> dbObjects, TokensSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.AccessToken))
            {
                dbObjects = dbObjects.Where(item => item.AccessToken == searchParams.AccessToken);
            }
            if (!string.IsNullOrEmpty(searchParams.RefreshToken))
            {
                dbObjects = dbObjects.Where(item => item.RefreshToken == searchParams.RefreshToken);
            }
            if (searchParams.IsActive.HasValue)
            {
                dbObjects = dbObjects.Where(item => item.IsActive == searchParams.IsActive.Value || item.RefreshTokenExpiresAt < DateTime.UtcNow);
            }

            return dbObjects;
        }

        protected override async Task<IList<Entities.Token>> BuildEntitiesListAsync(DefaultDbContext context, IQueryable<Token> dbObjects, object? convertParams, bool isFull)
        {
            return (await dbObjects.Include(item => item.User).ToListAsync()).Select(item => ConvertDbObjectToEntity(item)).ToList();
        }

        protected override Expression<Func<Token, int>> GetIdByDbObjectExpression()
        {
            return item => item.Id;
        }

        protected override Expression<Func<Entities.Token, int>> GetIdByEntityExpression()
        {
            return item => item.Id;
        }

        internal static Entities.Token ConvertDbObjectToEntity(Token dbObject)
        {
            if (dbObject == null) throw new ArgumentNullException(nameof(dbObject));

            return new Entities.Token(
                dbObject.Id,
                dbObject.AccessToken,
                dbObject.RefreshToken,
                UsersDal.ConvertDbObjectToEntity(dbObject.User),
                dbObject.IsActive,
                dbObject.CreatedAt,
                dbObject.AccessTokenExpiresAt,
                dbObject.RefreshTokenExpiresAt,
                dbObject.IpAddress,
                dbObject.UserAgent
            );
        }
    }
}
