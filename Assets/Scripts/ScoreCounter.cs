using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public float score {get; private set;}
    public float scoreLerpSpeed = 10f;
    public float shards {get; private set;}
    private float scoreLerp = 0f;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI shardText;
    public bool shardsUnlocked = false;
    public BigNumberHandler bigNumberHandler;
    private float prefUpdateTime = 0f;

    void Start(){
        bigNumberHandler = GetComponent<BigNumberHandler>();
        CheckPrefs();
    }

    private void CheckPrefs(){
        if(PlayerPrefs.HasKey("shardsUnlocked")){
            shardsUnlocked = Convert.ToBoolean(PlayerPrefs.GetInt("shardsUnlocked"));
        }
        else{
            PlayerPrefs.SetInt("shardsUnlocked", Convert.ToInt32(shardsUnlocked));
        }
        if(PlayerPrefs.HasKey("pebbles")){
            score = PlayerPrefs.GetFloat("pebbles");
            scoreLerp = score;
        }
        else{
            PlayerPrefs.SetFloat("pebbles", score);
        }
        if(PlayerPrefs.HasKey("shards")){
            shards = PlayerPrefs.GetFloat("shards");
        }
        else{
            PlayerPrefs.SetFloat("shards", shards);
        }
    }

    public void UpdatePrefs(){
        PlayerPrefs.SetFloat("pebbles", score);
        PlayerPrefs.SetFloat("shards", shards);
        PlayerPrefs.SetInt("shardsUnlocked", Convert.ToInt32(shardsUnlocked));
    }

    void Update(){
        if(shardsUnlocked){
            shardText.text = "Shards: " + bigNumberHandler.ConvertToString(shards);
        }
        else{
            shardText.text = "";
        }
        scoreLerp += (score - scoreLerp) * Time.deltaTime * scoreLerpSpeed;
        string indent;
        if(Mathf.Round(scoreLerp) > Mathf.Round(score)){indent = "-";}
        else if(Mathf.Round(scoreLerp) < Mathf.Round(score)){indent = "+";}
        else{
            scoreText.text = bigNumberHandler.ConvertToString(scoreLerp);
            return;
        }
        if(Mathf.Abs(score - scoreLerp) < score / 50000f / scoreLerpSpeed){
            scoreText.text = bigNumberHandler.ConvertToString(score);
            return;
        }
        scoreText.text = bigNumberHandler.ConvertToString(scoreLerp) + indent + bigNumberHandler.ConvertToString(Mathf.Abs(score - scoreLerp));
    }
    
    public void UpdateScore(float amount){
        score += amount;
    }

    public void UpdateShards(float amount){
        if(shardsUnlocked){
            shards += amount;
        }
    }

    public float GetCurrency(string currency){
        if(currency == "pebbles"){
            return score;
        }
        else if(currency == "shards"){
            return shards;
        }
        else{
            return 0f;
        }
    }
}
