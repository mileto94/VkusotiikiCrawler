using System.Collections.Generic;
using HtmlAgilityPack;

namespace VkusotiikiCrawler
{
    public interface IRecipeWebsite
    {
        string GetURLPath();
        void GetRecipeDataFromHTML(HtmlDocument htmlAgilityPackDocument, List<Recipe> recipes);
    }
}