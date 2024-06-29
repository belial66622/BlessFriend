using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeView : MonoBehaviour
{
    [SerializeField]
    public RecipeItem[] recipelist;

    public void Refresh()
    {
        foreach (var item in recipelist)
        { 
            item.RefreshItem();
        }
    }
}
