using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimationManager : MonoBehaviour
{
    public float animationSpeed; //amount of seconds to coroutine
    public Color cubeColor; //Color that all cubes will be changed to show algorithms current cube that its checking
    public Color pathColor; //Color that cubes will change into to show path from start to end
    public bool isPath; //bool that will be used by gridCubeAnimationHelper to determine if it should use cubeColor or pathColor
    public bool isSpeedVisualization; //bool that will be used by gridCubeAnimationHelper to determine which animation it should play
    public GameObject UIManager;
    public GameObject MainCamera;
    public bool backInitiated = false;
    public Vector3 startObjPos; //record start objects position before the animation to later make sure rigidbodies didnt mess up position
    public Vector3 endObjPos; //record end objects position before the animation to later make sure rigidbodies didnt mess up position
    public GameObject startObject;
    public GameObject endObject;

    private bool isFirstElement; //bool that checks if the coroutine just started to give the first cube rigidbody
    private bool foundEndSaved; //saves the passed in bool of foundEnd - used in spaceBarPrompt to change notification discription
    private GameObject[,] gridTopMatrix;
    private GameObject[,] gridMatrix;


    /* animates the gridCubes one by one from passed in linked list
     * if foundEnd is true animate the end object aswell when list reaches the last value
     * animationSpeed value changes the wait time between animation calls
     */
    public void AnimateGrid(LinkedList<GameObject> objectsToAnimate, bool foundEnd, LinkedList<GameObject> path = null)
    {
        animationSpeed = GetComponent<GridManager>().UIManager.GetComponent<UISide>().algoSpeed;
        //if animationSpeed is Medium/Slow/VerySlow then give user option to speed it up to VeryFast
        if(animationSpeed >= 0.5f)
        {
            this.gameObject.AddComponent<SpeedUpAnimationPrompt>();
        }
        foundEndSaved = foundEnd;
        startObject = GetComponent<GridManager>().startObject;
        endObject = GetComponent<GridManager>().endObject;
        isPath = false;
        isFirstElement = true;
        startObjPos = GetComponent<GridManager>().startObject.transform.position;
        endObjPos = GetComponent<GridManager>().endObject.transform.position;
        gridTopMatrix = GetComponent<GridManager>().gridTopMatrix;
        gridMatrix = GetComponent<GridManager>().gridMatrix;

        StartCoroutine(WaitForCubeAnimation(objectsToAnimate, foundEnd, path));
    }
    IEnumerator WaitForCubeAnimation(LinkedList<GameObject> objectsToAnimate, bool foundEnd, LinkedList<GameObject> path)
    {
        while(objectsToAnimate.Count > 0)
        {
            //animate cube then wait animationSpeed amount of seconds
            GameObject cube = objectsToAnimate.First.Value;
            // check to see if this is the first element thats being animated. if true give start cube rigidbody
            if (isFirstElement)
            {
                Rigidbody rb = cube.AddComponent<Rigidbody>();
                rb.mass = 100;
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            //check to see if there is weight on top to change its alpha and add rigid body to the cube under
            //get index of cube object
            string[] temp = cube.name.Split('-');
            int row = System.Int32.Parse(temp[1].Split(',')[0]);
            int col = System.Int32.Parse(temp[1].Split(',')[1]);
            if (gridTopMatrix[row,col]!= null && gridTopMatrix[row,col].name[1] == 'e')
            {
                GameObject weight = gridTopMatrix[row, col];
                //change alpha of the weight to see the cube color underneither better only if emission is off
                //weight.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0.15f);

                Rigidbody rb = cube.AddComponent<Rigidbody>();
                rb.mass = 100;
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            cube.GetComponent<Animator>().Play("CubeAnimation");
            //check if cube already has scipt 'gridCubeAnimationHelper' to prevent a bug. if true remove that first then add the new one
            if(cube.GetComponent<gridCubeAnimationHelper>() != null)
            {
                Destroy(cube.GetComponent<gridCubeAnimationHelper>());
            }
            //change cubes color into chosen 'cubeColor' and move its y positing up by 1 then move it back down after animation
            cube.AddComponent<gridCubeAnimationHelper>();
            // if foundEnd is true and list only has one value then animate path and give cube rigidbody
            if (objectsToAnimate.Count == 1 && foundEnd)
            {
                Rigidbody rb = cube.AddComponent<Rigidbody>();
                rb.mass = 100;
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            /* bug fix with Astar and start object (start object falls through the cube only on medium speed)
             * if algoIndex is 1 (AStar) and speed is 0.5 (medium) then move startObject Y coordiante by 1
             */
            if(animationSpeed == 0.5f && GetComponent<GridManager>().UIManager.GetComponent<UISide>().algoIndex == 1 && isFirstElement)
            {
                startObject.transform.position = new Vector3(startObjPos.x, startObjPos.y + 1, startObjPos.z);
            }
            isFirstElement = false;
            yield return new WaitForSeconds(animationSpeed);
            //remove first element from the list and go back into the while loop
            objectsToAnimate.RemoveFirst();
        }
        if (foundEnd)
        {
            StartCoroutine(WaitForPathAnimation(path));
        }
        else
        {
            //check if user didnt speed up animation then remove the script
            if (GetComponent<SpeedUpAnimationPrompt>() != null)
            {
                Destroy(GetComponent<SpeedUpAnimationPrompt>());
            }
            this.gameObject.AddComponent<SpaceBarPrompt>();
            GetComponent<SpaceBarPrompt>().InitializeValues(startObjPos, endObjPos, foundEndSaved);
        }
    }
    IEnumerator WaitForPathAnimation(LinkedList<GameObject> path)
    {
        isPath = true;
        while (path.Count > 0)
        {
            GameObject cube = path.First.Value;
            cube.GetComponent<Animator>().Play("CubeAnimation");
            //check if cube already has scipt 'gridCubeAnimationHelper' to prevent a bug. if true remove that first then add the new one
            if (cube.GetComponent<gridCubeAnimationHelper>() != null)
            {
                Destroy(cube.GetComponent<gridCubeAnimationHelper>());
            }
            cube.AddComponent<gridCubeAnimationHelper>();
            yield return new WaitForSeconds(animationSpeed);
            path.RemoveFirst();
        }
        //check if user didnt speed up animation then remove the script
        if(GetComponent<SpeedUpAnimationPrompt>() != null)
        {
            Destroy(GetComponent<SpeedUpAnimationPrompt>());
        }
        this.gameObject.AddComponent<SpaceBarPrompt>();
        GetComponent<SpaceBarPrompt>().InitializeValues(startObjPos, endObjPos, foundEndSaved);
    }

    //changes animation speed to 'VeryFast'
    public void ChangeAnimationSpeed()
    {
        animationSpeed = 0.01f;
    }

    //Animation for speed comparison visualization 
    public void SpeedVisualizationAnimation()
    {
        isSpeedVisualization = true;
        //remove everything from gridTop including start and end objects
        startObject.SetActive(false);
        endObject.SetActive(false);
        GetComponent<GridManager>().RemoveAllWalls();
        GetComponent<GridManager>().RemoveAllWeights();
        //close side UI
        UIManager.GetComponent<UIAnimationManager>().CloseSideUI(false, true);
        //replace main camera controls with speed visualization controls
        MainCamera.GetComponent<CameraMovement>().enabled = false;
        MainCamera.GetComponent<CameraRayCast>().enabled = false;
        MainCamera.AddComponent<CameraMovementSpeedVisualization>();
        //move cubes up leaving only as many cubes as algorithms available, to be used like a bar graph
        // leave cubes at [9,10] , [9,12] , [9,18] , [9,20]
        for (int i = 0; i <= gridMatrix.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= gridMatrix.GetUpperBound(1); j++)
            {
                gridMatrix[i, j].AddComponent<gridCubeAnimationHelper>();
            }
        }
    }
  
}
