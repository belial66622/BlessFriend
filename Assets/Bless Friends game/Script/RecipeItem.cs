using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItem : MonoBehaviour
{
    [SerializeField]
    Image doll;

    [SerializeField]
    Transform ingredientsParent;


    public void RefreshItem()
    { 
        doll.gameObject.SetActive(false);

        foreach (Transform child in ingredientsParent)
        { 
            child.gameObject.SetActive(false);
        }
    }


    public void UpdateItem(string doll, string []ingrerients ) 
    {
        int i = 0;

        this.doll.sprite = AssetManager.Instance.dollList.GetDoll(doll).DollImage;
        this.doll.gameObject.SetActive(true);

        foreach (var item in ingrerients)
        {
            var child = ingredientsParent.GetChild(i).GetComponent<Image>();
            child.sprite = AssetManager.Instance.ingredientsList.GetImage(ingrerients[i]);
            child.gameObject.SetActive(true);
            i++;
        }
    }
}
