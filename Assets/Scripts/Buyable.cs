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
    public int levels = 3;
    public int level = 1;
    public float[] amounts;
    public string upgradeType = "doubleJumps";
    public string upgradeVariety = "player";
    public string effectStringPrefix;
    public string[] effectStringSuffixes;
    public float xDistanceForDescription = 3f;
    [Header("MISC")]
    public float experienceParticlesSpawned = 10f;
    public Color experienceParticleColor;
    private bool showDescription = false;
    private bool buyCooldown = false;

    void Update(){
        if(level > levels){
            costText.text = "Max Level";
        }
        else{
            costText.text = "Cost: " + costs[level-1].ToString();
        }
        effectText.text = "Effect: " + effectStringPrefix + " " + effectStringSuffixes[level-1];
        CheckPlayerDistance();
        animator.SetBool("ShowDescription", showDescription);
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
        if(level > levels){return;}
        if(buyCooldown){return;}
        if(upgradeManager.checkPrice(costs[level-1])){
            if(upgradeVariety == "player"){
                upgradeManager.UpgradePlayerStat(upgradeType, amounts[level-1], costs[level-1]);
            }
            else if(upgradeVariety == "rock"){
                upgradeManager.UpgradeRockStat(upgradeType, amounts[level-1], costs[level-1]);
            }
            level++;
            buyCooldown = true;
            SpawnParticles();
            StartCoroutine(Cooldown());
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
