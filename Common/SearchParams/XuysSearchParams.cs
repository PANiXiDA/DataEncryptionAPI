namespace Common.SearchParams
{
    public class XuysSearchParams : BaseSearchParams
    {
        public bool? IsActive { get; set; }
        public XuysSearchParams() { }
        public XuysSearchParams(bool? isActive = null, string? searchQuery = null, int startIndex = 0, int? objectsCount = null)
            : base(startIndex, objectsCount, searchQuery)
        {
            IsActive = isActive;
        }
    }
}
