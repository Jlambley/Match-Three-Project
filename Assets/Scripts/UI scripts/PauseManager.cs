using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsButton;
    public SoundManager soundManager;

    public BoardController board;
    public bool paused = false;

    //player prefs
    public Image soundButton;
    public Image musicButton;
    public Sprite soundOnSprite, soundOffSprite,musicOnSprite, musicOffSprite;

    [Header("Transition")]

    public Animator outTransition;
    public float transitionTime = 1f;



    // Start is called before the first frame update
    void Start()
    {
        //In player prefs, the "sound" key is for sound, If sound == 0 then mute, if sound == 1 unmute
        KeyCheck();

        
            pausePanel.SetActive(false);
            board = GameObject.FindWithTag("Board").GetComponent<BoardController>();
        
    }


    public void KeyCheck()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = soundOffSprite;
            }
            else
            {
                soundButton.sprite = soundOnSprite;
            }

        }
        else
        {
            soundButton.sprite = soundOnSprite;
            PlayerPrefs.SetInt("Sound", 1);
        }

        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetInt("Music") == 0)
            {
                musicButton.sprite = musicOffSprite;
            }
            else
            {
                musicButton.sprite = musicOnSprite;
            }

        }
        else
        {
            musicButton.sprite = musicOnSprite;
            PlayerPrefs.SetInt("Music", 1);
        }

    }

    public void PauseGame()
    {
        if (board.currentState != GameState.wait && board.currentState != GameState.lose) //If the game state is not Wait and Lose
        {
            //then bring up the pause menu
            paused = !paused; //for bool variables, makes it the opposite of what it is. i.E if paused is true, calling this would make it false 
        }
    }

    public void SoundButton()
    {
        //In player prefs, the "sound" key is for sound, If sound == 0 then mute, if sound == 1 unmute
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = soundOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
            }
            else
            {
                soundButton.sprite = soundOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
            }
        }
        else
        {
            soundButton.sprite = soundOffSprite;
            PlayerPrefs.SetInt("Sound", 0);
        }
    }

    public void MusicButton()
    {
        //In player prefs, the "sound" key is for sound, If sound == 0 then mute, if sound == 1 unmute
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetInt("Music") == 0)
            {
                musicButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Music", 1);
                soundManager.adjustMusicVolume();
            }
            else
            {
                musicButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Music", 0);
                soundManager.adjustMusicVolume();
            }
        }
        else
        {
            musicButton.sprite = musicOnSprite;
            PlayerPrefs.SetInt("Music", 0);
            soundManager.adjustMusicVolume();
        }
    }

    public void ExitGame()
    {
        StartCoroutine(AnimateTransition());

    }

    void Update()
    {
        

        if(paused && !pausePanel.activeInHierarchy) //if we are paused and the pause panel is not active
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
        }
        if(!paused && pausePanel.activeInHierarchy) //if we are not paused and the menu is still active
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
        }
    }

    IEnumerator AnimateTransition()
    {

      //  outTransition.SetTrigger("Start"); //triggers transition
        yield return new WaitForSeconds(transitionTime*.25f); //waits for transition to finish
        SceneManager.LoadScene("Menu Scene"); ; //finally loads the level

    }
}
