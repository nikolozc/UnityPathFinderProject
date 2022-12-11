using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISettingsManager : MonoBehaviour
{
    public GameObject ProjectManager;
    public GameObject SettingsPanel;
    public Resolution[] resolutions;
    public Dropdown resolutionDropdown;
    public GameObject MainCamera;
    public SliderManager cameraMovementSpeedSlider;
    public GameObject VNCColorImage; // VisitedNodeColor preview image
    public SliderManager VNCSliderRed;
    public SliderManager VNCSliderGreen;
    public SliderManager VNCSliderBlue;
    public GameObject PNCColorImage; // PathNodeColor preview image
    public SliderManager PNCSliderRed;
    public SliderManager PNCSliderGreen;
    public SliderManager PNCSliderBlue;
    public GameObject WeightInputField;
    public GameObject MultiRunInputField;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        //turn resolution into string
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            //if current resolution matches the option then save the index so dropdown value can be changed into it
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetScreenResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width,res.height,Screen.fullScreen);
    }

    public void CloseSettingsPanel()
    {
        SettingsPanel.SetActive(false);
        GetComponent<UIAnimationManager>().sideUI.SetActive(true);

    }

    public void OpenSettingsPanel()
    {
        GetComponent<UIAnimationManager>().sideUI.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void SetQualitySettings(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetCameraMovementSpeed()
    {
        MainCamera.GetComponent<CameraMovement>().speed = (int)cameraMovementSpeedSlider.mainSlider.value;


    }

    public void SetVisitedNodeColor()
    {
        Color32 updatedColor = new Color32((byte)VNCSliderRed.mainSlider.value, (byte)VNCSliderGreen.mainSlider.value, (byte)VNCSliderBlue.mainSlider.value, 255);
        // set the preview image color
        VNCColorImage.GetComponent<Image>().color = updatedColor; 

        //change visited node color in gridAnimationManager
        ProjectManager.GetComponent<GridAnimationManager>().cubeColor = updatedColor;
    }

    public void SetPathNodeColor()
    {
        Color32 updatedColor = new Color32((byte)PNCSliderRed.mainSlider.value, (byte)PNCSliderGreen.mainSlider.value, (byte)PNCSliderBlue.mainSlider.value, 255);
        // set the preview image color
        PNCColorImage.GetComponent<Image>().color = updatedColor;
        ProjectManager.GetComponent<GridAnimationManager>().pathColor = updatedColor;
    }

    public void SetWeightAmount()
    {
        // change weight cost in gridAlgo script
        ProjectManager.GetComponent<GridAlgorithms>().weightCost = int.Parse(WeightInputField.GetComponent<TMP_InputField>().text);
    }

    public void SetMultiRun()
    {
        ProjectManager.GetComponent<GridAlgorithms>().multiRun = int.Parse(MultiRunInputField.GetComponent<TMP_InputField>().text);
    }
}
