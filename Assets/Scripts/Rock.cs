using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public ParticleSystem hitEffect;
    public ScoreCounter scoreCounter;
    public SpriteRenderer rockSprite;
    public float scoreMultiplier = 1f;
    public float minimumVelocity = 11f;
    public float rockStage = 1f;

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
    public void Collision(GameObject other, float prevVelocity, float smashPower){
        if(other.tag == "Player"){
            //instanitate the hit effect and make it point towards player
            float velocity = Mathf.Floor((prevVelocity - minimumVelocity) * smashPower);
            ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = effect.main;
            UnityEngine.Debug.Log(prevVelocity - minimumVelocity);
            if(velocity <= 0){
                velocity = 0;
            }else{
                HitCalculation(velocity);
            }
            main.maxParticles = (int)(velocity)*2;
            //change particlesystem's particle startspeed maximum based on velocity
            main.startSpeed = new ParticleSystem.MinMaxCurve(prevVelocity/2, prevVelocity/1.5f);
            Vector2 direction = transform.position - other.transform.position;
            direction.Normalize();
            effect.transform.rotation = Quaternion.Euler( Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, -90, 0);
        }
    }

    private void HitCalculation(float velocity){
        float scoreIncrease = Mathf.Floor(velocity  * scoreMultiplier);
        scoreCounter.UpdateScore(scoreIncrease);
    }
}
