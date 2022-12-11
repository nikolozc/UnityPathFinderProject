using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Michsky.UI.ModernUIPack;

public class CameraMovementSpeedVisualization : MonoBehaviour
{
    public bool inTransition; //determines if moving between main view and speed visualization cameras

    private GameObject projectManager;
    private GameObject UIManager;
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 targetPosition;
    private Vector3 velocity; //needed for smoothdamp
    private bool backInitiated;

    // Start is called before the first frame update
    void Start()
    {
        projectManager = GameObject.FindWithTag("ProjectManager");
        UIManager = GameObject.FindWithTag("UIManager");
        previousPosition = transform.position;
        previousRotation = transform.rotation;
        targetPosition = new Vector3(3.5f, 3f, -10f);
        inTransition = true;
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        backInitiated = projectManager.GetComponent<GridAnimationManager>().backInitiated;
        if (inTransition)
        {
            //move and rotate camera to target Position
            transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.5f), Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(targetPosition.x,targetPosition.y,targetPosition.z + 20) - transform.position), 15 * Time.deltaTime));
            //check if camera reached the target pos
            if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
            {
                inTransition = false;
                projectManager.AddComponent<BackToGridPrompt>();
            }
        }

        // go back to previous pos and rot if backInitiated was turned true
        if (backInitiated)
        {
            transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position,previousPosition,ref velocity, 0.5f), previousRotation);
            if (Vector3.Distance(transform.position, previousPosition) < 0.02f)
            {
                GetComponent<CameraMovement>().enabled = true;
                GetComponent<CameraRayCast>().enabled = true;
                projectManager.GetComponent<GridAnimationManager>().startObject.SetActive(true);
                projectManager.GetComponent<GridAnimationManager>().endObject.SetActive(true);
                projectManager.GetComponent<GridManager>().ResetGrid(false, projectManager.GetComponent<GridAnimationManager>().startObjPos, projectManager.GetComponent<GridAnimationManager>().endObjPos);
                UIManager.GetComponent<UIAnimationManager>().sideUI.SetActive(true);
                projectManager.GetComponent<GridAnimationManager>().isSpeedVisualization = false;
                projectManager.GetComponent<GridAnimationManager>().backInitiated = false;
                Destroy(this);
            }
        }
    }
}
