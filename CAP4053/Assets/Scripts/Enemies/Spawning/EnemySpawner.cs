using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;
    float xMin, xMax, yMin, yMax;
    void Start()
    {
        Vector3 center = boxCollider.transform.position + new Vector3(boxCollider.offset.x, boxCollider.offset.y, 0f);

    }
}
