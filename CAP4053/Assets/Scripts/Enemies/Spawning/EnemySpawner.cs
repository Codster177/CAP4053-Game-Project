using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;
    float xBound, yBound;
    void Start()
    {
        Vector3 center = boxCollider.transform.position + new Vector3(boxCollider.offset.x, boxCollider.offset.y, 0f);
    }
}
