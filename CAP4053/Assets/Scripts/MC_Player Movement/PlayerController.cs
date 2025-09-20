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

    public bool canDash = true;
    private bool isDashing = false;
    private float dashVelocity = 15f;
    private float dashTime = 0.5f;
    private float dashCoolDown = 3f;

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
        if (Input.GetButtonDown("Dash") && canDash)
        {
            animator.SetTrigger("mc-dash");
            StartCoroutine(Dash());
            Debug.Log("Left shift was pressed and Rainbowdash is saved!");
        }
    }

    void FixedUpdate()
    {
        // move player
        if (movementEnabled)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        
        if (isDashing)
        {
            return;
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

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        movementEnabled = false;
        rb.linearVelocity = new Vector2(movement.x * dashVelocity, movement.y * dashVelocity);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        movementEnabled = true;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
        
    }

}
