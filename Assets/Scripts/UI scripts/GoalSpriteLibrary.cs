using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class GoalSpriteLibrary : MonoBehaviour
{
    public Sprite White, Red, Blue, Green, Purple, Pink, Yellow, Fossil, Oil, Obsidian, Ruby, Emerald, Diamond;
    private string matchValue;
    private Sprite goalSprite;

    public Sprite GenerateRandomColour()  //Decides a colour
    {
        int id = Random.Range(0, 5); //Change this according to cases

        switch(id)
        {
            case 0: goalSprite = White; matchValue = "White"; break;
            case 1: goalSprite = Red; matchValue = "Red"; break;
            case 2: goalSprite = Blue; matchValue = "Blue"; break;
            case 3: goalSprite = Green; matchValue = "Green"; break;
            case 4: goalSprite = Purple; matchValue = "Purple"; break;
            case 5: goalSprite = Pink; matchValue = "Pink"; break;

        }

        return (goalSprite);
    }

    public string GetMatchValue()
    {
        return matchValue;
    }

    public Sprite GetSpecificSprite(string s) //Get specified sprite from library 
    {
        switch (s)
        {
            case "White": return White;
            case "Red": return Red;
            case "Blue": return Blue;
            case "Green": return Green;
            case "Purple": return Purple;
            case "Pink": return Pink;
            case "Yellow": return Yellow;
            case "Breakable":
            case "Fossil": return Fossil;
            case "Lock":
            case "Oil": return Oil;
            case "Obsidian": return Obsidian;
            case "Ruby": return Ruby;
            case "Emerald": return Emerald;
            case "Diamond": return Diamond;
        }
        return null;
    }

    public Sprite GetSpecificSprite(int c) //Overload grabs from Int instead
    {
        switch (c)
        {
            case 0: return White;
            case 1: return Red;
            case 2: return Blue;
            case 3: return Green;
            case 4: return Purple;
            case 5: return Pink;
            case 6: return Yellow;
            case 7: return Fossil;
            case 8: return Oil;
            case 9: return Obsidian;
            case 10: return Ruby;
            case 11: return Emerald;
            case 12: return Diamond;
        }
        return null;
    }
}
