using Abot.Crawler;
using Abot.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;

namespace VkusotiikiCrawler
{
    class Program
    {
        const string JSON_FILE_PATH = @"recipes.json";
        //static readonly char[] TRIM_CHRACTERS = new char[3] {' ', '\r', '\n' };
        //const string URL_PATH = "http://www.receptite.com/%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8-%D0%BE%D1%82/%D0%B1%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%BA%D1%83%D1%85%D0%BD%D1%8F";
        //const string URL_PATH = "http://kulinar.bg/%D0%91%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%9A%D1%83%D1%85%D0%BD%D1%8F-%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8_l.rl_rn.1.html";
        //const string URL_PATH = "http://recepti.gotvach.bg/r-56561-%D0%9B%D0%B5%D1%81%D0%BD%D0%BE_%D0%BF%D0%BE%D1%81%D1%82%D0%BD%D0%BE_%D0%BF%D1%80%D0%B5%D0%B4%D1%8F%D1%81%D1%82%D0%B8%D0%B5";
        //private static PoliteWebCrawler Crawler { get; set; }
        public static List<Recipe> Recipes { get; set; }

        static void Main(string[] args)
        {
            VkusotiikiCrawler crawler;
            JSONManager manager;
            Recipes = new List<Recipe>();
            IRecipeWebsite recipeWebsite = new KulinarBg();
            manager = new JSONManager(JSON_FILE_PATH);
            Recipes = manager.ReadRecipes();
            FindRecipeWithAlergies();
            crawler = new VkusotiikiCrawler(recipeWebsite, Recipes);
            crawler.InitiateCrawler();
            crawler.StartCrawler();
            manager.WriteRecipes(Recipes);
        }

        public static void FindRecipeWithAlergies()
        {
            foreach (var recipe in Recipes)
            {
                foreach (var item in recipe.Ingredients)
                {
                    foreach (var alergy in Recipe.ALERGIES)
                    {
                        if (item.Name.StartsWith(alergy))
                        {
                            recipe.IsAlergic = true;
                            break;
                        }
                    }
                } 
            }
        }
    }
}
