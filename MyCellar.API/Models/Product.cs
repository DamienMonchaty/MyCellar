using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace MyCellar.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MinLength(1)]
        public string Title { get; set; }
        [Required]
        [MinLength(1)]
        public string Description { get; set; }
        public int Quantity { get; set; }
        [Required]
        [MinLength(1)]
        public string ImgUrl { get; set; }
        [JsonIgnore]
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserProduct> UserProducts { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        [JsonIgnore]
        public int CategoryId { get; set; }
    }
}
