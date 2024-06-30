using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RecipePage : MonoBehaviour
{
    List<Recipe> ingredientslist = new();

    private int maxpage = 9;


    private Stack<int> nextpage =new();
    private Stack<int> prevpage = new();

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
            if (item.IsUnlocked)
            {
                continue;
            }
                item.IsUnlocked = SaveData.Instance.save.inventory.recipe.Any(x => x.RecipeId == item.RecipeId);
            
        }
        ShowRecipe();
    }

    private void OnDisable()
    {
        Clear();
        ShowRecipe();
    }

    public void Clear()
    {
        prevpage.Clear();
        nextpage.Clear();
    }
      
    private void ShowRecipe()
    {
        Clear();
        view.Refresh();
        int itemcount = 0;

        for (int i = 0 ; i < ingredientslist.Count; i++)
        {
            if (ingredientslist[i].IsUnlocked)
            {
                Debug.Log("masuk");
                view.recipelist[itemcount].UpdateItem(ingredientslist[i].DollNameRecipe, ingredientslist[i].DollIngredients);
                itemcount++;
            }

            if(itemcount >= maxpage-1)
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
        int itemnow = 0;

         if (prevpage.TryPeek(out var now))
            {
                if (now + 1 >= ingredientslist.Count) return;
                itemnow = now+1;
            }


        view.Refresh();
        for (int i = itemnow; i < ingredientslist.Count; i++)
        {
            if (ingredientslist[i].IsUnlocked)
            {
                view.recipelist[itemcount].UpdateItem(ingredientslist[i].DollNameRecipe, ingredientslist[i].DollIngredients);
                itemcount++;
            }

            if (itemcount >= maxpage-1)
            {
                prevpage.Push(i);
                return;
            }
        }

    }

    public void PrevPage()
    {
        int itemcount = 0;
        int itemnow = 0;


        if (prevpage.TryPop(out var item))
        {
            Debug.Log(item);

        }
        else
        {
            return;
        }

        view.Refresh();
        if (prevpage.TryPeek(out var now))
        {
            Debug.Log(now);
            itemnow = now;
        }


        for (int i = itemnow; i < ingredientslist.Count; i++)
        {
            if (ingredientslist[i].IsUnlocked)
            {
                Debug.Log($"jumlah item {itemcount} dam item ke - {i}");
                view.recipelist[itemcount].UpdateItem(ingredientslist[i].DollNameRecipe, ingredientslist[i].DollIngredients);
                itemcount++;
            }

            if (itemcount >= maxpage-1)
            {
                if(prevpage.Count==0)
                prevpage.Push(i);
                break;
            }
        }
    }
}
