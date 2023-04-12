using MyCellar.API.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace MyCellar.API.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string ImgUrl { get; set; }
        public Difficulty Difficulty { get; set; }
        public Caloric Caloric { get; set; }
        [JsonIgnore]
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
