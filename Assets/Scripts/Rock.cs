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

    void Update(){
        if(rockStage == 1f){
            hitEffect.startColor = new Color(1f, 1f, 1f, 1f);
            rockSprite.color = new Color(1f, 1f, 1f, 1f);
            scoreMultiplier = 1f;
        }else if(rockStage == 2f){
            hitEffect.startColor = new Color(0.9f, 0.7f, 0f, 1f);
            rockSprite.color = new Color(0.75f, 0.6f, 0f, 1f);
            scoreMultiplier = 3f;
        }
    }
    public void Collision(GameObject other, float prevVelocity, float smashPower, float boostValue){
        if(other.tag == "Player"){
            //instanitate the hit effect and make it point towards player
            float velocity = Mathf.Floor((prevVelocity - minimumVelocity) * smashPower);
            Shake(velocity);
            ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = effect.main;
            UnityEngine.Debug.Log(prevVelocity - minimumVelocity);
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
                shardMain.maxParticles = (int)Mathf.Floor(Mathf.Sqrt(shardIncrement));
            }
        }
    }

    private void HitCalculation(float velocity, float boostValue){
        float scoreIncrease = Mathf.Floor(velocity  * scoreMultiplier * boostValue);
        scoreCounter.UpdateScore(scoreIncrease);
        if(scoreIncrease > 0f){
            scoreCounter.UpdateShards(shardIncrement);
        }
    }

    public void Shake(float velocity){
        float shakePower = velocity/5f;
        if(shakePower <= 0f){return;}
        impulseSource.GenerateImpulseWithForce(shakePower);
    }
}
