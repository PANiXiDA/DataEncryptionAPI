using Entities;

namespace UI.Areas.Public.Models
{
    public class XuyModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public static XuyModel? FromEntity(Xuy obj)
        {
            return obj == null ? null : new XuyModel
            {
                Id = obj.Id,
                Name = obj.Name,
                Description = obj.Description,
                IsActive = obj.IsActive
            };
        }

        public static Xuy? ToEntity(XuyModel obj)
        {
            return obj == null ? null : new Xuy(
                obj.Id,
                obj.Name,
                obj.Description,
                obj.IsActive);
        }

        public static List<XuyModel> FromEntitiesList(IEnumerable<Xuy> list)
        {
            return list?.Select(FromEntity).Where(x => x != null).Cast<XuyModel>().ToList() ?? new List<XuyModel>();
        }

        public static List<Xuy> ToEntitiesList(IEnumerable<XuyModel> list)
        {
            return list?.Select(ToEntity).Where(x => x != null).Cast<Xuy>().ToList() ?? new List<Xuy>();
        }
    }
}
