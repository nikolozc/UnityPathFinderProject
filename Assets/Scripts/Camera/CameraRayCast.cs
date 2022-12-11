using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRayCast : MonoBehaviour
{
    public GameObject projectManager; //GameObject that holds Grid scripts

    private bool startEndObjectHit = false; //bool to check if start or end object was hit with raycast
    private GameObject startEndObject; //start/end object that got hit with raycast
    private GameObject UIManager;
    private string tool;

    void Start()
    {
        UIManager = projectManager.GetComponent<GridManager>().UIManager;
        tool = UIManager.GetComponent<UISide>().tool;
    }

    public void ToolChange(string toolString)
    {
        tool = toolString;
    }

    // Update is called once per frame
    void Update()
    {
        /*  raycast from camera to mouse cursor
         *  if ray hits emptycube on the grid - check to see if StartEndObject has been hit 
         *    if true move object on the empty grid else call "InstantiateWall" function from GridManager script
         *  if ray hits start or end object - move the object as long as mouse0 is held down and that emptycube on grid is not occupied
         */
        if (Input.GetMouseButton(0))
        {
            //make sure there is no UI infront of the game object first
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {

                    //check to see if start or end object was hit and if it was already hit or not
                    if (hit.collider.CompareTag("StartEndObject") && !startEndObjectHit)
                    {
                        startEndObjectHit = true;
                        startEndObject = hit.transform.gameObject;
                    }
                    //make sure start/end object shouldnt be moved - fix for bug with removeWall and removeWeight
                    else if (hit.collider.CompareTag("EmptyCube") && startEndObjectHit)
                    {
                        StartEndObjectShouldMove(hit.transform.gameObject);
                    }
                    //check to see if user wants to remove walls and if ray hit a wall
                    else if (tool == "RemoveWall" && hit.collider.CompareTag("Wall"))
                    {
                        if (!StartEndObjectShouldMove(hit.transform.gameObject))
                        {
                            projectManager.GetComponent<GridManager>().RemoveWall(hit.transform.gameObject);
                        }
                    }
                    //check to see if user wants to add weights and if ray hit an empty cube
                    else if (tool == "AddWeight" && hit.collider.CompareTag("EmptyCube"))
                    {
                        if (!StartEndObjectShouldMove(hit.transform.gameObject))
                        {
                            projectManager.GetComponent<GridManager>().InstantiateWeight(hit.transform.gameObject);
                        }
                    }
                    //check to see if user wants to remove weights and if ray hit a weight
                    else if (tool == "RemoveWeight" && hit.collider.CompareTag("Weight"))
                    {
                        if (!StartEndObjectShouldMove(hit.transform.gameObject))
                        {
                            projectManager.GetComponent<GridManager>().RemoveWeight(hit.transform.gameObject);
                        }
                    }
                    else if (tool == "AddWall" && hit.collider.CompareTag("EmptyCube"))
                    {
                        if (!StartEndObjectShouldMove(hit.transform.gameObject))
                        {
                            projectManager.GetComponent<GridManager>().InstantiateWall(hit.transform.gameObject);
                        }
                    }
                }
            }

        }
        //if startEndObject is true when letting go of left click then set it to false
        if (Input.GetMouseButtonUp(0) && startEndObjectHit)
        {
            startEndObjectHit = false;
        }
        //shortcut for add wall
        if (Input.GetKey("e"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.CompareTag("EmptyCube"))
                {
                    projectManager.GetComponent<GridManager>().InstantiateWall(hit.transform.gameObject);
                }
            }
        }
        //shortcut for remove wall
        if (Input.GetKey("r"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    projectManager.GetComponent<GridManager>().RemoveWall(hit.transform.gameObject);
                }
            }
        }

        //shortcut for add weight
        if (Input.GetKey("w"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.CompareTag("EmptyCube"))
                {
                    projectManager.GetComponent<GridManager>().InstantiateWeight(hit.transform.gameObject);
                }
            }
        }

        //shortcut for remove weight
        if (Input.GetKey("q"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.CompareTag("Weight"))
                {
                    projectManager.GetComponent<GridManager>().RemoveWeight(hit.transform.gameObject);
                }
            }
        }

    }

    /* accepts gameobject 'hit' and checks if startend object should be moved or not
     * move the start / end object and return true if it should be moved else return false
     */
    private bool StartEndObjectShouldMove(GameObject hit)
    {
        if (startEndObjectHit)
        {
            //if startEndObjects name starts with S then call moveStart else moveEnd
            if (startEndObject.name[0] == 'S')
            {
                projectManager.GetComponent<GridManager>().MoveStart(hit);
                return true;
            }
            else
            {
                projectManager.GetComponent<GridManager>().MoveEnd(hit);
                return true;
            }
        }
        else
        {
            return false;
        }
    }
}
