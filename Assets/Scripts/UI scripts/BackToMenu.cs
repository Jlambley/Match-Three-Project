using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public string sceneToLoad;
    private GameData gameData;
    private BoardController board;

    public void LoadBackMenu() //Sends the user back to menu
    {

        SceneManager.LoadScene(sceneToLoad); 

    }

    public void RetryStage() //When the player loses and wants to repeat the stage
    {
        SceneManager.LoadScene("Main Scene"); //reloads the current scene
    }

    private void LoseOutcome() //When the player has lost
    {

    }

    public void WinBackMenu() //If the player completed a stage but wants to go back to the menu
    {
        WinOutcome(); //Saves data
        LoadBackMenu(); //Returns to menu

    }

    private void WinOutcome() //Saves data after completing a stage
    {
        if (gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true; //Unlock next level
            gameData.Save();
        }
    }

    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<BoardController>();

    }
}
