using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{

    private BoardController board;
    public List<GameObject> currentMatches = new List<GameObject>(); //initialised list to store matches, 3match, 4match or 5match



    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>();
    }

    public void FindAllMatches() //Fetches the coroutine
    {
        StartCoroutine(FindAllMatchesCo());
    }
    

    // Checking a Match

    /// 
    /// Edges of the board are [x = 0--> Board Width-1] [y = 0 ---> Board Height-1] (Minus 1 because the array starts at 0
    /// [col-1, row-1] [col, row+1]   [col+1, row+1]
    /// [col-1, row]   [this piece]   [col+1, row]
    /// [col-1, row-1] [col, row-1]   [col+1, row-1]
    /// 
    /// All pieces are tagged in unity with their colour, so we check to see if neighbouring pieces are of the same colour
    

    private IEnumerator FindAllMatchesCo() //Scans the board array, checks if pieces adjacent to eachother are matched, if so sets their isMatched variable to true
    {
        //yield return new WaitForSeconds(.2f); 
        yield return null; //waits for the end of frame before continuing
        for (int w = 0; w < board.width; w++) //Scans the board
        {
            for (int h = 0; h < board.height; h++) 
            {
                GameObject currentPiece = board.allPieces[w, h];

                if (currentPiece != null) //Check to see if the dot we are looking at exists
                {
                    GamePiece currentPieceComponent = currentPiece.GetComponent<GamePiece>(); //Caches the get component 

                    //Horizontal Match 3 Check                     
                    if (w > 0 && w < board.width - 1) //check if we are between the second column and second to last column
                    {
                        GameObject leftPiece = board.allPieces[w - 1, h]; //cache the piece left of this one
                        GameObject rightPiece = board.allPieces[w + 1, h]; //cache the piece to the right
                      
                        if (leftPiece != null && rightPiece != null) //Check they both exist
                        {
                            GamePiece leftPieceComponent = leftPiece.GetComponent<GamePiece>(); //Caches get components if they exist
                            GamePiece rightPieceComponent = rightPiece.GetComponent<GamePiece>();

                            if (leftPiece.tag == currentPiece.tag && rightPiece.tag == currentPiece.tag) //then check to see if their colour tag is the same as this one
                            {  //If both tags match this piece then this is a Horizontal match of 3

                                IsRowBomb(leftPieceComponent, currentPieceComponent, rightPieceComponent); //Checks 3 parsed pieces for row bombs, 
                                IsColumnBomb(leftPieceComponent, currentPieceComponent, rightPieceComponent);  //if they're a row bomb, entire row is added to matched list
                                IsAreaBomb(leftPieceComponent, currentPieceComponent, rightPieceComponent); //Same for Columns and Area bombs


                                MatchThesePieces(leftPiece, currentPiece, rightPiece); //Checks each match to ensure they're not already in list, then adds them to matches

                            }
                        }
                    }

                    //Vertical Match 3 check
                    if (h > 0 && h < board.height - 1) //check if we are between the second row and second to last row
                    {
                        GameObject upPiece = board.allPieces[w, h + 1]; //cache the piece above of this one
                        GameObject downPiece = board.allPieces[w, h - 1]; //cache the piece below
                    
                        if (upPiece != null && downPiece != null) //Check they both exist
                        {
                            GamePiece upPieceComponent = upPiece.GetComponent<GamePiece>(); //Caches components if they do exist
                            GamePiece downPieceComponent = downPiece.GetComponent<GamePiece>();


                            if (upPiece.tag == currentPiece.tag && downPiece.tag == currentPiece.tag) //then check to see if their colour tag is the same as this one
                            {   //If both tags match this piece then this is a Vertical match of 3


                               IsColumnBomb(upPieceComponent, currentPieceComponent, downPieceComponent); //Checks parsed pieces for column bombs
                               IsRowBomb(upPieceComponent,currentPieceComponent,downPieceComponent); //Checks parsed pieces for row bombs
                               IsAreaBomb(upPieceComponent, currentPieceComponent, downPieceComponent);

                               MatchThesePieces(upPiece, currentPiece, downPiece);

                            }
                        }
                    }                                                                                               
                }            
            }
        } //End of Scanning
    }

    //Refactoring
  
    private void MatchThesePieces(GameObject piece1, GameObject piece2, GameObject piece3) //Adds 3 pieces to list
    {
        AddMatchToList(piece1);
        AddMatchToList(piece2);
        AddMatchToList(piece3);
    }

    private void AddMatchToList(GameObject listPiece) //Checks to see if we've added this piece to our list, if its not present, adds it to the list
    {
        if (!currentMatches.Contains(listPiece)) //if our list does NOT have this piece 
        {
            currentMatches.Add(listPiece); //then add the piece
        }

        listPiece.GetComponent<GamePiece>().isMatched = true;
    }
    

    // Checks for Row and Column Bombs, Area Bombs, 
    private void IsRowBomb(GamePiece piece1, GamePiece piece2, GamePiece piece3) //Checks 3 pieces, if they are row bombs, adds their rows to current matcheslist
    {
        if (piece1.isBombRow) //If any of the matched pieces are row bombs
        {
            currentMatches.Union(GetRowPieces(piece1.row)); //Add whole row to list
            board.BombRow(piece1.row); //Deals damage to any concrete blocks in the row
        }

        if (piece2.isBombRow)
        {
            currentMatches.Union(GetRowPieces(piece2.row)); //Add row to list
            board.BombRow(piece2.row);
        }

        if (piece3.isBombRow)
        {
            currentMatches.Union(GetRowPieces(piece3.row)); //Union joins two lists together, as opposed to adding each individual elements 
            board.BombRow(piece3.row);
        }
    }

    private void IsColumnBomb(GamePiece piece1, GamePiece piece2, GamePiece piece3) //Checks 3 pieces, if they are Column Bombs, adds their columns to list
    {
        if (piece1.isBombColumn) //If any of the matched pieces are column bombs
        {
            currentMatches.Union(GetColumnPieces(piece1.column)); //Add whole column to list
            board.BombColumn(piece1.column);
        }

        if (piece2.isBombColumn)
        {
            currentMatches.Union(GetColumnPieces(piece2.column)); //Add column to list
            board.BombColumn(piece2.column);
        }

        if (piece3.isBombColumn)
        {
            currentMatches.Union(GetColumnPieces(piece3.column)); //Union joins two lists together, as opposed to adding each individual elements 
            board.BombColumn(piece3.column);
        }
    }

    private void IsAreaBomb(GamePiece piece1, GamePiece piece2, GamePiece piece3) //Checks parsed values to see if they are area bombs
    {
        if (piece1.isAreaBomb) //If any of the matched pieces are Area bombs
        {
            currentMatches.Union(GetAdjacentPieces(piece1.column, piece1.row)); //add this piece and all adjacent pieces to matched list
        }

        if (piece2.isAreaBomb)
        {
            currentMatches.Union(GetAdjacentPieces(piece2.column, piece2.row)); //if the parsed piece is an area bomb
        }

        if (piece3.isAreaBomb)
        {
            currentMatches.Union(GetAdjacentPieces(piece3.column, piece3.row)); //Same for all 3 parsed pieces
        }        
    }


    
    //Mark all Pieces in a Cow/Column to matched, does not match existing colour bombs so they fall to the bottom
    public List<GameObject> GetColumnPieces(int column) //Grabs all the pieces in a column and sets them to matched
    {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < board.height; i++) //loops through column
        {
        if (board.allPieces[column, i] != null && !board.allPieces[column, i].GetComponent<GamePiece>().isColourBomb) //if a piece exists and is not a colour bomb
            {

                GamePiece piece = board.allPieces[column, i].GetComponent<GamePiece>(); //reference the gamepiece script attached
                if(piece.isBombRow)  //Check if this piece is a row bomb
                {
                    pieces.Union(GetRowPieces(i)).ToList(); //if a row bomb, add that row to the list
                }

                pieces.Add(board.allPieces[column, i]);
                piece.isMatched = true;
            }
        }
        return pieces;
    }

    public List<GameObject> GetRowPieces(int row) //Grabs all the pieces in a row and sets them to matched
    {
        List<GameObject> pieces = new List<GameObject>();
        for (int j = 0; j < board.width; j++) //loops through column
        {
            if (board.allPieces[j, row] != null && !board.allPieces[j, row].GetComponent<GamePiece>().isColourBomb) //if a piece exists and is not colour bomb
            {
                GamePiece piece = board.allPieces[j, row].GetComponent<GamePiece>(); //reference the gamepiece script attached
                if (piece.isBombColumn)  //Check if this piece is a row bomb
                {
                    pieces.Union(GetColumnPieces(j)).ToList(); //if a row bomb, add that row to the list
                }

                pieces.Add(board.allPieces[j, row]); //adds the piece to the list
                piece.isMatched = true;
            }
        }
        return pieces;
    }

    //Power Ups / bombs

    public void CheckBombs(MatchType matchType)
    {
        
        if(board.currentGamePiece != null) //Did the player move something?
        {
            if (board.currentGamePiece.isMatched && board.currentGamePiece.tag == matchType.colour)  //Is the piece they moved matched already?
            {
                board.currentGamePiece.isMatched = false;     //unmatch it
                
                //Decide what kind of bomb to make based on swuipe direction (swipe input version)


                /*      if    ((board.currentGamePiece.swipeAngle > -45 && board.currentGamePiece.swipeAngle <= 45) ||//If angle is this
                            (board.currentGamePiece.swipeAngle < -135 || board.currentGamePiece.swipeAngle >= 135))//Or this
                      { 
                          board.currentGamePiece.MakeRowBomb(board.currentGamePiece); //make row
                      }
                      else
                      {
                          board.currentGamePiece.MakeColumnBomb(board.currentGamePiece); //make column
                      }
                */


                //Switch statement decides what bomb ot make based on direction the last piece was swiped in (Cached char version)

                switch (board.lastSwipeDirection) //Makes a bomb at the last selected swipe, based on the direction of the swipe input
                {
                    case 'r': //horizontal bomb
                    case 'l': board.currentGamePiece.MakeRowBomb(board.currentGamePiece); break;

                    case 'u': //Vertical bomb
                    case 'd': board.currentGamePiece.MakeColumnBomb(board.currentGamePiece); break;
                }



            }
            else if (board.currentGamePiece.otherPiece != null) //Is there an other piece to swap with
            {
                GamePiece otherPiece = board.currentGamePiece.otherPiece.GetComponent<GamePiece>();
                if (otherPiece.isMatched && otherPiece.tag == matchType.colour) //is the other piece matched
                {
                    otherPiece.isMatched = false; //Unmatch it, and decide what bomb to make

                    /*      if ((board.currentGamePiece.swipeAngle > -45 && board.currentGamePiece.swipeAngle <= 45) ||//If angle is this
                                   (board.currentGamePiece.swipeAngle < -135 || board.currentGamePiece.swipeAngle >= 135))//Or this
                          {
                              otherPiece.MakeRowBomb(otherPiece);
                          }
                          else
                          {
                              otherPiece.MakeColumnBomb(otherPiece);
                          }*/


                    switch (board.lastSwipeDirection) //Makes a bomb at the last selected swipe, based on the direction of the swipe input
                    {
                        case 'r':  //Right or Left swipes create a horizontal bomb
                        case 'l': otherPiece.MakeRowBomb(otherPiece); break;

                        case 'u':  //Up or Down swipes make a Vertical bomb
                        case 'd': otherPiece.MakeColumnBomb(otherPiece); break;
                    }

                }
            }

        }
    }

    //Colour Bomb

    public void MatchPiecesofColour(string colour) //Matches ALL the pieces of parsed colour, (sets all isMatched bool to true)
    {
        for (int i = 0; i < board.width; i++) //Scans the board
        {
            for (int j = 0; j < board.height; j++)
            {
                //Check if that piece exists
                if (board.allPieces[i, j] != null)
                {
                    //Check  the tag on the piece
                    if (board.allPieces[i, j].tag == colour) //If its the same as our parameter colour
                    {
                        board.allPieces[i, j].GetComponent<GamePiece>().isMatched = true;
                    }

                }
            }
        }

    }    

    //Area Bomb

    List <GameObject> GetAdjacentPieces(int col, int row) //Scans through pieces in the immediate vacinity, 1 column left and right, one row above and below
    {
        List<GameObject> pieces = new List<GameObject>();

        for (int i = col-1; i <= col+1; i++) //Scans through 3 columns, left, current, right              [col-1,row+1]    [col,row+1]     [col+1, row+1]  ++
        {                                                                                             //  [col-1,row]  --  [col, row]  ++  [col+1, row]     
            for (int j = row-1; j <= row+1; j++) //Scans through 3 rows, above, current, below          [col-1, row-1]   [col, row-1]    [col+1,row-1]   --
            {
                //Check if the piece is inside the board
                if (i >= 0 && i <board.width && j >= 0 && j < board.height) //Checks that the piece isnt outside the board, not between [0,0][7,7] ect..
                {
                    if (board.allPieces[i, j] != null) //check to see the piece exists, and is not a blank tile ect
                    {

                        pieces.Add(board.allPieces[i, j]);
                        board.allPieces[i, j].GetComponent<GamePiece>().isMatched = true; //Grabs the component and changes it to matched
                    }
                }
            }
        }
        return pieces;
    }


}
