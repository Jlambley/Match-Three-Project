using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryAnimator : MonoBehaviour
{

    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("OnCollisionEnter2D");
        anim.SetTrigger("Get"); //Triggers the animation whenever a particle itsit
    }
    private void OnParticleTrigger()
    {
        Debug.Log("ParticleTrigger");
        anim.SetTrigger("Get"); //Triggers the animation whenever a particle itsit
    }

    public void Get()
    {
        Debug.Log("GET");
        anim.SetTrigger("Get"); //Triggers the animation whenever a particle itsit
    }
}
