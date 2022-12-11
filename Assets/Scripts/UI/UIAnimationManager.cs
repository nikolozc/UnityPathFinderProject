using System.Collections;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAnimationManager : MonoBehaviour
{
    public ModalWindowManager tutorialModalWindow; //modal window that opens up after welcome screen goes away
    public GameObject projectManager; 
    public GameObject sideUI;
    public GameObject sideUIOpenCloseButton; //button thats responsible for opening and closing sideUI
    public Image sideUIOpenCloseButtonImage;
    public Image sideUIOpenCloseButtonHImage;
    public Sprite sideUIOpenCloseSpriteLeft;
    public Sprite sideUIOpenCloseSpriteRight;
    public EventSystem eventSys;
    public Camera mainCamera;
    public GameObject visualizerButton; //button that start the chosen algorithm
    public GameObject algoSelector; //selects which algorithm should be run whe user click the visualize button
    public GameObject prefabUIPop; //prefab of gameobject holding spite that plays animation when sideUI is closed
    public GameObject speedSelector; //horizontal selector that determines how fast the algorithm animations will play
    public GameObject addWallButton; //button that changes the 'tool' to AddWall
    public GameObject removeWallButton; //button that changes the 'tool' to RemoveWall
    public GameObject addWeightButton; //button that changes the 'tool' to AddWeight
    public GameObject removeWeightButton; //button that changes the 'tool' to RemoveWeight
    public GameObject removeAllWallsButton; //button that removes all placed walls
    public GameObject removeAllWeightsButton; //button that removes all placed weights
    public GameObject generateRandomWallsButton; //button that generates random walls on the grid
    public GameObject generateRandomWeightsButton; //button that generates random weights on the grid
    public GameObject speedComparisonsButton; //button that displays the time comparisons of different algorithms 
    public GameObject openTutorialButton; //button that opens up tutorial window agian
    public GameObject openSettingsButton; //button that opens settings panel
    public Canvas canvas;

    private bool sideUIIsOpen = true;
    private GameObject[] arrayUIPop; //holds all UIPop gameobjects that got instantiated to later be destroyed 
    private GameObject[] sideUIObjects; //array of ui objects that need to be deactivated when closing the panel

    private void Start()
    {
        // set arrayUIPop size to the amount of gameobjects that will use prefabUIPop for the animation
        sideUIObjects = new GameObject[] { visualizerButton, algoSelector, speedSelector, addWallButton, removeWallButton,
        addWeightButton, removeWeightButton, removeAllWallsButton, removeAllWeightsButton, openTutorialButton, openSettingsButton, generateRandomWallsButton, generateRandomWeightsButton, speedComparisonsButton};
        arrayUIPop = new GameObject[sideUIObjects.Length];
    }
    //wait for welcomeWindow animation to finish until the tutorial panel and grid is activated
    public void WelcomeWindowClose()
    {
        StartCoroutine(WaitForWindowClose());
    }
    IEnumerator WaitForWindowClose()
    {
        yield return new WaitForSeconds(2);

        tutorialModalWindow.OpenWindow();
        projectManager.SetActive(true);
        mainCamera.GetComponent<CameraMovement>().enabled = true;
    }


    /* wait for sideUI to finish close animation to change button image
     * deactivate EventSystem and "CameraRayCast" script 
     * to make sure no ui can be clicked until animation is finished
     * accepts bool 'isCalledFromAlgo' to determine if the function is being called
     * from close button or algorithm start. if its false then reactivate camera raycast and eventsystem
     * second bool disableAfter determines if the UI should be disabled after animation
     */
    public void CloseSideUI(bool isCalledFromAlgo, bool disableAfter)
    {
        eventSys.gameObject.SetActive(false);
        mainCamera.GetComponent<CameraRayCast>().enabled = false;
        //deactivate all UI elements in SideUI that are in sideUIObjects array until opened agian
        DeactivateSideUIElements();

        sideUI.GetComponent<Animator>().Play("SideUIClose");
        StartCoroutine(WaitForSideUIClose(isCalledFromAlgo, disableAfter));
    }
    IEnumerator WaitForSideUIClose(bool isCalledFromAlgo, bool disableAfter)
    {
        yield return new WaitForSeconds(1);
        sideUIOpenCloseButtonImage.sprite = sideUIOpenCloseSpriteRight;
        sideUIOpenCloseButtonHImage.sprite = sideUIOpenCloseSpriteRight;
        if (!isCalledFromAlgo)
        {
            eventSys.gameObject.SetActive(true);
            mainCamera.GetComponent<CameraRayCast>().enabled = true;
        }
        sideUIOpenCloseButton.GetComponent<Animator>().Play("Normal");
        if (disableAfter)
        {
            sideUI.SetActive(false);
        }
    }


    /* wait for sideUI to finish open animation to change button image
     * deactivate EventSystem and "CameraRayCast" script 
     * to make sure no ui can be clicked until animation is finished
     */
    public void OpenSideUI()
    {
        eventSys.gameObject.SetActive(false);
        mainCamera.GetComponent<CameraRayCast>().enabled = false;
        //reactivate all UI elements in SideUI that are in sideUIObjects array
        ReactivateSideUIElements();
        sideUI.GetComponent<Animator>().Play("SideUIOpen");
        StartCoroutine(WaitForSideUIOpen());
    }
    IEnumerator WaitForSideUIOpen()
    {
        yield return new WaitForSeconds(1);
        sideUIOpenCloseButtonImage.sprite = sideUIOpenCloseSpriteLeft;
        sideUIOpenCloseButtonHImage.sprite = sideUIOpenCloseSpriteLeft;
        eventSys.gameObject.SetActive(true);
        mainCamera.GetComponent<CameraRayCast>().enabled = true;
        sideUIOpenCloseButton.GetComponent<Animator>().Play("Normal");
        GetComponent<UISide>().HighlightSelectedButton();
    }

    //if sideUI is open call close funtion else call open function
    public void HandleSideUIOpenCloseButtonClick()
    {
        if (sideUIIsOpen)
        {
            CloseSideUI(false,false);
        }
        else
        {
            OpenSideUI();
        }
        sideUIIsOpen = !sideUIIsOpen;
    }

    /*
     *  Additional Feature Not Finished    -- needs to constantly check if button animation state is Highlighted or Normal (not worth)
    //connects sideUICloseOpenButton onhover animation with sideUI Animator to play animation in sync
    public void SideUIOnHoverAnimationSync()
    {
        //check if sideUI is closed so the animation does not play when its open
        if (!sideUIIsOpen)
        {
            //sideUI.GetComponent<Animator>().SetBool("Highlighted", true);
        }
    }
    */

    /* Instantiate prefabUIPop for every element in sideUIObject and add it to 
     * arrayUIPop to later be destroyed after playing its animation 
     * then deactivate all elements in sideUIObject
     */
    private void DeactivateSideUIElements()
    {
        for(int i = 0;i < sideUIObjects.Length; i++)
        {
            //parent it so it can stretch its dimenshions to match sideUIObjects[i]
            arrayUIPop[i] = Instantiate(prefabUIPop, sideUIObjects[i].transform);
            //unparent it so it can play animation before getting destroyed
            arrayUIPop[i].transform.SetParent(canvas.transform);
            sideUIObjects[i].SetActive(false);
        }
        DestroyPrefabUIPops();
    }

    //reactivates all elements in sideUIObjects
    private void ReactivateSideUIElements()
    {
        for (int i = 0; i < sideUIObjects.Length; i++)
        {
            sideUIObjects[i].SetActive(true);
        }
    }

    //destroy everything in arrayUIPop after animation finishes
    private void DestroyPrefabUIPops()
    {
        for(int i =0;i < arrayUIPop.Length; i++)
        {
            StartCoroutine(WaitForPopAnimation(arrayUIPop[i]));
        }

    }

    IEnumerator WaitForPopAnimation(GameObject objectToBeDestroyed)
    {
        yield return new WaitForSeconds(1);
        Destroy(objectToBeDestroyed);
    }
}
