using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTiler : MonoBehaviour
{

    public int hitPoints;
    public GameObject hitVFX;
    private SpriteRenderer sprite;
    private GoalManager goalManager; //So we can update the goals if they want the user to break tiles to progress
    private SpriteChanger changer;


    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        changer = GetComponent<SpriteChanger>();
        goalManager = FindObjectOfType<GoalManager>(); 
    }
    

    public void TakeDamage(int damage) //Simple function to decrease the value of hitpoints, using argument value
    {
        StartCoroutine(DamageDelay(damage));
    }

    public IEnumerator DamageDelay(int dmg) //Waits a little before destroying the object so that they destroy with the pieces and not before
    {
        yield return new WaitForSeconds(.1f);
        hitPoints -= dmg;
        if (hitVFX != null)
        {
            Instantiate(hitVFX, GetComponent<Transform>().transform.position, Quaternion.identity); //Instantiate hit/death VFX

        }
        DeathCheck();
    }

    private void DeathCheck() //Checks to see if the piece has been destroyed
    {
        if (hitPoints > 0) //If the piece is not destroyed
        {
            changer.CheckSprite(hitPoints); //decrease alpha value (will probably change to edit the sprite to reflect breakage)
        }
        else if (hitPoints <= 0) 
        {
            CheckWithGoals(); //See if we needed to break a tile in our goals, and update accordingly

            Destroy(this.gameObject); //Destroy if hitPoints reaches 0 or less
        }
    }
    
    private void CheckWithGoals()
    {
        if (goalManager != null)
        {
            goalManager.CompareGoal(this.gameObject.tag);
            goalManager.UpdateGoalText();
        }
    }

    void MakeSpriteLighter() //Old Method Reduces opacity of color attached to sprite renderer by half
    {
        Color color = sprite.color; //Get the existing colour
        float newAlpha = color.a * .5f; //Takes the alpha value and reduces it by half
        sprite.color = new Color (color.r,color.g,color.b,newAlpha); //Assign the same colour, but with a new alpha value
    }

    private void OnDestroy() //On destroying the tile an update to the inventory will take place based on what tile was destroyed
    {
        if (hitPoints <= 0)
        {
            int tileId = 0;

            switch (gameObject.tag.ToString())
            {
                case "Obsidian": tileId = 9; break; //Obsidian
                case "Lock": tileId = 8; break; //OIL
                case "Breakable": tileId = 7; UpdateFossilText(); break; //Fossil
                case "Ruby": tileId = 10; break;
                case "Emerald": tileId = 11; break;
                case "Diamond": tileId = 12; break;
            }

            if (tileId != 0)
            {
                FindObjectOfType<ParticleManager>().SpawnCollectParticle(transform, this.tag.ToString());
                GameData gameData = FindObjectOfType<GameData>();
                gameData.saveData.bones[tileId]++; //increase the bone
            }

        }
    }

    private void UpdateFossilText()
    {
        var fossilobject = FindObjectOfType<FossilCountScript>();
        if (fossilobject != null)
        { fossilobject.UpdateFossilText();  }
    }

}
