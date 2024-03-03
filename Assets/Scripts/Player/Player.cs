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
    public TextMeshProUGUI autoJumpsToggledUIText;
    public TextMeshProUGUI autoJumpsLeftUIText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI redPadTimeLeftText;
    public TextMeshProUGUI greenPadTimeLeftText;
    public TextMeshProUGUI bluePadTimeLeftText;
    public ParticleSystem redBoostEffect;
    public ParticleSystem greenBoostEffect;
    public ParticleSystem blueBoostEffect;
    public TrailRenderer[] trails;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;
    public float jumps = 1f;
    private float jumpsLeft = 1f;
    public float xHoming = 0f;
    public float xHomingRange = 3f;
    public bool dashUnlocked = false;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public bool dashing = false;
    public bool dashAvailable = false;
    [Header("Rebound Values")]
    public float xReboundForce = 0.7f;
    public float yReboundMultiplier = 0.6f;
    public float rotationalReboundForce = 10f;
    public float rotationalMovementForce = 10f;
    public float rotationalMovementCap = 100f;
    [Header("Other Values")]
    public float smashPower = 1f;
    public float gravityPower = 1f;
    public float currentTrail = 1f;
    public float minimumAltitude = 1f;
    public float chanceToNotUseAutojump = 0f;
    [Header("Auto Jumps")]
    public float maxAutoJumps = 0f;
    public float autoJumpRegenSpeed = 5f;
    public float autoJumpsLeft = 0f;
    public bool autoJumpsToggled = false;
    [Header("Pads")]
    public float redBoostTimeLeft = 0f;
    public float redBoostValue = 5f;
    public float redPadDuration = 5f;
    public bool touchingGreenPad = false;
    public float greenPadPower = 2f;
    public float bluePadTimeLeft = 0f;
    public float bluePadDuration = 60f;
    public float bluePadPower = 0.1f;
    [Header("Tutorial Variables")]
    public bool tutorialCompleted = false;
    public Transform tutorialSpawn;
    public Transform normalSpawn;
    private bool gravityTripled = false;
    private bool gravityQuintupled = false;
    private bool grounded = false;
    private bool rebounding = false;
    private bool colliding = false;

    private Vector2 previousVelocity = new Vector2(0, 0);
    private float previousYMagnitude = 0;
    private bool upgrading = false;

    void Start()
    {
        rebounding = false;
        colliding = false;
        autoJumpsLeftUIText.text = "";
        autoJumpsToggledUIText.text = "";
        CheckPrefs();
        if(tutorialCompleted){
            transform.position = normalSpawn.position;
        }
        else{
            transform.position = tutorialSpawn.position;
        }
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
        if(PlayerPrefs.HasKey("playerXHomingRange")){
            xHomingRange = PlayerPrefs.GetFloat("playerXHomingRange");
        }
        else{
            PlayerPrefs.SetFloat("playerXHomingRange", xHomingRange);
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
        if(PlayerPrefs.HasKey("redPadDuration")){
            redPadDuration = PlayerPrefs.GetFloat("redPadDuration");
        }
        else{
            PlayerPrefs.SetFloat("redPadDuration", redPadDuration);
        }
        if(PlayerPrefs.HasKey("greenBoostValue")){
            greenPadPower = PlayerPrefs.GetFloat("greenBoostValue");
        }
        else{
            PlayerPrefs.SetFloat("greenBoostValue", greenPadPower);
        }
        if(PlayerPrefs.HasKey("tutorialCompleted")){
            tutorialCompleted = PlayerPrefs.GetInt("tutorialCompleted") == 1;
        }
        else{
            PlayerPrefs.SetInt("tutorialCompleted", 0);
        }
        if(PlayerPrefs.HasKey("dashUnlocked")){
            dashUnlocked = PlayerPrefs.GetInt("dashUnlocked") == 1;
        }
        else{
            PlayerPrefs.SetInt("dashUnlocked", 0);
        }
        if(PlayerPrefs.HasKey("bluePadPower")){
            bluePadPower = PlayerPrefs.GetFloat("bluePadPower");
        }
        else{
            PlayerPrefs.SetFloat("bluePadPower", bluePadPower);
        }
        if(PlayerPrefs.HasKey("bluePadDuration")){
            bluePadDuration = PlayerPrefs.GetFloat("bluePadDuration");
        }
        else{
            PlayerPrefs.SetFloat("bluePadDuration", bluePadDuration);
        }
        if(PlayerPrefs.HasKey("chanceToNotUseAutojump")){
            chanceToNotUseAutojump = PlayerPrefs.GetFloat("chanceToNotUseAutojump");
        }
        else{
            PlayerPrefs.SetFloat("chanceToNotUseAutojump", chanceToNotUseAutojump);
        }
    }
    public void UpdatePrefs(){
        PlayerPrefs.SetFloat("playerJumpHeight", jumpHeight);
        PlayerPrefs.SetFloat("playerSmashPower", smashPower);
        PlayerPrefs.SetFloat("playerGravityPower", gravityPower);
        PlayerPrefs.SetFloat("playerXHomingRange", xHomingRange);
        PlayerPrefs.SetFloat("playerAutoJumps", maxAutoJumps);
        PlayerPrefs.SetFloat("redBoostValue", redBoostValue);
        PlayerPrefs.SetFloat("greenBoostValue", greenPadPower);
        PlayerPrefs.SetFloat("bluePadPower", bluePadPower);
        PlayerPrefs.SetFloat("bluePadDuration", bluePadDuration);
        PlayerPrefs.SetFloat("chanceToNotUseAutojump", chanceToNotUseAutojump);
        PlayerPrefs.SetFloat("redPadDuration", redPadDuration);
        PlayerPrefs.SetInt("dashUnlocked", dashUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("tutorialCompleted", tutorialCompleted ? 1 : 0);
    }
    void Update()
    {
        GroundCollider();
        Movement();
        Jumping();
        AccelerateDown();
        autoJumpCalculations();
        boostCalculations();
        upgradeRebound();
        updateTrails();
        Dashing();
        updateAltitudeText();
        updatePadTimeLeftTexts();
        previousVelocity = rb.velocity;
        previousYMagnitude = Mathf.Abs(rb.velocity.y);

    }

    private void updatePadTimeLeftTexts(){
        if(redBoostTimeLeft > 0f){
            redPadTimeLeftText.gameObject.SetActive(true);
            redPadTimeLeftText.text = "Red Pad: " + (Mathf.Round(redBoostTimeLeft*100)/100f).ToString("F2") + " s";
        }
        else{
            redPadTimeLeftText.text = "";
            redPadTimeLeftText.gameObject.SetActive(false);
        }
        if(bluePadTimeLeft > 0f){
            bluePadTimeLeftText.gameObject.SetActive(true);
            bluePadTimeLeftText.text = "Blue Pad: " + (Mathf.Round(bluePadTimeLeft*100)/100f).ToString("F2") + " s";
        }
        else{
            bluePadTimeLeftText.text = "";
            bluePadTimeLeftText.gameObject.SetActive(false);
        }
        if(touchingGreenPad){
            greenPadTimeLeftText.gameObject.SetActive(true);
            greenPadTimeLeftText.text = "Green Pad: Active";
        }
        else{
            greenPadTimeLeftText.text = "";
            greenPadTimeLeftText.gameObject.SetActive(false);
        }
    }
    private void updateAltitudeText(){
        altitudeText.text = "Altitude: " + (Mathf.Round(transform.position.y) - minimumAltitude) + " units";
    }

    public void changeTrail(float number){
        currentTrail = number;
    }

    private void updateTrails(){
        for(int i = 0; i < trails.Length; i++){
            if(i == currentTrail-1f){
                trails[i].emitting = true;
            }
            else{
                trails[i].emitting = false;
            }
        }
    }

    private void upgradeRebound(){
        if(previousVelocity.y > 0 && upgrading){
            rb.velocity = new Vector2(rb.velocity.x, -previousVelocity.y);
        }
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
        if(bluePadTimeLeft > 0f){
            bluePadTimeLeft -= Time.deltaTime;
            blueBoostEffect.enableEmission = true;
        }
        else{
            blueBoostEffect.enableEmission = false;
        }
        greenBoostEffect.enableEmission = touchingGreenPad;
    }

    private void autoJumpCalculations(){
        autoJumpsLeftText.text = autoJumpsLeft + "/" + maxAutoJumps;
        if(Input.GetKeyDown("z") && autoJumpsLeft > 0){
            autoJumpsToggled = !autoJumpsToggled;
        }
        if(autoJumpsLeft <= 0f){
            autoJumpsToggled = false;
        }
        if(maxAutoJumps > 0f){
            if(autoJumpsToggled){
                autoJumpsToggledUIText.text = "Auto Jumps: On [Press z to toggle]";
            }
            else{
                autoJumpsToggledUIText.text = "Auto Jumps: Off [Press z to toggle]";
            }
        }
        if(maxAutoJumps > 0f){
            autoJumpsLeftUIText.text = "[" + autoJumpsLeft + "/" + maxAutoJumps + "]";
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
            upgrading = false;
        }
        if(other.gameObject.tag == "Rock"){
            CallCollision(other.gameObject);
            CollisionRebound(other.transform);
            colliding = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Upgrade" && !autoJumpsToggled && !upgrading){
            upgrading = true;
            if(previousVelocity.y > 0f){
                UnityEngine.Debug.Log(rb.velocity.y + " & " + previousVelocity.y);
                other.GetComponent<Buyable>().TryUpgrade();
                rock.GetComponent<Rock>().Shake(-1.5f);
            }
        }
        if(other.gameObject.tag == "RedPad"){
            redBoostTimeLeft = redPadDuration;
        }
        if(other.gameObject.tag == "GreenPad"){
            touchingGreenPad = true;
        }
        if(other.gameObject.tag == "BluePad"){
            bluePadTimeLeft = bluePadDuration;
        }
        if(other.gameObject.tag == "TripleGravity"){
            if(!gravityTripled){
                gravityTripled = true;
            }
        }
        if(other.gameObject.tag == "QuintipleGravity"){
            if(!gravityQuintupled){
                gravityQuintupled = true;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "TripleGravity"){
            if(gravityTripled){
                gravityTripled = false;
            }
        }
        if(other.gameObject.tag == "QuintipleGravity"){
            if(gravityQuintupled){
                gravityQuintupled = false;
            }
        }
        if(other.gameObject.tag == "GreenPad"){
            touchingGreenPad = false;
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
            if(bluePadTimeLeft > 0f){
                boostTotal *= 1f + (greenPadPower*bluePadPower);
            }
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
        if(dashing){return;}
        float x = Input.GetAxis("Horizontal");
        if(!Input.GetButton("InputRight") && !Input.GetButton("InputLeft")){
            x = Input.GetAxis("Horizontal") / 3f;
        }
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        rb.angularVelocity += rotationalMovementForce * rb.velocity.x * -Time.deltaTime;
        if(rock != null && Mathf.Abs(transform.position.x - rock.position.x) < xHomingRange && xHoming > 0f && !grounded && !rebounding){
            rb.velocity = new Vector2((rock.position.x - transform.position.x) * xHoming, rb.velocity.y);
            if(Input.GetButton("InputRight") || Input.GetButton("InputLeft")){
                rb.velocity = new Vector2(x * moveSpeed + (rock.position.x - transform.position.x) * xHoming * 0.25f, rb.velocity.y);
            }
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

    private void Dashing(){
        if(!dashUnlocked){return;}
        if(grounded && !dashing){
            dashAvailable = true;
        }
        if(dashAvailable && Input.GetKeyDown("space") && !rebounding && !dashing){
            //get horizontal and vertical input
            dashAvailable = false;
            dashing = true;
            StartCoroutine(DashTimer());
        }
    }

    IEnumerator DashTimer(){
        if(dashing == true){
            float dashTime = dashDuration;
            while(dashTime > 0){
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                rb.velocity = new Vector2(x * dashSpeed*1.25f, y * dashSpeed);
                dashTime -= Time.deltaTime;
                //apply a LOT of spin in the correct direction 
                float direction = 1f;
                if(x < 0f){
                    direction = -1f;
                }
                rb.angularVelocity = direction * 6000f;
                yield return null;
            }
            dashing = false;
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
        float tempJumpHeight = jumpHeight;
        if(gravityQuintupled){
            tempJumpHeight /= 2f;
        }
        else if(gravityTripled){
            tempJumpHeight /= 1.25f;
        }
        rb.velocity = new Vector2(rb.velocity.x, tempJumpHeight);
        jumpsLeft--;
    }

    private void AccelerateDown(){
        if(gravityTripled || gravityQuintupled){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (7.5f) * Time.deltaTime;
        }
        else if(rb.velocity.y < 0 || !Input.GetButton("Jump") && !autoJumpsToggled || rebounding || !tutorialCompleted){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f*gravityPower) * Time.deltaTime;
        }
    }
}
