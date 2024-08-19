using BL.Interfaces;
using Common.SearchParams;
using Dal.Interfaces;
using Entities;

namespace BL.Standard
{
    public class TokensBL : ITokensBL
    {
        private readonly ITokensDal _tokensDal;

        public TokensBL(ITokensDal tokensDal)
        {
            _tokensDal = tokensDal;
        }

        public async Task<int> AddOrUpdateAsync(Token entity)
        {
            entity.Id = await _tokensDal.AddOrUpdateAsync(entity);
            return entity.Id;
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _tokensDal.ExistsAsync(id);
        }

        public Task<bool> ExistsAsync(TokensSearchParams searchParams)
        {
            return _tokensDal.ExistsAsync(searchParams);
        }

        public Task<Token> GetAsync(int id, object? convertParams = null)
        {
            return _tokensDal.GetAsync(id, convertParams);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _tokensDal.DeleteAsync(id);
        }

        public Task<SearchResult<Token>> GetAsync(TokensSearchParams searchParams, object? convertParams = null)
        {
            return _tokensDal.GetAsync(searchParams, convertParams);
        }
    }
}
