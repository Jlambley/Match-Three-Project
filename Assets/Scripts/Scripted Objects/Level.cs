using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "world", menuName = "Level")]
public class Level : ScriptableObject
{

    [Header("Board Dimensions")]
    public int Width;
    public int height;

    [Header("Tile Layout ")]
    public TileType[] boardLayout;

    [Header("Available Pieces")]
    public GameObject[] pieces;

    [Header("Current Floor")]
    public int floor;

    [Header("Scoring")]
    public int[] scoreGoals; //Points needed to progress
    public BlankGoal[] levelGoals; //Pieces required to be broken

    [Header("Endgame Requirements")]
    public EndGameRequirements endGamerequirements; //Decides gamemode, Timed or Moves or unlimited
}
