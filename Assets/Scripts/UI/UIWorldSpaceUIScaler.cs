using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// NOT IN USE
public class UIWorldSpaceUIScaler : MonoBehaviour
{
    [SerializeField] private TMP_Text txtUIToBeScaled = null;
    [SerializeField] private bool isTop; //is it Top text (algo speed txt).
    private int scaleAmount = 2;
    private Vector3 prevPosition;

    private void OnMouseEnter()
    {
        // scale object
        txtUIToBeScaled.rectTransform.localScale = txtUIToBeScaled.rectTransform.localScale * scaleAmount;
        // move it away from original position based on if it was bottom txt or top
        prevPosition = txtUIToBeScaled.rectTransform.position;
        if (isTop)
        {
            txtUIToBeScaled.rectTransform.position = new Vector3(prevPosition.x+1, prevPosition.y+1, prevPosition.z);
        }
        else
        {
            txtUIToBeScaled.rectTransform.position = new Vector3(prevPosition.x-1, prevPosition.y-1, prevPosition.z);
        }
    }

    private void OnMouseExit()
    {
        // scale back to original size
        txtUIToBeScaled.rectTransform.localScale = txtUIToBeScaled.rectTransform.localScale / scaleAmount;

        // move it back to original position
        if (isTop)
        {
            txtUIToBeScaled.rectTransform.position = prevPosition;
        }
        else
        {
            txtUIToBeScaled.rectTransform.position = prevPosition;
        }
    }
}
