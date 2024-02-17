using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCollision : MonoBehaviour
{
    public bool colliding;
    private bool privateColliding;
    public bool afterJumps = false;
    private float afterJumpTimer = 0f;
    public float afterJumpTimerMax = 0.2f;
    // Start is called before the first frame update
    void Update()
    {
        if(!afterJumps){colliding = privateColliding; return;}
        if(afterJumpTimer > 0){
            afterJumpTimer -= Time.deltaTime;
        }
        else{
            colliding = privateColliding;
        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Ground"){
            privateColliding = true;
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.tag == "Ground"){
            privateColliding = false;
            afterJumpTimer = afterJumpTimerMax;
        }
    }
}
