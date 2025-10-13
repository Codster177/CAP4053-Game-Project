using UnityEngine;

public class TextPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float speed = 2f;
    public float scaleAmount = 0.05f; 

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = originalScale * scale;
    }
}
