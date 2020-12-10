using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLibrary : MonoBehaviour
{
    public Texture White, Red, Blue, Green, Purple, Pink, Yellow, Fossil, Oil, Obsidian, Ruby, Emerald, Diamond;

    // Start is called before the first frame update
    public Texture GetTexture(string s) //Get specified texture from library 
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
        Debug.Log("Returning Null");
        return null;
    }
    // Update is called once per frame

}
