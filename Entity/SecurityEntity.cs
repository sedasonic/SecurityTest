using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class SecurityEntity
    {
        [StringLength(12)]
        public required string Isin { get; set; }
        public decimal? Price { get; set; }
    }
}
