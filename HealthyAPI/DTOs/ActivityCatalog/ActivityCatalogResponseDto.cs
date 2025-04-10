using System;

namespace HealthyAPI.DTOs.ActivityCatalog
{
    public class ActivityCatalogResponseDto
    {
        public string ActivityCatalogID { get; set; }
        public string Name { get; set; }
        public int Minute { get; set; }
        public int Calories { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
