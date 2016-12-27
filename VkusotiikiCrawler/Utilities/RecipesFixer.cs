using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class RecipesFixer
    {
        public void FixRecipes(List<Recipe> recipes)
        {
            foreach (var item in recipes)
            {
                item.FixRecipeProblems();
            }
        }

        public void TrimRecipes(List<Recipe> recipes)
        {
            List<Recipe> recipesToRemove = new List<Recipe>();
            recipesToRemove = recipes.Where(t => Recipe.FORBIDDEN_TITLES.Any(s => t.Name.ToLower().Contains(s))).ToList();
            recipes.RemoveAll(t => recipesToRemove.Contains(t));
        }
    }
}
