using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 


public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public int score;
    public Image scoreBar;
    public int floorNumber;

    public GameObject descentButton;
    private BoardController board;

    private GameData gameData;
    private GoalManager goalManager;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>();
        gameData = FindObjectOfType<GameData>();
        score = 0; //READ IN SCORE FROM LAST ONE
        floorNumber = 0; //resets the floor
        goalManager = FindObjectOfType<GoalManager>();
        
       
        score = gameData.saveData.levelScores[board.level]; //Loads in previous score
        floorNumber = gameData.saveData.floorNumber[board.level]; //Loads in the floor number
        IncreaseBarValue(); //also resets the bar if score is 0, which it is at the start

    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "" + score; //Could use "" or score.ToString
    }


    public void IncreaseScore (int ammountToIncrease)
    {
        score += ammountToIncrease;
        SaveScores();
        IncreaseBarValue();       
    }

    private void SaveScores()
    {
        if (gameData != null) //checks to see if we have gamedata
        {
            int highScore = gameData.saveData.highScores[board.level]; //copy the value of the highscore attached to thi slevel
            if (score > highScore) //if our current score is higher 
            {
                gameData.saveData.highScores[board.level] = score; //then update the highscsore with current score
            }
            gameData.saveData.levelScores[board.level] = score;  //Also save our current score so that we can pause and come back
            gameData.Save(); //And save our data
        }

    }

    private void IncreaseBarValue() //Increments the UI element to fill in based on score / the  last goal
    {
        if (gameData != null) //checks to see if we have gamedata
        {

            if (board != null && scoreBar != null)
            {
                int length = board.scoreGoals.Length;
                //Length will return 3 because there are 3 objects, but the eof is element 2 so we have to -1 to this. If we had 1 array item length would return 1 but we know that the first elemnt resides at 0 
                //scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1]; //This makes the fillbar work on score rather than goals
                
                int collected = 0;
                int needed = 0;
                for (int i = 0; i < goalManager.levelGoals.Length; i++)  //cycles through all the goals
                {
                    collected = collected + goalManager.levelGoals[i].numberCollected; //adds together all collected
                    needed = needed + goalManager.levelGoals[0].numberNeeded; //And all needed
                    
                }
                scoreBar.fillAmount = (float)collected / (float)needed;


                gameData.Save(); //And save our data

                if (scoreBar.fillAmount >= 1) //If we ar at our max
                {
                    descentButton.SetActive(true);
                }
                else
                {
                    descentButton.SetActive(false);
                }


            }
        }
    }

    


    private void ResetScores() //Highscores not reset
    {
        score = 0;
        //floorNumber = 0;
        scoreBar.fillAmount = 0; //Reset fill
        SaveScores(); //Saves our reset
        //IncreaseFloorNumber() //Saves and displays our reset floor number
        IncreaseBarValue(); //Displays the reset in the values shown and also saves

    }

    //Progress button press

    private void IncreaseFloorNumber()
    {
        // INT FLOOR = floorNumber
        // gameData.saveData.FloorNumber[board.level] = floorNumber
    }
}
