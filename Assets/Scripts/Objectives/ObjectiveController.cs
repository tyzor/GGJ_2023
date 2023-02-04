using System;
using System.Collections.Generic;
using GGJ.Interactables;
using GGJ.Levels;
using GGJ.Utilities.Extensions;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ.Objectives
{
    public enum OBJECTIVE_TYPE
    {
        NONE,
        FIND,
        DESTROY,
        MOVE
    }
    public class ObjectiveController : MonoBehaviour
    {
        public static event Action<(OBJECTIVE_TYPE objective, File targetFile, FolderRoom targetRoom)> OnNewObjective;

        private RoomManager _roomManager;

        private OBJECTIVE_TYPE _currentObjective;
        private File _targetFile;
        private FolderRoom _targetRoom;

        //Flattened Dungeon Data
        //------------------------------------------------//
        private FolderRoom _rootFolderRoom;
        private List<FolderRoom> _folderRooms;
        private List<File> _files;

        //Unity Function
        //============================================================================================================//

        private void OnEnable()
        {
            //Temporary for debugging
            //------------------------------------------------//
            FileInteractable.OnPickedUpFile += OnPickedUpFile;
            FileInteractable.OnDroppedFile += OnDroppedFile;
            FileInteractable.OnRecycledFile += OnRecycledFile;
        }

        private void Start()
        {
            _roomManager = FindObjectOfType<RoomManager>();
        }

        private void OnDisable()
        {
            FileInteractable.OnPickedUpFile -= TryCompleteObjective;
            FileInteractable.OnDroppedFile -= TryCompleteObjective;
            FileInteractable.OnRecycledFile -= TryCompleteObjective;
            
            //Temporary for debugging
            //------------------------------------------------//
            FileInteractable.OnPickedUpFile -= OnPickedUpFile;
            FileInteractable.OnDroppedFile -= OnDroppedFile;
            FileInteractable.OnRecycledFile -= OnRecycledFile;
        }

        //Objective Generation
        //============================================================================================================//
        public void GenerateObjective()
        {
            //[0] NONE
            //[1] FIND
            //[2] DESTROY
            //[3] MOVE
            var newObjective = (OBJECTIVE_TYPE)Random.Range(1, 4);
            var targetFile = _files.GetRandomItem();
            var targetRoom = newObjective == OBJECTIVE_TYPE.MOVE ? _folderRooms.GetRandomItem() : default;

            SetupNewObjective(newObjective, targetFile, targetRoom);
        }
        private void SetupNewObjective(OBJECTIVE_TYPE newType, File targetFile, FolderRoom targetRoom)
        {
            _currentObjective = newType;
            _targetFile = targetFile;
            _targetRoom = null;
            
            //TODO Pick new Objective
            switch (_currentObjective)
            {
                case OBJECTIVE_TYPE.NONE:
                    return;
                case OBJECTIVE_TYPE.FIND:
                    FileInteractable.OnPickedUpFile += TryCompleteObjective;
                    break;
                case OBJECTIVE_TYPE.DESTROY:
                    FileInteractable.OnRecycledFile += TryCompleteObjective;
                    break;
                case OBJECTIVE_TYPE.MOVE:
                    _targetRoom = targetRoom;
                    FileInteractable.OnDroppedFile += TryCompleteObjective;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_currentObjective == OBJECTIVE_TYPE.MOVE)
            {
                Debug.Log($"Objective is {_currentObjective.ToString()} {_targetFile.GetFileNameExtension()} " +
                          $"to {targetRoom.GetAbsolutePath()}");

            }
            else
                Debug.Log($"Objective is {_currentObjective.ToString()} {_targetFile.GetFileNameExtension()}");

            OnNewObjective?.Invoke((_currentObjective, _targetFile, _targetRoom));
        }

        //Complete Objectives
        //============================================================================================================//
        
        private void TryCompleteObjective(FileInteractable fileInteractable)
        {
            if (CheckDidCompleteObjective(fileInteractable) == false)
                return;

            Debug.Log($"Completed Objective {_currentObjective.ToString()} {_targetFile.GetFileNameExtension()}");
            
            if (_currentObjective == OBJECTIVE_TYPE.DESTROY)
            {
                _files.Remove(_targetFile);
                _targetFile = null;
            }
            
            //TODO Complete Objective
            //TODO Give reward/points
            
            
            FileInteractable.OnPickedUpFile -= TryCompleteObjective;
            FileInteractable.OnDroppedFile -= TryCompleteObjective;
            FileInteractable.OnRecycledFile -= TryCompleteObjective;
            
            //Assign new Objective / Move to new dungeon
            GenerateObjective();
        }

        private bool CheckDidCompleteObjective(FileInteractable fileInteractable)
        {
            switch (_currentObjective)
            {
                case OBJECTIVE_TYPE.NONE:
                    return false;
                case OBJECTIVE_TYPE.FIND:
                case OBJECTIVE_TYPE.DESTROY:
                    return fileInteractable.FileData == _targetFile;
                case OBJECTIVE_TYPE.MOVE:
                    return fileInteractable.FileData == _targetFile && RoomManager.CurrentRoom.FolderRoomData == _targetRoom;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //============================================================================================================//

        public void SetCurrentDungeon(FolderRoom rootFolderRoom)
        {
            //------------------------------------------------//
            void GetFileFoldersRecursive(in FolderRoom folderRoom, ref List<File> files, ref List<FolderRoom> folderRooms)
            {
                if(folderRoom.Files != null && folderRoom.Files.Length > 0)
                    files.AddRange(folderRoom.Files);
                
                foreach (var folder in folderRoom.Subfolders)
                {
                    folderRooms.Add(folder);
                    GetFileFoldersRecursive(folder, ref files, ref folderRooms);
                }
            }

            //------------------------------------------------//
            _rootFolderRoom = rootFolderRoom;

            _files = new List<File>();
            _folderRooms = new List<FolderRoom>();
            GetFileFoldersRecursive(_rootFolderRoom, ref _files, ref _folderRooms);
        }

        //TEMPORARY Callbacks
        //============================================================================================================//

        private void OnPickedUpFile(FileInteractable fileInteractable)
        {
            Debug.Log($"Picked up file {fileInteractable.gameObject.name}", fileInteractable);
        }

        private void OnDroppedFile(FileInteractable fileInteractable)
        {
            Debug.Log($"Dropped file {fileInteractable.gameObject.name}", fileInteractable);
        }
        
        private void OnRecycledFile(FileInteractable fileInteractable)
        {
            Debug.Log($"Recycled file {fileInteractable.gameObject.name}", fileInteractable);
        }
        
        //============================================================================================================//
    }
}