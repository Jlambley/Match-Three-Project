using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    public GameObject startPanel, levelPanel, infoPanel;
    public Animator anim;
    public float transitionDuration = 0.5f;
    public SoundManager soundManager;

    public Image soundButton;
    public Image musicButton;
    public Sprite soundOnSprite, soundOffSprite, musicOnSprite, musicOffSprite;

    


    // Start is called before the first frame update
    void Start()
    {
        KeyCheck();
        startPanel.SetActive(true);
        levelPanel.SetActive(false);

    }

    // Update is called once per frame
    public void PlayGame()
    {
        startPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void HideLevelSelect()
    {
        StartCoroutine(AnimateOut());
    }

    private IEnumerator AnimateOut()
    {
        anim.SetTrigger("Out"); //Begins out animation
        yield return new WaitForSeconds(transitionDuration);//magic number 
        levelPanel.SetActive(false);

    }

    public void Home()//Now instead takes you to the shop
    {
        //startPanel.SetActive(true);
        StartCoroutine(AnimateOut());
        FindObjectOfType<ShopManager>().ShowShop();
        //levelPanel.SetActive(false);
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
            PlayerPrefs.SetInt("Sound", 1); //Creates a key
            PlayerPrefs.Save();
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
            PlayerPrefs.SetInt("Music", 1); //Creates a Key
            PlayerPrefs.Save();
        }
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
            soundButton.sprite = soundOnSprite; //If the player does not have a key then show sound playing
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
            musicButton.sprite = musicOnSprite; //Same with the music
        }

    }
}
