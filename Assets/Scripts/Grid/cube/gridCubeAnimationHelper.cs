using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class gridCubeAnimationHelper : MonoBehaviour
{
    private GameObject projectManager;
    private GameObject MainCamera;
    private Color cubeColor; //Color that cube will Lerp its color into
    private float animationSpeed; //passed in float from GridAnimationManager that determines the speed in which cube should animate
    private bool flag = false; //flag for cube reaching its target to move back down
    private bool isHalfDone = false; //flag for cube coming back down to its original position - used to disable the script
    private bool isFinished = false; //flag for cube finishing the moveTowards - this + check if color fully changed = destory script
    private bool isPath = false; //isPath bool taken from "GridAnimationManager" to determine color to choose for cube
    private Vector3 pos; //position set before moving the object on the Y axis - to be able to come back to its original position
    private Vector3 scale; //previous scale
    private bool isSpeedVisualization = false;
    private int randomX;
    private int randomZ;
    private bool inTransition; //bool taken from CameraMovementSpeedVis script that determines if camera is still in transition
    private bool backInitiated = false; //bool taken from notification-backToGrid, that determines if script should turn back from speed visualization animation

    // Start is called before the first frame update
    void Start()
    {
        projectManager = GameObject.FindWithTag("ProjectManager");
        MainCamera = GameObject.FindWithTag("MainCamera");
        isPath = projectManager.GetComponent<GridAnimationManager>().isPath;
        cubeColor = isPath ? projectManager.GetComponent<GridAnimationManager>().pathColor : projectManager.GetComponent<GridAnimationManager>().cubeColor;
        animationSpeed = projectManager.GetComponent<GridAnimationManager>().animationSpeed;
        pos = transform.position;
        scale = transform.localScale;
        isSpeedVisualization = projectManager.GetComponent<GridAnimationManager>().isSpeedVisualization;
        //generate 2 random values to add to x and z for speed visualization animation
        randomX = Random.Range(5, 25);
        randomZ = Random.Range(5, 25);
}

    // Update is called once per frame
    void Update()
    {
        // determine if it should play speed visualization animation or grid animation 
        if (!isSpeedVisualization)
        {
            //lerps from current color to specified 'cubeColor' speed of 1/8*lerpSpeed
            GetComponent<MeshRenderer>().material.color = Color.Lerp(
                GetComponent<MeshRenderer>().material.color, cubeColor, Mathf.PingPong(Time.time, 1 / (8 * animationSpeed)));
            //move the cube up speed of 2/lerpSpeed
            float step = (2 / animationSpeed) * Time.deltaTime;
            Vector3 target = new Vector3(pos.x, pos.y + 1, pos.z);

            /* check if it reached target if true set flag to true
             * also set isHalfDone to true to stop MoveTowards
             */
            if (Vector3.Distance(transform.position, target) < 0.005f)
            {
                flag = true;
                isHalfDone = true; 
            }
            if (isHalfDone && Vector3.Distance(transform.position, pos) < 0.005f)
            {
                step = 0;
                isFinished = true;
            }
            //keep moving it up until it reaches target then move it back down
            if (!flag)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, step);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pos, step);
            }

            //if movement is finished check if color Lerp finished if true turn emission on and destory self
            if (isFinished && GetComponent<MeshRenderer>().material.color == cubeColor)
            {
                gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", cubeColor * (isPath ? 4 : 2));
                Destroy(this);
            }
        }
        // Speed Visualization Animation
        else
        {
            // inTransition logic should be an event instead - temporary logic
            inTransition = MainCamera.GetComponent<CameraMovementSpeedVisualization>().inTransition;
            // backInitiated should also be an event call - temporary logic
            backInitiated = projectManager.GetComponent<GridAnimationManager>().backInitiated;
            //check if current cube is [9,10] , [9,12] , [9,18] , [9,20]
            if (gameObject.name == "gridCube-9,10" || gameObject.name == "gridCube-9,12" || gameObject.name == "gridCube-9,18" || gameObject.name == "gridCube-9,20")
            {
                // scale up the cube on y axis based on total average time for that algo after camera finishes transition
                if (!inTransition)
                {
                    //A*
                    if(gameObject.name == "gridCube-9,10")
                    {
                        //disable animator and reenable it after going back to main grid view - cant scale with animator enabled
                        GetComponent<Animator>().enabled = false;
                        float Yscale = (projectManager.GetComponent<GridAlgorithms>().weightedRuns == 0) ? 0 : (float)projectManager.GetComponent<GridAlgorithms>().totalTimeAStar / (projectManager.GetComponent<GridAlgorithms>().weightedRuns/2);
                        //max Yscale value is 6
                        if(Yscale > 6) { Yscale = 6; }
                        //min Yscale value is 0.3
                        if (Yscale < 0.3 && Yscale != 0) { Yscale = 0.3f; }

                        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, Yscale, 1), Time.deltaTime * 5);
                        transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, Yscale / 2, pos.z), Time.deltaTime * 5);

                        //change color to path color for the fastest (smallest time) weighted algo else to cube traversed color
                        cubeColor = ((float)projectManager.GetComponent<GridAlgorithms>().totalTimeAStar < (float)projectManager.GetComponent<GridAlgorithms>().totalTimeDijkstra) ? projectManager.GetComponent<GridAnimationManager>().pathColor : projectManager.GetComponent<GridAnimationManager>().cubeColor;

                        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", cubeColor * 3);

                        //enable cubes canvas
                        Transform cubeCanvas = transform.Find("CubeCanvas");
                        cubeCanvas.gameObject.SetActive(true);
                        //change cube canvas txts
                        cubeCanvas.transform.Find("txtAlgorithmType").GetComponent<TMP_Text>().text = "A Star";
                        cubeCanvas.transform.Find("txtAlgorithmSpeed").GetComponent<TMP_Text>().text = projectManager.GetComponent<GridAlgorithms>().totalTimeAStar.ToString();
                    }
                    //Dijkstra
                    if (gameObject.name == "gridCube-9,12")
                    {
                        GetComponent<Animator>().enabled = false;
                        float Yscale = (projectManager.GetComponent<GridAlgorithms>().weightedRuns == 0) ? 0 : (float)projectManager.GetComponent<GridAlgorithms>().totalTimeDijkstra / (projectManager.GetComponent<GridAlgorithms>().weightedRuns/2);
                        if (Yscale > 6) { Yscale = 6; }
                        //min Yscale value is 0.3
                        if (Yscale < 0.3 && Yscale != 0) { Yscale = 0.3f; }

                        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, Yscale, 1), Time.deltaTime * 5);
                        transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, Yscale / 2, pos.z), Time.deltaTime * 5);

                        cubeColor = ((float)projectManager.GetComponent<GridAlgorithms>().totalTimeDijkstra < (float)projectManager.GetComponent<GridAlgorithms>().totalTimeAStar) ? projectManager.GetComponent<GridAnimationManager>().pathColor : projectManager.GetComponent<GridAnimationManager>().cubeColor;

                        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", cubeColor * 3);

                        //enable cubes canvas
                        Transform cubeCanvas = transform.Find("CubeCanvas");
                        cubeCanvas.gameObject.SetActive(true);
                        //change cube canvas txts
                        cubeCanvas.transform.Find("txtAlgorithmType").GetComponent<TMP_Text>().text = "Dijkstra";
                        cubeCanvas.transform.Find("txtAlgorithmSpeed").GetComponent<TMP_Text>().text = projectManager.GetComponent<GridAlgorithms>().totalTimeDijkstra.ToString();
                    }
                    //BFS
                    if (gameObject.name == "gridCube-9,18")
                    {
                        GetComponent<Animator>().enabled = false;
                        float Yscale = (projectManager.GetComponent<GridAlgorithms>().notWeightedRuns == 0) ? 0 : (float)projectManager.GetComponent<GridAlgorithms>().totalTimeBFS / (projectManager.GetComponent<GridAlgorithms>().notWeightedRuns/2);
                        if (Yscale > 6) { Yscale = 6; }
                        //min Yscale value is 0.3
                        if (Yscale < 0.3 && Yscale != 0) { Yscale = 0.3f; }

                        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, Yscale, 1), Time.deltaTime * 5);
                        transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, Yscale / 2, pos.z), Time.deltaTime * 5);

                        cubeColor = ((float)projectManager.GetComponent<GridAlgorithms>().totalTimeBFS < (float)projectManager.GetComponent<GridAlgorithms>().totalTimeDFS) ? projectManager.GetComponent<GridAnimationManager>().pathColor : projectManager.GetComponent<GridAnimationManager>().cubeColor;

                        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", cubeColor * 3);

                        //enable cubes canvas
                        Transform cubeCanvas = transform.Find("CubeCanvas");
                        cubeCanvas.gameObject.SetActive(true);
                        //change cube canvas txts
                        cubeCanvas.transform.Find("txtAlgorithmType").GetComponent<TMP_Text>().text = "BFS";
                        cubeCanvas.transform.Find("txtAlgorithmSpeed").GetComponent<TMP_Text>().text = projectManager.GetComponent<GridAlgorithms>().totalTimeBFS.ToString();
                    }
                    //DFS
                    if (gameObject.name == "gridCube-9,20")
                    {
                        GetComponent<Animator>().enabled = false;
                        float Yscale = (projectManager.GetComponent<GridAlgorithms>().notWeightedRuns == 0) ? 0 : (float)projectManager.GetComponent<GridAlgorithms>().totalTimeDFS / (projectManager.GetComponent<GridAlgorithms>().notWeightedRuns/2);
                        if (Yscale > 6) { Yscale = 6; }
                        //min Yscale value is 0.3
                        if (Yscale < 0.3 && Yscale != 0) { Yscale = 0.3f; }

                        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, Yscale, 1), Time.deltaTime * 5);
                        transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, Yscale / 2, pos.z), Time.deltaTime * 5);

                        cubeColor = ((float)projectManager.GetComponent<GridAlgorithms>().totalTimeDFS < (float)projectManager.GetComponent<GridAlgorithms>().totalTimeBFS) ? projectManager.GetComponent<GridAnimationManager>().pathColor : projectManager.GetComponent<GridAnimationManager>().cubeColor;

                        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", cubeColor * 3);

                        //enable cubes canvas
                        Transform cubeCanvas = transform.Find("CubeCanvas");
                        cubeCanvas.gameObject.SetActive(true);
                        //change cube canvas txts
                        cubeCanvas.transform.Find("txtAlgorithmType").GetComponent<TMP_Text>().text = "DFS";
                        cubeCanvas.transform.Find("txtAlgorithmSpeed").GetComponent<TMP_Text>().text = projectManager.GetComponent<GridAlgorithms>().totalTimeDFS.ToString();
                    }
                }
                // back to grid was initiated so return all the values back to how they were previously
                if (backInitiated)
                {
                    GetComponent<Animator>().enabled = true;
                    transform.localScale = scale;
                    transform.position = pos;
                    gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
                    Transform cubeCanvas = transform.Find("CubeCanvas");
                    cubeCanvas.gameObject.SetActive(false);
                }
            }
            else
            {
                bool backInitiatedFlag = false; // bool that determines if code in backInitiated section is done fully 
                // +40 to Y to make sure they are not in view anymore
                Vector3 target = new Vector3(pos.x + randomX, pos.y + 40, pos.z + randomZ);
                /* check if it reached target if true set flag to true
                 */
                if (Vector3.Distance(transform.position, target) < 0.005f)
                {
                    flag = true;
                }
                //keep moving it up until it reaches target 
                if (!flag)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, 15 * Time.deltaTime);
                }
                // Backwards animation + change back isSpeedVisual back to false + reactivate start and end objects 
                if (backInitiated)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pos, 25 * Time.deltaTime);
                    if(Vector3.Distance(transform.position, pos) < 0.001f) backInitiatedFlag = true;
                }
                //finished
                if (backInitiatedFlag)
                {
                    Destroy(this);
                }
            }
        }
        
    }
}
