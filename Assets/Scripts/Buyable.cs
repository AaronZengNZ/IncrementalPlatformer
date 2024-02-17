using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Buyable : MonoBehaviour
{
    [Header("References")]
    public UpgradeManager upgradeManager;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI effectText;
    public Animator animator;
    public GameObject experienceParticle;
    [Header("Values")]
    public float[] costs;
    public string currency = "pebbles";
    public int levels = 3;
    public int level = 1;
    public float[] amounts;
    public string upgradeType = "doubleJumps";
    public string effectStringPrefix;
    public string effectStringSuffixPrefix;
    public string differentPrefix = "null";
    public bool showSuffix = true;
    public string boughtMaxSuffix = "[MAX]";
    public float xDistanceForDescription = 3f;
    [Header("MISC")]
    public float experienceParticlesSpawned = 10f;
    public Color experienceParticleColor;
    public string UpgradeName = "null";
    public bool infiniteBuyable = false;
    private bool showDescription = false;
    private bool buyCooldown = false;
    private float prefUpdateTime = 0f;

    void Start(){
        if(PlayerPrefs.HasKey("buyable_"+UpgradeName)){
            level = PlayerPrefs.GetInt("buyable_"+UpgradeName);
            if(level <= 0){
                PlayerPrefs.SetInt("buyable_"+UpgradeName, 1);
                level = 1;
            }
        }
        else{
            PlayerPrefs.SetInt("buyable_"+UpgradeName, 1);
            level = 1;
        }
    }

    public void UpdatePrefs(){
        PlayerPrefs.SetInt("buyable_"+UpgradeName, level);
    }

    void Update(){
        CheckPlayerDistance();
        animator.SetBool("ShowDescription", showDescription);
        if(level > levels){
            costText.text = "Max Level";
        }
        else{
            costText.text = "Cost: " + costs[level-1].ToString("n0");
        }
        if(differentPrefix != "null" && !(level > levels)){
            effectText.text = "Effect: " + differentPrefix;
        }
        else if(level > levels){
            effectText.text = "Effect: " + effectStringPrefix + " " + boughtMaxSuffix;
        }
        else if(showSuffix){
            effectText.text = "Effect: " + effectStringPrefix + " [" + effectStringSuffixPrefix
                 + upgradeManager.GetValue(upgradeType) + "->" + effectStringSuffixPrefix + 
                 (upgradeManager.GetValue(upgradeType) + amounts[level-1]) + "]";
        }
        else{
            effectText.text = "Effect: " + effectStringPrefix;
        }
    }
    private void CheckPlayerDistance(){
        if(Mathf.Abs(transform.position.x - upgradeManager.player.transform.position.x) < xDistanceForDescription){
            showDescription = true;
        }
        else{
            showDescription = false;
        }
    }
    public void TryUpgrade(){
        if(level > levels && !infiniteBuyable){return;}
        if(buyCooldown){return;}
        if(upgradeManager.checkPrice((costs[level-1]), currency)){
            upgradeManager.UpgradeStat(upgradeType, amounts[level-1], costs[level-1], currency);
            SpawnParticles();
            buyCooldown = true;
            StartCoroutine(Cooldown());
            if(infiniteBuyable && level > levels){return;}
            level++;
        }
    }

    public void SpawnParticles(){
        for(int i = 0; i < experienceParticlesSpawned; i++){
            GameObject particle = Instantiate(experienceParticle, transform.position, Quaternion.identity);
            particle.GetComponent<PlayerExperienceObject>().color = experienceParticleColor;
        }
    }

    IEnumerator Cooldown(){
        yield return new WaitForSeconds(0.5f);
        buyCooldown = false;
    } 
}
