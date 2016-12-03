using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class Recipe
    {
        public readonly static string[] ALERGIES = new string[] {"ядки", "орех", "лешни", "орех", "кашу", "бадем",
            "кестен", "шам-фъстъ", "соя", "пшени", "мляко", "млечн", "риба", "скумри", "шаран", "рибн",
            "горчица", "сирене", "яйц" };

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("ingredients")]
        public List<Ingredient> Ingredients { get; set; }

        [JsonProperty("is_alergic")]
        public bool IsAlergic { get; set; } = false;

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
        }
    }
}
