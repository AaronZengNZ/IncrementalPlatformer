using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public Rock rock;
    public GameObject redPad;
    private bool redPadActive = false;
    public ScoreCounter scoreCounter;
    [Header("Upgradable Stats")]
    public float playerJumpHeight = 8f;

    void Start(){
        redPad.SetActive(false);
    }

    public bool checkPrice(float cost, string currency)
    {
        if (scoreCounter.score >= cost && currency == "pebbles")
        {
            return true;
        }
        if (scoreCounter.shards >= cost && currency == "shards")
        {
            return true;
        }
        return false;
    }

    public void UpgradeStat(string stat, float amount, float cost, string currency)
    {
        if (HandleCost(currency, cost))
        {
            HandleUpgrade(stat, amount);
        }
    }

    public bool HandleCost(string currency, float cost)
    {
        if (currency == "pebbles")
        {
            if(checkPrice(cost, currency))
            {
                scoreCounter.UpdateScore(-cost);
                return true;
            }
        }
        else if (currency == "shards")
        {
            if(checkPrice(cost, currency))
            {
                scoreCounter.UpdateShards(-cost);
                return true;
            }
        }
        return false;
    }

    public void HandleUpgrade(string stat, float amount)
    {
        switch (stat)
        {
            case "rockStage":
                rock.rockStage += amount;
                break;
            case "rockValue":
                rock.scoreMultiplier += amount;
                break;
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
            case "autoJumps":
                player.maxAutoJumps += amount;
                player.autoJumpsLeft = player.maxAutoJumps;
                break;
            case "unlockShards":
                scoreCounter.shardsUnlocked = true;
                break;
            case "redPad":
                redPadActive = true;
                redPad.SetActive(true);
                break;
        }
    }
}
