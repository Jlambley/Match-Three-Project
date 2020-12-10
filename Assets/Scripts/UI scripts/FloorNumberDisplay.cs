using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorNumberDisplay : MonoBehaviour
{
    public Text text;
    public Text uIText;
    private GameData gameData;
    private BoardController board;
    private int floor;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>();
        gameData = FindObjectOfType<GameData>();
        //floor = board.level;
        floor = gameData.saveData.floorNumber[0];
        text.text = "Floor " + floor;
        uIText.text = "" + floor; 
    }

}
