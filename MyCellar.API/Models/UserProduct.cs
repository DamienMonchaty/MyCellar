using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MyCellar.API.Models
{
    public class UserProduct
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
