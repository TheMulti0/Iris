using System.Text.Json.Serialization;

namespace DashboardBackend.Models
{
    public class PageSearchParams
    {
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }
    }
}