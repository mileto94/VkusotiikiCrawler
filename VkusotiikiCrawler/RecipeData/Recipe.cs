using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class Recipe
    {
        //public static readonly char[] TRIM_CHRACTERS = new char[3] { ' ', '\r', '\n' };
        public readonly static string[] OTHER_ALERGIES = new string[] {"ядки", "орех", "лешни", "сусам", "фъстъ", "кашу", "бадем",
            "кестен", "шам-фъстъ", "соя", "пшени", "500", "горчица",  "целина" };
        public readonly static string[] MEAT = new string[] { "риба", "скумри", "шаран", "рибн", "кайма", "телешко", "овчо", "агнешко",
        "свинско", "суджук", "филе"};
        public readonly static string[] DAIRY = new string[] { "мляко", "млечн", "майонеза", "мед", "яйц", "сирене", "кашкавал" };

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("ingredients")]
        public List<Ingredient> Ingredients { get; set; }

        [JsonProperty("is_allergic")]
        public bool IsAllergic { get; set; } = false;

        [JsonProperty("difficulty")]
        public int Difficulty { get; set; } = 1;

        [JsonProperty("servings")]
        public int Servings { get; set; } = 1;

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
            FindAlergies();
            FindDifficulty();
            TrimTitle();
        }

        public void FindAlergies()
        {
            foreach (var ingredientName in Ingredients)
            {
                foreach (var item in ingredientName.Name.Split(' '))
                {
                    foreach (var allergy in OTHER_ALERGIES.Concat(MEAT).Concat(DAIRY))
                    {
                        if (item.StartsWith(allergy))
                        {
                            IsAllergic = true;
                            break;
                        }
                    }
                }
            }
        }

        public void FindDifficulty()
        {
            int ingredientsCount = Ingredients.Count;
            if (ingredientsCount <= 5)
            {
                Difficulty = 1;
            }
            else if (ingredientsCount > 5 && ingredientsCount <= 7)
            {
                Difficulty = 2;
            }
            else if (ingredientsCount > 7 && ingredientsCount <= 9)
            {
                Difficulty = 3;
            }
            else if (ingredientsCount > 9 && ingredientsCount <= 11)
            {
                Difficulty = 4;
            }
            else if (ingredientsCount > 11)
            {
                Difficulty = 5;
            }
        }

        public void TrimTitle()
        {
            if (Title != null)
            {
                string pattern1 = @"(\r\n)*";
                string pattern2 = @"( )*(\d)*$";
                string replacement = "";
                Regex rgx1 = new Regex(pattern1);
                Regex rgx2 = new Regex(pattern2);
                Title = rgx1.Replace(Title, replacement);
                Title = rgx2.Replace(Title, replacement);
            }
        }
    }
}
