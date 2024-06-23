using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class IngredientsItems : MonoBehaviour, IPointerClickHandler
{
    public UnityAction<string> GetimageEvent;

    public string ingredientName = "";

    public void GetImage()
    {
        GetimageEvent?.Invoke(ingredientName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetImage();
    }
}
