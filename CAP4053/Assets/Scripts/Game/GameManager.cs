using TMPro;
using System;
using UnityEngine;
using NavMeshPlus.Components;

public class GameManager : MonoBehaviour
{
    // Hey guys uhhhhhhh
    // I got a little carried away with making the GameManager efficient
    // If you have any questions about it ask but it'll probably take me
    // a bit to explain.

    public static GameManager publicGameManager;
    private GameState CurrentGameState;
    public static event Action<GameState> OnGameStateChanged;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float playerHealth;
    [SerializeField] private TMP_Text healthLabel;
    [SerializeField] private NavMeshSurface navMeshSurface;
    private GameObject playerGO;
    private PlayerAttack playerAttack;

    // Establishes the singleton as the static variable in the scene.
    void Awake()
    {
        publicGameManager = this;
        OnGameStateChanged = null;
    }
    // Quick Debug
    void Update()
    {
        healthLabel.text = "DEBUG: Health = " + playerHealth;
    }
    // Returns the player location as a vector.
    public Transform GetPlayerLocation()
    {
        return playerTransform;
    }
    // Deals a set amount of damage to the global player health.
    public void DealDamage(float damage)
    {
        MyUIManager.publicUIManager.HealthbarDamageAnim(damage);
        float newHealth = playerHealth - damage;
        if (newHealth <= 0)
        {
            newHealth = 0;
            ChangeGameState(GameState.Death);
        }
        playerHealth = newHealth;
    }
    // Return's the player health.
    public float GetPlayerHealth()
    {
        return playerHealth;
    }
    // Returns the current gamestate. 
    public GameState GetGameState()
    {
        return CurrentGameState;
    }

    // USE THE FOLLOWING THREE AS LITTLE AS POSSIBLE.
    // Returns player game object.
    public GameObject GetPlayerGO()
    {
        return playerGO;
    }
    public PlayerAttack GetPlayerAttack()
    {
        return playerAttack;
    }
    public void RegenerateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    public void SetPlayerGO(GameObject gameObject)
    {
        playerGO = gameObject;
        playerAttack = playerGO.GetComponentInChildren<PlayerAttack>();
    }
    // Changes the current gamestate to a new gamestate. Activates action OnGameStateChanged.
    public void ChangeGameState(GameState Event)
    {
        if (CurrentGameState != Event)
        {
            CurrentGameState = Event;
            OnGameStateChanged?.Invoke(Event);
        }
    }
}

// The set of gamestatest that the game can occupy.
public enum GameState
{
    MainGameplay,
    Death
}