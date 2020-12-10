using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FossilCountScript : MonoBehaviour
{
    public Text fossilText;
    GameData data;
    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<GameData>();
        UpdateFossilText();
    }

 
    public void UpdateFossilText() //called when the fossil is used or obtained
    {
        fossilText.text = data.saveData.bones[7].ToString(); //Sets the text to the ammount of fossils in the inventory

        if (data.saveData.bones[7] < 0 ) //if we somehow end up with minus numbers or null
        {
            fossilText.text = "0"; //set to zero
        }
    }

    public void DecreaseFossilCount()
    {
        if (data.saveData.bones[7] > 0)
        {
            data.saveData.bones[7]--;
        }
        UpdateFossilText();
    }

    //Activate item 
}
