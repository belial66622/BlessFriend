using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftPage : MonoBehaviour, IDragHandler, IEndDragHandler
{

    [SerializeField]
    private GameObject SlideItem;

    private Transform currentpage;

    [SerializeField]
    private Transform parentslide;

    [SerializeField]
    private GameObject Popup;

    private Vector3 originPos;


    private List<string> ingredientsused = new();

    int pageNumber = 9;

    int currentItem = 0;

    int pageMaxNumber = 0;

    private List<CraftItem> craftItems = new();

    [SerializeField]
    private List<Image> imagelist;

    List<Vector3> originalPosImage = new();

    private GameObject lingkaran;

    [SerializeField]
    private Sprite[] animationListImage;

    [SerializeField]
    private Image animationButton;

    [SerializeField]
    private Image Dolls;

    [SerializeField]
    private TextMeshProUGUI DollsName;


    private void Start()
    {
        UpdateItem();
        SaveData.Instance.updateEvent += UpdateItem;
        originPos = parentslide.transform.localPosition;

        foreach (var item in imagelist)
        {
            originalPosImage.Add(item.transform.localPosition);
        }
    }

    public void UpdateItem()
    {
        pageMaxNumber = 0;

        currentItem = 0;
        if (parentslide.childCount == 0)
        {
            currentpage = Instantiate(SlideItem, parentslide).transform;

            for (int i = 0; i < SaveData.Instance.save.inventory.ingredientsHave.Count; i++)
            {
                var child = currentpage.GetChild(currentItem).GetComponent<Image>();

                var itemchild = child.gameObject.GetComponent<CraftItem>();

                var item = SaveData.Instance.save.inventory.ingredientsHave[i];

                craftItems.Add(itemchild);

                if (item.AmountHold == 0)
                {
                    continue;
                }


                itemchild.OnClickEvent = AddIngredients;
                itemchild.SetItem(item.IngredientName, item.AmountHold, AssetManager.Instance.ingredientsList.GetImage(item.IngredientName));

                child.gameObject.SetActive(true);

                if (i + 1 >= SaveData.Instance.save.inventory.ingredientsHave.Count) { break; }
                

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

            for (int i = 0; i < SaveData.Instance.save.inventory.ingredientsHave.Count; i++)
            {
                var child = currentpage.GetChild(currentItem).GetComponent<Image>();

                var itemchild = child.gameObject.GetComponent<CraftItem>();

                var item = SaveData.Instance.save.inventory.ingredientsHave[i];

                craftItems.Add(itemchild);

                if (item.AmountHold == 0)
                {
                    continue;
                }


                itemchild.OnClickEvent = AddIngredients;
                itemchild.SetItem(item.IngredientName, item.AmountHold, AssetManager.Instance.ingredientsList.GetImage(item.IngredientName));

                child.gameObject.SetActive(true);

                if (i + 1 >= SaveData.Instance.save.inventory.ingredientsHave.Count)
                { break; }
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

    public void AddIngredients(string name)
    {
        ingredientsused.Add(name);
        var imagesprite = AssetManager.Instance.ingredientsList.GetImage(name);

        var image = imagelist.Find(x => !x.gameObject.activeInHierarchy);

        if(image is not null)
        {
            image.sprite = imagesprite;
            image.gameObject.SetActive(true);
        }
    }

    public void CheckCraft()
    { 
    
    }

    public void ResetIngredient()
    {
        ingredientsused.Clear();
        foreach (var item in craftItems)
        { 
            item.ResetImage();
        }

        foreach (var item in imagelist)
        { 
            item.gameObject.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var dif = (eventData.pressPosition.x - eventData.position.x) / (10);
        parentslide.transform.position -= new Vector3(dif, 0, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var distance = currentpage.GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log(originPos.x - (distance * pageMaxNumber));

        if (parentslide.localPosition.x < (originPos.x - (distance * pageMaxNumber)))
        {
            parentslide.localPosition = new Vector3(originPos.x - distance * pageMaxNumber, originPos.y, originPos.z);
        }
        else if (parentslide.localPosition.x > 0)
        {
            parentslide.localPosition = originPos;
        }
    }

    IEnumerator Craft(int i, int jumlahitem)
    {
        float t = 0;
        while (t > 0)
        {
            yield return null;
            imagelist[i].transform.position = Vector2.Lerp(originalPosImage[i], lingkaran.transform.position, t);
            t += Time.deltaTime;
        }
        imagelist[i].transform.position = Vector2.Lerp(originalPosImage[i], lingkaran.transform.position, 1);
        imagelist[i].gameObject.SetActive(false);

        if (i < jumlahitem-1)
        {
            StartCoroutine(Craft(i + 1 , jumlahitem));
        }
        else if (i < 2)
        { 
            StartCoroutine(Craft(i + 1, jumlahitem)); 
        }
        else
        {
            StartCoroutine(Animation());
        }
    }

    public void Skip()
    {
        for (int i = 0; i < imagelist.Count; i++)
        {
            imagelist[i].gameObject.SetActive(false);
        }
    }

    IEnumerator Animation()
    {
        Recipe recipe = null;
        foreach (var item in AssetManager.Instance.recipeList.RecipeList)
        {
            if (craftItems.Count == item.DollIngredients.Length)
            {
                bool match = true;
                foreach (var data in item.DollIngredients)
                {
                    if (!craftItems.Any(x => x.Ingname == data))
                    {
                        match = false;
                        break;
                    }                    
                }
                if (match)
                {
                    recipe = item;
                    break;
                }
            }
        }

        if (recipe is not null)
        {
            Dolls.sprite = AssetManager.Instance.dollList.GetDoll(recipe.DollNameRecipe).DollImage;
            DollsName.SetText($"{recipe.DollNameRecipe}");
        }
        else
        {
            Dolls.sprite = Resources.Load<Sprite>("boneka/Fail-Doll");
            DollsName.SetText($"Fail");

        }

        yield return new WaitForSeconds(.5f);
        animationButton.sprite = animationListImage[1];
        yield return new WaitForSeconds(.5f);
        animationButton.sprite = animationListImage[2];
        yield return new WaitForSeconds(.5f);
        animationButton.sprite = animationListImage[3];
        yield return new WaitForSeconds(.5f);
        animationButton.sprite = animationListImage[0];

        Dolls.gameObject.SetActive(true);

    }

    public void Reset()
    {
        for (int i = 0; i < imagelist.Count; i++)
        {
            imagelist[i].transform.position = originalPosImage[i];
        }
    }
}
