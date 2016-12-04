using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VkusotiikiCrawler
{
    class ReceptiteBg : IRecipeWebsite
    {
        const string URL_PATH = "http://www.receptite.com/%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8-%D0%BE%D1%82/%D0%B1%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%BA%D1%83%D1%85%D0%BD%D1%8F";

        public string GetURLPath() => URL_PATH;

        public void GetRecipeDataFromHTML(HtmlDocument htmlAgilityPackDocument, List<Recipe> recipes)
        {
            List<string> recipeIngredients = new List<string>();
            var recipeTitleElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//div[@class='title_rec_big box_width_min']")?.First();
            string recipeName = recipeTitleElement?.InnerText;

            if (recipeName != null)
            {
                var recipeIngredientsElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//li[@class='productAdd']");
                if (recipeIngredientsElement != null)
                {
                    foreach (var item in recipeIngredientsElement)
                    {
                        recipeIngredients.Add(item.InnerText);
                    }
                }

                var recipeInstructionsElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//div[@class='recepta_prigotviane']")?.First();
                string recipeInstructions = recipeInstructionsElement?.InnerText;


                Recipe newRecipe = new Recipe();
                newRecipe.Title = recipeName;
                newRecipe.Instructions = recipeInstructions;
                foreach (var item in recipeIngredients)
                {
                    newRecipe.Ingredients.Add(new Ingredient(item));
                }

                recipes.Add(newRecipe);
            }
        }
    }
}
