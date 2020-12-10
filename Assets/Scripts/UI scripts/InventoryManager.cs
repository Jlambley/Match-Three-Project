using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Animator anim;
    public float transitionDuration;

    public GameObject EquipmentImage;

    public BoneInventoryElement[] boneHolders;
    private BoardController board;


    public void Start()
    {
        board = FindObjectOfType<BoardController>();
    }
    public void ShowInventory()
    {
        
        if (board != null) //If we have a board present
        {
            if (board.currentState == GameState.move) //and our game state is move
            {
                board.currentState = GameState.pause; //then pause and open up the inventory

                inventoryPanel.SetActive(true);
                //Do an animation?
                for (int i = 0; i < boneHolders.Length; i++)
                {
                    boneHolders[i].UpdateText(); //Updates each of the text elements in the inventory
                }
            }

        }
        else //if we do not have a board, then open up the inventory anyway
        {
            inventoryPanel.SetActive(true);
            //Do an animation?
            for (int i = 0; i < boneHolders.Length; i++)
            {
                boneHolders[i].UpdateText(); //Updates each of the text elements in the inventory
            }
        }
        
     

        

    }

    public void HideInventory()
    {
        if (board != null) //If we have a board present
        {
            if (board.currentState == GameState.pause) //and our game state is paused
            {
                board.currentState = GameState.move; //then unpause and animate
                StartCoroutine(Animate());
            }
        }
        else //If we dont have a board then just animate anyway
        {
            StartCoroutine(Animate());
        }
    }

    public IEnumerator Animate()
    {
        anim.SetTrigger("Out"); //Begin exit animation
        yield return new WaitForSeconds(transitionDuration); //wait for it to finish
        inventoryPanel.SetActive(false); //before deactivating panel

    }

    public void ItemEquip(int e)
    {
        PlayerPrefs.SetInt("Equipment", e); //Equipment player prefs stores an int for the item equipped during the game. Can only be set outside the game before a run
        FindObjectOfType<GameData>().saveData.equipped = e;
        ChangeEquipment();
    }

    public void ChangeEquipment()
    {
        if (PlayerPrefs.HasKey("Equipment"))
        {
            EquipmentImage.GetComponent<Image>().enabled = true;

            switch (PlayerPrefs.GetInt("Equipment"))
            {
                case 1: EquipmentImage.GetComponent<Image>().sprite = FindObjectOfType<ShopManager>().Trex; break; //Trex
                case 2: EquipmentImage.GetComponent<Image>().sprite = FindObjectOfType<ShopManager>().Iguana; break; //Iguana
                case 3: EquipmentImage.GetComponent<Image>().sprite = FindObjectOfType<ShopManager>().Tri; break; //Triceratops

            }

            
        }

    }
    


}



