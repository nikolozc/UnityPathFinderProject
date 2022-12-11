using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public int speed;

    private float minX; //minimum X camera can move to 
    private float maxX; //maximum X camera can move to
    private float minZ; //minimum Z camera can move to 
    private float maxZ; //maximum Z camera can move to
    private float minY; //minimum Y camera can move to 
    private float maxY; //maximum Y camera can move to
    private float screenBorderThickness = 10f; //how close your cursor has to be at the edge of the screen to move

    // Start is called before the first frame update
    void Start()
    {
        //initialize min and max values for camera movement
        GameObject[,] gridMatrix = GetComponent<CameraRayCast>().projectManager.GetComponent<GridManager>().gridMatrix;
        GameObject firstElement = gridMatrix[0, 0];
        minX = firstElement.transform.position.x;
        maxZ = firstElement.transform.position.z;
        GameObject lastElement = gridMatrix[gridMatrix.GetUpperBound(0), gridMatrix.GetUpperBound(1)];
        maxX = lastElement.transform.position.x;
        minZ = lastElement.transform.position.z;
        minY = 5;
        maxY = 30;
    }

    // Update is called once per frame
    void Update()
    {
        // get cursor location to see if its at edge of the screen
        Vector2 cursorPosition = Input.mousePosition;

        //zoom out if either left shift is click or scrollwheel is turn forwards
        if ((Input.GetKey("left shift") || (Input.GetAxis("Mouse ScrollWheel") < 0f)) && transform.position.y <= maxY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + speed * 2 * Time.deltaTime, transform.position.z);
        }
        //zoom in if either left ctrl is click or scrollwheel is turn backwards
        if ((Input.GetKey("left ctrl") || (Input.GetAxis("Mouse ScrollWheel") > 0f)) && transform.position.y >= minY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - speed * 2 * Time.deltaTime, transform.position.z); 
        }
        //move up if either up arrow key is click or mouse cursor is at the edge of the screen
        if (Input.GetKey("up") || cursorPosition.y >= Screen.height - screenBorderThickness && transform.position.z <= maxZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime);
        }
        //move down if either down arrow key is click or mouse cursor is at the edge of the screen 
        if (Input.GetKey("down") || cursorPosition.y <= screenBorderThickness && transform.position.z >= minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime);
        }
        //move right if either right arrow key is click or mouse cursor is at the edge of the screen
        if (Input.GetKey("right") || cursorPosition.x >= Screen.width - screenBorderThickness && transform.position.x <= maxX)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        //move left if either left arrow key is click or mouse cursor is at the edge of the screen
        if (Input.GetKey("left") || cursorPosition.x <= screenBorderThickness && transform.position.x >= minX)
        {
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }
}
