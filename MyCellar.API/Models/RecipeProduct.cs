using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MyCellar.API.Models
{
    public class RecipeProduct
    {
        public int RecipeId { get; set; }
        [JsonIgnore]
        public virtual Recipe Recipe { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
