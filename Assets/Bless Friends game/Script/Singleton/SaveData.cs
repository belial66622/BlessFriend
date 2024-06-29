using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Environment = System.Environment;

public class SaveData : MonoBehaviour
{
    string savedGamesPath;

    public Save save;

    string SaveName = "SaveData.game";

    public HomePage homePage;

    public CraftPage craftPage;

    public ShopPage shopPage;

    public RecipePage recipePage;

    public bool IsGameOn = false;

    public float time = 0f;

    private float resetTime = 86400f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

#if UNITY_STANDALONE_WIN
        savedGamesPath =
           Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
        savedGamesPath += "/My Games/MyGameName/";
#elif UNITY_EDITOR_WIN
        savedGamesPath =
   Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
        savedGamesPath += "/My Games/MyGameName/";
#else
        savedGamesPath = Application.persistentDataPath + "/";
#endif        
        Loading();

        updateData();

        recipePage.UpdateList();

    }

    IEnumerator TimeCount()
    {
        while (IsGameOn)
        {
            yield return null;
            time -= Time.deltaTime;
        }

    }

    public void ResetTime()
    {
        time = resetTime;
    }

    public static SaveData Instance { get; private set; }

    public void updateData()
    {
        craftPage.UpdateItem();
        shopPage.UpdateItem();
        homePage.UpdateItem();
        StopAllCoroutines();
        StartCoroutine(TimeCount());
    }

    public void Saving()
    {
        if (!save.FirstTime)
        {
            save.timeSave.time = time;
        }
        string destination = Path.Combine(savedGamesPath, SaveName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination));


            var data = JsonConvert.SerializeObject(save);

            using (FileStream stream = new FileStream(destination, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            }
            Debug.Log(data);
        }
        catch (Exception e)
        {
            Debug.LogError("error " + destination + "\n" + e);
        }
        updateData();
    }

    private void OnApplicationQuit()
    {
        if (save.timeSave.timesave < System.DateTime.Now)
        {
            save.timeSave.timesave = System.DateTime.Now;
        }
        Saving();
    }

    public void Loading() 
    {
        string destination = Path.Combine(savedGamesPath, SaveName);
        Save saveData = new();
        if (File.Exists(destination))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(destination, FileMode.Open))
                {
                    using (StreamReader read = new(stream))
                    {
                        dataToLoad = read.ReadToEnd();
                    }
                }

                save = JsonConvert.DeserializeObject<Save>(dataToLoad) ;
                Debug.Log(dataToLoad);
                if (save.timeSave.timesave < System.DateTime.Now)
                {
                    var diff = System.DateTime.Now - save.timeSave.timesave;
                    time = save.timeSave.time - (float)diff.TotalSeconds;
                }
                else
                {
                    time = save.timeSave.time;
                }
                if (save.FirstTime == false)
                {
                    IsGameOn = false;
                }
                else
                { 
                    IsGameOn = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("error" + destination + "\n" + e);
            }
        }
        else
        {
            save = saveData;
            Saving();
        }

        
    }

    public int GetMoney()
    {
        return SaveData.Instance.save.economy.Money;
    }

    public int SetMoney(int change)
    {
        SaveData.Instance.save.economy.Money += change;
        Saving();
        return SaveData.Instance.save.economy.Money;
    }


    public IngredientsHave GetIngredients(string name)
    {
        foreach (var item in SaveData.Instance.save.inventory.ingredientsHave)
        {
            if (item.IngredientName == name)
            {
                return item;
            }
        }

        return new IngredientsHave();
    }
    public void SetIngredients(string name, int amount , int money = 0)
    {
        foreach (var item in SaveData.Instance.save.inventory.ingredientsHave)
        {
            if (item.IngredientName == name)
            {
                item.AmountHold += amount;
                SaveData.Instance.SetMoney(amount * -money);
                return;
            }
        }

        SaveData.Instance.save.inventory.ingredientsHave.Add(new IngredientsHave {
        IngredientName= name,
        AmountHold = amount
        });
        SaveData.Instance.SetMoney(amount * money);
    }

    public void SetIngredients(List<string> name, int amount, int money = 0)
    {
        foreach (var item in SaveData.Instance.save.inventory.ingredientsHave)
        {
            foreach (var data in name)
            {
                if (item.IngredientName == data)
                {
                    item.AmountHold += amount;
                }
            }
        }

        SaveData.Instance.SetMoney(0);
    }



    public DollHave GetDolls(string name)
    {
        foreach (var item in SaveData.Instance.save.inventory.dollHave)
        {
            if (item.dollName == name)
            {
                return item;
            }
        }

        return new DollHave();
    }

    public void SetDolls(string name , int amount = 1)
    {
        foreach (var item in SaveData.Instance.save.inventory.dollHave)
        {
            if (item.dollName == name)
            {
                item.AmountHold += amount;
                Saving();
                return;
            }
        }

        SaveData.Instance.save.inventory.dollHave.Add(new DollHave
        {
            dollName = name,
            AmountHold = amount
        });

        Saving();
    }

    public RecipeHave GetRecipes(string name)
    {
        foreach (var item in SaveData.Instance.save.inventory.recipe)
        {
            if (item.RecipeId == name)
            {
                return item;
            }
        }

        return new RecipeHave();
    }

    public void setRecipe(string name)
    {
        foreach (var item in SaveData.Instance.save.inventory.recipe)
        {
            if (item.RecipeId == name)
            {
                return;
            }
        }

        SaveData.Instance.save.inventory.recipe.Add(new RecipeHave
        {
            RecipeId = name,
        });
        Saving();
    }

}


public class Save
{

    public Inventory inventory { get; set; } = new();

    public Economy economy { get; set; } = new();

    public TimeSave timeSave { get; set; } = new();

    public bool FirstTime { get; set; } = true;
}


public class Inventory 
{

    public List<RecipeHave> recipe { get; set; } = new();

    public List<DollHave> dollHave { get; set; } = new();

    public List<IngredientsHave> ingredientsHave { get; set; } = new();
}


public class Economy
{

    public int Money { get; set; }
}


public class DollHave
{

    public string dollName { get; set; }

    public int AmountHold { get; set; }
}


public class IngredientsHave
{
    public string IngredientName { get; set; }
    public int AmountHold { get; set; }
}


public class RecipeHave
{

    public string RecipeId { get; set; }
}


public class TimeSave
{
    public DateTime timesave { get; set; } = System.DateTime.Now;
    public float time { get; set; } = 86400f;
}
