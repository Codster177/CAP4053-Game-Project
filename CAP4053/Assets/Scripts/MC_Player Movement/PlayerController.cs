using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;

    [SerializeField] private float moveSpeed = 5f, dashCoolDown = 3f, dashTime = 0.5f, dashVelocity = 15f;
    [SerializeField] private bool canDash = true;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private bool isDashing = false, movingUp = false, movementEnabled = true;

    // Start() establishes PlayerDeath as a function that listens to OnGameStateChanged, and gets
    // components.
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
        animator.SetFloat("Speed", (movement.sqrMagnitude * moveSpeed));
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
            float diagFix = 1f;
            if (movement.x != 0 && movement.y != 0)
            {
                diagFix = 0.75f;
            }
            Vector3 fixedMovement = (movement * moveSpeed * diagFix * Time.fixedDeltaTime);
            transform.position += new Vector3(fixedMovement.x, fixedMovement.y, 0f);
            // rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        if (isDashing)
        {
            return;
        }
    }

    // Enables or disables movement.
    public void AllowMovement(bool newState)
    {
        movementEnabled = newState;
    }

    // Asks player if conditions are occuring where the enemy can or cannot hit them.
    public bool CanEnemyHit(bool hitWhileDash)
    {
        if (!isDashing || hitWhileDash)
        {
            return true;
        }
        return false;
    }

    // Called OnGameStateChanged. Activates the player death. Triggers animation.
    void PlayerDeath(GameState newGameState)
    {
        if (newGameState == GameState.Death)
        {
            AllowMovement(false);
            animator.SetTrigger("Die");
            if (deathScreen) deathScreen.SetActive(true);
            StartCoroutine(DeathCoroutine());

        }
    }
    // Destroys player 2.2 seconds (animation length) after coroutine is called.
    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2.2f);
        Destroy(gameObject);
    }

    // Sends the player forward for the dash while disabling movement, then enables movement after the dash
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        movementEnabled = false;
        rb.linearVelocity = new Vector2(movement.x * dashVelocity, movement.y * dashVelocity);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        movementEnabled = true;
        rb.linearVelocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;

    }

    // Reads collisions to stop dash if the player hits a wall.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (isDashing)
            {
                isDashing = false;
                movementEnabled = true;
                rb.linearVelocity = new Vector2(0f, 0f);
            }
        }
    }

}
