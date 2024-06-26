using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankU.Audio;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
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

    [Serialize]
    private List<string> ingredientsused = new();

    int pageNumber = 9;

    int currentItem = 0;

    int pageMaxNumber = 0;

    private List<CraftItem> craftItems = new();

    [SerializeField]
    private List<Image> imagelist;

    [SerializeField]
    List<Vector3> originalPosImage = new();


    [SerializeField]
    private Sprite[] animationListImage;

    [SerializeField]
    private Image animationButton;

    [SerializeField]
    private Image Dolls;

    [SerializeField]
    private TextMeshProUGUI DollsName;

    [SerializeField]
    private GameObject skipone;
    [SerializeField]
    private GameObject skiptwo;
    [SerializeField]
    private GameObject skipthree;

    bool get= false;

    Vector3 slideposition;

    [SerializeField]
    GameObject firstTime;

    [SerializeField]
    GameObject firstTimeExplanation;

    bool isFirstTime = false;

    public void Initialize()
    {
        if (SaveData.Instance.save.FirstTime)
        {
            isFirstTime = true;
            FirstTime();
        }
        UpdateItem();
        originPos = parentslide.transform.localPosition;

        slideposition = parentslide.transform.position;
        foreach (var item in imagelist)
        {
            originalPosImage.Add(item.transform.position);
        }


    }

    private void FirstTime()
    {
        firstTime.SetActive(true) ;
        if (SaveData.Instance.save.inventory.ingredientsHave.Where(x => x.IngredientName == "onde2").Sum(x => x.AmountHold) < 2)
        {
            SaveData.Instance.SetIngredients("onde2", 2); 
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

                var asset = AssetManager.Instance;

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

                if (item.AmountHold <= 0)
                {

                    child.gameObject.SetActive(false);
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
                    currentItem = 0;
                    pageMaxNumber++;
                    if (pageMaxNumber < parentslide.childCount)
                    {
                        currentpage = parentslide.GetChild(pageMaxNumber);
                    }
                    else
                    {
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

        if (image is not null)
        {
            image.sprite = imagesprite;
            image.gameObject.SetActive(true);
        }

        AudioManager.Instance.PlaySFX("ButtonSound");
    }

    public void CheckCraft()
    {

        if (ingredientsused.Count == 0) return;

        crafting();
    }


    private void crafting()
    {
        Recipe recipe = null;
        foreach (var item in AssetManager.Instance.recipeList.RecipeList)
        {

            if (ingredientsused.Count == item.DollIngredients.Length)
            {
                bool match = true;
                foreach (var data in ingredientsused)
                {
                    if (item.DollIngredients.Contains(data))
                    {
                        Debug.Log($"match {data}");
                        continue;
                    }
                    else
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    Debug.Log("match");
                    recipe = item;
                    get = true;
                    break;
                }
            }
        }

        if (recipe is not null)
        {

            SaveData.Instance.SetDolls(recipe.DollNameRecipe);
            Dolls.sprite = AssetManager.Instance.dollList.GetDoll(recipe.DollNameRecipe).DollImage;
            DollsName.SetText($"{recipe.DollNameRecipe}");
            SaveData.Instance.setRecipe(recipe.RecipeId);
            SaveData.Instance.save.FirstTime = false;
        }
        else
        {
            if (SaveData.Instance.save.FirstTime)
            {
                FirstTime();
                return;
            }

            Dolls.sprite = Resources.Load<Sprite>("boneka/Fail-Doll");
            DollsName.SetText($"Fail");
        }

        SaveData.Instance.SetIngredients(ingredientsused, -1);
        StartCoroutine(Craft(0, ingredientsused.Count));

        skipone.SetActive(true);

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
        var dif = (eventData.pressPosition.x - eventData.position.x) ;

        parentslide.transform.position = slideposition - new Vector3(dif, 0, 0);
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

        slideposition = parentslide.transform.position;
    }

    IEnumerator Craft(int i, int jumlahitem)
    {
        AudioManager.Instance.PlaySFX("SuckSound");
        float t = 0;
        while (t < 1)
        {
            yield return null;
            imagelist[i].transform.position = Vector3.Lerp(originalPosImage[i], animationButton.transform.position, t);
            t += Time.deltaTime;
        }
          imagelist[i].transform.position = Vector3.Lerp(originalPosImage[i], animationButton.transform.position, 1);
        imagelist[i].gameObject.SetActive(false);

        if (i < jumlahitem-1 && i < 2)
        {
            StartCoroutine(Craft(i + 1 , jumlahitem));
        }
        else
        {
            Skip();
        }
    }

    public void Skip()
    {
        StopAllCoroutines();
        for (int i = 0; i < imagelist.Count; i++)
        {
            imagelist[i].gameObject.SetActive(false);
        }

        skipone.SetActive(false);
        skiptwo.SetActive(true);
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        AudioManager.Instance.PlaySFX("CraftSound");
        yield return new WaitForSeconds(.6f);
        animationButton.sprite = animationListImage[1];
        yield return new WaitForSeconds(.6f);
        animationButton.sprite = animationListImage[2];
        yield return new WaitForSeconds(.6f);
        animationButton.sprite = animationListImage[3];
        yield return new WaitForSeconds(.6f);
        animationButton.sprite = animationListImage[0];
        Show();
    }
    public void Show()
    {
        if (get)
        {
            AudioManager.Instance.PlaySFX("SuccessSound");
        }

        else
        {
            AudioManager.Instance.PlaySFX("FailSound");
        }
        StopAllCoroutines();
        Dolls.gameObject.SetActive(true);

        skiptwo.SetActive(false);
        skipthree.SetActive(true);
    }

    public void Reset()
    {
        StopAllCoroutines();
        for (int i = 0; i < imagelist.Count; i++)
        {
            imagelist[i].transform.position = originalPosImage[i];
        }

        animationButton.sprite = animationListImage[0];
        skipone.SetActive(false);
        skiptwo.SetActive(false);
        skipthree.SetActive(false);
        Dolls.gameObject.SetActive(false);
        ResetIngredient();

        if (isFirstTime)
        {
            SaveData.Instance.DisableFirst();
            firstTimeExplanation.SetActive(true);
            isFirstTime = false;
            SaveData.Instance.updateData();
        }
    }

    public void DisableFirst()
    {
        firstTime.SetActive(false);
        firstTimeExplanation.SetActive(false);
    }
}
