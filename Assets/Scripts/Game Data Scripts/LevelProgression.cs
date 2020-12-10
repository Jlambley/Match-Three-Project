using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    public ItemData generationItems;

    // 0 = Fossils, 1 = Oil, 3 = Obsidian, 4 = Ruby, 5 = Emerald, 6 = Diamond

    private void Start()
    {
        if (generationItems.items.Length != 6)
        {
            generationItems.items = new int[6];
        }
    }

    public ItemData LevelProgress(int level, int floor) //using specified level and floor, will return a generation array based on parsers
    {
       
        switch (level) //Allows for this code to be used across multiple levels
        {
            case 0: return Level1Items(floor); 
            //Other levels can go here
        }
        Debug.Log("level specified (" + level + ") Does not exist, also floor is [" + floor + "]"); 
        return null;
        
    }


    private ItemData Level1Items(int floor) //returns item data for the floor
    {
        Debug.Log("Floor :" + floor);
        switch (floor) 
        {
            case 1: CreateItems(0, 0, 0, 0, 0, 0); break;
            case 5: CreateItems(1, 0, 0, 0, 0, 0); break; //One fossil spawns per stage
            case 10: CreateItems(1, 1, 0, 0, 0, 0); break; //One fossil and 1 oil spawns 
            case 15: CreateItems(1, 1, 1, 0, 0, 0); break; //One Fossil, oil, and obsidian
            case 20: CreateItems(1, 1, 1, 1, 0, 0); break; //first ruby tile appears
            case 25: CreateItems(2, 1, 1, 1, 0, 0); break; //Two fossils 
            case 30: CreateItems(2, 2, 1, 1, 0, 0); break;
            case 35: CreateItems(2, 2, 2, 1, 0, 0); break;
            case 40: CreateItems(2, 2, 2, 0, 1, 0); break; //First Emerald with two Fossils, oil and obsidian
            case 45: CreateItems(3, 2, 2, 1, 0, 0); break;
            case 50: CreateItems(3, 3, 2, 1, 0, 0); break;
            case 55: CreateItems(3, 3, 3, 1, 0, 0); break;
            case 60: CreateItems(4, 4, 0, 0, 0, 0,      0, 8, 0, 0, 0, 0); break; //Oil trap
            case 63: CreateItems(4, 4, 0, 0, 0, 0,      0, 9, 0, 0, 0, 0); break; //Oil trap
            case 65: CreateItems(3, 4, 0, 0, 0, 0,      5, 10, 0, 0, 0, 0); break; //Oil trap
            case 70: CreateItems(0, 0, 5, 0, 0, 0,      0, 0, 10, 0, 0, 0); break; //Obsidian walls
            case 75: CreateItems(0, 0, 0, 2, 1, 1,      0, 0, 0, 5, 4, 0); break; //First diamond and Lots of gems
            case 80: CreateItems(0, 0, 0, 2, 2, 1,      0, 0, 0, 5, 5, 0); break; 
            case 85: CreateItems(0, 0, 3, 1, 1, 1,      0, 0, 5, 2, 2, 0); break; 
            case 90: CreateItems(0, 0, 1, 0, 1, 1,      0, 0, 4, 0, 3, 2); break; 
            case 95: CreateItems(0, 0, 2, 0, 1, 1,      0, 0, 5, 0, 5, 2); break; //Lots of gems
            case 98: CreateItems(0, 0, 10, 0, 0, 0,     0, 0, 18, 0, 0, 0); break; //second to last level
            case 99: CreateItems(0, 0, 0, 0, 0, 10,     0, 0, 0, 0, 0, 15); break; //up to 15 diamonds!
        }
        return generationItems;
    }


    private int[] CreateItems(int fossils, int oil, int obisidian, int ruby, int emerald, int diamond) //retruns an array based on the inputs specified in the method
    {
        Debug.Log("Accesing Normal Method");
        generationItems.items[0] = fossils; //sets the defaults
        generationItems.items[1] = oil;
        generationItems.items[2] = obisidian;
        generationItems.items[3] = ruby;
        generationItems.items[4] = emerald;
        generationItems.items[5] = diamond;

        generationItems.fMin = fossils; //sets our minimum so that our goals can be displayed from this data
        generationItems.oMin = oil;
        generationItems.bMin = obisidian;
        generationItems.rMin = ruby;
        generationItems.eMin = emerald;
        generationItems.dMin = diamond;


        return generationItems.items;

    }

    //METHOD OVERLOADER, if this one is specified instead, the returned value will be generated between the min and max
    //Meaning that the possible outcomes for CreateItems(001000,004000) would be   {001000, 002000, 003000, 004000}
    private int[] CreateItems(int fossils, int oil, int obisidian, int ruby, int emerald, int diamond, int fm, int om, int bm, int rm, int em, int dm) //the fm stands for fossilMax
    {
        Debug.Log("Accesing OVERLOAD Method");
        if (fm > fossils) { generationItems.fMax = fm; } //If our max input is greater than our min input, update the max
        if (om > oil)       { generationItems.oMax = om; }
        if (bm > obisidian) { generationItems.bMax = bm; }
        if (rm > ruby)      { generationItems.rMax = rm; }
        if (em > emerald)   { generationItems.eMax = em; }
        if (dm > diamond)   { generationItems.dMax = dm; }

        generationItems.fMin = fossils; //Sets our min in our data
        generationItems.oMin = oil;
        generationItems.bMin = obisidian;
        generationItems.rMin = ruby;
        generationItems.eMin = emerald;
        generationItems.dMin = diamond;

        generationItems.items[0] = fossils; //sets the defaults if no random range is needed
        generationItems.items[1] = oil;
        generationItems.items[2] = obisidian;
        generationItems.items[3] = ruby;
        generationItems.items[4] = emerald;
        generationItems.items[5] = diamond;

        if (fm != 0) { generationItems.items[0] = Random.Range(fossils, fm); } //If a random range is specified (0>) then randomise
        if (om != 0) { generationItems.items[1] = Random.Range(oil, om); }
        if (bm != 0) { generationItems.items[2] = Random.Range(obisidian, bm); }
        if (rm != 0) { generationItems.items[3] = Random.Range(ruby, rm); }
        if (em != 0) { generationItems.items[4] = Random.Range(emerald, em); }
        if (dm != 0) { generationItems.items[5] = Random.Range(diamond, dm); }

        return generationItems.items;

    }

}
