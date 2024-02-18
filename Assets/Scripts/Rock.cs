using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Rock : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;
    public ParticleSystem hitEffect;
    public ParticleSystem shardEffect;
    public ScoreCounter scoreCounter;
    public SpriteRenderer rockSprite;
    public float scoreMultiplier = 1f;
    public float minimumVelocity = 11f;
    public float rockStage = 1f;
    public float shakeTime = 0.2f;
    public float shardIncrement = 1f;
    public float maxXDistance = 5f;

    void Start(){
        CheckPrefs();
    }

    private void CheckPrefs(){
        if(PlayerPrefs.HasKey("rockStage")){
            rockStage = PlayerPrefs.GetFloat("rockStage");
        }
        else{
            PlayerPrefs.SetFloat("rockStage", 1f);
        }
        if(PlayerPrefs.HasKey("rockValue")){
            scoreMultiplier = PlayerPrefs.GetFloat("rockValue");
        }
        else{
            PlayerPrefs.SetFloat("rockValue", 1f);
        }
        if(PlayerPrefs.HasKey("shardIncrement")){
            shardIncrement = PlayerPrefs.GetFloat("shardIncrement");
        }
        else{
            PlayerPrefs.SetFloat("shardIncrement", 1f);
        }
    }

    public void UpdatePrefs(){
        PlayerPrefs.SetFloat("rockStage", rockStage);
        PlayerPrefs.SetFloat("rockValue", scoreMultiplier);
        PlayerPrefs.SetFloat("shardIncrement", shardIncrement);
    }

    void Update(){
        if(rockStage == 1f){
            hitEffect.startColor = new Color(1f, 1f, 1f, 1f);
            rockSprite.color = new Color(1f, 1f, 1f, 1f);
        }else if(rockStage == 2f){
            hitEffect.startColor = new Color(0.9f, 0.7f, 0f, 1f);
            rockSprite.color = new Color(0.75f, 0.6f, 0f, 1f);
        }
    }
    public void Collision(GameObject other, float prevVelocity, float smashPower, float boostValue, float shakePower = 1f){
        if(other.tag == "Player"){
            //instanitate the hit effect and make it point towards player
            float velocity = Mathf.Floor((prevVelocity - minimumVelocity) * smashPower * 0.5f);
            Shake(velocity, shakePower);
            ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = effect.main;
            if(velocity <= 0){
                velocity = 0;
            }else{
                HitCalculation(velocity,boostValue);
            }
            main.maxParticles = (int)(velocity)*2;
            //change particlesystem's particle startspeed maximum based on velocity
            main.startSpeed = new ParticleSystem.MinMaxCurve(prevVelocity/2, prevVelocity/1.5f);
            Vector2 direction = transform.position - other.transform.position;
            direction.Normalize();
            effect.transform.rotation = Quaternion.Euler( Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, -90, 0);
            if(scoreCounter.shardsUnlocked && velocity > 0){
                ParticleSystem shard = Instantiate(shardEffect, transform.position, effect.transform.rotation);
                ParticleSystem.MainModule shardMain = shard.main;
                shardMain.maxParticles = (int)Mathf.Floor(Mathf.Sqrt(shardIncrement * boostValue));
            }
        }
    }

    private void HitCalculation(float velocity, float boostValue = 1f){
        float scoreIncrease = Mathf.Floor(velocity  * scoreMultiplier * boostValue);
        scoreCounter.UpdateScore(scoreIncrease);
        if(scoreIncrease > 0f){
            scoreCounter.UpdateShards(shardIncrement*boostValue);
        }
    }

    public void Shake(float velocity, float shakePowerTemp = 1f){
        //find the x distance to player
        float xDistance = Mathf.Abs(transform.position.x - GameObject.Find("Player").transform.position.x);
        float xShake = 1 - (xDistance / maxXDistance);
        if(velocity < 0){xShake = 1f;}
        else if(xShake <= 0f){return;}
        if(xShake > 1f){xShake = 1f;}
        float shakePower = velocity/5f * xShake * shakePowerTemp;
        if(shakePower <= -1f){return;}
        if(shakePower >= 5f){
            shakePower = 5f + Mathf.Sqrt(shakePower-5f);
        }
        impulseSource.GenerateImpulseWithForce(shakePower);
    }
}
