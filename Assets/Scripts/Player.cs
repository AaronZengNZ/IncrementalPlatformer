using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public GameObject groundCollider;
    public Transform rock;
    public TextMeshProUGUI autoJumpsLeftText;
    public ParticleSystem redBoostEffect;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;
    public float jumps = 1f;
    private float jumpsLeft = 1f;
    public float xHoming = 0f;
    public float xHomingRange = 3f;

    [Header("Rebound Values")]
    public float xReboundForce = 0.7f;
    public float yReboundMultiplier = 0.6f;
    public float rotationalReboundForce = 10f;
    public float rotationalMovementForce = 10f;
    public float rotationalMovementCap = 100f;
    [Header("Other Values")]
    public float smashPower = 1f;
    public float gravityPower = 1f;
    public float maxAutoJumps = 0f;
    public float autoJumpRegenSpeed = 5f;
    public float autoJumpsLeft = 0f;
    public bool autoJumpsToggled = false;
    public float redBoostTimeLeft = 0f;
    public float redBoostValue = 5f;
    private bool gravityTripled = false;

    private bool grounded = false;
    private bool rebounding = false;
    private bool colliding = false;

    private Vector2 previousVelocity = new Vector2(0, 0);
    private float previousYMagnitude = 0;

    void Start()
    {
        rebounding = false;
        colliding = false;
        CheckPrefs();
    }

    void CheckPrefs(){
        if(PlayerPrefs.HasKey("playerJumpHeight")){
            jumpHeight = PlayerPrefs.GetFloat("playerJumpHeight");
        }
        else{
            PlayerPrefs.SetFloat("playerJumpHeight", jumpHeight);
        }
        if(PlayerPrefs.HasKey("playerSmashPower")){
            smashPower = PlayerPrefs.GetFloat("playerSmashPower");
            if(smashPower <= 0f){
                PlayerPrefs.SetFloat("playerSmashPower", 1f);
                smashPower = 1f;
            }
        }
        else{
            PlayerPrefs.SetFloat("playerSmashPower", 1f);
        }
        if(PlayerPrefs.HasKey("playerGravityPower")){
            gravityPower = PlayerPrefs.GetFloat("playerGravityPower");
        }
        else{
            PlayerPrefs.SetFloat("playerGravityPower", gravityPower);
        }
        if(PlayerPrefs.HasKey("playerXHoming")){
            xHoming = PlayerPrefs.GetFloat("playerXHoming");
        }
        else{
            PlayerPrefs.SetFloat("playerXHoming", xHoming);
        }
        if(PlayerPrefs.HasKey("playerAutoJumps")){
            maxAutoJumps = PlayerPrefs.GetFloat("playerAutoJumps");
        }
        else{
            PlayerPrefs.SetFloat("playerAutoJumps", maxAutoJumps);
        }
        if(PlayerPrefs.HasKey("redBoostValue")){
            redBoostValue = PlayerPrefs.GetFloat("redBoostValue");
        }
        else{
            PlayerPrefs.SetFloat("redBoostValue", redBoostValue);
        }
    }
    public void UpdatePrefs(){
        PlayerPrefs.SetFloat("playerJumpHeight", jumpHeight);
        PlayerPrefs.SetFloat("playerSmashPower", smashPower);
        PlayerPrefs.SetFloat("playerGravityPower", gravityPower);
        PlayerPrefs.SetFloat("playerXHoming", xHoming);
        PlayerPrefs.SetFloat("playerAutoJumps", maxAutoJumps);
        PlayerPrefs.SetFloat("redBoostValue", redBoostValue);
    }
    void Update()
    {
        GroundCollider();
        Movement();
        Jumping();
        AccelerateDown();
        autoJumpCalculations();
        boostCalculations();
        previousVelocity = rb.velocity;
        previousYMagnitude = Mathf.Abs(rb.velocity.y);
    }

    public bool useAutoJump(float amount = 1f){
        if(autoJumpsLeft >= amount){
            autoJumpsLeft -= amount;
            return true;
        }
        return false;
    }

    private void boostCalculations(){
        if(redBoostTimeLeft > 0f){
            redBoostTimeLeft -= Time.deltaTime;
            redBoostEffect.enableEmission = true;
        }
        else{
            redBoostTimeLeft = 0f;
            redBoostEffect.enableEmission = false;
        }
    }

    private void autoJumpCalculations(){
        autoJumpsLeftText.text = autoJumpsLeft + "/" + maxAutoJumps;
        if(Input.GetKeyDown("z") && autoJumpsLeft > 0){
            autoJumpsToggled = !autoJumpsToggled;
        }
        if(autoJumpsLeft <= 0f){
            autoJumpsToggled = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Rock"){
            colliding = false;
        }
        if(other.gameObject.tag == "Ground"){
            if(autoJumpsToggled){
                autoJumpsLeft -= 1f;
            }
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
                rock.GetComponent<Rock>().Shake(-1.5f);
            }
        }
        if(other.gameObject.tag == "RedPad"){
            redBoostTimeLeft = 5f;
        }
        if(other.gameObject.tag == "TripleGravity"){
            if(!gravityTripled){
                gravityTripled = true;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "TripleGravity"){
            if(gravityTripled){
                gravityTripled = false;
            }
        }
    }

    private void GroundCollider(){
        groundCollider.transform.position = new Vector2(transform.position.x, transform.position.y);
        grounded = groundCollider.GetComponent<PlayerGroundCollision>().colliding;
    }

    private void CallCollision(GameObject other){
        if(colliding){return;}
        float boostTotal = 1f;
        if(redBoostTimeLeft > 0f){
            boostTotal *= redBoostValue;
        }
        other.GetComponent<Rock>().Collision(this.gameObject, previousYMagnitude, smashPower, boostTotal);
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

    public float GetAbsNumber(float num){
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
        if(rock != null && Mathf.Abs(transform.position.x - rock.position.x) < xHomingRange && xHoming > 0f && !grounded && !rebounding){
            rb.velocity = new Vector2(x * moveSpeed * 1.25f + (rock.position.x - transform.position.x) * xHoming, rb.velocity.y);
            rb.angularVelocity += rotationalMovementForce * rb.velocity.x * -Time.deltaTime;
        }
        //if distance to rock is less than xHomingRange, move towards rock
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
        else if(autoJumpsToggled && autoJumpsLeft > 0f && grounded == true){
            RegisterJump();
            rebounding = false;
        }
    }

    private void RegisterJump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        jumpsLeft--;
    }

    private void AccelerateDown(){
        if(gravityTripled){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (3f*gravityPower) * Time.deltaTime;
        }
        else if(rb.velocity.y < 0 || !Input.GetButton("Jump") && !autoJumpsToggled || rebounding){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f*gravityPower) * Time.deltaTime;
        }
    }
}
