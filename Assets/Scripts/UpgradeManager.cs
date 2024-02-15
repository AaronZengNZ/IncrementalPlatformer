using System.Security.Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public Rock rock;
    public GameObject redPad;
    public bool redPadActive = false;
    public GameObject door1;
    public bool door1Active = true;
    public ScoreCounter scoreCounter;
    public TextMeshProUGUI savingText;
    public GameObject playerClone;
    public Transform playerCloneInstantiateLocation;

    [Header("Saving")]
    public float saveInterval = 15f;
    [Header("Upgradable Stats")]
    private float prefUpdateTime = 0f;
    public int playerClones = 0;
    private int playerClonesInstantiated = 0;

    void Start(){
        StartCoroutine(UpdatePrefInterval());
        CheckPrefs();
    }

    void Update(){
        redPad.SetActive(redPadActive);
        door1.SetActive(door1Active);
        if(playerClonesInstantiated < playerClones){
            InstantiatePlayerClone();
        }
    }

    private void InstantiatePlayerClone(){
        GameObject clone = Instantiate(playerClone, playerCloneInstantiateLocation.position, Quaternion.identity);
        playerClonesInstantiated++;
    }

    IEnumerator UpdatePrefInterval(){
        while(true){
            savingText.text = "";
            yield return new WaitForSeconds(saveInterval);
            savingText.text = "Saving...";
            UpdateAllPrefs();
            UnityEngine.Debug.Log("Saved Data");
            yield return new WaitForSeconds(3f);
            savingText.text = "";
        }
    }

    public void UpdateAllPrefs(){
        player.UpdatePrefs();
        rock.UpdatePrefs();
        scoreCounter.UpdatePrefs();
        UpdateThisPrefs();
        //find all objects with 'upgrade' tag
        GameObject[] buyables = GameObject.FindGameObjectsWithTag("Upgrade");
        foreach(GameObject buyable in buyables){
            buyable.GetComponent<Buyable>().UpdatePrefs();
        }
    }

    private void CheckPrefs(){
        if(PlayerPrefs.HasKey("redPad")){
            redPadActive = Convert.ToBoolean(PlayerPrefs.GetInt("redPadActive"));
            UnityEngine.Debug.Log("Got old redPadActive: " + redPadActive);
        }
        else{
            PlayerPrefs.SetInt("redPad", Convert.ToInt32(false));
            redPadActive = false;
            UnityEngine.Debug.Log("Made new redPadActive: " + redPadActive);
        }
        if(PlayerPrefs.HasKey("door1")){
            door1Active = Convert.ToBoolean(PlayerPrefs.GetInt("door1Active"));
            UnityEngine.Debug.Log("Got old door1Active: " + door1Active);
        }
        else{
            PlayerPrefs.SetInt("door1", Convert.ToInt32(true));
            door1Active = true;
            UnityEngine.Debug.Log("Made new door1Active: " + door1Active);
        }
        if(PlayerPrefs.HasKey("playerClones")){
            playerClones = PlayerPrefs.GetInt("playerClones");
        }
        else{
            PlayerPrefs.SetInt("playerClones", 0);
            playerClones = 0;
        }
    }

    private void UpdateThisPrefs(){
        if(redPadActive){
            PlayerPrefs.SetInt("redPadActive", Convert.ToInt32(true));
        }
        else{
            PlayerPrefs.SetInt("redPadActive", Convert.ToInt32(false));
        }
        if(door1Active){
            PlayerPrefs.SetInt("door1Active", Convert.ToInt32(true));
        }
        else{
            PlayerPrefs.SetInt("door1Active", Convert.ToInt32(false));
        }
        PlayerPrefs.SetInt("playerClones", playerClones);
        UnityEngine.Debug.Log("Updated redPad: " + Convert.ToBoolean(PlayerPrefs.GetInt("redPadActive")));
    }

    public void ClearPrefs(){
        PlayerPrefs.DeleteAll();
        //reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                break;
            case "unlockDoor1":
                door1Active = false;
                break;
            case "playerClones":
                playerClones += (int)amount;
                InstantiatePlayerClone();
                break;
            case "redBoostValue":
                player.redBoostValue += amount;
                break;
        }
    }
}
