using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    class KulinarBg : IRecipeWebsite
    {
        private readonly char[] TRIM_CHRACTERS = new char[3] {' ', '\r', '\n' };
        private const string URL_PATH = "http://kulinar.bg/%D0%91%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%9A%D1%83%D1%85%D0%BD%D1%8F-%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8_l.rl_rn.1.html";

        public void GetRecipeDataFromHTML(HtmlDocument htmlAgilityPackDocument, List<Recipe> recipes)
        {
            List<string> recipeIngredients = new List<string>();
            var recipeTitleElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//div[@class='recipeHead']")?.First();
            string recipeName = recipeTitleElement?.InnerText.TrimEnd(TRIM_CHRACTERS).TrimStart(TRIM_CHRACTERS);

            if (recipeName != null)
            {
                var recipeIngredientsElements = htmlAgilityPackDocument.DocumentNode.SelectNodes("//div[@class='item mb20 fs14 color3']");
                if (recipeIngredientsElements != null)
                {
                    foreach (var item in recipeIngredientsElements)
                    {
                        var unit = item.ChildNodes[1].InnerText;
                        var quantity = item.ChildNodes[2].InnerText;
                        recipeIngredients.Add(unit + " " + quantity);
                    }
                }

                string recipeInstructions = String.Empty;
                var recipeInstructionsElements = htmlAgilityPackDocument.DocumentNode.SelectNodes("//li[@class='item mb15 relative fs14 articleText']");
                if (recipeInstructionsElements != null)
                {
                    foreach (var item in recipeInstructionsElements)
                    {
                        if (item.ChildNodes.Count <= 3)
                        {
                            recipeInstructions += item.ChildNodes[2].InnerText.TrimEnd(TRIM_CHRACTERS).TrimStart(TRIM_CHRACTERS);
                        }
                        else
                        {
                            recipeInstructions += item.ChildNodes[3].InnerText.TrimEnd(TRIM_CHRACTERS).TrimStart(TRIM_CHRACTERS);
                        }
                    }
                }

                var recipeDurationElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//span[@class='fs24 bold']")?.First();
                string recipeDuration = recipeDurationElement?.InnerText.Trim();



                if (recipes.Where(s => s.Title == recipeName).Count() == 0)
                {
                    Recipe newRecipe = new Recipe();
                    newRecipe.Title = recipeName;
                    newRecipe.Instructions = recipeInstructions;
                    foreach (var item in recipeIngredients)
                    {
                        newRecipe.Ingredients.Add(new Ingredient(item));
                    }
                    newRecipe.Duration = recipeDuration;

                    recipes.Add(newRecipe);
                }
                else
                {
                    Console.WriteLine($"Recipe with name {recipeName} is alredy there!");
                }
            }

        }

        public string GetURLPath() => URL_PATH;
    }
}
