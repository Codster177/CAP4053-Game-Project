using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private bool movingUp = false, movementEnabled = true;

    void Start()
    {
        GameManager.OnGameStateChanged += PlayerDeath;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        // flip sprite (left/right)
        if (movement.x > 0)
        {
            spriteRenderer.flipX = true; // face right
            movingUp = false;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = false; // face left
            movingUp = false;
        }

        // set animator parameter
        animator.SetFloat("Speed", movement.sqrMagnitude);
        animator.SetBool("MovingUp", movingUp);

        //looking for rainbow(dash)
        if (Input.GetButtonDown("Dash"))
        {
            Debug.Log("Left shift was pressed and Rainbowdash is saved!");
        }

        //light attack
        if (Input.GetButtonDown("LightAttack"))
        {
            Debug.Log("You weakling...");
        }

        //heavy attack
        if (Input.GetButtonDown("HeavyAttack"))
        {
            Debug.Log("Why are you seeing this... weirdo");
        }
    }

    void FixedUpdate()
    {
        // move player
        if (movementEnabled)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void AllowMovement(bool newState)
    {
        movementEnabled = newState;
    }

    void PlayerDeath(GameState newGameState)
    {
        if (newGameState == GameState.Death)
        {
            AllowMovement(false);
            animator.SetTrigger("Die");
            StartCoroutine(DeathCoroutine());
        }
    }
    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2.2f);
        Destroy(gameObject);
    }

}
