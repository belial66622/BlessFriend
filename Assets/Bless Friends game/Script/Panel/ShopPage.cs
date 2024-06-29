using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopPage : MonoBehaviour , IDragHandler , IEndDragHandler
{
    [SerializeField]
    TextMeshProUGUI moneyText;

    [SerializeField]
    TextMeshProUGUI ownedText;

    [SerializeField]
    Image image;

    [SerializeField]
    private GameObject SlideItem;

    private Transform currentpage;

    [SerializeField]
    private Transform parentslide;

    [SerializeField]
    private GameObject Popup;

    private Vector3 originPos;

    string nameHold;

    int pageNumber = 9;

    int currentItem = 0;

    int pageMaxNumber= 0;

    Vector3 slideposition;

    public void Initialize()
    {
        UpdateItem();
        originPos = parentslide.transform.localPosition;

        slideposition = parentslide.transform.position;
    }

    public void TakeIgredients(string name)
    {
        Popup.SetActive(true);
        moneyText.SetText($"Money ${SaveData.Instance.GetMoney()}");
        ownedText.SetText($"Owned ({SaveData.Instance.GetIngredients(name).AmountHold})");
        image.sprite = AssetManager.Instance.ingredientsList.GetImage(name);
        nameHold = name;
    }

    public void BuyIngredients(int value = 1)
    {
        if (value * 10 <= SaveData.Instance.GetMoney())
        {
            SaveData.Instance.SetIngredients(nameHold, value, value * -10);
            TakeIgredients(nameHold);
        }
    }

    public void UpdateItem()
    {
        pageMaxNumber = 0;

        currentItem = 0;
        if (parentslide.childCount == 0)
        {
            currentpage = Instantiate(SlideItem, parentslide).transform;

            for (int i = 0; i < AssetManager.Instance.ingredientsList.ingredients.Count; i++)
            {
                var child = currentpage.GetChild(currentItem).GetComponent<Image>();

                var itemchild = child.gameObject.GetComponent<IngredientsItems>();

                itemchild.GetimageEvent = TakeIgredients;
                itemchild.ingredientName = AssetManager.Instance.ingredientsList.ingredients[i].name;
                child.sprite = AssetManager.Instance.ingredientsList.ingredients[i].image;

                child.gameObject.SetActive(true);

                if (i + 1 >= AssetManager.Instance.ingredientsList.ingredients.Count) break;
                currentItem++;
                if (currentItem >= pageNumber)
                {
                    currentItem = 0;
                    pageMaxNumber++;
                    currentpage = Instantiate(SlideItem, parentslide).transform;
                }
            }
        }

        else
        {  
            //check number of child
            currentpage = parentslide.GetChild(pageMaxNumber);

            for (int i = 0; i < AssetManager.Instance.ingredientsList.ingredients.Count; i++)
            {
                var child = currentpage.GetChild(currentItem).GetComponent<Image>();

                var itemchild = child.gameObject.GetComponent<IngredientsItems>();

                itemchild.GetimageEvent = TakeIgredients;
                itemchild.ingredientName = AssetManager.Instance.ingredientsList.ingredients[i].name;
                child.sprite = AssetManager.Instance.ingredientsList.ingredients[i].image;

                child.gameObject.SetActive(true);

                if (i + 1 >= AssetManager.Instance.ingredientsList.ingredients.Count) break;
                
                currentItem++;
                if (currentItem >= pageNumber)
                {
                    if (pageMaxNumber < parentslide.childCount)
                    {
                        currentItem = 0;
                        pageMaxNumber++;
                        currentpage = parentslide.GetChild(pageMaxNumber);
                    }
                    else
                    {
                        currentItem = 0;
                        pageMaxNumber++;
                        currentpage = Instantiate(SlideItem, parentslide).transform;
                    }
                }
            }

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var dif = (eventData.pressPosition.x - eventData.position.x);
    
        parentslide.transform.position = slideposition - new Vector3( dif, 0, 0 );
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var distance = currentpage.GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log(originPos.x - (distance * pageMaxNumber));

        if (parentslide.localPosition.x < (originPos.x - (distance*pageMaxNumber)))
        {
            parentslide.localPosition = new Vector3 (originPos.x - distance*pageMaxNumber, originPos.y, originPos.z );
        }
        else if (parentslide.localPosition.x > 0)
        {
            parentslide.localPosition = originPos;
        }

        slideposition = parentslide.transform.position;
    }
}
