using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.Tilemaps;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager publicGenerationManager;
    [SerializeField] GenerateSegments segmentGenerator;
    [SerializeField] private GenerationDirection testGenerationDirection;
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    void Awake()
    {
        publicGenerationManager = this;
    }

    void Start()
    {
        stopwatch.Start();
        segmentGenerator.GenerateSegment(new Vector2(0f, 0f), testGenerationDirection);
        stopwatch.Stop();
        Debug.Log($"GenerationManager: Time taken to generate rooms: {stopwatch.ElapsedMilliseconds} ms.");
    }
    public static GenerationDirection GetOppositeDirection(GenerationDirection direction)
    {
        if (direction == GenerationDirection.right)
        {
            return GenerationDirection.left;
        }
        else if (direction == GenerationDirection.left)
        {
            return GenerationDirection.right;
        }
        else if (direction == GenerationDirection.up)
        {
            return GenerationDirection.down;
        }
        else if (direction == GenerationDirection.down)
        {
            return GenerationDirection.up;
        }
        else
        {
            return GenerationDirection.nullType;
        }
    }
}

public enum GenerationDirection
{
    nullType,
    up,
    right,
    down,
    left
}