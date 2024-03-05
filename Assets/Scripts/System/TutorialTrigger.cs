using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public bool isTriggered = false;
    public TutorialScript tutorialScript;

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player" && !isTriggered){
            tutorialScript.NextTutorialStage();
            isTriggered = true;
        }
    }
}
