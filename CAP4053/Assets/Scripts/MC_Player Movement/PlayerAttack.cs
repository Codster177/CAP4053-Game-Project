using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private bool canAttack = true;
    private float lightAttackTime = 0.5f;
    void Update()
    {
        //light attack
        if (Input.GetButtonDown("LightAttack") && canAttack)
        {
            StartCoroutine(lightAttack());
            Debug.Log("You weakling...");
        }

        //heavy attack
        if (Input.GetButtonDown("HeavyAttack"))
        {
            Debug.Log("Why are you seeing this... weirdo");
        }
    }

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
    }

    IEnumerator lightAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(lightAttackTime);
        canAttack = true;
    }
}
