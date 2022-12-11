using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject gridCube; //prefab of cube - gridMatrix cubes
    public GameObject start; //prefab of start object
    public GameObject end; //prefab of end object
    public GameObject wall; //prefab of wall object
    public GameObject weight; //prefab of weight object
    public GameObject[,] gridMatrix; //matrix of GameObject (gridCubes)  
    public GameObject[,] gridTopMatrix; //matrix of GameObject (top of gridMatrix - startobject,endobject,walls,and weights)
    public int[] startIndex; //array holding start objects position [row,col]
    public int[] endIndex; //array holding end objects position [row,col]
    public GameObject UIManager; //UIManager object holding UI scripts 
    public GameObject startObject; //start object instantiated from start prefab
    public GameObject endObject; //end object instantiated from end prefab

    private Vector3[,] gridMatrixCubePositions;//saved positions of cubes from gridMatrix, used to put them back into their initial spots after animation
    private float gap = 0.2f; // gap between gridCubes - make sure to change animation values aswell when resizing
    private Color defaultCubeColor; //holds the initial Color of gridCubes
    private Vector3 endObjectPos; //to set distanceToEndNode for cubes A* - HARD CODED! NEED TO CHANGE THIS IF GRID SIZE CHANGED


    // Start is called before the first frame update
    void Start()
    {
        endObjectPos = new Vector3(9.75f, 0, 0.4f);
        gridMatrix = new GameObject[20, 30];
        gridTopMatrix = new GameObject[20, 30];
        gridMatrixCubePositions = new Vector3[20, 30];
        PopulateMatrix();
        defaultCubeColor = gridMatrix[0, 0].GetComponent<MeshRenderer>().material.color;

        //instantiate start and end at their default positions and add them to gridTopMatrix
        int row = 8;
        int col = 10;
        float x = gridMatrix[row, col].transform.position.x;
        float z = gridMatrix[row, col].transform.position.z;
        startObject = Instantiate(start, new Vector3(x-0.3f, 0.4f, z), Quaternion.Euler(new Vector3(0, -90f, 0)));
        startObject.tag = "StartEndObject";
        startObject.name = "Start";
        startObject.AddComponent<BoxCollider>();
        startObject.GetComponent<BoxCollider>().center = new Vector3(0, 0.005f, -0.003f);
        startObject.GetComponent<BoxCollider>().size = new Vector3(0.01f, 0.0075f, 0.01f);
        gridTopMatrix[row, col] = startObject;
        startIndex = new int[] { row, col };
        row = 8;
        col = 20;
        x = gridMatrix[row, col].transform.position.x;
        z = gridMatrix[row, col].transform.position.z;
        endObject = Instantiate(end, new Vector3(x+0.25f, 0.5f, z), Quaternion.identity);
        endObject.tag = "StartEndObject";
        endObject.name = "End";
        endObject.AddComponent<BoxCollider>();
        endObject.GetComponent<BoxCollider>().center = new Vector3(-0.25f, 0.375f, 0);
        endObject.GetComponent<BoxCollider>().size = new Vector3(1, 0.75f, 1);
        gridTopMatrix[row, col] = endObject;
        endIndex = new int[] { row, col };

    }

    //populates gridMatrix with gridCubes
    private void PopulateMatrix()
    {
        //x and z coordiantes for instantiated cubes
        float x = -14.25f;
        float z = 10;

        //loop through the gridMatrix and add instantiated cubes in the matrix
        for (int i = 0;i <= gridMatrix.GetUpperBound(0); i++)
        {
            for(int j = 0;j <= gridMatrix.GetUpperBound(1); j++)
            {
                GameObject cube = Instantiate(gridCube, new Vector3(x, 0, z), Quaternion.identity);
                //name it to easily get index of the cube by its name
                cube.name = "gridCube-"+i+","+j;
                x += 1 + gap;
                gridMatrix[i, j] = cube;
                gridMatrixCubePositions[i, j] = cube.transform.position;
                //set the distance to the end node for a*
                float dist = Vector3.Distance(cube.transform.position, endObjectPos);
                cube.GetComponent<gridCubeNode>().distanceToEndNode = (float)Math.Round(dist * 1000f) / 1000f;
            }
            z -= 1 + gap;
            x = -14.25f;
        }
    }

    /*  accepts GameObject "gridCube" as parameter and 
     *  Instantiates Wall prefab ontop of the passed GameObject
     *  then adds the instantiated object in gridTopMatrix
     */
    public void InstantiateWall(GameObject EmptyCube)
    {
        //get index of EmptyCube object
        string[] temp = EmptyCube.name.Split('-');
        int row = System.Int32.Parse(temp[1].Split(',')[0]);
        int col = System.Int32.Parse(temp[1].Split(',')[1]);
        //if the top of the EmptyCube is empty then instantiate a wall
        //this is to prevent a bug if ray still hits gridCube that has something ontop
        if(gridTopMatrix[row,col] == null)
        {
            //get x and z coordinate of EmptyCube
            float x = EmptyCube.transform.position.x;
            float z = EmptyCube.transform.position.z;
            GameObject newWall = Instantiate(wall, new Vector3(x, 1.5f, z), Quaternion.identity);
            newWall.name = "Wall-" + row + "," + col;
            gridTopMatrix[row, col] = newWall;
        }
    }

    /*  accepts GameObject "gridCube" as parameter and 
     *  Instantiates Weight prefab ontop of the passed GameObject
     *  then adds the instantiated object in gridTopMatrix
     */
    public void InstantiateWeight(GameObject EmptyCube)
    {
        //get index of EmptyCube object
        string[] temp = EmptyCube.name.Split('-');
        int row = System.Int32.Parse(temp[1].Split(',')[0]);
        int col = System.Int32.Parse(temp[1].Split(',')[1]);
        //if the top of the EmptyCube is empty then instantiate a weight
        //this is to prevent a bug if ray still hits gridCube that has something ontop
        if (gridTopMatrix[row, col] == null)
        {
            //get x and z coordinate of EmptyCube
            float x = EmptyCube.transform.position.x;
            float z = EmptyCube.transform.position.z;
            GameObject newWeight = Instantiate(weight, new Vector3(x, 0.87f, z), Quaternion.Euler(new Vector3(0, 0, -90)));
            newWeight.name = "Weight-" + row + "," + col;
            gridTopMatrix[row, col] = newWeight;
        }
    }

    //destorys the passed in wall gameobject and removes it from gridTopMatrix
    public void RemoveWall(GameObject wallToBeRemoved)
    {
        //get index of the passed object
        string[] temp = wallToBeRemoved.name.Split('-');
        int row = System.Int32.Parse(temp[1].Split(',')[0]);
        int col = System.Int32.Parse(temp[1].Split(',')[1]);
        gridTopMatrix[row, col] = null;
        Destroy(wallToBeRemoved.GetComponent<BoxCollider>());
        StartCoroutine(WaitForWallAnimation(wallToBeRemoved));
    }
    IEnumerator WaitForWallAnimation(GameObject wallToBeRemoved)
    {
        wallToBeRemoved.GetComponent<Animator>().Play("WallDestroyAnimation");
        yield return new WaitForSeconds(0.5f);
        Destroy(wallToBeRemoved);
    }

    //destorys the passed in weight gameobject and removes it from gridTopMatrix
    public void RemoveWeight(GameObject weightToBeRemoved)
    {
        //get index of the passed object
        string[] temp = weightToBeRemoved.name.Split('-');
        int row = System.Int32.Parse(temp[1].Split(',')[0]);
        int col = System.Int32.Parse(temp[1].Split(',')[1]);
        gridTopMatrix[row, col] = null;
        Destroy(weightToBeRemoved.GetComponent<BoxCollider>());
        StartCoroutine(WaitForWeightAnimation(weightToBeRemoved));
    }
    IEnumerator WaitForWeightAnimation(GameObject weightToBeRemoved)
    {
        weightToBeRemoved.GetComponent<Animator>().Play("WeightDestroyAnimation");
        yield return new WaitForSeconds(0.5f);
        Destroy(weightToBeRemoved);
    }

    //goes through gridTopMatrix and Destorys all wall objects
    public void RemoveAllWalls()
    {
        for (int i = 0; i <= gridTopMatrix.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= gridTopMatrix.GetUpperBound(1); j++)
            {
                if (gridTopMatrix[i, j] != null && gridTopMatrix[i, j].CompareTag("Wall"))
                {
                    Destroy(gridTopMatrix[i, j]);
                    gridTopMatrix[i, j] = null;
                }
            }
        }
    }

    //goes through gridTopMatrix and Destorys all weight objects
    public void RemoveAllWeights()
    {
        for (int i = 0; i <= gridTopMatrix.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= gridTopMatrix.GetUpperBound(1); j++)
            {
                if (gridTopMatrix[i, j] != null && gridTopMatrix[i, j].CompareTag("Weight"))
                {
                    Destroy(gridTopMatrix[i, j]);
                    gridTopMatrix[i, j] = null;
                }
            }
        }
    }

    /*  accepts GameObject "gridCube" as parameter and 
     *  moves start prefab ontop of the passed GameObject
     *  then changes the startIndex accordingly
     */
    public void MoveStart(GameObject EmptyCube)
    {
        //get index of EmptyCube object
        string[] temp = EmptyCube.name.Split('-');
        int row = System.Int32.Parse(temp[1].Split(',')[0]);
        int col = System.Int32.Parse(temp[1].Split(',')[1]);
        //if the top of the EmptyCube is empty then move start object ontop of it
        if (gridTopMatrix[row, col] == null)
        {
            //get x and z coordinate of EmptyCube
            float x = EmptyCube.transform.position.x;
            float z = EmptyCube.transform.position.z;
            //set gridTopMatrix at startIndex to null then set startIndex to new location
            gridTopMatrix[startIndex[0], startIndex[1]] = null;
            startIndex[0] = row;
            startIndex[1] = col;
            gridTopMatrix[row, col] = startObject;
            startObject.transform.position = new Vector3(x-0.3f, 0.4f, z);
        }
    }

    /*  accepts GameObject "gridCube" as parameter and 
     *  moves end prefab ontop of the passed GameObject
     *  then changes the endIndex accordingly
     */
    public void MoveEnd(GameObject EmptyCube)
    {
        //get index of EmptyCube object
        string[] temp = EmptyCube.name.Split('-');
        int row = System.Int32.Parse(temp[1].Split(',')[0]);
        int col = System.Int32.Parse(temp[1].Split(',')[1]);
        //if the top of the EmptyCube is empty then move start object ontop of it
        if (gridTopMatrix[row, col] == null)
        {
            //get x and z coordinate of EmptyCube
            float x = EmptyCube.transform.position.x;
            float z = EmptyCube.transform.position.z;
            //set gridTopMatrix at startIndex to null then set startIndex to new location
            gridTopMatrix[endIndex[0], endIndex[1]] = null;
            endIndex[0] = row;
            endIndex[1] = col;
            gridTopMatrix[row, col] = endObject;
            endObject.transform.position = new Vector3(x+0.25f, 0.5f, z);
            //set all cubes distanceToEndNode to new values based on the ends new position
            Vector3 newEndPos = gridMatrix[row, col].transform.position;
            ResetDistanceValues(newEndPos);
        }
    }

    /* accepts Vector3 position of cube under Endnode.
     * Itterate though the gridMatrix and set all cubes distanceToEndNode to new value based on given Vector3
     */
    private void ResetDistanceValues(Vector3 endPos)
    {
        for (int i = 0; i <= gridMatrix.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= gridMatrix.GetUpperBound(1); j++)
            {
                float dist = Vector3.Distance(gridMatrix[i,j].transform.position, endPos);
                gridMatrix[i, j].GetComponent<gridCubeNode>().distanceToEndNode = (float)Math.Round(dist * 1000f) / 1000f;
            }
        }
    }

    /* accepts two Vector3 which are start and end position set before animating the grid and checks if they moved too much
     * then goes through the gridMatrix and resets all values in gridCubeNode and resets color
     * re-enables EventSystem, camera raycast, pulls out sideUI  
     */
    public void ResetGrid(bool onlyTimer, Vector3 startPos = default(Vector3), Vector3 endPos = default(Vector3))
    {
        /* because rigidbodies can cause unexpected issues check if startObject has moved too much from
         * where they were before animating the grid if true put them back to their previous location that got set before animation
         */
        if (!onlyTimer)
        {
            if (Vector3.Distance(startObject.transform.position, startPos) > 0.1f)
            {
                startObject.transform.position = startPos;
            }
            if (Vector3.Distance(endObject.transform.position, endPos) > 0.1f)
            {
                endObject.transform.position = endPos;
            }
        }
        //reset grid cube values
        for (int i = 0; i <= gridMatrix.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= gridMatrix.GetUpperBound(1); j++)
            {
                GameObject cube = gridMatrix[i, j];
                cube.GetComponent<gridCubeNode>().visited = false;
                cube.GetComponent<gridCubeNode>().previousNode = null;
                cube.GetComponent<gridCubeNode>().distance = int.MaxValue;
                cube.GetComponent<gridCubeNode>().AStarDistance = 100000;
                cube.transform.position = gridMatrixCubePositions[i, j];
                if (!onlyTimer)
                {
                    //remove emission from the cube
                    cube.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
                    //check if cube still has scipt 'gridCubeAnimationHelper' to prevent a bug. if true remove it
                    if (cube.GetComponent<gridCubeAnimationHelper>() != null)
                    {
                        Destroy(cube.GetComponent<gridCubeAnimationHelper>());
                    }
                    cube.GetComponent<MeshRenderer>().material.color = defaultCubeColor;
                    //remove rigidbody from the cube if it has one
                    if (cube.GetComponent<Rigidbody>() != null)
                    {
                        Destroy(cube.GetComponent<Rigidbody>());
                    }
                    //if cube has a wight on top reset its alpha value back to 120
                    /*if (gridTopMatrix[i,j]!= null && gridTopMatrix[i,j].name[1] == 'e')
                    {
                        gridTopMatrix[i, j].GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0.47f);
                    }*/
                }
            }
        }
        //set the openCloseButton alpha back to 1
        Image[] tempArray = UIManager.GetComponent<UISide>().closeSideUIButton.GetComponentsInChildren<Image>();
        for (int i = 0; i < tempArray.Length; i++)
        {
            Color tempColor = tempArray[i].color;
            tempColor.a = 1;
            UIManager.GetComponent<UISide>().closeSideUIButton.GetComponentsInChildren<Image>()[i].color = tempColor;
        }
        //open sideUI which also re-enalbes eventsystem and camera raycast
        UIManager.GetComponent<UIAnimationManager>().OpenSideUI();
    }

    /* goes through the grid top and puts a weight on empty spaces at random.
    //     50% chance for an empty cube to get a weight on top 
    */
    public void GenerateRandomWeights()
    {
        for (int i = 0; i <= gridTopMatrix.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= gridTopMatrix.GetUpperBound(1); j++)
            {
                // if gridtop is not empty then return
                if (gridTopMatrix[i, j] != null) { continue; }
                // random int from 1-10, 5 evens and 5 odds = 50%
                int rand = UnityEngine.Random.Range(1, 11);
                if(rand % 2 == 0)
                {
                    InstantiateWeight(gridMatrix[i, j]);
                }
            }
        }
    }

    /* TEMPORARY CODE
    //     does the same thing as "GenerateRandomWeights", but it should be maze algorithm
    */
    public void GenerateRandomWalls()
    {
        for (int i = 0; i <= gridTopMatrix.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= gridTopMatrix.GetUpperBound(1); j++)
            {
                // if gridtop is not empty then return
                if (gridTopMatrix[i, j] != null) { continue; }
                // random int from 1-10, 5 evens and 5 odds = 50%
                int rand = UnityEngine.Random.Range(1, 11);
                if (rand % 2 == 0)
                {
                    InstantiateWall(gridMatrix[i, j]);
                }
            }
        }
    }
}