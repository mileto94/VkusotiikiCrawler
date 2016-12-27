using AustinHarris.JsonRpc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class CrawlerRecipesService : JsonRpcService, ICrawlerRecipesService
    {
        [JsonRpcMethod]
        public string GetRecipeData()
        {
            var recipesToJson = JsonConvert.SerializeObject(VkusotiikiCrawler.Recipes);
            return recipesToJson;
        }
    }
}
