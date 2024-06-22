using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IngredientsList;

[CreateAssetMenu(fileName ="Dolls",menuName ="Bless/Dolls")]
public class Dolls : ScriptableObject
{
    public List<Doll> doll;

    public void InitializeFirstData()
    {
        doll.Clear();
        int i = 0;
        var data = Resources.LoadAll<Sprite>("boneka");
        foreach (var item in data)
        {
            doll.Add(new Doll(i.ToString(), item.name,0,item));
            i++;
        }
    }

    public bool Check()
    {
        bool i = true;
        foreach (var item in doll)
        {
            if (string.IsNullOrWhiteSpace(item.DollId) || string.IsNullOrWhiteSpace(item.DollName))
            {
                i = false;
                return i;
            }
        }

        return i;
    }

    public Doll GetDoll(string dollName)
    {
        return doll.Find(x => x.DollName == dollName);
    }

}


[System.Serializable]
public class Doll
{
    public string DollId;
    public string DollName;
    public int DollMoneyAmount;
    public Sprite DollImage;

    public Doll(string dollId, string dollName, int dollMoneyAmount, Sprite dollImage)
    {
        DollId = dollId;
        DollName = dollName;
        DollMoneyAmount = dollMoneyAmount;
        DollImage = dollImage;
    }
}
