using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private bool movingUp = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        //looking for rainbow(dash)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Left shift was pressed");
        }

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
    }

    void FixedUpdate()
    {
        // move player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void MoveFromDamage(Vector2 direction, float enemyKnockback)
    {
        Vector2 scaledDirection = direction * enemyKnockback;
        rb.AddForce(scaledDirection);
    }

}
