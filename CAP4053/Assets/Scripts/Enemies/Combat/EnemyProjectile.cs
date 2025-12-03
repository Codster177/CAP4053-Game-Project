using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private bool canDamageDashingPlayer = false;
    void Start()
    {
        //destroys bullet after 5 seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                
                player.DealDamageToPlayer(damageAmount, canDamageDashingPlayer);
                Debug.Log($"Bullet hit player! Dealt {damageAmount} damage.");
            }

            
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            Destroy(gameObject);
        }
    }
}