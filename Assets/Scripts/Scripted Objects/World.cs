using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds information for all of the levels
[CreateAssetMenu (fileName = "World", menuName = "World")]
public class World : ScriptableObject //Inherits scriptable object architecture from unity
{
    public Level[] levels;


}
