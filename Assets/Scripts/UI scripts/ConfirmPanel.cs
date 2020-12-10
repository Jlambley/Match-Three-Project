using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public int level;
    public Level randomisedLevel;
    private GameData gameData;

    public string sceneToLoad;
    
  


    [Header("UI Content")]

    public int floorNumber;
    public Text floorText;
    public int highScore; //Highest score of the level
    public Text highText;
    public int currentScore; //current score of the level
    public Text currentText; //Displays theCurrentScore
    

    [Header("UI Switching")]
    public GameObject resetPanel;
    public GameObject selectPanel; //level select panel
    public Image canRestart, noRestart;
    public Text typeCurrent, typeHigh; //Keys to clarify which is the highscore and which is the current score of the level. Allowing to pause and return

    [Header("Transition")]
    private bool isLoading;
    public Animator transition;
    public Animator blackTransition;
    public float transitionTime = 5f; //Loading into level transition
    public Animator animPanel, animReset;
    public ParticleSystem bonesTransition;
    public ParticleSystem blackBonesTransition;


    // Start is called before the first frame update
    private void Start()
    {
        isLoading = false;
        gameData = FindObjectOfType<GameData>();
        LoadData();
        SetText();
        FindObjectOfType<InventoryManager>().ChangeEquipment(); //Updates to show the equipment icon
    }

    public void LoadData()
    {
        if (gameData != null)
        {
                 
        }
        SetText();

    }

    private void SetText()
    {
        if (gameData != null)
        {
            floorNumber = gameData.saveData.floorNumber[0];
            floorText.text = "" + floorNumber;

            highScore = gameData.saveData.highScores[level - 1];
            highText.text = "" + highScore;

            currentScore = gameData.saveData.levelScores[level - 1];
            currentText.text = "" + currentScore;

            if (currentScore > 0) //If the user currently has a score I.E the game is currently being played
            {
                highText.text = ""; //Hide the highscore to display the current score
                typeHigh.gameObject.SetActive(false);
                typeCurrent.gameObject.SetActive(true);
                noRestart.enabled = false;
                canRestart.enabled = true; //Shows a coloured button so the user can reset their progress 

            }
            else //If there is no current session
            {
                currentText.text = ""; //Hide the current score
                typeHigh.gameObject.SetActive(true);
                typeCurrent.gameObject.SetActive(false);
                noRestart.enabled = true; //Shows a greyed out button so the level cannot be restarted
                canRestart.enabled = false;
            }

        }
    }


    public void Cancel()
    {
        
        selectPanel.SetActive(true);
        StartCoroutine(AnimateOut(animPanel));
    }

   public void LoadMainScene()
   {
        if (floorNumber > 0) //any level greater than 0 the level has been randomised 
        {
            Instantiate(blackBonesTransition, Vector3.zero, Quaternion.identity); //Start the bone rain particle fx
        }
        else
        {
            Instantiate(bonesTransition, Vector3.zero, Quaternion.identity); //Start the bone rain particle fx
        }

        if (!isLoading) //By having this instantiate out of this IF, the user can press the button multiple times for more bones to fall before the load. Possibly really not good for mobile but very fun.
        {
            isLoading = true; //Stop the player loading a scene twice or triggering animations more than once ect..
            PlayerPrefs.SetInt("Current Level", level - 1);

            StartCoroutine(AnimateTransition());
            
        }
        
   }

    public void ResetAndLoadMainScene()
    {
        Instantiate(bonesTransition, Vector3.zero, Quaternion.identity); //Start the bone rain particle fx
        if (!isLoading) //By having this instantiate out of this IF, the user can press the button multiple times for more bones to fall before the load. Possibly really not good for mobile but very fun.
        {
            isLoading = true; //Stop the player loading a scene twice or triggering animations more than once ect..
            PlayerPrefs.SetInt("Current Level", level - 1);
            gameData.saveData.levelScores[level - 1] = 0; //resets score
            gameData.saveData.floorNumber[level - 1] = 0;
            gameData.Save(); //saves the reset
            floorNumber = 0;

            randomisedLevel.floor = 0;
            randomisedLevel.boardLayout = new TileType[0];

            StartCoroutine(AnimateTransition());

        }

    }



    public void ShowResetPanel()
    {
        resetPanel.SetActive(true);
    }

    public void HideResetPanel()
    {

        StartCoroutine(AnimateOut(animReset));  //sends the Out trigger to the panel 
    }

    private IEnumerator AnimateOut(Animator panel)
    {
        panel.SetTrigger("Out");
        yield return new WaitForSeconds(transitionTime*0.18f);//magic number but is the length of its only transition
        panel.gameObject.SetActive(false);
    }

    //animate transition

    IEnumerator AnimateTransition() //Animate either a new level or a randomsied one
    {
        if (floorNumber > 0)
        {
            sceneToLoad = "Random Scene";
            //Change bones to black

            yield return new WaitForSeconds(1f); //waits for bones to start 

            blackTransition.SetTrigger("Start"); //triggers transition
            yield return new WaitForSeconds(transitionTime); //waits for transition to finish
        }
        else
        {

            yield return new WaitForSeconds(1f); //waits for bones to start 

            transition.SetTrigger("Start"); //triggers transition
            yield return new WaitForSeconds(transitionTime); //waits for transition to finish
        }
        SceneManager.LoadScene(sceneToLoad); //finally loads the level
    }
}
