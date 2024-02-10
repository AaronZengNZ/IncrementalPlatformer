using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCollision : MonoBehaviour
{
    public bool colliding;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Ground"){
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.tag == "Ground"){
            colliding = false;
        }
    }
}
