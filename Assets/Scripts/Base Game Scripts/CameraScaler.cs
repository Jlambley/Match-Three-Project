using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 8;
    public float Yoffset = 1; // Brings the camera down to offset the UI elements
    private BoardController board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>(); 
        if(board!= null) //If we have our board (Defensive programming)
        {
            RepositionCamera(board.width-1, board.height-1); //Moves the camera based on board size
        }
    }

    void RepositionCamera(float x, float y) 
        //Repositions the camera halfway between both values (x/2 , y/2), resulting in a centeral camera
    {
        Vector3 tempPosition = new Vector3( x/2, y/2+Yoffset, cameraOffset ); 
        //Creates temporary position at half the x and y (width and height)
        transform.position = tempPosition; //Updates camera position 

        if (board.width > board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }
}
//Iphones 9 / 16 to .3 decimal points

//If Width > Height, Orthographic size = width/2 + padding   /   .625
//if height > width, orthographic size = height/2 + padding