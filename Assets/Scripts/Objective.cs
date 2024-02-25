using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [Header("References")]
    public UpgradeManager upgradeManager;
    public ScoreCounter scoreCounter;
    [Header("Objective")]
    public string objectiveName = "placeholder";
    public string objectiveVariable = "";
    public string objectiveVariableType = "upgrade";
    public bool showDifferentCompletionBarText = false;
    public string differentCompletionBarText = "";
    public string differentCompletionBarCompletedText = "";
    public float currentObjectiveProgress = 0f;
    public float objectiveNumberMaximum = 11f;
    public float objectiveNumberMinimum = 8f;
    public Transform objectiveLocation;
    public bool trailUnlock = false;

    void Start(){
        upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        scoreCounter = GameObject.Find("ScoreCounter").GetComponent<ScoreCounter>();
    }

    void Update(){
        UpdateObjectiveVariable();
    }

    private void UpdateObjectiveVariable(){
        if(objectiveVariableType == "upgrade"){
            currentObjectiveProgress = upgradeManager.GetValue(objectiveVariable);
        }
        if(objectiveVariableType == "currency"){
            currentObjectiveProgress = scoreCounter.GetCurrency(objectiveVariable);
        }
    }

    public float getObjectiveProgress(){
        if((currentObjectiveProgress - objectiveNumberMinimum) * (1f / (objectiveNumberMaximum - objectiveNumberMinimum)) > 1f){
            return 1f;
        }
        return (currentObjectiveProgress - objectiveNumberMinimum) * (1f / (objectiveNumberMaximum - objectiveNumberMinimum));
    }

    public bool getObjectiveFinished(){
        return currentObjectiveProgress >= objectiveNumberMaximum;
    }

    public string GetDifferentCompletionBarText(){
        if(currentObjectiveProgress >= objectiveNumberMaximum){
            return differentCompletionBarCompletedText;
        }
        else{
            return differentCompletionBarText;
        }
    }
}
