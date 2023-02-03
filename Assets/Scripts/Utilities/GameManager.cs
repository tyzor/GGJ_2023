using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Interactables;
using GGJ.Levels;
using GGJ.Utilities.Extensions;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;

namespace GGJ.Utilities
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerPrefab;
        private RoomManager _roomManager;

        private GameObject _currentPlayer;
        private Transform _currentPlayerTransform;

        [SerializeField]
        private DungeonProfile[] dungeonProfiles;

        private int _currentDungeonIndex;

        //============================================================================================================//

        // Start is called before the first frame update
        private void Start()
        {
            _roomManager = FindObjectOfType<RoomManager>();

            var folderRoom = _roomManager.GenerateDungeon(dungeonProfiles[0]);

            _currentPlayer = Instantiate(playerPrefab);
            _currentPlayerTransform = _currentPlayer.transform;
            
            _roomManager.SetRoom(-1, folderRoom);
        }

        //============================================================================================================//

    }
}
