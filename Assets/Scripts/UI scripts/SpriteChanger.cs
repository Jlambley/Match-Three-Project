using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    private SpriteRenderer rend;
    public Sprite Hp3, Hp2, Hp1, Hp0;

    // Start is called before the first frame update

    public void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void CheckSprite(int hp)
    {
        switch (hp)
        {
            case 3: if (Hp3 != null) { rend.sprite = Hp3; } break; //If we have an image, set it to that image,
            case 2: if (Hp2 != null) { rend.sprite = Hp2; } break;
            case 1: if (Hp1 != null) { rend.sprite = Hp1; } break;
            case 0: if (Hp0 != null) { rend.sprite = Hp0; } rend.sortingLayerID = 0; break;  //and then remove it from sorting layer
        }

        
    }

    
}
