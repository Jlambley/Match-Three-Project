using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem collectionVFX;
    public TextureLibrary textureLib;
    public Transform collectionPoint;



    public void SpawnCollectParticle(Transform t, string s)
    {
      // Debug.Log("Transform [" + t + "] string: " + s);
       ParticleSystem particle = Instantiate(collectionVFX, t.position, Quaternion.identity);
       particle.GetComponent<CollectorAnimation>().target = collectionPoint;
       particle.GetComponent<ParticleSystemRenderer>().material.EnableKeyword("_NORMALMAP");
       particle.GetComponent<ParticleSystemRenderer>().material.SetTexture("_MainTex", textureLib.GetTexture(s));
       
       Destroy(particle.gameObject, 2f);

        //Spawn the particle at the death position provided
        //Set the target to our collection point
        //Then set the picture to the corresponding texture in the library based on the string of the tag provided
    }

}
