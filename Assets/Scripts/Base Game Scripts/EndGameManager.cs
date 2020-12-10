using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Time,
    Free
}

[System.Serializable] //Makes class appear inside unity
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue; //Used for time and moves so is a counter
}

public class EndGameManager : MonoBehaviour
{ 
    public EndGameRequirements requirements;
    public GameObject movesLabel, timeLabel;
    public Text counterLabel;
    public int currentCounterValue;
    public float timerSeconds;

    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;


    private BoardController board;
    private FadePanelController fade;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>();
        fade = FindObjectOfType<FadePanelController>();

        SetGameType();
        SetupGame();
    }

    void SetupGame()
    {
        
        currentCounterValue = requirements.counterValue;

        switch (requirements.gameType) //Check the gametype and run setup 
        {
            case GameType.Free: //If our endgame is free then...
                movesLabel.SetActive(false); //Set our labels to hide
                timeLabel.SetActive(false);
                break; 

            case GameType.Moves: 
                movesLabel.SetActive(true); //Make sure only moves is active
                timeLabel.SetActive(false);                
                
                break;

            case GameType.Time:
                timerSeconds = 1;
                movesLabel.SetActive(false); //Make sure only time is active
                timeLabel.SetActive(true);

                break;
        }

        counterLabel.text = "" + currentCounterValue;          

    }

    void SetGameType() //READS FROM SCRIPTABLE OBJECT
    {
        if (board.world != null)
        {
           if (board.level < board.world.levels.Length) //If we are in range
           {
                if (board.world.levels[board.level] != null)
                { 
                    requirements = board.world.levels[board.level].endGamerequirements; //Loads game type
                }
           } 
        }

    }


    public void DecreaseCounterValue()
    {

        if (board.currentState != GameState.pause) //If our game is not paused then decrease the counter
        {

            currentCounterValue--; //always decrease counter
            counterLabel.text = "" + currentCounterValue; //Update the text
            if (currentCounterValue <= 0)//If we have 0 moves on our counter
            {

                currentCounterValue = 0; //Make sure to always say 0 and not go negative
                counterLabel.text = "" + currentCounterValue; //And display 0 in the text
                LoseGame(); //Shows the player the defeat screen | Victory screen (WinGame) is called by the Goal Manager

            }
        }
    }

    public void WinGame()
    {
        board.currentState = GameState.win;

        victoryPanel.SetActive(true); //sets the correct panel to display
        defeatPanel.SetActive(false);
        fade.GameOver(); //animates and brings in UI Panel
    }

    public void LoseGame() 
    {
        board.currentState = GameState.lose; //Switch gamestate to lose

        defeatPanel.SetActive(true); //sets the correct panel to display
        victoryPanel.SetActive(false);
        fade.GameOver(); //Brings in UI Panel
        Debug.LogWarning("LOSER! Out of time or moves");
        fade.GameOver(); //Brings in UI Panel

    }

    // Update is called once per frame
    void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounterValue >0 )//&& board.currentState != GameState.pause)     //If we're on timer mode AND the time isnt at zero AND our board state isn't paused
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
