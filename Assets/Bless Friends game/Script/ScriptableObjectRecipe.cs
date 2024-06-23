using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO",menuName ="Bless/Recipe")]
public class ScriptableObjectRecipe : ScriptableObject
{
    public List<Recipe> RecipeList;

}

[System.Serializable]
public class Recipe 
{
    public string RecipeId;
    public bool IsUnlocked;
    public string DollNameRecipe;
    public string [] DollIngredients;

    public Recipe(string recipeId,bool isUnlocked,string dollNameRecipe, string[] dollIngredients)
    {
        RecipeId = recipeId;
        IsUnlocked = isUnlocked;
        DollNameRecipe = dollNameRecipe;
        DollIngredients = dollIngredients;
    }

    public Recipe()
    {
        RecipeId = "";
        IsUnlocked = false;
        DollNameRecipe = "";
        DollIngredients = new string[0];
    }


    public Recipe Clone()
    {
        Recipe clone = new (RecipeId, IsUnlocked, DollNameRecipe, DollIngredients);
        return clone;
    }

}
