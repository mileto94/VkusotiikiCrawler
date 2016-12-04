﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class Recipe
    {
        public readonly static string[] ALERGIES = new string[] {"ядки", "орех", "лешни", "орех", "кашу", "бадем",
            "кестен", "шам-фъстъ", "соя", "пшени", "500", "мляко", "млечн", "риба", "скумри", "шаран", "рибн",
            "горчица", "сирене", "кашкавал", "яйц", "целина" };

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("ingredients")]
        public List<Ingredient> Ingredients { get; set; }

        [JsonProperty("is_allergic")]
        public bool IsAllergic { get; set; } = false;

        [JsonProperty("difficulty")]
        public int Difficulty { get; set; } = 1;

        [JsonProperty("servings")]
        public int Servings { get; set; } = 1;

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
            FindAlergies();
            FindDifficulty();
        }

        public void FindAlergies()
        {
            foreach (var ingredientName in Ingredients)
            {
                foreach (var item in ingredientName.Name.Split(' '))
                {
                    foreach (var allergy in ALERGIES)
                    {
                        if (item.StartsWith(allergy))
                        {
                            IsAllergic = true;
                            break;
                        } 
                    }
                }
            }
        }

        public void FindDifficulty()
        {
            int ingredientsCount = Ingredients.Count;
            if (ingredientsCount <= 5)
            {
                Difficulty = 1;
            }
            else if (ingredientsCount > 5 && ingredientsCount <= 7)
            {
                Difficulty = 2;
            }
            else if (ingredientsCount > 7 && ingredientsCount <= 9)
            {
                Difficulty = 3;
            }
            else if (ingredientsCount > 9 && ingredientsCount <= 11)
            {
                Difficulty = 4;
            }
            else if (ingredientsCount > 11)
            {
                Difficulty = 5;
            }
        }
    }
}