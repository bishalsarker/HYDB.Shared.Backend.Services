namespace HYDB.Services.Models
{
    public class QueryScript
    {
        public string DataSource { get; set; }
        public string MutationType { get; set; }
        public string[] Fields { get; set; }
        public string Filter { get; set; }
        public string Condition { get; set; }
    }
}
