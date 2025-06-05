using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Dal.DbModels;
using Dal.Interfaces;
using Common.SearchParams;

namespace Dal.SQL
{
    public class XuysDal : BaseDal<DefaultDbContext, Xuy, Entities.Xuy, int, XuysSearchParams, object>, IXuysDal
    {
        protected override bool RequiresUpdatesAfterObjectSaving => false;

        public XuysDal(DefaultDbContext context) : base(context) { }

        protected override Task UpdateBeforeSavingAsync(DefaultDbContext context, Entities.Xuy entity, Xuy dbObject, bool exists)
        {
            dbObject.Name = entity.Name;
            dbObject.Description = entity.Description;
            dbObject.IsActive = entity.IsActive;
            return Task.CompletedTask;
        }

        protected override IQueryable<Xuy> BuildDbQuery(DefaultDbContext context, IQueryable<Xuy> dbObjects, XuysSearchParams searchParams)
        {
            if (searchParams.IsActive.HasValue)
            {
                dbObjects = dbObjects.Where(item => item.IsActive == searchParams.IsActive.Value);
            }
            if (!string.IsNullOrEmpty(searchParams.SearchQuery))
            {
                dbObjects = dbObjects.Where(item => item.Name.Contains(searchParams.SearchQuery));
            }
            return dbObjects.OrderBy(item => item.Id);
        }

        protected override async Task<IList<Entities.Xuy>> BuildEntitiesListAsync(DefaultDbContext context, IQueryable<Xuy> dbObjects, object? convertParams, bool isFull)
        {
            return (await dbObjects.ToListAsync()).Select(ConvertDbObjectToEntity).ToList();
        }

        protected override Expression<Func<Xuy, int>> GetIdByDbObjectExpression()
        {
            return item => item.Id;
        }

        protected override Expression<Func<Entities.Xuy, int>> GetIdByEntityExpression()
        {
            return item => item.Id;
        }

        internal static Entities.Xuy ConvertDbObjectToEntity(Xuy dbObject)
        {
            if (dbObject == null) throw new ArgumentNullException(nameof(dbObject));

            return new Entities.Xuy(
                dbObject.Id,
                dbObject.Name,
                dbObject.Description,
                dbObject.IsActive
            );
        }
    }
}
