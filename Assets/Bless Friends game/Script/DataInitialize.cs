using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInitialize : MonoBehaviour
{
    public Dolls doll;
    public IngredientsList bahan;


    public void dollInit()
    { 
        doll.InitializeFirstData();
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
}
