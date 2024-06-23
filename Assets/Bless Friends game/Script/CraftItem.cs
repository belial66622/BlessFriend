using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftItem : MonoBehaviour , IPointerClickHandler
{
    public string Ingname = "";

    public UnityAction<string> OnClickEvent; 

    [SerializeField] TextMeshProUGUI amounttext;
    [SerializeField] Image image;

    int amountori;

    int currentamount;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentamount <= 0) return;
        OnClickEvent?.Invoke(Ingname);
        currentamount--;
        amounttext.SetText($"{currentamount}x");
    }

    public void SetItem(string name, int amount, Sprite sprite)
    { 
        amountori = amount;
        currentamount = amount;
        Ingname= name;
        amounttext.SetText($"{amount}x");
        image.sprite = sprite;
    }

    public void ResetImage()
    {
        currentamount = amountori;
        amounttext.SetText($"{amountori}x");
    }

}
