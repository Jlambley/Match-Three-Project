using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    [Header("Board Variables")]
    public float pauseTime = .6f;
    public int column, row; //location
    public int targetX, targetY; //The position of THIS dot that will be moved to the otherPiece
    public int previousColumn, previousRow;

    [Header("Piece Stats / Powerup")]
    public bool isMatched = false;
    public bool isBombColumn;
    public bool isBombRow;
    public bool isColourBomb;
    public bool isAreaBomb;
    public GameObject columnArrow, rowArrow, colourBomb, areaBomb;


    [Header("Swipe stats")]
    public GameObject otherPiece;   //The piece to swap with
    public float swapSpeed = .6f;
    public float swipeAngle = 0; //measure of angle between drag/drop touch input [45:135 Up] [-45:-135 Down] [-45 : 45 Right] [-135:135 Left] 
    public float swipeResist = 1f; //Smooths out value of swiping to ensure taps on elements wont swipe them
    private Vector2 firstTouchPosition = Vector2.zero; //swipe start 
    private Vector2 endTouchPosition = Vector2.zero;   //swipe end
    private Vector2 tempPosition;

    private BoardController board;  //Reference to the board
    private FindMatches findMatches; //Reference to MatchFinder
    private HintManager hintManager;
    private EndGameManager endGameManager;
    private GameData gameData;

    // Initialisation
    void Start()
    {
        board = GameObject.FindWithTag("Board").GetComponent<BoardController>(); //Faster reference to the board, better than find object as tags are more scarse
                                                                                 // board           = FindObjectOfType<BoardController>(); //Creates a reference of the board
        findMatches = FindObjectOfType<FindMatches>();
        hintManager = FindObjectOfType<HintManager>();
        endGameManager = FindObjectOfType<EndGameManager>();

        isBombColumn = false; //Makes sure no powerups are present on startup
        isBombRow = false;
        isColourBomb = false;
        isAreaBomb = false;

        gameData = FindObjectOfType<GameData>(); //find our game data

        /* 
        targetX = (int)transform.position.x; //Saves position of X Value
        targetY = (int)transform.position.y;
        
        
        column = targetX; //Assigns it to the column
        row = targetY;
        previousColumn = column;
        previousRow = row; */
    }


    private void OnMouseOver()    //This is for testing and Debug only, Keycodes will turn elements into a row or column bomb. Elements can be both and execute accordingly
    {
        if (Input.GetKeyDown(KeyCode.V)) //creates Vertical Row Bomb
        {
            TransmuteColumn();
        }

        if (Input.GetKeyDown(KeyCode.H)) //creates Horizontal column bomb
        {
            TransmuteRow();

        }

        if (Input.GetKeyDown(KeyCode.C)) //creates Colour Bomb
        {
            isColourBomb = true;
            GameObject marker = Instantiate(colourBomb, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;

        }

        if (Input.GetKeyDown(KeyCode.A)) //creates Area Bomb
        {
            TransmuteArea();

        }

    } //Debug debug debug debug end

    //Implementing This debugfeature into the game as ITEMS

    private void TransmuteColumn()
    {
        isBombColumn = true;
        GameObject marker = Instantiate(columnArrow, transform.position, Quaternion.identity); //Marker is an overlay that indicates what kind of powerup is on a piece
        marker.transform.parent = this.transform;
    }

    private void TransmuteRow()
    {
        isBombRow = true;
        GameObject marker = Instantiate(rowArrow, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }

    private void TransmuteArea()
    {
        isAreaBomb = true;
        GameObject marker = Instantiate(areaBomb, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }


    void Update()
    {
        /* Changes colour of matched objects
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f); //Lowers the opacity of sprites that have been matched
        }
        */

        targetX = column;
        targetY = row;
        //Absolute value is always the same, nomatter whichever way you subtract i.e: 10-3=7  ,  3-10 =7 (absolute value), real value is -7. Absolute is never negative

        if (Mathf.Abs(targetX - transform.position.x) > .1) // (Horizontal)
        {
            //Move towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, swapSpeed); //Animates pieces to move

            if (board.allPieces[column, row] != this.gameObject) //if SELECTED coloumn/row isnt the same as this piece
            {
                board.allPieces[column, row] = this.gameObject; //then make sure it is
                findMatches.FindAllMatches();
            }

        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1) //(Vertical)
        {
            //Move towards the target 
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, swapSpeed); //Animates

            if (board.allPieces[column, row] != this.gameObject) //if SELECTED coloumn/row isnt the same as this piece
            {
                board.allPieces[column, row] = this.gameObject; //then make sure it is
                findMatches.FindAllMatches();
            }

        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }

    }

    //User Input
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move) //checks if the player can move pieces
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(firstTouchPosition);
        }

        //Then destroy the hint if there was one
        if (hintManager != null) //If we have a hint manager
        {
            hintManager.DestroyHint(); //Destroy the hint shown when any piece is touched
        }

        


    }
    private void OnMouseUp()
    {
        if (board.currentState == GameState.move) //checks if the player can move pieces
        {
            endTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

        if (board.powerupPiece != 0 && board.currentState == GameState.pause)
        {
            if (gameData.saveData.bones[7] > 0)
            {
                switch (board.powerupPiece)
                {
                    case 3: TransmuteRow(); break;
                    case 2: TransmuteColumn(); break;
                    case 1: TransmuteArea(); break;

                }

                FindObjectOfType<FossilCountScript>().DecreaseFossilCount();
            }

            FindObjectOfType<ItemUI>().DeactivateItem();


        }
    }


    //Math
    // Converting position start-end into an angle, that can then be used to determine which direction was swiped
    void CalculateAngle()
    { //if our difference is greater than our swipe resist then execute
        if (Mathf.Abs(endTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(endTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = GameState.wait; //Stops player from moving pieces 


            // math breakdown:   Arctan( y Difference / x Difference) = Angle (direction) of swipe
            swipeAngle = Mathf.Atan2(endTouchPosition.y - firstTouchPosition.y, endTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            //Arctan returns a value in radians so you have to times by 180 then divide by pi
            Debug.Log(swipeAngle);
            MovePieces();
            board.currentGamePiece = this; //Saves the users last selection to the board
        }
        else
        {
            board.currentState = GameState.move;
        }
    }


    //Movement
    //We want to swap our piece in one of four directions, we do this by increasing or decreasing either the X or Y value, by 1 or -1.
    void MovePieces()  //  Key:  Right [x+1 ,y]   Up [x, y+1]   Left [x-1 ,y]   Down [x, y-1]
    {
        Debug.Log("Okay so we at move pieces rn");

        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) //Right Swipe + condition to ensure no OOB swaps
        {
            MovePiecesNew(Vector2.right);
            board.lastSwipeDirection = 'r';
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) //Up Swipe
        {
            MovePiecesNew(Vector2.up);
            board.lastSwipeDirection = 'u';
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) //Left Swipe, has to be OR because cant be bigger and less at the same time. Unity mechanics
        {
            MovePiecesNew(Vector2.left);
            board.lastSwipeDirection = 'l';
            /* Old way of moving pieces
                SetPreviousPosition();
                otherPiece = board.allPieces[column - 1, row];
                otherPiece.GetComponent<GamePiece>().column += 1; //column = column  -1; Could use -- here instead;
                column -= 1;*/
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) //Down Swipe
        {
            MovePiecesNew(Vector2.down);
            board.lastSwipeDirection = 'd';
        }
        else
        {
            board.currentState = GameState.move;
        }


    }

    void MovePiecesNew(Vector2 direction) //Actually moves the pieces
    {

        otherPiece = board.allPieces[column + (int)direction.x, row + (int)direction.y];                                                                       // (int) recasts the float to an interger so it can be used in the array
        SetPreviousPosition();

        if (board.lockedTiles[column, row] == null && board.lockedTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherPiece != null) //If otherPiece exists
            {
                otherPiece.GetComponent<GamePiece>().column += -1 * (int)direction.x;
                otherPiece.GetComponent<GamePiece>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCondition());
            }
            else //If it doesn't exist
            {
                board.currentState = GameState.move;
            }
        }
        else //If it doesn't exist
        {
            board.currentState = GameState.move;
        }

    }

    void SetPreviousPosition()
    {
        previousColumn = column;
        previousRow = row;
    }


    void FindMatches() //old Way of finding matches
    { //If our column is greater than 0 
        /*
        if (column > 0 && column < board.width - 1)  //Checks pieces to the immediate left and right of this piece
        {
            GameObject leftPiece1 = board.allPieces[column - 1, row];
            GameObject rightPiece1 = board.allPieces[column + 1, row];

            if (leftPiece1 != null && rightPiece1 != null) //ensures we dont call function if we're at the edge
            {           

                if (leftPiece1.tag == this.gameObject.tag && rightPiece1.tag == this.gameObject.tag)
                {
                leftPiece1.GetComponent<GamePiece>().isMatched = true; //Left neighbour is matched
                rightPiece1.GetComponent<GamePiece>().isMatched = true;//so is right
                isMatched = true; //so is this piece
                }
            }
        }

        if (row > 0 && row < board.height - 1) //Checks pieces to the immediate up and down of this piece
        {
            GameObject upPiece1 = board.allPieces[column, row+1];
            GameObject downPiece1 = board.allPieces[column,  row-1];

            if (upPiece1 != null & downPiece1 != null)
            {
                if (upPiece1.tag == this.gameObject.tag && downPiece1.tag == this.gameObject.tag)
                {
                    upPiece1.GetComponent<GamePiece>().isMatched = true; //Left neighbour is matched
                    downPiece1.GetComponent<GamePiece>().isMatched = true;//so is right
                    isMatched = true; //so is this piece
                }
            }
        } */
    }


    public IEnumerator CheckMoveCondition()
    {
        yield return new WaitForSeconds(pauseTime); //Stops the user from spamming swipe inputs

        if (otherPiece != null) //if we have selected an other piece, i.e if we swaped
        {
            CheckForColourBomb(); //Quickly check if its one is a colour bomb


            if (!isMatched && !otherPiece.GetComponent<GamePiece>().isMatched)   //If we are not matched and our other piece isnt matched
            {
                otherPiece.GetComponent<GamePiece>().row = row; //swap other piece back to where we are now
                otherPiece.GetComponent<GamePiece>().column = column;
                row = previousRow; //our piece goes back to were it was
                column = previousColumn;
                yield return new WaitForSeconds(pauseTime);

                board.currentGamePiece = null; //removes the users last selection to the board
                otherPiece = null; //as well as the other piece
                board.currentState = GameState.move; // allow the player to move pieces again

            }
            else //if we did find a match
            {

                board.DestroyMatches(); //run the destroyer in BoardController.cs   
                EndgameUpdate(); //Update the gameManger
            }
            // otherPiece = null;           
        }
        else //if we swiped along the edge and didn't get an otherPiece
        {
            board.currentState = GameState.move; //Then change it back to move 
        }
    }

    //Colour Bomb checker

    private void CheckForColourBomb()
    {
        if (isColourBomb)
        {
            //This piece is a color bomb and the other piece is the colour to destroy
            findMatches.MatchPiecesofColour(otherPiece.tag); //Activate the colour bomb and destroy all colours the same as the piece we are swapping with
            isMatched = true; //Just in case
        }

        if (otherPiece.GetComponent<GamePiece>().isColourBomb) //In the unlikely event that both swapped pieces are colour bombs, or the other piece is a colour bomb
        {
            findMatches.MatchPiecesofColour(this.gameObject.tag); //Then destroy all colours of this piece aswell
            otherPiece.GetComponent<GamePiece>().isMatched = true;
        }
    }

    private void EndgameUpdate()
    {
        if (endGameManager != null)
        {
            if (endGameManager.requirements.gameType == GameType.Moves) //If we have a gamemanager witth a gametype moves 
            {
                endGameManager.DecreaseCounterValue();
            }
        }
    }


    //Bomb Factory

    public void MakeRowBomb(GamePiece pieceToBecomeBomb)
    {
        if (!pieceToBecomeBomb.isColourBomb && !pieceToBecomeBomb.isBombColumn && !pieceToBecomeBomb.isAreaBomb) //If we are not already a colour or column or area bomb
        {
            pieceToBecomeBomb.isBombRow = true; //Become Bomb
            GameObject marker = Instantiate(rowArrow, transform.position, Quaternion.identity);
            marker.transform.parent = pieceToBecomeBomb.transform; //Make Marker a child of the piece
        }
    }

    public void MakeColumnBomb(GamePiece pieceToBecomeBomb)
    {
        if (!pieceToBecomeBomb.isColourBomb && !pieceToBecomeBomb.isBombRow && !pieceToBecomeBomb.isAreaBomb) //If we are not already a colour or row or area bomb
        {
            pieceToBecomeBomb.isBombColumn = true; //Become Bomb
            GameObject marker = Instantiate(columnArrow, transform.position, Quaternion.identity);
            marker.transform.parent = pieceToBecomeBomb.transform; //Make Marker a child of the piece
        }
    }

    public void MakeColourBomb(GamePiece pieceToBecomeBomb)
    {
        if (!pieceToBecomeBomb.isBombColumn && !pieceToBecomeBomb.isAreaBomb && !pieceToBecomeBomb.isBombRow) //If we are not already a row/col bomb or area
        {
            pieceToBecomeBomb.isColourBomb = true; //become a colour bomb
            GameObject marker = Instantiate(colourBomb, transform.position, Quaternion.identity);
            marker.transform.parent = pieceToBecomeBomb.transform; //Make Marker a child of the piece

            //this.gameObject.tag == "Coluorless"; stops the rainbow piece staying its colour
        }
    }

    public void MakeAreaBomb(GamePiece pieceToBecomeBomb)
    {
        if (!pieceToBecomeBomb.isColourBomb && !pieceToBecomeBomb.isBombColumn && !pieceToBecomeBomb.isBombRow) //If we are not already a colour bomb / col/row bomb
        {
            pieceToBecomeBomb.isAreaBomb = true;
            GameObject marker = Instantiate(areaBomb, transform.position, Quaternion.identity);
            marker.transform.parent = pieceToBecomeBomb.transform;

        }
    }


    public void OnDestroy()
    {

        if (isMatched )
        {
            int boneId = 99; //default  

            if (gameData != null)

            {
                switch (gameObject.tag.ToString())
                {
                    case "White": boneId = 0; break;
                    case "Red": boneId = 1; break;
                    case "Blue": boneId = 2; break;
                    case "Green": boneId = 3; break;
                    case "Purple": boneId = 4; break;
                    case "Pink": boneId = 5; break;
                    case "Yellow": boneId = 6; break;
                }

                if (boneId != 99) //if our id is not the default
                {
                    if (isColourBomb)
                    {
                        //The placeholder for yellow in the particle library is the rainbow skull
                        FindObjectOfType<ParticleManager>().SpawnCollectParticle(transform, "Yellow");
                    }
                    else
                    {
                        
                        FindObjectOfType<ParticleManager>().SpawnCollectParticle(transform, this.tag.ToString());
                        //Sends information about this piece to the particle manager
                        //which will thens spawn a particle at this position
                        //Based on the tag of this piece
                    }


                    //Do Animation

                    gameData.saveData.bones[boneId]++; //increase the bone count
                }
            }
        }

    }

}

