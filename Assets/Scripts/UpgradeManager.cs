using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public Rock rock;
    public ScoreCounter scoreCounter;
    [Header("Upgradable Stats")]
    public float playerJumpHeight = 8f;

    public bool checkPrice(float cost){
        if(scoreCounter.score >= cost){
            return true;
        }
        return false;
    }

    public void UpgradePlayerStat(string stat, float amount, float cost){
        if(checkPrice(cost)){
            scoreCounter.UpdateScore(-cost);
            switch(stat){
                case "jumpHeight":
                    player.jumpHeight += amount;
                    break;
                case "doubleJump":
                    player.jumps += amount;
                    break;
                case "smashPower":
                    player.smashPower += amount;
                    break;
                case "gravityWell":
                    player.gravityPower += amount;
                    break;
                case "xHoming":
                    player.xHoming += amount;
                    break;
            }
        }
    }

    public void UpgradeRockStat(string stat, float amount, float cost){
        if(checkPrice(cost)){
            scoreCounter.UpdateScore(-cost);
            switch(stat){
                case "rockStage":
                    rock.rockStage += amount;
                    break;
            }
        }
    }
}
