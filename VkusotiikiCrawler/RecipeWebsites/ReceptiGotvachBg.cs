using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VkusotiikiCrawler
{
    class ReceptiGotvachBg : IRecipeWebsite
    {
        private readonly char[] TRIM_CHRACTERS = new char[3] { ' ', '\r', '\n' };
        const string URL_PATH = "http://recepti.gotvach.bg/r-56561-%D0%9B%D0%B5%D1%81%D0%BD%D0%BE_%D0%BF%D0%BE%D1%81%D1%82%D0%BD%D0%BE_%D0%BF%D1%80%D0%B5%D0%B4%D1%8F%D1%81%D1%82%D0%B8%D0%B5";

        public string GetURLPath() => URL_PATH;

        public void GetRecipeDataFromHTML(HtmlDocument htmlAgilityPackDocument, List<Recipe> recipes)
        {

            var recipeElement = htmlAgilityPackDocument.GetElementbyId("recipe");
            var instructionsElement = htmlAgilityPackDocument.GetElementbyId("instructions");
            var instructionsText = instructionsElement.InnerText;
        }
    }
}
