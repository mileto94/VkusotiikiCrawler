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
        static readonly char[] TRIM_CHRACTERS = new char[3] {' ', '\r', '\n' };
        //const string URL_PATH = "http://www.receptite.com/%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8-%D0%BE%D1%82/%D0%B1%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%BA%D1%83%D1%85%D0%BD%D1%8F";
        const string URL_PATH = "http://kulinar.bg/%D0%91%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D1%81%D0%BA%D0%B0-%D0%9A%D1%83%D1%85%D0%BD%D1%8F-%D1%80%D0%B5%D1%86%D0%B5%D0%BF%D1%82%D0%B8_l.rl_rn.1.html";
        //const string URL_PATH = "http://recepti.gotvach.bg/r-56561-%D0%9B%D0%B5%D1%81%D0%BD%D0%BE_%D0%BF%D0%BE%D1%81%D1%82%D0%BD%D0%BE_%D0%BF%D1%80%D0%B5%D0%B4%D1%8F%D1%81%D1%82%D0%B8%D0%B5";
        private static PoliteWebCrawler Crawler { get; set; }
        public static List<Recipe> Recipes { get; set; }

        static void Main(string[] args)
        {
            Recipes = new List<Recipe>();
            ReadRecipesFromJsonFile();
            InitiateCrawler();
            StartCrawler();
            WriteRecipesIntoJsonFile();
        }

        private static void ReadRecipesFromJsonFile()
        {
            try
            {
                string fileContent = File.ReadAllText(JSON_FILE_PATH);
                if (!String.IsNullOrWhiteSpace(fileContent))
                {
                    Recipes = JsonConvert.DeserializeObject<List<Recipe>>(fileContent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Console.WriteLine($"Successfully read from the file and found {Recipes.Count} recipes.");
            }
        }

        private static void WriteRecipesIntoJsonFile()
        {
            try
            {
                var recipesToJson = JsonConvert.SerializeObject(Recipes);

                // Write the text asynchronously to a new file
                using (StreamWriter outputFile = new StreamWriter(JSON_FILE_PATH, false, Encoding.UTF8))
                {
                    outputFile.Write(recipesToJson);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Console.WriteLine($"Successfully wrote in the file and found {Recipes.Count} recipes.");
            }
        }

        private static void StartCrawler()
        {
            //This is synchronous, it will not go to the next line until the crawl has completed
            CrawlResult result = Crawler.Crawl(new Uri(URL_PATH));

            if (result.ErrorOccurred)
                Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            else
                Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
        }

        private static void InitiateCrawler()
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            crawlConfig.CrawlTimeoutSeconds = 100;
            crawlConfig.MaxConcurrentThreads = 10;
            crawlConfig.MaxPagesToCrawl = 1000;
            //crawlConfig.UserAgentString = "abot v1.0 http://code.google.com/p/abot";

            //Will use the manually created crawlConfig object created above
            Crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);

            Crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            Crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            Crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            Crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
        }

        private static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        private static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

            var htmlAgilityPackDocument = crawledPage.HtmlDocument; //Html Agility Pack parser
            var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //AngleSharp parser

            KulinarBg(htmlAgilityPackDocument);
            //ReceptiteBg(htmlAgilityPackDocument);
            //ReceptiteGotvachBg(htmlAgilityPackDocument);

        }

        private static void KulinarBg(HtmlDocument htmlAgilityPackDocument)
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

                string recipeInstructions = "";
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



                if (Recipes.Where(s => s.Title == recipeName).Count() == 0)
                {
                    Recipe newRecipe = new Recipe();
                    newRecipe.Title = recipeName;
                    newRecipe.Instructions = recipeInstructions;
                    foreach (var item in recipeIngredients)
                    {
                        newRecipe.Ingredients.Add(new Ingredient(item));
                    }
                    newRecipe.Duration = recipeDuration;

                    Recipes.Add(newRecipe);
                }
                else
                {
                    Console.WriteLine($"Recipe with name {recipeName} is alredy there!");
                }
            }
        }

        private static void ReceptiteBg(HtmlDocument htmlAgilityPackDocument)
        {
            List<string> recipeIngredients = new List<string>();
            var recipeTitleElement = htmlAgilityPackDocument.DocumentNode.SelectNodes("//div[@class='title_rec_big box_width_min']")?.First();
            string recipeName = recipeTitleElement?.InnerText.Trim();

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

                Recipes.Add(newRecipe);

            }
        }

        private static void ReceptiGotvachBg(HtmlDocument htmlAgilityPackDocument)
        {
            var recipeElement = htmlAgilityPackDocument.GetElementbyId("recipe");

            var instructionsElement = htmlAgilityPackDocument.GetElementbyId("instructions");
            var instructionsText = instructionsElement.InnerText;
        }

        private static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        private static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
