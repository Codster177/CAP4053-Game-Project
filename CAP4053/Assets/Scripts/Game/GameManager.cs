using TMPro;
using System;
using UnityEngine;

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
    public Vector3 GetPlayerLocation()
    {
        return playerTransform.position;
    }
    public void DealDamage(float damage)
    {
        float newHealth = playerHealth - damage;
        if (newHealth <= 0)
        {
            newHealth = 0;
            ChangeGameState(GameState.Death);
        }
        playerHealth = newHealth;
    }
    public float GetPlayerHealth()
    {
        return playerHealth;
    }

    public GameState GetGameState()
    {
        return CurrentGameState;
    }
    public void ChangeGameState(GameState Event)
    {
        if (CurrentGameState != Event)
        {
            CurrentGameState = Event;
            OnGameStateChanged?.Invoke(Event);
            Debug.Log("Changed");
            Debug.Log(OnGameStateChanged);
        }
    }
}

public enum GameState
{
    MainGameplay,
    Death
}