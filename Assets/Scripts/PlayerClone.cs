using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClone : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public GameObject groundCollider;
    public Player player;
    public Transform rock;
    [Header("Movement")]
    public float jumpHeight = 5f;
    public float xHoming = 1.5f;
    public float xHomingRange = 10f;
    [Header("Rebound Values")]
    public float xReboundForce = 0.7f;
    public float yReboundMultiplier = 0.6f;
    public float rotationalReboundForce = 10f;
    public float rotationalMovementForce = 10f;
    public float rotationalMovementCap = 100f;
    [Header("Other Values")]
    public float smashPower = 1f;
    public float gravityPower = 1f;
    public float autoJumpsLeft = 0f;
    public float greenPadPower = 2f;
    public float redPadPower = 5f;
    public float bluePadPower = 0.1f;
    public bool touchingGreenPad = false;
    public float doubleMultiTime = 0f;
    public bool autoJumpsToggled = false;
    private bool grounded = false;
    private bool rebounding = false;
    private bool colliding = false;

    private Vector2 previousVelocity = new Vector2(0, 0);
    private float previousYMagnitude = 0;

    private bool spawned = false;
    // Start is called before the first frame update
    void Start()
    {
        spawned = false;
        grounded = true;
        rebounding = false;
        player = GameObject.Find("Player").GetComponent<Player>();
        rock = GameObject.Find("Rock").transform;
        autoJumpsToggled = true;
        CopyPlayerStats();
        StartCoroutine(Spawning());
    }

    private void CopyPlayerStats(){
        jumpHeight = player.jumpHeight;
        xReboundForce = player.xReboundForce;
        yReboundMultiplier = player.yReboundMultiplier;
        rotationalReboundForce = player.rotationalReboundForce;
        rotationalMovementForce = player.rotationalMovementForce;
        rotationalMovementCap = player.rotationalMovementCap;
        smashPower = player.smashPower;
        gravityPower = player.gravityPower;
        autoJumpsLeft = player.autoJumpsLeft;
        greenPadPower = player.greenPadPower;
        touchingGreenPad = player.touchingGreenPad;
        redPadPower = player.redBoostValue;
        doubleMultiTime = player.bluePadTimeLeft;
        bluePadPower = player.bluePadPower;
    }

    // Update is called once per frame
    void Update()
    {
        if(!spawned){return;}
        CopyPlayerStats();
        GroundCollider();
        Movement();
        Jumping();
        AccelerateDown();
        previousVelocity = rb.velocity;
        previousYMagnitude = Mathf.Abs(rb.velocity.y);
    }

    IEnumerator Spawning(){
        yield return new WaitForSeconds(1f);
        spawned = true;
    }

    private void GroundCollider(){
        groundCollider.transform.position = new Vector2(transform.position.x, transform.position.y);
        grounded = groundCollider.GetComponent<PlayerGroundCollision>().colliding;
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Rock"){
            colliding = false;
        }
        if(other.gameObject.tag == "Ground"){
            player.useAutoJump();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Ground"){
            rebounding = false;
        }
        if(other.gameObject.tag == "Rock"){
            CallCollision(other.gameObject);
            CollisionRebound(other.transform);
            colliding = true;
        }
    }

    private void CallCollision(GameObject other){
        if(colliding){return;}
        float boostTotal = 1f;
        if(touchingGreenPad){
            boostTotal *= greenPadPower;
            if(doubleMultiTime > 0f){
                boostTotal *= 1f + (redPadPower * bluePadPower);
            }
        }
        float screenShakeMagnitude = 0.5f;
        GameObject[] clones = GameObject.FindGameObjectsWithTag("PlayerClone");
        float amountOfClones = clones.Length;
        screenShakeMagnitude = 0.7f / (0.8f + (amountOfClones * 0.2f));
        other.GetComponent<Rock>().Collision(this.gameObject, previousYMagnitude, smashPower, boostTotal, screenShakeMagnitude);
    }

    private void CollisionRebound(Transform other){
        //bounce away from the rock
        Vector2 direction = transform.position - other.position;
        direction.Normalize();
        rb.velocity = new Vector2(
            (direction.x + player.GetAbsNumber(direction.x) * 0.1f) * xReboundForce, direction.y * yReboundMultiplier * Mathf.Abs(previousVelocity.y));
        //apply rotational velocity based on direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.angularVelocity = rotationalReboundForce * angle;
        if(grounded){return;}
        rebounding = true;
    }

    private void Movement(){
        if(rebounding && !grounded){return;}
        if(rock != null && Mathf.Abs(transform.position.x - rock.position.x) < xHomingRange && xHoming > 0f && !grounded && !rebounding){
            rb.velocity = new Vector2((rock.position.x - transform.position.x) * xHoming, rb.velocity.y);
            rb.angularVelocity += rotationalMovementForce * rb.velocity.x * -Time.deltaTime;
        }
        if(rb.angularVelocity > rotationalMovementCap){
            rb.angularVelocity = rotationalMovementCap;
        }
        else if(rb.angularVelocity < -rotationalMovementCap){
            rb.angularVelocity = -rotationalMovementCap;
        }
    }

    private void Jumping(){
        if(autoJumpsLeft > 0f && grounded == true){
            RegisterJump();
            rebounding = false;
        }
    }
    private void RegisterJump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
    }

    private void AccelerateDown(){
        if(rb.velocity.y < 0 || rebounding){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f*gravityPower) * Time.deltaTime;
        }
    }
}
