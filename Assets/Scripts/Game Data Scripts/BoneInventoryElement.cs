using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoneInventoryElement : MonoBehaviour
{

    public int boneId; //Set inside Unity
    private int value;
    public Text ammountText;
    private GameData gameData;
    public Image boneIcon;


    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
    }

    public void UpdateText() //updates text based on bone ID set in editor
    {
        gameData = FindObjectOfType<GameData>();

        if (gameData != null)
        {
            value = gameData.saveData.bones[boneId]; //saves the value
            ammountText.text = "" + value;
            UpdateImage();
        }
    }

    public void UpdateImage()
    {
        if (value <= 0) //if we do not have any bones of a certain type
        {
            boneIcon.color = Color.black; //(0,0,0,255)
            ammountText.text = "?";
        }
        else
        {
            boneIcon.color = Color.white; //(255,255,255,255)
        }
    }

}
