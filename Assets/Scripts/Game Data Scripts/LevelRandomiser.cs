using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelRandomiser : MonoBehaviour
{

    [Header("Worlds + Level")]
    private GameData gameData;
    public World world;
    public World randomisedWorld;

    public Level entryLevel;  //level to use as a base
    public Level randiteLevel; //level to be randomised

    

    public LevelProgression levelProgression;
    public ItemData itemData;
    
    [Header("Board + Tiles")]
    public int level;
    public BoardController board;
    public TileType[] newBoardLayout; //default size of 11
    public TileType addition;
    //private int iterations;
    public List<TileType> layoutElements = new List<TileType>();

    [Header("Misc")]
    public GameObject maskAnim;
    public GoalSpriteLibrary goalLib;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>();
        gameData = FindObjectOfType<GameData>();
        layoutElements.Clear(); //clears the list
                                //newBoardLayout = new TileType[2];
                                //iterations = 0;
        itemData = levelProgression.generationItems;


       


    }

    private void AnimateDescent()
    {
        Debug.Log("animateDescent is active");
        maskAnim.SetActive(true);
    }

    public void DescentButton() 
    {
        if (board.currentState == GameState.move) //Ensures we can only descend when the player has control
        {
            StartCoroutine(RandomiseAndDescend());
        }
    }

    private IEnumerator RandomiseAndDescend() //button press
    {
        board.currentState = GameState.pause; //stops player doing anything
        AnimateDescent();    //Animation

        yield return new WaitForSeconds(3f); //wait for 2 secodns while animation plays
                                             //RandomiseBoardLayout();

        //Load new Scene

        PlayerPrefs.SetInt("Current Level", board.level);
        Debug.Log("[00B] BoardLevel " + board.level.ToString());

        Debug.Log("[00S] savedata.floornumber[0] = " + gameData.saveData.floorNumber[0]);
        board.level++;

        Debug.Log("[11B] BoardLevel++ " + board.level.ToString());
        gameData.saveData.floorNumber[0] = board.level; //saves the floor number in the data

        Debug.Log("[11S] savedata.floornumber[0] = " + gameData.saveData.floorNumber[0]);


        //gameData.saveData.floorNumber[board.level] = gameData.saveData.floorNumber[board.level++]; //saves the floor number in the data
        PlayerPrefs.SetInt("Current Level", board.level);   


        gameData.Save(); //And save our data
        SceneManager.LoadScene("Random Scene"); //finally loads the level
    }
    private void RandomiseBoardLayout()
    {
        CopyLevelSetup(); //Makes a copy of the current level
        SaveFloor();

        GetGenerationData();
        //CreateTiles(TileProperties.Obsidian, 8); //creates 3 random obisidan tiles
        //create tiles x y z 
        
        
        //Gives coords to the list elements created
        GenerateCoords();

        //After all tiles have been created we need to save them to the new layout
        AddListEntriesToLayout();

        GoalRandomiser(); //Randomises the goal 

        randiteLevel.boardLayout = newBoardLayout; //saves the tiles into the board layout
        
    }
    private void CopyLevelSetup() //copies components of the entry level but increases the score goal and floor number
    {
        if (gameData.saveData.floorNumber[board.level] > 0)
        {
            entryLevel = randiteLevel; //If we have decended at least once, then use the randite level to make edits to itself
        }

        randiteLevel.Width = entryLevel.Width; //Copy basic setup
        randiteLevel.height = entryLevel.height;
        randiteLevel.pieces = entryLevel.pieces;
        randiteLevel.endGamerequirements = entryLevel.endGamerequirements;


        int newGoal = entryLevel.scoreGoals[0];
        newGoal *= (int)1.2; //increases score goal by 10% each time
        randiteLevel.scoreGoals[0]= newGoal; 
        randiteLevel.floor = entryLevel.floor; 
        randiteLevel.floor++;//increases the floor number by 1 each time
    }

    private void SaveFloor()
    {

        if (gameData != null) //checks to see if we have gamedata
        {
            gameData.saveData.floorNumber[board.level] = randiteLevel.floor; //saves the floor number in the data
            gameData.Save(); //And save our data
        }
    }

    private void GetGenerationData()
    {
       itemData = levelProgression.LevelProgress(board.level, randiteLevel.floor); //grabs data based on level and floor

        if (itemData.items[0] != 0) { CreateTiles(TileProperties.Breakable, itemData.items[0]); } //If our data is not 0, then create that many tiles of fossils (breakable) in the list
        if (itemData.items[1] != 0) { CreateTiles(TileProperties.Lock, itemData.items[1]); } //lock = oil
        if (itemData.items[2] != 0) { CreateTiles(TileProperties.Obsidian, itemData.items[2]); }
        if (itemData.items[3] != 0) { CreateTiles(TileProperties.Ruby, itemData.items[3]); }
        if (itemData.items[4] != 0) { CreateTiles(TileProperties.Emerald, itemData.items[4]); }
        if (itemData.items[5] != 0) { CreateTiles(TileProperties.Diamond, itemData.items[5]); }

        // items[3] = ruby, [4] = emerald, [5] = diamond, need creating (copies of obsidian but with more health
    }

    private void CreateTiles(TileProperties typee, int howMany)
    {
        //addition.tileProperties = type; //set our tile type from parsed
        for (int i = 0; i < howMany; i++) //Adds entries to the list i ammount of times with parsed typing
        {
            AddListEntry(typee); //and add the entry
        }
    }

    private void AddListEntry(TileProperties type) //takes in a tipe and adds the list entry with blank coords
    {
        TileType entry = new TileType(); //Initialises our entry
        entry.tileProperties = type; //asigns it the properites of the tile

        layoutElements.Add(entry); //Adds the entry to the list

    }

    private void GenerateCoords()
    {
        int needed = layoutElements.Count;

        TileType buffer = new TileType();


        for (int i = 0; i < needed; i++)
        {
            Randomise(buffer);

            if (DoesListContain(buffer, layoutElements))
            {
                int maxIterations = 0;
                while (DoesListContain(buffer, layoutElements) && maxIterations < 50) //if the cord already exists, and our max itertaions have not reached 50
                {
                    Randomise(buffer); //Then randomise  our buffer
                    maxIterations++; //Increment our iterations and check again
                    Debug.Log("Coord taken, now randermising, Iterations at [" + maxIterations + "]");
                }

            }

            if (!DoesListContain(buffer, layoutElements))
            {
                layoutElements[i].x = buffer.x;
                layoutElements[i].y = buffer.y;
            }

        }

    }

    private bool DoesListContain(TileType check, List<TileType> list) //checks a list for the parsed coord and returns true if it does
    {

        for (int i = 0; i < list.Count; i++)
        {
            if (check.x == list[i].x && check.y == list[i].y)
            {
                return true;//this coord is taken
            }
        }
        return false;
    }
    private void Randomise(TileType tile)
    {
        tile.x = Random.Range(0, randiteLevel.Width - 1);
        tile.y = Random.Range(0, randiteLevel.height - 1);

    }

    private void AddListEntriesToLayout() //Takes our list, creates a new board layout length then adds all elements to the new layout
    {
        newBoardLayout = new TileType[layoutElements.Count]; //Makes new board layout as big as our list

        for (int a = 0; a < newBoardLayout.Length; a++) //for the entire of the new length
        {
            newBoardLayout[a] = layoutElements[a]; //chosen elements corresponds with our list element
        }
        Debug.Log("Randomised level edits from list now saved in board[]");

    }

    public void GoalRandomiser()
    {
        randiteLevel.levelGoals[0] = GenerateGoal();
    }

    private BlankGoal GenerateGoal()
    {
        BlankGoal newGoal = new BlankGoal(); //sets up goal

        newGoal.goalSprite = goalLib.GenerateRandomColour(); //returns a random task and corresponding sprite
        newGoal.matchValue = goalLib.GetMatchValue();
        newGoal.numberCollected = 0; //set ammount 
        newGoal.numberNeeded = Random.Range(3, 7); //Hardcoded please change 

        return newGoal;
    }
}



