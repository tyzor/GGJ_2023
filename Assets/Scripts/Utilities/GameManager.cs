using Cinemachine;
using GGJ.Levels;
using GGJ.Objectives;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;

namespace GGJ.Utilities
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;
        
        [SerializeField]
        private GameObject playerPrefab;
        private RoomManager _roomManager;
        private ObjectiveController _objectiveController;

        private GameObject _currentPlayer;
        private Transform _currentPlayerTransform;

        [SerializeField]
        private DungeonProfile[] dungeonProfiles;

        private int _currentDungeonIndex;
        private FolderRoom _dungeonRoot;

        //============================================================================================================//

        // Start is called before the first frame update
        private void Start()
        {
            _roomManager = FindObjectOfType<RoomManager>();
            _objectiveController = FindObjectOfType<ObjectiveController>();

            _currentPlayer = Instantiate(playerPrefab);
            _currentPlayerTransform = _currentPlayer.transform;
            
            virtualCamera.Follow = _currentPlayerTransform;

            SetupDungeon(_currentDungeonIndex);
        }

        //============================================================================================================//

        private void SetupDungeon(int index)
        {
            var rootFolderRoom = _roomManager.GenerateDungeon(dungeonProfiles[index]);
            
            _roomManager.SetRoom(-1, rootFolderRoom);
            _objectiveController.SetCurrentDungeon(rootFolderRoom);
            
            _objectiveController.GenerateObjective();
        }
    }
}
