using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public GameObject groundCollider;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;
    public float jumps = 1f;
    private float jumpsLeft = 1f;

    [Header("Other Values")]
    public float xReboundForce = 0.7f;
    public float yReboundMultiplier = 0.6f;
    public float rotationalReboundForce = 10f;
    public float rotationalMovementForce = 10f;
    public float rotationalMovementCap = 100f;
    public float smashPower = 1f;
    public float gravityPower = 1f;

    private bool grounded = false;
    private bool rebounding = false;
    private bool colliding = false;

    private Vector2 previousVelocity = new Vector2(0, 0);
    private float previousYMagnitude = 0;

    void Start()
    {
        rebounding = false;
        colliding = false;
    }

    void Update()
    {
        GroundCollider();
        Movement();
        Jumping();
        AccelerateDown();
        previousVelocity = rb.velocity;
        previousYMagnitude = Mathf.Abs(rb.velocity.y);
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Rock"){
            colliding = false;
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

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Upgrade"){
            if(previousVelocity.y > 0){
                rb.velocity = new Vector2(rb.velocity.x, -previousVelocity.y);
                other.GetComponent<Buyable>().TryUpgrade();
            }
            //buy upgrade
        }
    }

    private void GroundCollider(){
        groundCollider.transform.position = new Vector2(transform.position.x, transform.position.y);
        grounded = groundCollider.GetComponent<PlayerGroundCollision>().colliding;
    }

    private void CallCollision(GameObject other){
        if(colliding){return;}
        other.GetComponent<Rock>().Collision(this.gameObject, previousYMagnitude, smashPower);
    }

    private void CollisionRebound(Transform other){
        //bounce away from the rock
        Vector2 direction = transform.position - other.position;
        direction.Normalize();

        rb.velocity = new Vector2(
            (direction.x + GetAbsNumber(direction.x) * 0.1f) * xReboundForce, direction.y * yReboundMultiplier * Mathf.Abs(previousVelocity.y));
        //apply rotational velocity based on direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.angularVelocity = rotationalReboundForce * angle;
        if(grounded){return;}
        rebounding = true;
    }

    private float GetAbsNumber(float num){
        if(num < 0){
            return -1;
        }
        else{
            return 1;
        }
    }

    private void Movement(){
        if(rebounding && !grounded){return;}
        float x = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        rb.angularVelocity += rotationalMovementForce * rb.velocity.x * -Time.deltaTime;
        //cap angular velocity
        if(rb.angularVelocity > rotationalMovementCap){
            rb.angularVelocity = rotationalMovementCap;
        }
        else if(rb.angularVelocity < -rotationalMovementCap){
            rb.angularVelocity = -rotationalMovementCap;
        }
    }

    private void Jumping(){
        if(grounded){
            jumpsLeft = jumps;
        }
        if(Input.GetButton("Jump") && grounded == true || Input.GetButtonDown("Jump") && jumpsLeft > 0 && jumps > 1){
            RegisterJump();
            rebounding = false;
        }
    }

    private void RegisterJump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        jumpsLeft--;
    }

    private void AccelerateDown(){
        if(rb.velocity.y < 0 || !Input.GetButton("Jump") || rebounding){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f*gravityPower) * Time.deltaTime;
        }
    }
}
