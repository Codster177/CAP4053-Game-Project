using UnityEngine;

public class RoomTrigger : MonoBehaviour
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
        roomController.QueueRoom(transform);
        roomController.SpawnEnemies();
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (!CheckTagForPlayer(collision))
        {
            return;
        }
        roomController.DequeueRoom(transform);
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
