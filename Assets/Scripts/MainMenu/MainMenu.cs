using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public Animator[] animators;
    public Animator transitionAnimator;
    public Button continueButton;
    private bool transitioning = false;
    public GameObject newGameConfirmation;
    // Start is called before the first frame update
    void Start()
    {
        if(newGameConfirmation != null){
            newGameConfirmation.SetActive(false);
        }
    }

    void Update(){
        if(newGameConfirmation == null){return;}
        if(PlayerPrefs.HasKey("tutorialCompleted")){
            continueButton.interactable = true;
        }else{
            continueButton.interactable = false;
        }
    }

    public void ToggleNewGameConfirmation(bool target){
        newGameConfirmation.SetActive(target);
    }

    public void NewGameWithBenifits(){
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("pebbles", 10000000f);
        PlayerPrefs.SetFloat("shards", 50000f);
        PlayerPrefs.SetInt("shardsUnlocked", 1);
        PlayerPrefs.SetInt("buyable_shardsUnlock", 2);
        StartGame();
    }

    public void NewGame(){
        PlayerPrefs.DeleteAll();
        StartGame();
    }

    public void StartGame(){
        if(!transitioning){
            StartCoroutine(NextScene());
        }
    }

    IEnumerator NextScene(){
        transitioning = true;
        transitionAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("GameScene");
        transitioning = false;
    }

    public void EnterMainMenu(){
        if(!transitioning){
            StartCoroutine(enterMainMenu());
        }
    }

    IEnumerator enterMainMenu(){
        transitioning = true;
        transitionAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MainMenu");
        transitioning = false;
    }

    public void ToggleHovered(int index)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            if (i == index)
            {
                animators[i].SetBool("Hover", true);
            }
        }
    }
    public void ToggleUnhovered(int index)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            if (i == index)
            {
                animators[i].SetBool("Hover", false);
            }
        }
    }
}
