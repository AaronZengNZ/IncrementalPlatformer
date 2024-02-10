using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public float score {get; private set;}
    public float scoreLerpSpeed = 10f;
    private float scoreLerp = 0f;
    public TextMeshProUGUI scoreText;

    void Update(){
        scoreLerp += (score - scoreLerp) * Time.deltaTime * scoreLerpSpeed;
        string indent;
        if(Mathf.Round(scoreLerp) > Mathf.Round(score)){indent = "-";}
        else if(Mathf.Round(scoreLerp) < Mathf.Round(score)){indent = "+";}
        else{
            scoreText.text = Mathf.RoundToInt(scoreLerp).ToString();
            return;
        }
        if(Mathf.Abs(score - scoreLerp) < score / 50000f / scoreLerpSpeed){
            scoreText.text = Mathf.RoundToInt(score).ToString();
            return;
        }
        scoreText.text = Mathf.RoundToInt(scoreLerp).ToString() + indent + Mathf.RoundToInt(Mathf.Abs(score - scoreLerp)).ToString();
    }
    
    public void UpdateScore(float amount){
        score += amount;
    }
}
