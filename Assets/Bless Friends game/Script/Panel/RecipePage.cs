using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RecipePage : MonoBehaviour
{
    List<Recipe> ingredientslist;

    private int currentPage = 0;

    private int maxpage = 9;

    private int lastItemIndex = 0;

    private Stack<int> nextpage;
    private Stack<int> prevpage;

    [SerializeField]
    private RecipeView view;


    public void UpdateList()
    {
        ingredientslist.Clear();
        foreach (var item in AssetManager.Instance.recipeList.RecipeList)
        {
            ingredientslist.Add(new Recipe(item.RecipeId, item.IsUnlocked, item.DollNameRecipe, item.DollIngredients));
        }

        foreach (var item in ingredientslist)
        {
            if (item.IsUnlocked) continue;

            item.IsUnlocked = SaveData.Instance.save.inventory.recipe.Any(x => x.RecipeId == item.RecipeId);
        }
        ShowRecipe();
    }

    private void OnDisable()
    {
        prevpage.Clear();
        nextpage.Clear();
        ShowRecipe();
    }

      
    private void ShowRecipe()
    {
        int itemcount = 0;

        for (int i = 0 ; i < ingredientslist.Count; i++)
        {
            if (ingredientslist[i].IsUnlocked)
            {
                view.recipelist[itemcount].UpdateItem(ingredientslist[i].DollNameRecipe, ingredientslist[i].DollIngredients);
                itemcount++;
            }

            if(itemcount >= maxpage)
            {
                prevpage.Push(i);
                return; 
            }
        }

        prevpage.Push(ingredientslist.Count + 1);
    }

    public void NextPage()
    {
        int itemcount = 0;

        if (prevpage.Peek() + 1 == ingredientslist.Count) return;

        for (int i = prevpage.Peek(); i < ingredientslist.Count; i++)
        {
            if (ingredientslist[i].IsUnlocked)
            {
                view.recipelist[itemcount].UpdateItem(ingredientslist[i].DollNameRecipe, ingredientslist[i].DollIngredients);
                itemcount++;
            }

            if (itemcount >= maxpage)
            {
                prevpage.Push(i);
                return;
            }
        }

        prevpage.Push(ingredientslist.Count + 1);
    }

    public void PrevPage()
    {
        int itemcount = 0;
        int itemnow = 0; 

        if (prevpage.TryPop(out var item))
        {
            if (prevpage.TryPeek(out var now))
            { 
                itemnow= now;
            }
        }
        else
        {
            return;
        }

        for (int i = itemnow; i < ingredientslist.Count; i++)
        {
            if (ingredientslist[i].IsUnlocked)
            {
                view.recipelist[itemcount].UpdateItem(ingredientslist[i].DollNameRecipe, ingredientslist[i].DollIngredients);
                itemcount++;
            }

            if (itemcount >= maxpage)
            {
                break;
            }
        }
    }
}
