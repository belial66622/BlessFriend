using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HomePage : MonoBehaviour
{
    List<doll> dollAcquired = new();

    [SerializeField]
    Dolls dolllist;

    [SerializeField]
    private Image[] item;

    int pageNumber = 9;

    int currentPage = 0;

    // Start is called before the first frame update
    void Start()
    {
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
        UpdateItem(0);
    }

    public void UpdateItem(int page)
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
    }
}


[SerializeField]
public struct doll
{
    public Doll dolls;
    public int amount;
}