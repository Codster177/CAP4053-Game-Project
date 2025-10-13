using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomProgressionManager : MonoBehaviour
{
    public static RoomProgressionManager Instance;

    //hash to hold how many rooms we've visited
    private HashSet<RoomController> visitedRooms = new HashSet<RoomController>();
    private int roomsVisited = 0;

    [SerializeField] private UpgradeUIManager upgradeUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterRoomEntry(RoomController room)
    {
        // checks to see if rooms were visited
        if (visitedRooms.Contains(room)) return;

        visitedRooms.Add(room);
        roomsVisited++;

        Debug.Log($"RoomProgressionManager: Entered {roomsVisited} unique rooms.");

        if (roomsVisited % 3 == 0)
        {
            TriggerUpgradeChoice();
        }
    }

    private void TriggerUpgradeChoice()
    {
        if (upgradeUI == null)
        {
            Debug.LogError("RoomProgressionManager: UpgradeUI reference is missing!");
            return;
        }

        
        StartCoroutine(DelayedUpgradeUI());
    }

    //delays pause for upgrade so camera can move
    private IEnumerator DelayedUpgradeUI()
    {
        yield return new WaitForSeconds(2f); //delayed time when entering room
        Time.timeScale = 0f; 
        upgradeUI.ShowUpgradeOptions();
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
