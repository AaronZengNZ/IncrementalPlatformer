using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TipsScript : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI tipText;
    [Header("Variables")]
    public string[] tips;
    public int[] usedTips;
    public int currentTip = 0;
    public string shownTipString = "";
    public string currentTipString = "";
    public float tipDuration = 10f;
    public float currentTipDuration = 10f;
    public float characterDuration = 0.05f;
    public bool deletingText = false;
    public bool addingText = false;
    public bool textEmpty = false;

    void Start(){
        shownTipString = "";
        deletingText = false;
        addingText = false;
        NewTip();
        StartCoroutine(TypeText());
        currentTipDuration = 0f;
    }
    void Update()
    {
        tipText.text = shownTipString;
        if(addingText){return;}
        currentTipDuration+=Time.deltaTime;
        if(currentTipDuration >= tipDuration){
            if(textEmpty){
                NewTip();
                StartCoroutine(TypeText());
                currentTipDuration = 0f;
            }else if(!deletingText){
                StartCoroutine(DeleteText());
            }
        }
    }

    IEnumerator TypeText(){
        textEmpty = false;
        addingText = true;
        for(int i = 0; i < currentTipString.Length; i++){
            shownTipString += currentTipString[i];
            tipText.text = shownTipString;
            yield return new WaitForSeconds(0.05f);
        }
        addingText = false;
    }

    IEnumerator DeleteText(){
        deletingText = true;
        for(int i = 0; i < currentTipString.Length; i++){
            shownTipString = shownTipString.Remove(shownTipString.Length-1);
            tipText.text = shownTipString;
            yield return new WaitForSeconds(0.05f / 2f);
        }
        deletingText = false;
        textEmpty = true;
    }

    void NewTip(){
        currentTip = Random.Range(0, tips.Length);
        if(usedTips.Length == tips.Length){
            usedTips = new int[0];
        }
        while(Contains(usedTips, currentTip)){
            currentTip = Random.Range(0, tips.Length);
        }
        int[] newUsedTips = new int[usedTips.Length+1];
        for(int i = 0; i < usedTips.Length; i++){
            newUsedTips[i] = usedTips[i];
        }
        newUsedTips[usedTips.Length] = currentTip;
        usedTips = newUsedTips;
        currentTipString = tips[currentTip];
    }

    public bool Contains(int[] array, int value){
        for(int i = 0; i < array.Length; i++){
            if(array[i] == value){
                return true;
            }
        }
        return false;
    }
}
