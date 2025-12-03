using UnityEngine;

public class EnemySpawnTrigger : MonoBehaviour
{
    private RoomController roomController;

    public void SetRoomController(RoomController newController)
    {
        roomController = newController;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CheckTagForPlayer(collision))
        {
            return;
        }
        roomController.SpawnEnemies();
    }
    bool CheckTagForPlayer(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
