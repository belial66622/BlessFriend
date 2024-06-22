using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering;
using Environment = System.Environment;


public class SaveData : MonoBehaviour
{
    string savedGamesPath;

    private void Awake()
    {
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
    }

    public Save save;

    string SaveName = "SaveData.game";

    public void Saving()
    {
        string destination = Path.Combine(savedGamesPath, SaveName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination));


            var data = JsonUtility.ToJson(save,true);

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
        string destination = Path.Combine(Application.persistentDataPath, SaveName);
        Save saveData = new();
        if(File.Exists(destination)) 
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

                save = JsonUtility.FromJson<Save>(dataToLoad);
            }
            catch (Exception e) 
            {
                Debug.LogError("error" + destination + "\n" + e);
            }
        }
    }
}

[Serializable]
public class Save
{
    public Inventory inventory;
    public Economy economy;
    public TimeSave timeSave;
    public bool FirstTime;
}

[Serializable]
public class Inventory 
{
    public Recipe recipe;
    public DollHave dollHave;
}

[Serializable]
public class Economy
{ 
    public int Money;
}

[Serializable]
public class DollHave
{
    public string dollName;
    public int AmountHold;
}

[Serializable]
public class TimeSave
{
    public DateTime timesave;
    public float time;
}
