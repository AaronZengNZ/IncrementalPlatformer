using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperienceObject : MonoBehaviour
{
    public SpriteRenderer objectSprite;
    public Color color;
    public float startingThrowForce = 10f;
    public float chasePlayerSpeed = 5f;
    public float maxSizeMulti = 2f;
    public Rigidbody2D rb;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale *= Random.Range(1f, maxSizeMulti);
        player = GameObject.Find("Player").transform;
        startingThrowForce *= Random.Range(0.75f, 1.25f);
        StartingThrow();
        chasePlayerSpeed *= Random.Range(0.8f, 1.2f);
    }

    private void StartingThrow(){
        //throw in a random direction
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(0.5f, 1f);
        rb.AddForce(new Vector2(randomX, randomY) * startingThrowForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        objectSprite.color = color;
        if(player != null){
            ChasePlayer();
        }
    }

    private void ChasePlayer(){
        Vector2 direction = player.position - transform.position;
        direction.Normalize();
        rb.velocity *= 1 / (1 + Time.deltaTime * 5f);
        rb.velocity += direction * chasePlayerSpeed * Time.deltaTime;
        chasePlayerSpeed += Time.deltaTime * 15f;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            //add to player's experience
            Destroy(gameObject);
        }
    }
}
