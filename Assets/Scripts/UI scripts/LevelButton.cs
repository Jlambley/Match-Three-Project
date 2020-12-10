using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelButton : MonoBehaviour
{

    [Header ("Active")]
        
    public bool isActive;
    public Sprite activeSprite, lockedSprite;


    public Image[] stars;
    private Image buttonImage;
    private Button levelButton;


    [Header("LEvel UI")]
    public Text levelText;
    public int level;
    public GameObject confirmPanel;

    private GameData gameData;

    private PiecePreview preview;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        levelButton = GetComponent<Button>();
        preview = FindObjectOfType<PiecePreview>();
        LoadData();
        ShowLevel(); //Decides what level each Prefab should be
        DecideSprite(); //Then allocates sprites to show if its unlocked or not

    }

    void LoadData()
    {
        if (gameData != null) //If our game Data Exists
        {
            //decide if the level is active
            if (gameData.saveData.isActive[level - 1])  //if its level 1 then we want to check the zero index because thats how they're stored

            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
            
        }
    }

    void DecideSprite() //checks to see if level is locked or not
    {
      if (isActive)
      {
            buttonImage.sprite = activeSprite;
            levelText.enabled = true;
            levelButton.enabled = true;
      }
        else
        {
            buttonImage.sprite = lockedSprite;
            levelText.enabled = false;
            levelButton.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelText.text = "" + level;

    }



    

    public void ConfirmPanel()
    {
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
        preview.LoadPreview(level-1);
        confirmPanel.GetComponent<ConfirmPanel>().LoadData(); //Makes the confirm panel load in
        

    }
}
