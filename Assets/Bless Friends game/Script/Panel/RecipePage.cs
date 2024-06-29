using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipePage : MonoBehaviour
{
    List<Recipe> ingredientsUnlocked;

    private int currentPage = 0;

    private void Start ()
    {
        
    }

    private void UpdateList()
    {
        ingredientsUnlocked.Clear();
        foreach (var item in AssetManager.Instance.recipeList.RecipeList)
        {
            ingredientsUnlocked.Add(new Recipe(item.RecipeId, item.IsUnlocked, item.DollNameRecipe, item.DollIngredients));
        }

        foreach (var item in ingredientsUnlocked)
        {
            if (item.IsUnlocked) continue;

            item.IsUnlocked = SaveData.Instance.save.inventory.recipe.Any(x => x.RecipeId == item.RecipeId);
        }
    }

    private void OnEnable()
    {
        currentPage = 0;
    }


    private void ShowRecipe()
    { 
        
    }
}
