using BL.Interfaces;
using Common.SearchParams;
using Dal.Interfaces;
using Entities;

namespace BL.Standard
{
    public class XuysBL : IXuysBL
    {
        private readonly IXuysDal _xuysDal;

        public XuysBL(IXuysDal xuysDal)
        {
            _xuysDal = xuysDal;
        }

        public async Task<int> AddOrUpdateAsync(Xuy entity)
        {
            entity.Id = await _xuysDal.AddOrUpdateAsync(entity);
            return entity.Id;
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _xuysDal.ExistsAsync(id);
        }

        public Task<bool> ExistsAsync(XuysSearchParams searchParams)
        {
            return _xuysDal.ExistsAsync(searchParams);
        }

        public Task<Xuy> GetAsync(int id, object? convertParams = null)
        {
            return _xuysDal.GetAsync(id, convertParams);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _xuysDal.DeleteAsync(id);
        }

        public Task<SearchResult<Xuy>> GetAsync(XuysSearchParams searchParams, object? convertParams = null)
        {
            return _xuysDal.GetAsync(searchParams, convertParams);
        }
    }
}
