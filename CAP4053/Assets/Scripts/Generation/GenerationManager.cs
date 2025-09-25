using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager publicGenerationManager;
    [SerializeField] private GameObject roomPrefabs;
    [SerializeField] private Vector2 xOffset, yOffset;

}