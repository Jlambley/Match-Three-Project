using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim, gameInfoAnim;
    private float unpausetime = 1f;

    public void OkButton()
    {
        if (panelAnim != null && gameInfoAnim != null) //null reference exception
        StartCoroutine(HidePanel());
        StartCoroutine(GameStart());
        panelAnim.SetTrigger("Out");
        gameInfoAnim.SetTrigger("Out");
        Debug.Log("Fading Panel");
        
        
    }

    public IEnumerator HidePanel()
    {
        if (gameInfoAnim != null)
        {
            yield return new WaitForSeconds(2f);
            gameInfoAnim.SetTrigger("Off");
        }
    }


    public IEnumerator GameStart()
    {
        yield return new WaitForSeconds(unpausetime); //waits for 1f, could add another wait.... (ready?? yield wait 2f) ....(go! gamestate.move)
        BoardController board = FindObjectOfType<BoardController>(); //references the hoard, only happens once so no point creating a public reference
        board.currentState = GameState.move;

    }

    public void GameOver()
    {
        panelAnim.SetTrigger("Game Over");

    }


}
