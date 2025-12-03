using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameOverUI deathScreen;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private float moveSpeed = 5f, dashCoolDown = 3f, dashTime = 0.5f, dashVelocity = 2f;
    [SerializeField] private bool canDash = true;
    [SerializeField] private DashParticles dashPS;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private EffectHolder effectHolder;
    private bool isDashing = false, movingUp = false, movementEnabled = true, canBeHit = true;

    // Start() establishes PlayerDeath as a function that listens to OnGameStateChanged, and gets
    // components.
    void Start()
    {
        GameManager.OnGameStateChanged += PlayerDeath;
        GameManager.publicGameManager.SetPlayerGO(gameObject);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        effectHolder = GetComponent<EffectHolder>();
        effectHolder.AddEffect(new Invincibility(4));

        if (playerAttack != null)
        {
            playerAttack.SetPlayerController(this);
        }
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
            dashPS.flipXSprite(true);
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = false; // face left
            movingUp = false;
            dashPS.flipXSprite(false);
        }

        // set animator parameter
        animator.SetFloat("Speed", (movement.sqrMagnitude * moveSpeed));
        animator.SetBool("MovingUp", movingUp);
        // float spriteIndex = dashPS.addCheckSprite(spriteRenderer.sprite);
        // dashPS.setSprite(spriteIndex);

        //looking for rainbow(dash)
        if (Input.GetButtonDown("Dash") && canDash)
        {
            // animator.SetTrigger("mc-dash");
            StartCoroutine(Dash());
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

    public void SetCanAttack(bool newState)
    {
        playerAttack.SetCanAttack(newState);
    }

    public void SetCanBeHit(bool newCanBeHit)
    {
        canBeHit = newCanBeHit;
    }

    [SerializeField] private float playerHealth = 100f;

    public void DealDamageToPlayer(float damageAmount, bool hitWhileDash)
    {
        if (isDashing && !hitWhileDash)
            return;
        if (!canBeHit)
            return;
        
        if (GameManager.publicGameManager != null)
        {
            GameManager.publicGameManager.DealDamage(damageAmount);
            Debug.Log($"Hit! Damage: {damageAmount}");
        }

        playerHealth -= damageAmount;

        //commented out for testing purposes

        //if (playerHealth <= 0)
        //{
        //    // canBeHit = false; // prevents hitting after death
        //    Debug.Log("Player died!");
        //    PlayerDeath(GameState.Death);
        //}
    }

    // Called OnGameStateChanged. Activates the player death. Triggers animation.
    void PlayerDeath(GameState newGameState)
    {
        if (newGameState == GameState.Death)
        {
            effectHolder.RemoveAllEffects();
            effectHolder.DisableEffects();
            AllowMovement(false);
            playerAttack.SetCanAttack(false);
            animator.SetTrigger("Die");

            CameraManager.publicCameraManager.FocusOnPlayer(6f);
            StartCoroutine(DeathCoroutine());
        }
    }

    public void AttackAnimation()
    {
        StartCoroutine(playerAttack.TriggerAnimation());
    }


    // Destroys player 2.2 seconds (animation length) after coroutine is called.
    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2.2f);
        spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
    }

    // Sends the player forward for the dash while disabling movement, then enables movement after the dash
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        movementEnabled = false;
        dashPS.playStop(true);
        MyUIManager.publicUIManager.DashbarAnim();
        rb.linearVelocity = new Vector2(movement.x * dashVelocity * moveSpeed, movement.y * dashVelocity * moveSpeed);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        movementEnabled = true;
        dashPS.playStop(false);
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
    //reference funtion to increase speed by percentage
    public void IncreaseMoveSpeedByPercentage(float percent)
    {
        moveSpeed += moveSpeed * (percent / 100f);
        Debug.Log($"Player move speed increased! New speed: {moveSpeed}");
    }

}
