using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "Data", menuName = "Item Data")]
public class ItemData : ScriptableObject
{

    [Header("Generation Items")]

    public int fMin; //fossil minimum spawn
    public int fMax; //fossil max spawn
    public int oMin, oMax; //oil max and min
    public int bMin, bMax; //obsidian max and min
    public int rMin, rMax, eMin, eMax, dMin, dMax; //ruby, emerald and diamond min and max spawn

    [Header("Gen Array ")]
    public int[] items;

}
