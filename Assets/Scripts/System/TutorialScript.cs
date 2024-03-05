using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TutorialScript : MonoBehaviour
{
    private bool doTutorial = false;
    public Player playerScript;
    [TextArea(3,3)] public string[] tutorialTexts;
    public string[] tutorialTextTitles;
    public float currentTutorialStage = 0f;
    public TextMeshProUGUI tutorialTitleText;
    public TextMeshProUGUI tutorialIntructionalText;
    public GameObject tutorialPanel;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("tutorialCompleted") == 1){
            doTutorial = false;
        }else{
            doTutorial = true;
        }
    }

    void Update(){
        if(playerScript.tutorialCompleted == true){
            doTutorial = false;
        }
        tutorialPanel.SetActive(doTutorial);
        if(doTutorial){
            tutorialTitleText.text = tutorialTextTitles[(int)currentTutorialStage];
            tutorialIntructionalText.text = tutorialTexts[(int)currentTutorialStage];
        }
    }

    public void NextTutorialStage(){
        currentTutorialStage++;
        if(currentTutorialStage >= tutorialTexts.Length){
            doTutorial = false;
        }
    }
}
