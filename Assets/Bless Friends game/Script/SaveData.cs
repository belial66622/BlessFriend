using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Environment = System.Environment;

public class SaveData : MonoBehaviour
{
    string savedGamesPath;

    public Save save;

    string SaveName = "SaveData.game";
   
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
        Debug.Log(JsonUtility.ToJson(save));
    }


    public static SaveData Instance { get; private set; }


    public void Saving()
    {
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
            Debug.Log(destination);
        }
        catch (Exception e)
        {
            Debug.LogError("error" + destination + "\n" + e);
        }
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

    public bool FirstTime { get; set; }
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
 public DateTime timesave { get; set; } = new();
    public float time { get; set; }
}
