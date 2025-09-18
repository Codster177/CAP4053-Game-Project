using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // flip sprite (left/right)
        if (movement.x > 0)
            transform.localScale = new Vector3(-1, 1, 1); // face right
        else if (movement.x < 0)
            transform.localScale = new Vector3(1, 1, 1); // face left

        // set animator parameter
        animator.SetFloat("Speed", movement.sqrMagnitude);
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
