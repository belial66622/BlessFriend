using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPage : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI moneyText;

    [SerializeField]
    TextMeshProUGUI ownedText;

    [SerializeField]
    Image image;

    [SerializeField]
    IngredientsList SoIngerdients;

    [SerializeField]
    private GameObject SlideItem;

    private Transform currentpage;

    [SerializeField]
    private Transform parentslide;

    int pageNumber = 9;

    int currentItem = 0;

    private void Start()
    {
        UpdateItem();
    }

    public void TakeIgredients(string name)
    {
        moneyText.SetText($"Money ${SaveData.Instance.save.economy.Money}");
        ownedText.SetText($"Owned ({SaveData.Instance.GetIngredients(name).AmountHold})");
        image.sprite = SoIngerdients.GetImage(name);
    }

    public void UpdateItem()
    {
        currentpage = Instantiate(SlideItem, parentslide).transform;

        for (int i = 0; i < SoIngerdients.ingredients.Count; i++)
        {
            var child = currentpage.GetChild(currentItem).GetComponent<Image>() ;

            child.sprite = SoIngerdients.ingredients[i].image;
            
            child.gameObject.SetActive(true);

            currentItem++;
            if (currentItem >= pageNumber)
            {
                currentItem = 0;
                currentpage = Instantiate(SlideItem, parentslide).transform;
            }
        }
    }
}
