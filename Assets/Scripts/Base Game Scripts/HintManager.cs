using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [SerializeField] //Lets me spy on the manager to see if its acessed the board controller object sucessfully 
    private BoardController board;
    public float hintCooldown; //How long until the player can use a hint again
    private float hintCooldownSeconds; //The actual variable that will countdown

    public GameObject HintParticle; //VFX to show hint
    public GameObject currentHint; //The piece that will be highlighted if the player wants a hint

    [Header("Button Function")]
    public GameObject hintButton;
    public Sprite noHint, yesHint, betweenHint;
    private bool canHint;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>(); //Only works if there is only ever 1 board in the scene
        ResetSeconds();
        canHint = false;
    }

    private void ResetSeconds()
    {
        
        hintCooldownSeconds = hintCooldown; //Reset the seconds delay
        CantHint();

    }
    
    ///First I want to find all possible matches on the board,              -- FindAllHintMatches()
    ///Then I want to pick one of those matches at random,                  -- PickRandomFromHintList()
    ///Then we want to create the hint, at the position of the chosen match -- MarkHint();
    ///Then destroy the hint we just created
    

    void Update()
    {
        if (board.currentState == GameState.move || board.currentState == GameState.wait)
        {
            hintCooldownSeconds -= Time.deltaTime; //countsdown every frame if we are in gamestate move or wait
        }

        if(hintCooldownSeconds <= 0 ) //If our seconds a are less or equal to zero AND there is no current hint AND the player can move a piece
        {
            //Set timer visablilty to zero
            AllowHint(); //Sets the image of the hint button to green

            if (currentHint == null && board.currentState == GameState.wait && canHint) //if we don't have a hint out and our current state is waiting and we can hint,
            {
                hintButton.GetComponent<Image>().sprite = betweenHint; //change colour to red
                canHint = false; //makes sure the user can't activate a hint during a wait period
            }
        }
        else
        {
            CantHint();
        }




        if (currentHint != null && board.currentState == GameState.wait) //if we have a hint on the board, and the gamestate is in waiting
        {
            DestroyHint(); //gets rid of hint
        }
    }

    private void AllowHint()
    {
        canHint = true;
        hintButton.GetComponent<Image>().sprite = yesHint; //sets colour to green
    }

    private void CantHint()

    {
        canHint = false; //makes sure the user can't activate a hint
        hintButton.GetComponent<Image>().sprite = noHint; //sets colour to grey
    }

    public void HintButtonPress()
    {
        if (canHint && currentHint == null && hintButton.GetComponent<Image>().sprite == yesHint) //if a hint is allowed and we don't already have a hint on the board and the button is green
        {
            MarkHint(); //Shows the user a hint
            ResetSeconds(); //reset the seconds
        }
    }


    private void MarkHint()
    {
        GameObject move = PickRandomFromHintList();
        if (move != null) //If pick randomly returned a gameobject, making move NOT null, then we can mark it as a hint move
        {
            currentHint = Instantiate(HintParticle, move.transform.position, Quaternion.identity); //Create a hint particle at Move's position
            
        }
    }


    GameObject PickRandomFromHintList()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllHintMatches();

        if (possibleMoves.Count > 0) //If moves exist inside the list
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null; //If we do not have any possible moves, then we return null
    }
    List<GameObject> FindAllHintMatches() //Creates/returns a list of gameObjects that can be swiped either right or down to create a match
    {
        List<GameObject> possibleMoves = new List<GameObject>();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allPieces[i, j] != null) //null reference check
                {
                    if (i < board.width - 1) //and we are not at the edge or 1 from the edge (MIGHT NEED TO BE -2)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right)) 
                            //Switch this piece one at a time, with their right neihbour and if a match is found, will return true
                        {
                            possibleMoves.Add(board.allPieces[i, j]); 
                            //If this piece can be switched and returns backa  match, then add this piece to the list of possible moves
                        }
                    }
                    if (j < board.height - 1) //If we are not at the top or one from the top (MIGHT NEED TO BE -2)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up)) 
                            //goes through one at a time, switching every pieace with the one above it, and if a match is found will return true
                        {
                            possibleMoves.Add(board.allPieces[i, j]); 
                            //Same again for down switch 
                        }
                    }
                }
            }
        }
        return possibleMoves; //Once all pieces are looped and switch, a list of pieces that could make a match is returned
    }

 
    //Destroy the hint
    public void DestroyHint()
    {
        if (currentHint != null) //If this hint exists
        {
            Destroy(currentHint); //Destroy it
            currentHint = null; //Set it to null
            ResetSeconds();
        }
    }


}
