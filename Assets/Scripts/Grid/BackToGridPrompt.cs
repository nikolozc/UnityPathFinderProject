using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToGridPrompt : MonoBehaviour
{
    public NotificationManager notificationBackToGrid;

    private void Start()
    {
        notificationBackToGrid = GameObject.FindGameObjectWithTag("NotificationBackToGrid").GetComponent<NotificationManager>();
        notificationBackToGrid.OpenNotification();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            notificationBackToGrid.CloseNotification();
            GetComponent<GridAnimationManager>().backInitiated = true;
            Destroy(this);
        }
    }
}
