using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredients", menuName = "Bless/Ingredients")]
public class IngredientsList : ScriptableObject
{
    [SerializeField]
    public List<Ingredients> ingredients;
    // Start is called before the first frame update
    public void InitializeFirstData()
    {
        ingredients.Clear();
        int i = 0;
        var data = Resources.LoadAll<Sprite>("ingredients");
        foreach (var item in data)
        {
            ingredients.Add(new Ingredients(i.ToString(), item, item.name));
            i++;
        }
    }

    public bool Check()
    {
        bool i = true;
        foreach (var item in ingredients)
        {
            if (string.IsNullOrWhiteSpace(item.Id) || string.IsNullOrWhiteSpace(item.Id))
            {
                i = false;
                return i;
            }
        }

        return i;
    }


    [System.Serializable]
    public class Ingredients
    {
        public string Id;
        public Sprite image;
        public string name;

        public Ingredients(string id, Sprite image, string name)
        {
            Id = id;
            this.image = image;
            this.name = name;
        }
    }
}
