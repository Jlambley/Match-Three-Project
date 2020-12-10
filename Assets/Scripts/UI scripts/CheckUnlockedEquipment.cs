using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckUnlockedEquipment : MonoBehaviour
{
    public GameObject trexBlocker, triBlocker, iguanaBlocker;



    // A simple script that loads when the change equipment button is clicked
    void Start()
    {
       if(PlayerPrefs.HasKey("UnlockedTrex"))
       {
            trexBlocker.SetActive(false);
       }
        if (PlayerPrefs.HasKey("UnlockedTricera"))
        {
            triBlocker.SetActive(false);
        }
        if (PlayerPrefs.HasKey("UnlockedIguana"))
        {
            iguanaBlocker.SetActive(false);
        }
    }

}
