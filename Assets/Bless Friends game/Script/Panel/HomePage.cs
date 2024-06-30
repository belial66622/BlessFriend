using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomePage : MonoBehaviour , IDragHandler , IEndDragHandler
{
    List<doll> dollAcquired = new();

    [SerializeField]
    Dolls dolllist;

    [SerializeField]
    private GameObject SlideItem;

    private Transform currentpage;

    [SerializeField]
    private Transform parentslide;

    int pageNumber = 9;

    int currentItem = 0;

    private Vector3 originPos;

    int pageMaxNumber = 0;

    List<Transform> itemDolls = new();

    Vector3 slideposition;

    int money_acquired;

    [SerializeField]
    TextMeshProUGUI timeCount;

    // Start is called before the first frame update

    public void Initialize()
    {
        originPos = parentslide.transform.localPosition;
        slideposition= parentslide.transform.position;
        UpdateItem();
    }
    public void GetDoll()
    {
        dollAcquired.Clear();
        foreach (var item in SaveData.Instance.save.inventory.dollHave)
        {
            var newdoll = dolllist.GetDoll(item.dollName);
            dollAcquired.Add(new doll
            {
                dolls = new Doll(newdoll.DollId, newdoll.DollName, newdoll.DollMoneyAmount, newdoll.DollImage),
                amount = item.AmountHold
            }
            );
        }
    }

    public void setTime(int hour , int minute)
    {
        if (hour > 0)
            timeCount.SetText($"{hour} hours:{ minute:00} minutes ");
        else
        {
            timeCount.SetText($"{minute:00} minutes ");
        }
    }

    public void AddMoney()
    {
        foreach (var item in dollAcquired)
        {
            money_acquired += item.dolls.DollMoneyAmount * item.amount;
        }

        SaveData.Instance.SetMoney(money_acquired);
    }

    public void UpdateItem()
    {
        disableall();
        GetDoll();

        if (dollAcquired.Count == 0) return;
        pageMaxNumber = 0;

        currentItem = 0;
        if (parentslide.childCount == 0)
        {
            currentpage = Instantiate(SlideItem, parentslide).transform;
            itemDolls.Add(currentpage);

            for (int i = 0; i < dollAcquired.Count; i++)
            {
                currentpage.GetChild(currentItem).GetComponent<Image>().sprite = dollAcquired[i].dolls.DollImage;
                currentpage.GetChild(currentItem).gameObject.SetActive(true);

                if (i + 1 >= dollAcquired.Count) break;
                currentItem++;
                if (currentItem >= pageNumber)
                {
                    pageMaxNumber++;
                    currentItem = 0;
                    currentpage = Instantiate(SlideItem, parentslide).transform;
                    itemDolls.Add(currentpage);
                }
            }
        }

        else
        {
            //check number of child
            currentpage = parentslide.GetChild(pageMaxNumber);

            for (int i = 0; i < SaveData.Instance.save.inventory.ingredientsHave.Count; i++)
            {
                currentpage.GetChild(currentItem).GetComponent<Image>().sprite = dollAcquired[i].dolls.DollImage;
                currentpage.GetChild(currentItem).gameObject.SetActive(true);

                if (i + 1 >= dollAcquired.Count) break;
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
                        itemDolls.Add(currentpage);
                    }
                }
            }
        }
    }


    public void disableall()
    {
        foreach (var datas in itemDolls)
        {
            foreach (Transform data in datas)
            {
                data.gameObject.SetActive(false);
            }
        }
    }

    /*    public void UpdateItem()
        {
            currentpage = Instantiate(SlideItem, parentslide).transform;

            for (int i = 0; i < dollAcquired.Count; i++)
            {
                currentpage.GetChild(currentItem).GetComponent<Image>().sprite = dollAcquired[i].dolls.DollImage;

                if (i + 1 >= dollAcquired.Count) break;
                currentItem++;
                if (currentItem >= pageNumber)
                {
                    currentItem = 0;
                    currentpage = Instantiate(SlideItem, parentslide).transform;
                }
            }
        }*/

    /*public void UpdateItem(int page)
    {
        int itemindex = 0;
        var temp = currentPage + page;
        var pageMax = (float)dollAcquired.Count / (float)pageNumber;
        if (dollAcquired.Count == 0) return;
        else if (temp == 0 && page < Mathf.FloorToInt(pageMax))
        {
            for (int i = 0; i < dollAcquired.Count; i++)
            {
                item[itemindex].sprite = dollAcquired[i].dolls.DollImage;
                itemindex++;
            }
            currentPage = temp;
        }
        else if (temp == 0 && page > Mathf.FloorToInt(pageMax))
        {
            for (int i = 0; i < (pageNumber * (temp + 1)); i++)
            {
                item[itemindex].sprite = dollAcquired[i].dolls.DollImage;
                itemindex++;
            }

            currentPage = temp;
        }
        else if (temp <= Mathf.FloorToInt(pageMax))
        {
            Debug.Log("3");
            for (int i = (pageNumber * temp); i < (pageNumber * (temp + 1)); i++)
            {
                item[itemindex].sprite = dollAcquired[i].dolls.DollImage;
                itemindex++;
            }

            currentPage = temp;
        }

        else if (temp < Mathf.CeilToInt(pageMax))
        {
            Debug.Log("4");
            for (int i = (pageNumber * temp); i < dollAcquired.Count; i++)
            {
                item[itemindex].sprite = dollAcquired[i].dolls.DollImage;
                itemindex++;
            }
        }

        else
        {
            return;
        }
    }*/
    public void OnDrag(PointerEventData eventData)
    {
        var dif = (eventData.pressPosition.x - eventData.position.x);

        parentslide.transform.position = slideposition - new Vector3(dif, 0, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var distance = currentpage.GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log(originPos.x - (distance * pageMaxNumber));

        Debug.Log(originPos.x);
        Debug.Log(distance);
        Debug.Log(pageMaxNumber);

        Debug.Log(parentslide.localPosition);

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

}


[SerializeField]
public struct doll
{
    public Doll dolls;
    public int amount;
}