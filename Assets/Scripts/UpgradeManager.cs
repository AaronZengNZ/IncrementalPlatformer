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
    public GameObject greenPad;
    public bool greenPadActive = false;
    public GameObject bluePad;
    public bool bluePadActive = false;
    public GameObject door1;
    public bool door1Active = true;
    public GameObject door2;
    public bool door2Active = true;
    public GameObject stairs;
    public bool stairsActive = false;
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
        door2.SetActive(door2Active);
        greenPad.SetActive(greenPadActive);
        bluePad.SetActive(bluePadActive);
        stairs.SetActive(stairsActive);
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
        GameObject objectivesManager = GameObject.Find("ObjectivesManager");
        objectivesManager.GetComponent<ObjectivesManager>().SetPrefs();
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
        }
        else{
            PlayerPrefs.SetInt("redPad", Convert.ToInt32(false));
            redPadActive = false;
        }
        if(PlayerPrefs.HasKey("door1")){
            door1Active = Convert.ToBoolean(PlayerPrefs.GetInt("door1Active"));
        }
        else{
            PlayerPrefs.SetInt("door1", Convert.ToInt32(true));
            door1Active = true;
        }
        if(PlayerPrefs.HasKey("door2")){
            door2Active = Convert.ToBoolean(PlayerPrefs.GetInt("door2Active"));
        }
        else{
            PlayerPrefs.SetInt("door2", Convert.ToInt32(true));
            door2Active = true;
        }
        if(PlayerPrefs.HasKey("playerClones")){
            playerClones = PlayerPrefs.GetInt("playerClones");
        }
        else{
            PlayerPrefs.SetInt("playerClones", 0);
            playerClones = 0;
        }
        if(PlayerPrefs.HasKey("greenPad")){
            greenPadActive = Convert.ToBoolean(PlayerPrefs.GetInt("greenPadActive"));
        }
        else{
            PlayerPrefs.SetInt("greenPad", Convert.ToInt32(false));
            greenPadActive = false;
        }
        if(PlayerPrefs.HasKey("bluePad")){
            bluePadActive = Convert.ToBoolean(PlayerPrefs.GetInt("bluePadActive"));
        }
        else{
            PlayerPrefs.SetInt("bluePad", Convert.ToInt32(false));
            bluePadActive = false;
        }
        if(PlayerPrefs.HasKey("stairsActive")){
            stairsActive = Convert.ToBoolean(PlayerPrefs.GetInt("stairsActive"));
        }
        else{
            PlayerPrefs.SetInt("stairsActive", Convert.ToInt32(false));
            stairsActive = false;
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
        if(door2Active){
            PlayerPrefs.SetInt("door2Active", Convert.ToInt32(true));
        }
        else{
            PlayerPrefs.SetInt("door2Active", Convert.ToInt32(false));
        }
        if(greenPadActive){
            PlayerPrefs.SetInt("greenPadActive", Convert.ToInt32(true));
        }
        else{
            PlayerPrefs.SetInt("greenPadActive", Convert.ToInt32(false));
        }
        if(bluePadActive){
            PlayerPrefs.SetInt("bluePadActive", Convert.ToInt32(true));
        }
        else{
            PlayerPrefs.SetInt("bluePadActive", Convert.ToInt32(false));
        }
        if(stairsActive){
            PlayerPrefs.SetInt("stairsActive", Convert.ToInt32(true));
        }
        else{
            PlayerPrefs.SetInt("stairsActive", Convert.ToInt32(false));
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
            case "completeTutorial":
                PlayerPrefs.SetInt("tutorialCompleted", 1);
                player.tutorialCompleted = true;
                break;
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
                player.xHomingRange += amount;
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
            case "unlockDoor2":
                door2Active = false;
                break;
            case "playerClones":
                playerClones += (int)amount;
                InstantiatePlayerClone();
                break;
            case "redBoostValue":
                player.redBoostValue += amount;
                break;
            case "shardGain":
                rock.shardIncrement += amount;
                break;
            case "greenPad":
                greenPadActive = true;
                break;
            case "greenBoostValue":
                player.greenPadPower += amount;
                break;
            case "activateStairs":
                stairsActive = true;
                break;
            case "unlockDash":
                player.dashUnlocked = true;
                break;
            case "bluePad":
                bluePadActive = true;
                break;
            case "bluePadPower":
                player.bluePadPower += amount;
                break;
            case "bluePadDuration":
                player.bluePadDuration += amount;
                break;
            case "saveAutojump":
                player.chanceToNotUseAutojump += amount;
                break;
            case "redPadDuration":
                player.redPadDuration += amount;
                break;
        }
    }

    public float GetValue(string upgradeType){
        switch (upgradeType)
        {
            case "rockStage":
                return rock.rockStage;
            case "rockValue":
                return rock.scoreMultiplier;
            case "jumpHeight":
                return player.jumpHeight;
            case "doubleJump":
                return player.jumps;
            case "smashPower":
                return player.smashPower;
            case "gravityWell":
                return player.gravityPower;
            case "xHoming":
                if(player.xHomingRange > 6){
                    return 1;
                }else {return 0;}
            case "autoJumps":
                return player.maxAutoJumps;
            case "unlockShards":
                return turnToInt(scoreCounter.shardsUnlocked);
            case "redPad":
                return turnToInt(redPadActive);
            case "unlockDoor1":
                return turnToInt(door1Active, true);
            case "unlockDoor2":
                return turnToInt(door2Active, true);
            case "playerClones":
                return playerClones;
            case "redBoostValue":
                return player.redBoostValue;
            case "shardGain":
                return rock.shardIncrement;
            case "greenPad":
                return turnToInt(greenPadActive);
            case "greenBoostValue":
                return player.greenPadPower;
            case "completeTutorial":
                return turnToInt(player.tutorialCompleted);
            case "activateStairs":
                return turnToInt(stairsActive);
            case "unlockDash":
                return turnToInt(player.dashUnlocked);
            case "bluePad":
                return turnToInt(bluePadActive);
            case "bluePadPower":
                return player.bluePadPower;
            case "bluePadDuration":
                return player.bluePadDuration;
            case "saveAutojump":
                return player.chanceToNotUseAutojump;
            case "redPadDuration":
                return player.redPadDuration;
        }
        return 0;
    }

    private float turnToInt(bool variable, bool reverse = false){
        if(variable){
            if(reverse){
                return 0;
            }
            return 1;
        }
        if(reverse){
            return 1;
        }
        return 0;
    }
}
