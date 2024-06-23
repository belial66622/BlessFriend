using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataInitialize : MonoBehaviour
{
    public Dolls doll;
    public IngredientsList bahan;
    public TextAsset texts;
    public List<string> set;
    public ScriptableObjectRecipe recipe;


    public void dollInit()
    { 
        doll.InitializeFirstData(recipe);
    }


    public void ingredientInit()
    {
        bahan.InitializeFirstData();
    }

    public void check()
    {
        Debug.Log("doll "+ doll.Check());

        Debug.Log("bahan " +bahan.Check());
    }

    // Start is called before the first frame update
    public void text()
    {
        recipe.RecipeList.Clear();
        // Splitting the dataset in the end of line
        set = texts.text.Trim().Split('\n').ToList() ;
        int x = 0;
        foreach (var item in set)
        { 
            var resep = item.Split(",");
            List<string> kata = new();
            for (int i = 1; i < resep.Length; i++)
            {
                kata.Add(resep[i].Trim());
            }
            recipe.RecipeList.Add(new Recipe(x.ToString(), false, resep[0],kata.ToArray()));
            x++;
        }


        // Iterating through the split dataset in order to spli into rows
        /*for (var i = 1; i < splitDataset.Length; i++)
        {
            string[] row = splitDataset[i].Split(new char[] { ',' });
            print(row[i]);
        }*/
    }
}
