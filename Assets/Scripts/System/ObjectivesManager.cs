using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectivesManager : MonoBehaviour
{
    [Header ("ImportantReferences")]
    public Animator animator;
    public TextMeshProUGUI openCloseText;
    public Animator animator2;
    public TextMeshProUGUI openCloseText2;
    public Animator animator3;
    public TextMeshProUGUI openCloseText3;
    public Animator animatorBigTab;
    public GameObject bigTab;
    public GameObject[] bigTabComponents;
    public GameObject[] lockedTrailObjects;
    public Button[] trailButtons;
    public NotificationScript notificationScript;

    [Header ("References")]
    public Player playerScript;
    public TextMeshProUGUI objectiveText;
    public Slider objectiveProgressSlider;
    public TextMeshProUGUI objectiveProgressText;
    public Objective[] objectives;
    public Transform playerTransform;
    public Transform objectiveLocation;
    public Transform objectiveArrow;
    public SpriteRenderer objectiveArrowSprite;
    public float objectiveDistanceToHide = 8f;
    public float objectiveDistanceHidden = 4f;
    public GameObject objectiveArrowSpriteObj;
    public TextMeshProUGUI objectiveArrowToggledText;
    [Header("Variables")]
    public bool showObjectives = false;
    public bool showTab2 = false;
    public bool showTab3 = false;
    public bool showBigTab = false;
    private float currentObjectiveProgress;
    private float lerpedObjectiveProgress;
    public float lerpSpeed = 0.2f;
    public float currentObjective = 1f;
    public float objectiveChangeDuration = 3f;
    public float changingObjective;
    public float trailsUnlocked = 1f;
    private float currentComponent = -1f;
    private bool objectiveArrowActivated = true;
    private string notificationText = "null";
    private Color notificationTextColor;
    // Start is called before the first frame update
    void Start()
    {
        showObjectives = true;
        showTab2 = true;
        showTab3 = false;
        showBigTab = false;
        CheckPrefs();
    }

    void Update(){
        animator.SetBool("Showing", showObjectives);
        animator2.SetBool("Showing", showTab2);
        animator3.SetBool("Showing", showTab3);
        animatorBigTab.SetBool("Showing", true);
        bigTab.SetActive(showBigTab);
        lerpedObjectiveProgress = Mathf.Lerp(lerpedObjectiveProgress, currentObjectiveProgress, Time.deltaTime * (1f / lerpSpeed));
        objectiveArrowSpriteObj.SetActive(objectiveArrowActivated);
        playerScript.trailsUnlocked = trailsUnlocked;
        GetObjectiveVariables();
        GetNextObjective();
        PointToObjective();
        LockedTrails();
    }

    public void ToggleObjectiveArrow(){
        objectiveArrowActivated = !objectiveArrowActivated;
        if(objectiveArrowActivated){
            objectiveArrowToggledText.text = "X";
        }
        else{
            objectiveArrowToggledText.text = "";
        }
    }

    private void LockedTrails(){
        for(int i = 0; i < lockedTrailObjects.Length; i++){
            if(i < trailsUnlocked){
                lockedTrailObjects[i].SetActive(false);
                trailButtons[i].interactable = true;
            }
            else{
                lockedTrailObjects[i].SetActive(true);
                trailButtons[i].interactable = false;
            }
        }
    }

    public void ActivateBigTabComponent(float component){
        if(component == 999f){
            showBigTab = false;
            return;
        }
        if(component == currentComponent){
            showBigTab = !showBigTab;
        }else{
            showBigTab = true;
            for(int i = 0; i < bigTabComponents.Length; i++){
                bigTabComponents[i].SetActive(false);
            }
            bigTabComponents[(int)component].SetActive(true);
        }
        currentComponent = component;
    }
    
    private void PointToObjective(){
        objectiveArrow.position = playerTransform.position;
        objectiveArrow.gameObject.SetActive(true);
        Vector3 direction = objectiveLocation.position - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        objectiveArrow.rotation = Quaternion.Euler(0f, 0f, angle);
        float distance = Vector2.Distance(playerTransform.position, objectiveLocation.position);
        if(distance < objectiveDistanceToHide){
            float transparency = (distance - objectiveDistanceHidden) / (objectiveDistanceToHide - objectiveDistanceHidden) / 2f;
            objectiveArrowSprite.color = new Color(.5f, .6f, .7f, transparency);
        }
        else{
            objectiveArrowSprite.color = new Color(.5f, .6f, .7f, .5f);
        }
    }

    private void CheckPrefs(){
        if(PlayerPrefs.HasKey("currentObjective")){
            currentObjective = PlayerPrefs.GetFloat("currentObjective");
        }
        else{
            PlayerPrefs.SetFloat("currentObjective", 0f);
        }
        if(PlayerPrefs.HasKey("trailsUnlocked")){
            trailsUnlocked = PlayerPrefs.GetFloat("trailsUnlocked");
        }
        else{
            PlayerPrefs.SetFloat("trailsUnlocked", 1f);
        }
    }

    public void SetPrefs(){
        PlayerPrefs.SetFloat("currentObjective", currentObjective);
        PlayerPrefs.SetFloat("trailsUnlocked", trailsUnlocked);
    }

    private void GetNextObjective(){
        if(objectives[(int)currentObjective].getObjectiveFinished() && currentObjective < objectives.Length - 1){
            changingObjective += Time.deltaTime;
            if(changingObjective >= objectiveChangeDuration){
                changingObjective = 0f;
                if(objectives[(int)currentObjective].trailUnlock){
                    trailsUnlocked++;
                    if(objectives[(int)currentObjective].isNotification == true){
                        notificationScript.ShowNotification(notificationText, notificationTextColor, "trail");
                    }
                }
                currentObjective++;
            }
        }
    }
    private void GetObjectiveVariables(){
        if(objectives[(int)currentObjective].objectiveLocation != null){
            objectiveLocation = objectives[(int)currentObjective].objectiveLocation;
        }
        currentObjectiveProgress = objectives[(int)currentObjective].getObjectiveProgress();
        objectiveProgressSlider.value = lerpedObjectiveProgress;
        if(objectives[(int)currentObjective].showDifferentCompletionBarText){
            objectiveProgressText.text = "Currently: " + objectives[(int)currentObjective].GetDifferentCompletionBarText();
        }
        else{
            objectiveProgressText.text = "Currently: [" + objectives[(int)currentObjective].currentObjectiveProgress + "/" + objectives[(int)currentObjective].objectiveNumberMaximum + "]";
        }
        objectiveText.text = objectives[(int)currentObjective].objectiveName;
        if(objectives[(int)currentObjective].isNotification == true){
            notificationText = objectives[(int)currentObjective].notificationText;
            notificationTextColor = objectives[(int)currentObjective].notificationTextColor;
        }
    }

    public void ToggleObjectives(float objectiveNum = 1f){
        if(objectiveNum == 1f){
            showObjectives = !showObjectives;
            if(showObjectives){
                openCloseText.text = "-";
            }
            else{
                openCloseText.text = "+";
            }
        }
        if(objectiveNum == 2f){
            showTab2 = !showTab2;
            if(showTab2){
                openCloseText2.text = "-";
            }
            else{
                openCloseText2.text = "+";
            }
        }
        if(objectiveNum == 3f){
            showTab3 = !showTab3;
            if(showTab3){
                openCloseText3.text = "-";
            }
            else{
                openCloseText3.text = "+";
            }
        }
    }
}
