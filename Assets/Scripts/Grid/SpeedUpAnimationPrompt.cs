using System.Collections;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class SpeedUpAnimationPrompt : MonoBehaviour
{
    private NotificationManager notificationSpeedUp; //popup notification telling user to click spacebar to switch speed to 'Fast'

    /* Pop up top left prompting user to press spacebar
     * change algorithm animation speed to veryfast if user clicks spacebar
     */
    void Start()
    {
        notificationSpeedUp = GameObject.FindGameObjectWithTag("NotificationSpeedUp").GetComponent<NotificationManager>();
        StartCoroutine(WaitForPopUp());
    }
    //wait 8 seconds until popup comes up
    IEnumerator WaitForPopUp()
    {
        yield return new WaitForSeconds(8);
        notificationSpeedUp.OpenNotification();
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            notificationSpeedUp.CloseNotification();
            GetComponent<GridAnimationManager>().ChangeAnimationSpeed();
            Destroy(this);
        }

    }
}
