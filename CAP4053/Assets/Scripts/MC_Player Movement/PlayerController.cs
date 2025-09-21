using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f, dashCoolDown = 3f, dashTime = 0.5f, dashVelocity = 15f;
    [SerializeField] private bool canDash = true;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private bool isDashing = false, movingUp = false, movementEnabled = true;

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

    public void AllowMovement(bool newState)
    {
        movementEnabled = newState;
    }

    public bool CanEnemyHit(bool hitWhileDash)
    {
        if (!isDashing || hitWhileDash)
        {
            return true;
        }
        return false;
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
        rb.linearVelocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;

    }

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
