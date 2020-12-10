using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Exposes class to the editor so it can be edited
public class BlankGoal //Data container created to store goal data
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}


public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanelUI> currentGoals = new List<GoalPanelUI>();
    public GameObject goalPrefab;
    public GameObject largeGoalPrefab;
    public GameObject goalMenuContainer, goalGameContainer;

    private EndGameManager endGameManager;
    private BoardController board;
    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++) 
        {
            //Creates a new goal panel at the goal intro parent position
            GameObject goal = Instantiate(largeGoalPrefab, goalMenuContainer.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalMenuContainer.transform, false);
            //Set the image and text of the goal
            GoalPanelUI panel = goal.GetComponent<GoalPanelUI>();//cache
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "" + levelGoals[i].numberNeeded;
            levelGoals[i].numberCollected = 0; //Resets number collected since Scripted Objects save progress


            //Create a new Goal Panel at the goalGameParent
            GameObject gameGoal = Instantiate(goalPrefab, goalGameContainer.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameContainer.transform, false);

           
            panel = gameGoal.GetComponent<GoalPanelUI>();//cache
            currentGoals.Add(panel); //Adds the panel to the list

            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            

        } //Parents the goal to the in game ui elements and the larger UI menu

    }

    // Start is called before the first frame update
    void Start()
    {
        endGameManager = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<BoardController>();
        GetGoals();
        SetupGoals();
     
    }

    void GetGoals() //reads from scritpable object on world
    {

        if (board != null)
        {
            if (board.world != null)
            {
                if (board.level < board.world.levels.Length) //If we are in range
                {
                    if (board.world.levels[board.level] != null)
                    {
                        levelGoals = board.world.levels[board.level].levelGoals; //Loads game type
                        
                    }
                }
            }
        }
    }




    public void UpdateGoalText()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++) //loops through array of level goals (blank goals) 
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;

            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded) //If we have our needed ammount or more
            {
                goalsCompleted++; //Increment goals completed
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded; //Change the text to be (needed)/(needed) so it doesn't over stack
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            Victory();
            Debug.Log("All goals have been completed");
        }
    }

    private void Victory()
    {
        if (endGameManager != null)
        {
           // endGameManager.WinGame(); //If the manager exists, begin the endgame function to trigger the victory screen
        }
    }


    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++) 
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        
        }

    }

}
