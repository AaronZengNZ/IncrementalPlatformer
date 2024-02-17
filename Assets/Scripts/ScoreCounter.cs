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
    private float prefUpdateTime = 0f;

    void Start(){
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
            shardText.text = "Shards: " + Mathf.RoundToInt(shards).ToString("n0");
        }
        else{
            shardText.text = "";
        }
        scoreLerp += (score - scoreLerp) * Time.deltaTime * scoreLerpSpeed;
        string indent;
        if(Mathf.Round(scoreLerp) > Mathf.Round(score)){indent = "-";}
        else if(Mathf.Round(scoreLerp) < Mathf.Round(score)){indent = "+";}
        else{
            scoreText.text = Mathf.RoundToInt(scoreLerp).ToString("n0");
            return;
        }
        if(Mathf.Abs(score - scoreLerp) < score / 50000f / scoreLerpSpeed){
            scoreText.text = Mathf.RoundToInt(score).ToString("n0");
            return;
        }
        scoreText.text = Mathf.RoundToInt(scoreLerp).ToString("n0") + indent + Mathf.RoundToInt(Mathf.Abs(score - scoreLerp)).ToString("n0");
    }
    
    public void UpdateScore(float amount){
        score += amount;
    }

    public void UpdateShards(float amount){
        if(shardsUnlocked){
            shards += amount;
        }
    }
}
