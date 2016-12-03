using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    class Ingredient
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
        [JsonProperty("unstructured_data")]
        public string UnstructuredData { get; set; }

        public Ingredient()
        {

        }

        public Ingredient(string ingredients)
        {
            var splittedIngredients = ingredients.Split(' ');
            if (splittedIngredients.Count() == 3)
            {
                Quantity = splittedIngredients[0];
                Unit = splittedIngredients[1];
                Name = splittedIngredients[2];
            }
            else
            {
                Quantity = splittedIngredients[0];
                Unit = splittedIngredients[1];
                string ingredientName = "";
                for (int i = 2; i < splittedIngredients.Count(); i++)
                {
                    ingredientName += splittedIngredients[i] + " ";
                }
                Name = ingredientName;
            }

            UnstructuredData = ingredients;
        }
    }
}
