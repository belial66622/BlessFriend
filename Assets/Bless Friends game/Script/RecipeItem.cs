using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeItem : MonoBehaviour
{
    GameObject doll;

    Transform ingredientsParent;


    public void RefreshItem()
    { 
        doll.gameObject.SetActive(false);

        foreach (Transform child in ingredientsParent)
        { 
            child.gameObject.SetActive(false);
        }
    }
}
