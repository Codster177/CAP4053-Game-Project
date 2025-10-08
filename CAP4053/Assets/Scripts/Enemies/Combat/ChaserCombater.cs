using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChaserCombater : EnemyCombater
{
    protected ChaserController chaserController;
    [SerializeField] private List<GameObject> inRange;
    [SerializeField] private float damageAmount, attackSpeed, enemyKnockback, knockbackTime;
    [SerializeField] private bool hitWhileDash;
    private Coroutine currentAttack;

    void Start()
    {
        chaserController = controller as ChaserController;
    }
    // Recognizes the player when encountering them in the enemies trigger collider.
    void Update()
    {
        if (currentAttack == null)
        {
            currentAttack = StartCoroutine(StartAttack());
        }
        if (inRange.Count != 0)
        {
            chaserController.SetPosition(false, transform.position);
        }
        else
        {
            chaserController.SetPosition(true, new Vector3());
        }
    }
    private IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(attackSpeed);
        for (int i = 0; i < inRange.Count; i++)
        {
            Vector2 knockbackDir = Knockback.CalculateDir(inRange[i].transform.position, controller.transform.position);
            HitWithKnockback(inRange[i], damageAmount, knockbackDir, knockbackTime, hitWhileDash);
        }
        currentAttack = null;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks tag to be player.
        if (collision.tag == "Player")
        {
            // Calculates the knockback direction, and sends the data to the EnemyCombater to knockback the player.
            if (!inRange.Contains(collision.gameObject))
            {
                inRange.Add(collision.gameObject);
            }
        }
        else
        {
            return;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        // Checks tag to be player.
        if (collision.tag == "Player")
        {
            // Calculates the knockback direction, and sends the data to the EnemyCombater to knockback the player.
            if (inRange.Contains(collision.gameObject))
            {
                inRange.Remove(collision.gameObject);
            }
            if (currentAttack != null)
            {
                StopCoroutine(currentAttack);
                currentAttack = null;
            }
        }
        else
        {
            return;
        }
    }
}
