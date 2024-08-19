namespace Common.SearchParams
{
    public class TokensSearchParams : BaseSearchParams
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool? IsActive { get; set; }
        public TokensSearchParams() { }
        public TokensSearchParams(int startIndex = 0, int? objectsCount = null) : base(startIndex, objectsCount) { }
    }
}
