using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    class Recipe
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("instructions")]
        public string Instructions { get; set; }
        [JsonProperty("duration")]
        public string Duration { get; set; }
        [JsonProperty("ingredients")]
        public List<Ingredient> Ingredients { get; set; }

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
        }
    }
}
