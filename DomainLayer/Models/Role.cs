using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DomainLayer.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual List<User> Users { get; set; }
    }
}
