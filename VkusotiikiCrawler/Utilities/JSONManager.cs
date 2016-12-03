using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class JSONManager
    {
        private string _JSONFilePath;

        public JSONManager(string JSONFilePath)
        {
            _JSONFilePath = JSONFilePath;
        }

        public List<Recipe> ReadRecipes()
        {
            List<Recipe> recipes = new List<Recipe>();
            try
            {
                string fileContent = File.ReadAllText(_JSONFilePath);
                if (!String.IsNullOrWhiteSpace(fileContent))
                {
                    recipes = JsonConvert.DeserializeObject<List<Recipe>>(fileContent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Console.WriteLine($"Successfully read from the file and found {recipes.Count} recipes.");
            }

            return recipes;
        }

        public void WriteRecipes(List<Recipe> recipes)
        {
            try
            {
                var recipesToJson = JsonConvert.SerializeObject(recipes);

                using (StreamWriter outputFile = new StreamWriter(_JSONFilePath, false, Encoding.UTF8))
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
                Console.WriteLine($"Successfully wrote in the file and found {recipes.Count} recipes.");
            }
        }
    }
}
