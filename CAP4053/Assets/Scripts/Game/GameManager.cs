using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager publicGameManager;

    void Awake()
    {
        publicGameManager = this;
    }
}
