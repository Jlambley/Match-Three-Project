using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour
{
    public Sprite rex, tri, iguana;
    public GameObject itemHolder, fossilHolder;
    private bool isUsingItem;
    BoardController board;
    public GameObject selectedVFX;

    private Color oldIcon, newIcon;
    // Start is called before the first frame update
    void Start()
    {
        oldIcon = itemHolder.GetComponent<Image>().color;
        newIcon = new Color(100f, 87f, 100f); //A purple hue
        board = FindObjectOfType<BoardController>();
        
        selectedVFX.SetActive(false);
        isUsingItem = false;

        if (PlayerPrefs.HasKey("Equipment"))
        {
            Debug.Log("EQUIPMENT PLAYER PREFS = " + PlayerPrefs.GetInt("Equipment"));
            itemHolder.SetActive(true);
            fossilHolder.SetActive(true);
            switch (PlayerPrefs.GetInt("Equipment"))
            {
                case 0: itemHolder.SetActive(false); fossilHolder.SetActive(false);break; //if we have no item selected then dont show the holders
                case 1: itemHolder.GetComponent<Image>().sprite = rex;break;
                case 2: itemHolder.GetComponent<Image>().sprite = iguana; break;
                case 3: itemHolder.GetComponent<Image>().sprite = tri; break;

            }

        }
        else
        {
            itemHolder.SetActive(false);
            fossilHolder.SetActive(false);
        }

    }


    public void ActivateItem() //Works together with the board controller to power up a piece into a bomb piece. It changes an int in the board controller so the next piece touched will turn into one of 3 bombs
    {
        
         
        if (board.currentState == GameState.move)
        {

            if (FindObjectOfType<GameData>().saveData.bones[7] > 0)
            {
                if (!isUsingItem) //If we are not using item then begin to activate
                {
                    if (PlayerPrefs.HasKey("Equipment"))
                    {
                        board.currentState = GameState.pause;
                        isUsingItem = true;

                        switch (PlayerPrefs.GetInt("Equipment"))
                        {
                            case 1: board.powerupPiece = 1; break; //Gets the board ready to convert a piece based on input
                            case 2: board.powerupPiece = 2; break;
                            case 3: board.powerupPiece = 3; break;

                        }
                        selectedVFX.SetActive(true);
                        itemHolder.GetComponent<Image>().color = newIcon; //makes it purpleish
                                                                          //animate
                    }
                }

                else //If we are already using the item then cancel
                {
                    DeactivateItem();
                }


            }
            

            
        }
    }
    public void DeactivateItem()
    {
        isUsingItem = false;
        board.currentState = GameState.move;
        selectedVFX.SetActive(false);
        itemHolder.GetComponent<Image>().color = oldIcon; //returns original colour
        board.powerupPiece = 0;

    }
}
