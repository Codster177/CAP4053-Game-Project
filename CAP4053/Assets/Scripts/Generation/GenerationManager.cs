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
}

public enum GenerationDirection
{
    nullType,
    up,
    right,
    down,
    left
}