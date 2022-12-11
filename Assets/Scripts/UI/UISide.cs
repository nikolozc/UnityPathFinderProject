using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISide : MonoBehaviour
{
    public HorizontalSelector algoSelector; //horizontal selector that determines which algorithm should run when user clicks visualize button
    public HorizontalSelector speedSelector; //horizontal selector that determines how fast the algorithm animations will play
    public float algoSpeed; //speed that grid animations will play in
    public Camera mainCamera; 
    public EventSystem eventSys;
    public GameObject closeSideUIButton; //button thats responsible for opening and closing the sideUI
    public GameObject projectManager;
    public GameObject visualizeButton; //button that start the chosen algorithm
    public string tool; //string that is used by raycast that determines what basic click should do (add wall,remove wall,etc)
    public GameObject previousTool; //this is to keep track of the last tool - used for highighting (disable last highlighted button)s
    public int algoIndex = 0; //matches the option index of algoSelector to know which algorithm should be run

    /* change the algoIndex to match the 
    * algoSelector index when next/prev button is clicked on the selector
    */
    public void HandleAlgoSelectorChange()
    {
        algoIndex = algoSelector.index;
        ChangeVisualizeToolTip();
    }

    //changes VisualizeButton tooltip content based on algorithm chosen
    private void ChangeVisualizeToolTip()
    {
        switch (algoIndex)
        {
            //Dijkstra's Algo
            case 0:
                visualizeButton.GetComponent<TooltipContent>().description = "Visualize Dijkstra's Algorithm!(Weighted)";
                break;
            //A* Search
            case 1:
                visualizeButton.GetComponent<TooltipContent>().description = "Visualize A* Search Algorithm!(Weighted)";
                break;
            //BFS 
            case 2:
                visualizeButton.GetComponent<TooltipContent>().description = "Visualize Breadth First Search Algorithm!(Un-Weighted) - Will remove all weights if any";
                break;
            //DFS 
            case 3:
                visualizeButton.GetComponent<TooltipContent>().description = "Visualize Depth First Search Algorithm!(Un-Weighted) - Will remove all weights if any";
                break;
        }
    }

    //changes algoSpeed based on the choice from horizontal selector
    public void HandleSpeedSelectorChange()
    {
        switch (speedSelector.index)
        {
            //VeryFast
            case 0:
                algoSpeed = 0.01f;
                break;
            //Fast
            case 1:
                algoSpeed = 0.1f;
                break;
            //Medium
            case 2:
                algoSpeed = 0.5f;
                break;
            //Slow
            case 3:
                algoSpeed = 1;
                break;
            //VerySlow
            case 4:
                algoSpeed = 2;
                break;
        }
    }

    // close sideUI and start algorithm based on the algoIndex
    public void HandleVisualizeButtonClick()
    {
        /* Close side UI and show that it can't be opened by making the open button alpha < 1
         * CloseSideUI with parameter true will make sure to keep eventsystem and camera raycast turned off until algorithm is finished
         */
        GetComponent<UIAnimationManager>().CloseSideUI(true, false);
        Image[] tempArray = closeSideUIButton.GetComponentsInChildren<Image>();
        for (int i = 0; i < tempArray.Length; i++)
        {
            Color tempColor = tempArray[i].color;
            tempColor.a = 0.1f;
            closeSideUIButton.GetComponentsInChildren<Image>()[i].color = tempColor;
        }

        //start the algorithm
        switch (algoIndex)
        {
            case 0:
                projectManager.GetComponent<GridAlgorithms>().Initialize("Dijkstra", false);
                break;
            case 1:
                projectManager.GetComponent<GridAlgorithms>().Initialize("AStar", false);
                break;
            case 2:
                //clear all weights before running bfs - bfs is unweighted algorithm
                projectManager.GetComponent<GridManager>().RemoveAllWeights();
                projectManager.GetComponent<GridAlgorithms>().Initialize("BFS", false);
                break;
            case 3:
                //clear all weights before running bfs - dfs is unweighted algorithm
                projectManager.GetComponent<GridManager>().RemoveAllWeights();
                projectManager.GetComponent<GridAlgorithms>().Initialize("DFS", false);
                break;
        }
    }

    public void ToolAddWall()
    {
        SetButtonHighlight(GetComponent<UIAnimationManager>().addWallButton);
        tool = "AddWall";
        mainCamera.GetComponent<CameraRayCast>().ToolChange(tool);
    }
    public void ToolRemoveWall()
    {
        SetButtonHighlight(GetComponent<UIAnimationManager>().removeWallButton);
        tool = "RemoveWall";
        mainCamera.GetComponent<CameraRayCast>().ToolChange(tool);
    }
    public void ToolAddWeight()
    {
        SetButtonHighlight(GetComponent<UIAnimationManager>().addWeightButton);
        tool = "AddWeight";
        mainCamera.GetComponent<CameraRayCast>().ToolChange(tool);
    }
    public void ToolRemoveWeight()
    {
        SetButtonHighlight(GetComponent<UIAnimationManager>().removeWeightButton);
        tool = "RemoveWeight";
        mainCamera.GetComponent<CameraRayCast>().ToolChange(tool);
    }

    //let previously highlighted tool go back to normal state and highlight given gameobject instead
    private void SetButtonHighlight(GameObject toBeHighlighted)
    {
        previousTool.GetComponent<Button>().animationTriggers.normalTrigger = "Normal";
        //disable and enable the button again to update its trigger state (it was stuck on highlighted until you hover over the button)
        previousTool.GetComponent<Button>().enabled = false;
        previousTool.GetComponent<Button>().enabled = true;
        previousTool = toBeHighlighted;
        previousTool.GetComponent<Button>().animationTriggers.normalTrigger = "Highlighted";
    }

    //re-highlights the button that was selected before sideUI closed
    public void HighlightSelectedButton()
    {
        switch (tool)
        {
            case "AddWall":
                GetComponent<UIAnimationManager>().addWallButton.GetComponent<Button>().Select();
                break;
            case "RemoveWall":
                GetComponent<UIAnimationManager>().removeWallButton.GetComponent<Button>().Select();
                break;
            case "AddWeight":
                GetComponent<UIAnimationManager>().addWeightButton.GetComponent<Button>().Select();
                break;
            case "RemoveWeight":
                GetComponent<UIAnimationManager>().removeWeightButton.GetComponent<Button>().Select();
                break;
        }
    }

    //closes the side ui and reopens the Tutorial modal window
    public void TutorialReopen()
    {
        GetComponent<UIAnimationManager>().CloseSideUI(false,true);
        GetComponent<UIAnimationManager>().tutorialModalWindow.OpenWindow();
    }


    public void SettingsButtonClick()
    {
        GetComponent<UISettingsManager>().OpenSettingsPanel();
    }

    public void SpeedComparisonsButtonClick()
    {
        // continue only if atleast 1 timer data was collected
        if(projectManager.GetComponent<GridAlgorithms>().totalTimeAStar != 0 || projectManager.GetComponent<GridAlgorithms>().totalTimeBFS != 0)
        {
            // visualize speed comparisons 
            projectManager.GetComponent<GridAnimationManager>().SpeedVisualizationAnimation();
        }
        // give user notification that theres no data collected
        else
        {
            Debug.Log("no data collected");
        }
    }

    public void GenerateRandomWeightsButtonClick()
    {
        projectManager.GetComponent<GridManager>().GenerateRandomWeights();
    }
    public void GenerateRandomWallsButtonClick()
    {
        projectManager.GetComponent<GridManager>().GenerateRandomWalls();
    }

}