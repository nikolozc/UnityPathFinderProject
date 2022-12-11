using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class UITutorialManager : MonoBehaviour
{
    public GameObject prev; //button that switches description to the previous prompt
    public GameObject next; //button that switches description to the next prompt
    public ModalWindowManager modalWindow; 
    public GameObject skipButtonText; //button that closes the modalWindow
    public Image skipButtonIconImage;
    public GameObject skipButtonHText; //highlighted
    public Image skipButtonIconHImage; //highlighted
    public Sprite skipButtonIconTrue;
    public Sprite skipButtonIconFalse;
    public Image descriptionImage; //image next to description text

    public Sprite page3; //sprite for page 3
    public Sprite page4; //sprite for page 4

    private int descIndex = 0; //index for array of strings 'descriptions' to determine which page the user is on
    private bool isSkip = true; //bool that determines if the skip button should say skip or start
    private string[] descriptions; //array of string that will be shown in modalWindow 

    void Start()
    {
        //set up descriptions
        descriptions = new string[6];
        descriptions[0] = "Welcome to PathFinder Unity!\nYou can press the Skip button at any time to get started right away.\n\n" +
            "CONTROLS\nUse arrow keys/mouse to move the camera around and use Left Shift and Left Ctrl/scroll wheel to zoom in/out";

        descriptions[1] = "This application visualizes various pathfinding algorithms, " +
            "which are adapted for a 2D grid where movements from a node to another have a cost of 1 (for weighted algorithms)\n";

        descriptions[2] = "Choose an algorithm you want to see in action and set the animation speed.\n" +
            "Out of the four algorithms implemented DFS and BFS are unweighted and do not take weight nodes into account, whereas A* and Dijkstra do." +
            " Out of the four pathfinding algorithm options DFS is the worst and does not guarantee the shortest path, the other three do.";

        descriptions[3] = "You are able to add wall and weight nodes by " +
            "either choosing the desired 'tool' from the UI and clicking on the grid or by using keyboard shortcuts\n" +
            "Remove Weight - Q\nAdd Weight - W\nAdd Wall - E\nRemove Wall - R\n" +
            "Walls are impenetrable, weights however are more 'costly' to move though (cost of 10)";

        descriptions[4] = "Everytime you run weighted algorithm this application will visualize chosen alogrithm and run all the other weighted algorithms in the background to collect run time speed data, same goes for unweighted algorithms.\n Click 'Speed Comparisons' button to see comparison between algorithm run times.";

        descriptions[5] = "Once you've set everything up you can click the 'VISUALIZE' button and see algorithm of your choice in action!\n" +
            "If you want to see all the scripts that went into making this application visit my github github.com/nikolozc";
    }

    //changes descriptionImage based on descIndex
    private void ChangeImage()
    {
        switch (descIndex)
        {
            case 0:
                //set alpha to 0 to hide the image
                descriptionImage.color = new Color(1, 1, 1, 0);
                break;
            case 1:
                descriptionImage.color = new Color(1, 1, 1, 0);
                break;
            case 2:
                //set alpha to 1 to show the image
                descriptionImage.color = new Color(1, 1, 1, 1);
                descriptionImage.sprite = page3;
                break;
            case 3:
                descriptionImage.color = new Color(1, 1, 1, 1);
                descriptionImage.sprite = page4;
                break;
            case 4:
                descriptionImage.color = new Color(1, 1, 1, 0);
                break;
        }
    }
    /*  ModalWindow UI - Tutorial button function that changes
     *  the main text to what ever is stored in "tutorialDescriptions" at index+1
     *  
     *  If index reaches the end of the array deactivate the "Next" button until "Prev" button is pressed
     *  If index is 1 after button press then reactivate the "Prev" button 
     */
    public void TutorialNextButton()
    {
        descIndex++;
        if (descIndex == 1)
        {
            prev.SetActive(true);
        }
        else if(descIndex == descriptions.Length - 1)
        {
            next.SetActive(false);
            TutorialSkipButtonChange();
        }
        modalWindow.descriptionText = descriptions[descIndex];
        modalWindow.UpdateUI();
        ChangeImage();
    }


    /*  ModalWindow UI - Tutorial button function that changes
     *  the main text to what ever is stored in "tutorialDescriptions" at index-1
     *  
     *  If index is 0 after button press then deactivate the "Prev" button until "Next" button is pressed
     *  If index is array.length-2 after button press then reactivate the "Next" button 
     */
    public void TutorialPrevButton()
    {
        descIndex--;
        if (descIndex == 0)
        {
            prev.SetActive(false);
        }
        else if (descIndex == descriptions.Length - 2)
        {
            next.SetActive(true);
            TutorialSkipButtonChange();
        }
        modalWindow.descriptionText = descriptions[descIndex];
        modalWindow.UpdateUI();
        ChangeImage();
    }

        /* change the text and icon of the “skip” button
         * if index reaches array.length-1 based on “isSkip” boolean 
         */
        public void TutorialSkipButtonChange()
    {
        if (isSkip)
        {
            skipButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Start";
            skipButtonIconImage.sprite = skipButtonIconFalse;
            skipButtonHText.GetComponent<TMPro.TextMeshProUGUI>().text = "Start";
            skipButtonIconHImage.sprite = skipButtonIconFalse;
        }
        else
        {
            skipButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Skip";
            skipButtonIconImage.sprite = skipButtonIconTrue;
            skipButtonHText.GetComponent<TMPro.TextMeshProUGUI>().text = "Skip";
            skipButtonIconHImage.sprite = skipButtonIconTrue;
        }
        isSkip = !isSkip;
    }

}
