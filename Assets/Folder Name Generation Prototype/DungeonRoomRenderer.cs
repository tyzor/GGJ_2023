using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonRoomRenderer : MonoBehaviour
{
    [SerializeField] private TMP_Text breadcrumbTitle;
    [SerializeField] private ComputerFileSystemGenerator cfsg;

    [SerializeField] private List<DungeonRoom> dungeonRooms;
    [SerializeField] private List<Folder> dungeonFolders;
    [SerializeField] private List<File> dungeonFiles;

    private DungeonRoom startingRoom;
    private DungeonRoom currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        dungeonRooms = cfsg.DungeonRooms();
        dungeonFolders = cfsg.DungeonFolders();
        dungeonFiles = cfsg.DungeonFiles();

        startingRoom = dungeonRooms[0];
        currentRoom = startingRoom;

        UpdateBreadcrumbForCurrentRoom();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateBreadcrumbForCurrentRoom()
    {
        breadcrumbTitle.text = "YOU ARE HERE: " + currentRoom.RoomName;
    }
}
