using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    public enum RoomType { Basic, Health, Gold, Loot}
    [SerializeField] Transform playerTransform;
    public bool isActive = false;
    public RoomType roomType1;
    public RoomType roomType2;
    public int room1Level = 1;
    public int room2Level = 1;

    private void Start()
    {
        // Randomly assign the two room types
        roomType1 = (RoomType)Random.Range(0, System.Enum.GetValues(typeof(RoomType)).Length);
        roomType2 = (RoomType)Random.Range(0, System.Enum.GetValues(typeof(RoomType)).Length);
        room1Level = Random.Range(1, 3);
        room2Level = Random.Range(1, 3);
    }
    private void Update()
    {
        if (!isActive)
            return;
        
    }


    public void RoomCompleted()
    {
        isActive = true;
    }

    public void OpenDoor()
    {
        if (isActive)
        {
            int room = Random.Range(1, 2);
            if (room == 1)
            {
                GameObject.Find("DungeonManager").GetComponent<RoomGenerator>().GenerateRoom(roomType1, room1Level, new Vector3(0f,0f,0f));
            }
            else
            {
                GameObject.Find("DungeonManager").GetComponent<RoomGenerator>().GenerateRoom(roomType2, room2Level, new Vector3(0f,0f,0f));
            }
        }
    }
    public string getRoomType(RoomType room)
    {
        switch (room)
        {
            case RoomType.Basic:
                return "Basic";
            case RoomType.Health:
                return "Health";
            case RoomType.Gold:
                return "Gold";
            case RoomType.Loot:
                return "Loot";
            default:
                return "UnknownRoom";
        }
    }
}
