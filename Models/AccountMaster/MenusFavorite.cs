namespace MROCoatching.DataObjects.Models.AccountMaster
{
    public class MenusFavorite : Audit
    {
        public long id { get; set; }
        public long MenuId { get; set; }
        //public string Name { get; set; }
        //public string Icon { get; set; }
        //public string Url { get; set; }
        //public long? ParentId { get; set; }
        //public string Privilages { get; set; }
        //public string Description { get; set; }
        public string ForWho { get; set; }
    }
}
