using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary; //read and write but not to a text file so encrypts minor code


[Serializable]
public class SaveData
{
  
    public bool[] isActive;
    public int[] floorNumber;
    public int[] levelScores; //Stores the current score of the level (currently treated as floor number)
    public int[] highScores; //Stores the highest score
   // public float[] stageProgress; //remembers the fill rate of each level

    public int[] bones; //0 WHITE, 1 RED, 2 BLUE, 3 GREEN, 4 PURPLE, 5 PINK, 6 YELLOW, 7, FOSSIL, 8 OIL, 9, OBSIDIAN, 10 RUBY, 11 EMERALD, 12 DIAMOND
    public int equipped;
}




public class GameData : MonoBehaviour
{
    public static GameData gameData; //Enables singelton pattern
    public SaveData saveData;
    

    // Start is called before the first frame update
    void Awake()
    {
       // gameData = FindObjectOfType<GameData>().GetComponent<GameData>();
                     
        if (gameData == null) //If we do not have any gameData
        {
            
            DontDestroyOnLoad(this.gameObject); //Dont destroy this basic one
            gameData = this; //This becomes the gameData and we do not destroy
            gameData.saveData.isActive[0] = true; //Sets the first level as open
        }

        else
        {
            Destroy(this.gameObject);  //Otherwise destroy this object if they dont have priority
        }
        LoadData(); //Then load
    }


    public void Save() 
    {

        BinaryFormatter formatter = new BinaryFormatter();                                              //Create a binary formatter which can read + write binary files
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);     //Create a route from the program to the file
        SaveData data = new SaveData();                                                     //Create a blank to save to
        data = saveData;                                                                    //Create a copy of saved data

        formatter.Serialize(file, data); //Save the data in the file
        file.Close(); //Close the data stream
        Debug.Log("Saved Data");

    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

    public void LoadData()
    {
        //check if the savegame file exists
        if(File.Exists(Application.persistentDataPath + "/player.dat")) //If informmation exists load it
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("Loaded Data");
        }
    }


}
