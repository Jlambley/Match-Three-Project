using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiecePreview : MonoBehaviour
{
    public GameObject[] piece;
    public World world;
    // Start is called before the first frame update
  
    public void LoadPreview(int level)
    {
        for (int i = 0; i < world.levels[level].pieces.Length; i++) //For the length of the pieces avaliable, set the images to the level data
        {
            piece[i].SetActive(true);
            piece[i].GetComponent<Image>().sprite = world.levels[level].pieces[i].GetComponent<SpriteRenderer>().sprite;
        }

        for (int i = world.levels[level].pieces.Length; i < piece.Length; i++) 
        {
            piece[i].SetActive(false);
            //piece[i].GetComponentInParent<Image>().gameObject.SetActive(false);
        }
    }

}
