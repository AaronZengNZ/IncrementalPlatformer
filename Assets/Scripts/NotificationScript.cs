using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotificationScript : MonoBehaviour
{
    public Animator notificationAnimator;
    public TextMeshProUGUI notificationTitle;
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI secondaryNotificationText;
    IEnumerator Notify(){
        notificationAnimator.SetBool("Show", true);
        yield return new WaitForSeconds(1f);
        notificationAnimator.SetBool("Show", false);
    }

    public void ShowNotification(string text, string template = "none"){
        if(template == "trail"){
            notificationTitle.text = "Trail Unlocked";
            notificationText.text = "[" + text + "] trail has been unlocked!";
            secondaryNotificationText.text = "Equip it by clicking on the [swap trail] button in [misc].";
        }
        StartCoroutine(Notify());
    }
}
