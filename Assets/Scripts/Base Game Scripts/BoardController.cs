using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState //Enums are essentailly booleans with extra values, meaning they can only be one of the specified arguments
{
    wait, //pauses player control while the board clears and refills
    move, //The player can move
    win, //Victory condition
    lose, //Defeat Condition
    pause //Game is paused, player can't move and timer stopped ect
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string colour;
}


public enum TileProperties
{
    Breakable,
    Blank,
    Lock,
    Obsidian,
    Ruby,
    Emerald,
    Diamond,
    Normal
}

[System.Serializable]
public class TileType
{   
    public int x, y;
    public TileProperties tileProperties;
}


public class BoardController : MonoBehaviour
{
    [Header("Scriptable Object")]
    public World world;
    public int level;

    [Header("Player State")]
    public GameState currentState = GameState.move;
    public GamePiece currentGamePiece;
    public char lastSwipeDirection;
    public int powerupPiece = 0;

    [Header("Match Spy")]
    public MatchType matchType;

    [Header("Board Config")]
    public int offset;
    public int width, height; //CHANGED BY SCRIPTABLE OBJECT
   


    [Header("Layout")]
    public TileType[] boardLayout;  //CBSO      An array of X and Y coordinates with an Enum attached to describe the tiles properties


    [Header("Tiles")]                                           //Types of objects that appear behind pieces such as breakable tiles, or empty tiles
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject obsidianTilePrefab, rubyTilePrefab, emeraldTilePrefab, diamondTilePrefab; //Our tiles that cant be moved or swapped with only broken by matches made around it

    private BackgroundTiler[,] obsidianTiles;
    private BackgroundTiler[,] breakableTiles;                  //2D array used to store the locations [x,y] of any breakable tiles
    public BackgroundTiler[,] lockedTiles;
    private bool[,] blankSpaces;                                //2D array used to store if a tile should be blank (true) or not (false)

    [Header("Spawn Elements")]                                  //Actual pieces the user can swipe and match with
    public GameObject[] colours; //CBSO (PIECES)                //array to store the colours to generate     
    public GameObject[,] allPieces;                             //A 2D array to store all the "pieces", game objects
    

    [Header("Visual Effects")]
    public GameObject destroyVFX;                               //Particle fx


    [Header("Points and Scores")]
    public int basePieceValue = 20; 
    private int streakValue = 1;
    public int[] scoreGoals; //CBSO                             //Ammount of points user needs to achieve before moving on

    //Objects that need to be found on startup
    private FindMatches findMatches;   //Find matches script
    private ScoreManager scoreManager; //Score Manager script   
    private SoundManager soundManager;
    private GoalManager goalManager;

    [Header("Misc")]
    public float refillDelay = .6f;
    //Magic Number Removal Squad (Hardcoded variables I have refactored)
    private float particleLifetime = .6f;
 
    private float pauseTime = .4f;



    // INITIALISATION   // Variables  camelCase, Methods PascalCase

    void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }

        if (world != null) //Check we have a Scriptable object
        {
            if (level < world.levels.Length) //If we are in range
            {


                if (world.levels[level] != null) //Check it has a level
                {
                    width = world.levels[level].Width; //Loads board dimensions
                    height = world.levels[level].height;
                    colours = world.levels[level].pieces; //Loads the pieces to spawn in
                    scoreGoals = world.levels[level].scoreGoals; //Loads goals
                    boardLayout = world.levels[level].boardLayout; //Loads the breakable tiles / blockers
                }
            }
        }
    }

    void Start()
    {
        blankSpaces = new bool[width, height]; //Creates two dimensional array with the set WIDTH and HEIGHT variables, meaning each tile has a true or false value
        allPieces = new GameObject[width, height]; //Two dimensional array to store GameObjects
        breakableTiles = new BackgroundTiler[width, height]; //Two dimensional array to store Tile Information
        lockedTiles = new BackgroundTiler[width, height];
        obsidianTiles = new BackgroundTiler[width, height];

        findMatches = FindObjectOfType<FindMatches>();
        scoreManager = FindObjectOfType<ScoreManager>(); //One score manager only and this will work
        soundManager = FindObjectOfType<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
        SetUp();
        currentState = GameState.pause;
        powerupPiece = 0;
    }

    // Board Setup 
    private void SetUp() //Creates a board of tiles with specified width and height, arranges them 1 unit appart and names them with their position
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockedTiles();
        GenerateObsidianTiles();

        for (int w = 0; w < width; w++) //loops through width no of times (this is left to right, 1 -- X)
        {
            for (int h = 0; h < height; h++)
            {
                //Quickly check if the board needs to generate any blank spaces
                if (!blankSpaces[w, h]) //&& !obsidianTiles[w, h]) //If false, and we don't need to generate a blank space
                {
                    //Then continue to create a piece


                    Vector2 tempPosition = new Vector2(w, h + offset); //Temporary local vector to assign h + w, The offset works to animate new pieces
                    Vector2 tilePosition = new Vector2(w, h);

                    GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity); //creates a board tile at position 0,0 in unity, then 0,1, then 0,2.....
                    tile.transform.parent = this.transform; //Attaches it to the board object 
                    tile.name = "Tile [" + w + "," + h + "]"; // changes name to look like: Tile [w,h]
                                                              //tile.name = "Tile [" + tile.transform.position.x.ToString() + "," + tile.transform.position.y.ToString() + "]"; // Names based on Position rather than for loop

                    if (!obsidianTiles[w,h]){ 
                    // Creating our pieces

                    int colourToUse = Random.Range(0, colours.Length); // creates a random number between, 
                                                                       // 0 and array max to choose a colour (no in array)
                    int maxIterations = 0; //ensures the while does not loop infinitely
                    while (MatchesAtGen(w, h, colours[colourToUse]) && maxIterations < 50) //checks to see if our current selected colour, 
                    {                                                                               //will create a match 3 
                        colourToUse = Random.Range(0, colours.Length); //While will loop if the method called returns a true statement, 
                        maxIterations++;                                //will continue to do so if true again ect..
                            Debug.Log(maxIterations);
                    }
                    maxIterations = 0; //resets max for the next loop

                    GameObject dot = Instantiate(colours[colourToUse], tempPosition, Quaternion.identity); //Creates an entry into the array, with a random range
                    dot.GetComponent<GamePiece>().row = h;
                    dot.GetComponent<GamePiece>().column = w;
                    dot.transform.parent = this.transform;
                    dot.name = "Skull  (" + w + "," + h + ")"; // changes name to look like: Tile [w,h]
                    allPieces[w, h] = dot;
                    }
                }
            }
        }

    }

    //Set true/false values to tiles that don't want pieces to spawn in them. 
    private void GenerateLockedTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++) //look at all the tiles in the layout
        {
            if (boardLayout[i].tileProperties == TileProperties.Lock) //If the tile properties == lock tile
            {
                //Create a locked tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); //Create a position from current place in loop 
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity); //Creates the breakable tile
                lockedTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTiler>();  //This stores the backgroundTile inside the breakable tiles array, at this location
            }                                                            //Storing them here once generated allows us to check the array to see exactly where any background tiles are present.
        }


    }

    private void GenerateObsidianTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++) //look at all the tiles in the layout
        {
            if (boardLayout[i].tileProperties == TileProperties.Obsidian) //If the tile properties == obsidian tile
            {
                //Create a locked tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); //Create a position from current place in loop 
                GameObject tile = Instantiate(obsidianTilePrefab, tempPosition, Quaternion.identity); //Creates the breakable tile
                obsidianTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTiler>();  //This stores the backgroundTile inside the breakable tiles array, at this location
            }                                                            //Storing them here once generated allows us to check the array to see exactly where any background tiles are present.

            if (boardLayout[i].tileProperties == TileProperties.Ruby) //If the tile properties == obsidian tile
            {
                //Create a locked tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); //Create a position from current place in loop 
                GameObject tile = Instantiate(rubyTilePrefab, tempPosition, Quaternion.identity); //Creates the breakable tile
                obsidianTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTiler>();  //This stores the backgroundTile inside the breakable tiles array, at this location
            }

            if (boardLayout[i].tileProperties == TileProperties.Emerald) //If the tile properties == obsidian tile
            {
                //Create a locked tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); //Create a position from current place in loop 
                GameObject tile = Instantiate(emeraldTilePrefab, tempPosition, Quaternion.identity); //Creates the breakable tile
                obsidianTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTiler>();  //This stores the backgroundTile inside the breakable tiles array, at this location
            }

            if (boardLayout[i].tileProperties == TileProperties.Diamond) //If the tile properties == obsidian tile
            {
                //Create a locked tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); //Create a position from current place in loop 
                GameObject tile = Instantiate(diamondTilePrefab, tempPosition, Quaternion.identity); //Creates the breakable tile
                obsidianTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTiler>();  //This stores the backgroundTile inside the breakable tiles array, at this location
            }

            //Check for Ruby, Emerald and Diamond Tiles
        }


    }


    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++) //Loops through the length of the array
        {
            if (boardLayout[i].tileProperties == TileProperties.Blank) //If the tile properties == blank
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true; //Then set a true value to the space at the X and Y attached to the tileType class board layout is.
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++) //look at all the tiles in the layout
        {
            if (boardLayout[i].tileProperties == TileProperties.Breakable) //If the tile properties == breakable
            {
                //Create a breakable tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); //Create a position from current place in loop 
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity); //Creates the breakable tile
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTiler>();  //This stores the backgroundTile inside the breakable tiles array, at this location
            }                                                            //Storing them here once generated allows us to check the array to see exactly where any background tiles are present.
        }
    }

        //Generate clean, match free board at start

        /// Because of the way the board generates, [0,0] [0,1] [0,2]..... [1,0] [1,1] ect
        /// Rows to the right and above may not exist when we run our checks for matches
        /// So to get aroudn this we need to check for pieces Down and Left when pieces spawn
        /// We also need to check left2 (left-left)  and down2 (down-down) to make sure there isnt 3 in a row
        /// 

        private bool MatchesAtGen(int column, int row, GameObject piece)//Checks to ensure we don't generate any matches (3 in a row) at the start || Is also called after a deadlock to reshuffle the board without any matches
        {
        if (column > 1 && row > 1)
        {
            if (allPieces[column - 1, row] != null && allPieces[column - 2, row] != null) //Null catchers to stop unity throwing an error
            { 
                if (allPieces[column - 1, row].tag == piece.tag && allPieces[column - 2, row].tag == piece.tag) //checks if left and left2 are the same as piece parsed in
                {
                    return true; //They match
                }
            }

            if(allPieces[column, row-1] != null && allPieces[column, row-2] != null)
            {
                if (allPieces[column, row - 1].tag == piece.tag && allPieces[column, row - 2].tag == piece.tag) //checks if down and down2 are the same as piece parsed in
                {
                    return true; //They match
                }
            }

        }
        else if (column <= 1 || row <= 1) //if our column or row is at the edge, or one away from the edge
        {
            if (row > 1)
            {
                if (allPieces[column, row-1] != null && allPieces[column, row-2] != null)
                {
                    if (allPieces[column, row - 1].tag == piece.tag && allPieces[column, row - 2].tag == piece.tag) //Checks the elements beneath 
                        return true;
                }
            }

            if (column > 1)
            {
                if (allPieces[column - 1, row] != null && allPieces[column - 2, row] != null)
                {
                    if (allPieces[column - 1, row].tag == piece.tag && allPieces[column - 2, row].tag == piece.tag) //Checks the elements to the left 
                        return true;
                }
            }
        }


        return false;
        }




    //Destroying Matches

    public void DestroyMatches() //Function for destroying all matches
    {
        //How many elements are in the matched pieces list from find matches?
        if (findMatches.currentMatches.Count >= 4) //if we have 4 or more matched Elements [THIS IS INNEFICIENT AND WILL GENERATE COLUMN/ROW BOMBS IF YOU USE A RAINBOW TO DESTROY EXACTLY 4 OR 7 PIECES
        {
            CheckToMakeBombs();
        }

        findMatches.currentMatches.Clear(); //Removes the destroyed pieces from the match finder list once entire loop is finished

        for (int i = 0; i < width; i++) //Goes through all the columns
        {
            for (int j = 0; j < height; j++) //All the rows
            {
                if (allPieces[i, j] != null) //Checks to see if piece is valid (present) 
                {
                    
                    DestroyMatchesAt(i, j); //and checks to destroy the match 
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }



    private void DestroyMatchesAt(int column, int row) //Function for actually destroying a pieces in a match
    {
        

        if (allPieces[column, row].GetComponent<GamePiece>().isMatched) //checks if parsed piece is matched
        {

            //Does a tile need to break 
            if(breakableTiles[column, row] != null) //If there IS  breakable tile at this location
            {                
                breakableTiles[column, row].TakeDamage(1);  //Reduce its health by 1  
                
                if(breakableTiles[column,row].hitPoints <= 0 || breakableTiles[column,row] == null) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    breakableTiles[column, row] = null; //Remove it from the array
                }
            }

            if (lockedTiles[column, row] != null) //If there IS  breakable tile at this location
            {
                lockedTiles[column, row].TakeDamage(1);  //Reduce its health by 1  

                if (lockedTiles[column, row].hitPoints <= 0 || lockedTiles[column, row] == null) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    lockedTiles[column, row] = null; //Remove it from the array
                }
            }

            DamageObsidian(column, row);

            if (goalManager != null) //null ref prevention
            {
                goalManager.CompareGoal(allPieces[column, row].tag.ToString());
                goalManager.UpdateGoalText();

            }

            
            if(soundManager!=null)//Check to see if we have a sound manager
            { //This happens per piece, could be changed
                soundManager.PlayRandomDestroyNoise(); //Play a random noise from the destroy array
            }
                                              
            GameObject particle = Instantiate(destroyVFX, allPieces[column, row].transform.position, Quaternion.identity);
            Destroy(particle, particleLifetime);//VFX particles last .6 seconds
            //NEED A WAIT FOR PARTICLES
            Destroy(allPieces[column, row]); //and then destroy it if it is

            //Increase score for destroying pieces
            scoreManager.IncreaseScore(basePieceValue * streakValue); //Increase score based on combo

            allPieces[column, row] = null; //set this set in the array to null
            //elements that moved will need their previous positions reset
        }
    }

    private void DamageObsidian (int column, int row) //damages any concrete piece nearby
    {
        if (column > 0) //Damages the piece to the left, so we must not be at the left edge, therefore the minimum of column+1
        {
            if (obsidianTiles[column -1, row]) 
            {
                obsidianTiles[column - 1, row].TakeDamage(1);
                if (obsidianTiles[column-1, row].hitPoints <= 0 ) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    obsidianTiles[column-1, row] = null; //Remove it from the array
                }
            }
        }

        if (column < width -1) //Damages the piece to the right 
        {
            if (obsidianTiles[column + 1, row])
            {
                obsidianTiles[column + 1, row].TakeDamage(1);
                if (obsidianTiles[column + 1, row].hitPoints <= 0 ) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    obsidianTiles[column + 1, row] = null; //Remove it from the array
                }
            }
        }

        if (row < height-1) //Damages the piece above
        {
            if (obsidianTiles[column, row + 1])
            {
                obsidianTiles[column , row+1].TakeDamage(1);
                if (obsidianTiles[column, row + 1].hitPoints <= 0) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    obsidianTiles[column, row + 1] = null; //Remove it from the array
                }
            }
        }

        if (row > 0) //Damages the piece below
        {
            if (obsidianTiles[column, row-1])
            {
                obsidianTiles[column , row-1].TakeDamage(1);
                if (obsidianTiles[column, row -1].hitPoints <= 0) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    obsidianTiles[column, row -1] = null; //Remove it from the array
                }
            }
        }
    }

    public void BombRow (int row)
    {
        for (int i = 0; i < width; i++) 
        {
            
            if(obsidianTiles[i,row])
            { //if obsidian tile exists, then do damage to it
                obsidianTiles[i, row].TakeDamage(1);

                if (obsidianTiles[i, row].hitPoints <= 0) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    obsidianTiles[i, row] = null; //Remove it from the array
                }
            }
            
        }
    }

    public void BombColumn(int column)
    {
        for (int j = 0; j < height; j++)
        {
            if (obsidianTiles[column, j])
            { //if obsidian tiles exists, then do damage to it
                obsidianTiles[column, j].TakeDamage(1);

                if (obsidianTiles[column, j].hitPoints <= 0) //once reduced, check to see if the tiles hitpoints are less than or equal to 0 OR if the tile has been destroyed
                {
                    obsidianTiles[column, j] = null; //Remove it from the array
                }
            }
        }
       
    }


    private void CheckToMakeBombs()
    {
        //How many objects are in findMatches currentMatches?
        if(findMatches.currentMatches.Count > 3) //Greater than 3 means we could have a special bomb be made
        {
            
            MatchType typeOfMatch = ColumnOrRow(); //returns 0,1,2 or 3 depending on the match it finds
            if (typeOfMatch.type == 1)
            {
                //Make a colour bomb
                if (currentGamePiece != null && currentGamePiece.isMatched && currentGamePiece.tag == typeOfMatch.colour) //and the piece is matched and the piece is of the same colour and if the piece exists
                {
                   
                            currentGamePiece.MakeColourBomb(currentGamePiece); //and make it one
                            currentGamePiece.isMatched = false; //then unmatch it so it stays on the board

                        
                }
                else //Same again if the other piece exits
                {
                    if (currentGamePiece.otherPiece != null)  //if its matched, exists, and is of this colour
                    {
                        GamePiece otherPiece = currentGamePiece.otherPiece.GetComponent<GamePiece>(); //Cache component of other piece
                        if (otherPiece.isMatched && otherPiece.tag == typeOfMatch.colour)
                        {
                        otherPiece.MakeColourBomb(otherPiece); //then make it one
                        otherPiece.isMatched = false; //and unmatch it so it stays on the board
                        }
                            
                    }
                }
                

            }
            else if (typeOfMatch.type == 2)
            {
                //make a area bomb
                if (currentGamePiece != null && currentGamePiece.isMatched && currentGamePiece.tag == typeOfMatch.colour) //if the piece exists
                {

                    currentGamePiece.isMatched = false; //then unmatch it so it stays on the board
                    currentGamePiece.MakeAreaBomb(currentGamePiece); //and make it one

                }
                
                else if (currentGamePiece.otherPiece != null)
                {
                    GamePiece otherPiece = currentGamePiece.otherPiece.GetComponent<GamePiece>(); //Cache component of other piece

                    if (otherPiece.isMatched && otherPiece.tag == typeOfMatch.colour) //if its matched
                    {
                            
                        otherPiece.MakeAreaBomb(otherPiece); //then make it one
                        otherPiece.isMatched = false; //and unmatch it so it stays on the board
                            
                    }
                }                
                

            }
            else if(typeOfMatch.type == 3)
            {

               // if (findMatches.currentMatches != null)
              //  {
                    findMatches.CheckBombs(typeOfMatch); //will run the method to check for bombs and will add column or row bombs only 
               // }
               // else if (findMatches.currentMatches == null)
               // {
               // Method that allows new pieces to fall and create column or row bombs if the rng makes a set of 4

               // }
            }
        }

    }

    private MatchType ColumnOrRow() //Checks the matches to see if there is 5 pieces in the same row/column, and return true or false if so
    {
       //make a copy of the current matches
       List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>; //adds matches as a list of GO's

        matchType.type = 0; //creates a blank match
        matchType.colour = "";

       for (int i = 0; i < matchCopy.Count; i++)   //Cycle through matchCopy and decide if a bomb needs to be made
       {
            GamePiece thisPiece = matchCopy[i].GetComponent<GamePiece>(); //Store the gamepiece
            string colour = matchCopy[i].tag; //saves the string with the name of the tag

            int column = thisPiece.column;
            int row = thisPiece.row;
            int columnMatches = 0;
            int rowMatches = 0;

            //cycle through the rest of the peices and compare

            for (int j = 0; j < matchCopy.Count; j++) //Adds our matches of this tag
            {
                GamePiece nextPiece = matchCopy[j].GetComponent<GamePiece>(); //store the next piece

                if(nextPiece == thisPiece)
                {
                    continue; //Iterate to the next part of the forloop
                }

                if(nextPiece.column == thisPiece.column && nextPiece.tag == colour) //Is the next piece in the same column and has the same tag
                {
                    columnMatches++;
                }

                if (nextPiece.row == thisPiece.row && nextPiece.tag == colour) //Is the next piece in the same row and has the same tag
                {
                    rowMatches++;
                }

            }

            //return 3 if column or row match (priority)
            //return 2 if adjacent
            //return 1 if its a colour bomb
            if(columnMatches == 4 || rowMatches == 4)
            {
                matchType.type = 1;
                matchType.colour = colour;
                return matchType;
            }
            else if(columnMatches == 2 && rowMatches ==2)
            {
                matchType.type = 2;
                matchType.colour = colour;
                return matchType;
            }
            else if(columnMatches == 3 || rowMatches == 3)
            {
                matchType.type = 3;
                matchType.colour = colour;
                return matchType;
            }
       }

        matchType.type = 0;
        matchType.colour = "";
        return matchType;

        /*   int numberHorizontal = 0;
           int numberVertical = 0;

           GamePiece firstPiece = findMatches.currentMatches[0].GetComponent<GamePiece>(); //Grab the piece first added to the match list
           if (firstPiece != null) //Null reference exception prevention
           {
               foreach (GameObject currentPiece in findMatches.currentMatches) //goes through all pieces in current matches
               {
                   GamePiece piece = currentPiece.GetComponent<GamePiece>(); //Caches the get component, helps optimisation
                   if (piece.row == firstPiece.row)
                   {
                       numberHorizontal++;
                   }
                   if (piece.column == firstPiece.column)
                   {
                       numberVertical++;
                   }
               }
           }
           return (numberVertical == 5 || numberHorizontal == 5); //retrun true if numberVertical or numberHorizontal is equal to 5*/
    }


    //Collapsing rows to introduce more game pieces upon clearing coroutine

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(refillDelay * 0.5f);
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                //Check if tile is not already filled, is not a blank space, or an obsidian tile
                if (!blankSpaces[w, h] && allPieces[w, h] == null && !obsidianTiles[w,h])                    
                {
                    //loop from the space above to the top of the column
                    for (int i = h+1; i < height; i++)
                    {
                        //If a piece is found 
                        if(allPieces[w,i] != null)
                        {
                            //move that piece to this empty space
                            allPieces[w, i].GetComponent<GamePiece>().row = h;
                            
                            allPieces[w, i] = null;//Set that spot to be null

                            break; //break out of loop
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay*0.5f); //Half the time a refill takes
        StartCoroutine(FillBoard());
    }


    //Refilling the board

    private IEnumerator FillBoard() //puts new pieces into the game and checks if any matches are made 
    {
        
        yield return new WaitForSeconds(refillDelay);
        RefillBoard(); //Places new pieces
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard()) // Checks to see if there are matches
        {
            DestroyMatches(); //destroy
            streakValue ++;//Increase the streak value by 1 if another match was made on refill (combo mechanic)

            //StartCoroutine(FillBoard());
            
            yield break; //If we find a match (if matchesonboard returns true) then break this coroutine as DestroyMatches will call Decrease Row/Col which will call RefillBoard.

        }
        // findMatches.currentMatches.Clear(); //cleans the list of matches
        currentGamePiece = null; //prevents some unity errors
        
        //yield return new WaitForSeconds(refillDelay); //Now check to see if at a deadlock

        if(IsDeadlocked())
        {
            Debug.LogWarning("Is deadlocked!!!");
            StartCoroutine( ShuffleBoardPieces()); //If so then shuffle board peieces
        }
        yield return new WaitForSeconds(refillDelay*.5f);
        if (MatchesOnBoard())
        {
            StartCoroutine(FillBoard());
            yield break;
        }

        currentState = GameState.move; //Allows player to move pieces again
        streakValue = 1; //Streak is over, Reset the value

      /*  if (MatchesOnBoard())
        {
            while (MatchesOnBoard()) // Checks to see if there are matches
            {
                DestroyMatches(); //destroy
                streakValue++;//Increase the streak value by 1 if another match was made on refill (combo mechanic)          
                yield break; //If we find a match (if matchesonboard returns true) then break this coroutine as DestroyMatches will call Decrease Row/Col which will call RefillBoard.
            }

        }*/

    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allPieces[i,j] == null && !blankSpaces[i,j] && !obsidianTiles[i,j]) 
                    //if the space is empty AND its not a blank space AND is not an obsidian tile

                {
                    Vector2 tempPosition = new Vector2(i, j + offset); 
                    //create a new vector in the arrays at this position, + offset to animate into place

                    int colourToUse = Random.Range(0, colours.Length); //generate a colour 

                    //----------------
                    //This chunk will make the refill pieces not match with the existing board, 
                    //making it impossible to get a free match from the tiles that fall down
                    int maxIterations = 0;
                    while(MatchesAtGen(i,j,colours[colourToUse]) && maxIterations < 50)
                    {
                        maxIterations++;
                        colourToUse = Random.Range(0, colours.Length); //generate another colour

                    }
                    maxIterations = 0; //Reset max iterations so we can loop again
                    //Comment this out to enable a mechanic where pieces can be kind and fall into matches 
                    //-----------------

                    GameObject piece = Instantiate(colours[colourToUse], tempPosition, Quaternion.identity); 
                    //spawn new piece at the position, with generated colour

                    allPieces[i, j] = piece; //asign newly generated piece to the empty slot in the array

                    piece.GetComponent<GamePiece>().column = i; //Ensure piece knows its new location
                    piece.GetComponent<GamePiece>().row = j;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        findMatches.FindAllMatches(); //finds matches

        for (int i = 0; i < width; i++) //loop through our array
        { 
            for (int j = 0; j < height; j++)
            {
                if(allPieces[i,j]!= null) //if its not noll
                {
                    if (allPieces[i, j].GetComponent<GamePiece>().isMatched) //check if its matched
                    {
                        return true; //return true
                    }
                }
            }
        }
        return false;

    }

    // //////////////////////////////////////////////////////  //
    // Deadlock Prevention Mechanism (Virtual Switching and Checking)

    private void VirtuallySwitchPieces(int column, int row, Vector2 direction) //Swaps the cells contents 
    {                                        
        if (allPieces[column + (int)direction.x, row + (int)direction.y] != null) 
            //Checks if the piece is from our 2D array, stops any switch checks with blank or obsidian spaces 
        {
            GameObject holder = allPieces[column + (int)direction.x, row + (int)direction.y] as GameObject; 
            //Acess the cell with direction modifier, and copy it to holder
            allPieces[column + (int)direction.x, row + (int)direction.y] = allPieces[column, row]; 
            //Acess the cell with direction modifier, and set its values to the cell without a modifier
            allPieces[column, row] = holder; 
            //Then acess the cell without a modifier, and set it, to what we copied in the holder
            //Literally just swaps whats in [x,y] with whatever is in [x+d, y+d] and vice versa.
        }
    }

    private bool VirtuallyCheckForMatches()
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (allPieces[i, j] != null) //If the piece in our array exits
                {
                    //check pieces to the right
                    if (i < width - 2) //Before we perform check, make sure that pieces can exist to the right 
                    {
                        
                        if (allPieces[i + 1, j] != null && allPieces[i + 2, j] != null) 
                            //If the pieces to the right and two to the right exist,  /// Searches for matches [x][?][?] >>>
                        {
                            if (allPieces[i + 1, j].tag == allPieces[i, j].tag && allPieces[i + 2, j].tag == allPieces[i, j].tag) 
                                //If both pieces to the right and two to the right are the same colour (tag)
                            {
                                return true; //Then a match can be made and there is no deadlock
                            }

                        }
                    }

                    if (j < height - 2) //check pieces to the right
                    {   
                        //Check pieces above                                                                                                                   
                        if (allPieces[i, j + 1] != null && allPieces[i, j + 2] != null) //Check they exist to prevent null exception error 
                        {

                            if (allPieces[i, j + 1].tag == allPieces[i, j].tag && allPieces[i, j + 2].tag == allPieces[i, j].tag)
                                //If pieces above, and two above are the same colour (tag)
                            {
                                return true;
                            }
                        }
                    }
                }           
            }        
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        //Switches peices based on [col/row] and [column+direction.x / row+direction.y]
        VirtuallySwitchPieces(column, row, direction); 
        if (VirtuallyCheckForMatches())//Now with these pieces swapped, check if a match can be made 
        {      //If a match can be made (true)
            VirtuallySwitchPieces(column, row, direction); //Swap pieces again, reversing the swap we just made
            return true; //breaks the method so we only ever swap twice max
        }

        VirtuallySwitchPieces(column, row, direction); //even if returns false, we swap those pieces back
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++) 
        { 
            for (int j = 0; j < height; j++) 
            {             
                if (allPieces[i,j] != null) //null reference check
                {
                    if (i < width -1) //and we are not at the edge or 1 from the edge (MIGHT NEED TO BE -2)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right)) 
                    //Switch all pieces one at a time, with their right neihbour and if a match is found, will return true
                        {
                            return false; //So the board is not deadlocked
                        }
                    }

                    if (j < height -1) //If we are not at the top or one from the top (MIGHT NEED TO BE -2)
                    {
                        if(SwitchAndCheck(i,j,Vector2.up)) 
                    //goes through one at a time, switching every pieace with the one above it, and if a match is found will return true
                        {
                            return false; //So again we are not deadlocked
                        }
                    }
                }
            }
        }
        return true; 
        //If no false returns break this method , and we've cycled through every possible Right or Up swipe then the board is deadlocked

    }


    //Shuffle board pieces around

    private IEnumerator ShuffleBoardPieces() //Rearranges the current pieces on the board in a new order
    {
        yield return new WaitForSeconds(pauseTime);

        List<GameObject> newBoard = new List<GameObject>(); //Create a new list of game objects


        for (int i = 0; i < width; i++) //Add every piece to this list
        {
            for (int j = 0; j < height; j++)
            {
                if (allPieces[i, j] != null) //&& piece isnt blocked
                {
                    newBoard.Add(allPieces[i, j]); //places piece[i,j]  inside new list, looping through all pieces and adding them subsequently

                }

            }
        }

        for (int i = 0; i < width; i++) //for every spot on the board
        {
            for (int j = 0; j < height; j++)
            {
               if(!blankSpaces [i,j] && !obsidianTiles[i,j]) //If this shouldnt be a blank space, true if there should be and false if there shouldnt be so we want to complete the If, only if there is NOT a blank space or obsidian
               {
                    int pieceToUse = Random.Range(0, newBoard.Count); //create a random number between 0 and the list length |||| NOTE: List length is .Count | array length is .Length               
                    int maxIterations = 0; //ensures the while does not loop infinitely
                    //This while ensures the shuffle does not create a match of 3 upon shuffling
                    while (MatchesAtGen(i, j, newBoard[pieceToUse]) && maxIterations < 50) //checks to see if our current randomly selected piece, will create a match 3 
                    {
                        pieceToUse = Random.Range(0, newBoard.Count); //If our piece does return a match... Change the random selector to select another piece
                        maxIterations++; //And increment our iterations by 1
                        Debug.Log("Shuffling board :" + maxIterations); //Then because were in a while, Matches at Gen will check if our new random piece makes a match ect...
                    }
                    maxIterations = 0; //resets max for the next loop after matches at gen returns false, or max iterations reaches 50+

                    //If this step goes along, and we break out of the matches at gen, then we will have a random piece to assign into our allPieces array.
                    ///Matches at gen will virtually place our ranomly selected game object into allPieces and check the allPieces array for any matches
                    ///If no matches return (MatchesAtGen = false) then the while will stop because our randomly selected piece would not cause a match if we place it at [i,j] of all Pieces.
                    //Therefore it is okay to then grab the component and assign its column and row to I and J and place it inside all pieces.
                    ///Because MatchesAtGen checks down and left, (The oppsite way in which new pieces are added), it ensures that it wont fire a falsePositive, because its only checking pieces
                    ///That are new in the array.

                    GamePiece piece = newBoard[pieceToUse].GetComponent<GamePiece>(); //cache the gamepiece belonging at the random integer of newboard



                    piece.column = i; //assign column and row to the piece
                    piece.row = j;
                    allPieces[i, j] = newBoard[pieceToUse]; //The new random int board piece is copied to allPieces i,j
                    newBoard.Remove(newBoard[pieceToUse]); //Get rid of random int board piece from list, Next time this is called, the list will decrease
               }
            }
        }

        //Now the board has been rearranged, check for a deadlock, if its deadlocked shuffle again and check once more for deadlock, however this could cause infinite loop
        if (IsDeadlocked()) //If true
        {
            StartCoroutine (ShuffleBoardPieces()); //Shuffle again
            //Probably need a max times so we dont infinite loop :( probably will only happen with very small boards, or many different coloured pieces
        }
    }


}
