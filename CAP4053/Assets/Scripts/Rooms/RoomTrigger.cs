using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private RoomController roomController;
    private bool insideRoom;

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
        insideRoom = true;
        roomController.QueueRoom(transform);

        // spawns enemies only after we left the first room
        roomController.SpawnEnemies();
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (!CheckTagForPlayer(collision))
        {
            return;
        }
        insideRoom = false;
        roomController.DequeueRoom(transform);

    }
    public bool GetInsideRoom()
    {
        return insideRoom;
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
