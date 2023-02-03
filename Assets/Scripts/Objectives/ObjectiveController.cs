using System;
using GGJ.Interactables;
using GGJ.Levels;
using UnityEngine;

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
        public static event Action OnNewObjective;

        private RoomManager _roomManager;

        //FIXME Marked as Serialized for testing
        [SerializeField]private OBJECTIVE_TYPE _currentObjective;
        [SerializeField]private FileInteractable _objectiveTarget;
        private Room _roomTarget;

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
            SetupNewObjective();
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

        //============================================================================================================//

        private void SetupNewObjective()
        {
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
                    FileInteractable.OnDroppedFile += TryCompleteObjective;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Debug.Log($"Objective is {_currentObjective.ToString()} {_objectiveTarget.name}");
            OnNewObjective?.Invoke();
        }

        private void SelectATargetFile()
        {
            throw new NotImplementedException();
        }

        private void TryCompleteObjective(FileInteractable fileInteractable)
        {
            if (CheckDidCompleteObjective(fileInteractable) == false)
                return;
            
            //TODO Complete Objective
            //TODO Give reward/points
            //TODO Assign new Objective / Move to new dungeon
            Debug.Log($"Completed Objective {_currentObjective.ToString()} {_objectiveTarget.name}");
            
            
            FileInteractable.OnPickedUpFile -= TryCompleteObjective;
            FileInteractable.OnDroppedFile -= TryCompleteObjective;
            FileInteractable.OnRecycledFile -= TryCompleteObjective;
            
            SetupNewObjective();
        }

        private bool CheckDidCompleteObjective(FileInteractable fileInteractable)
        {
            switch (_currentObjective)
            {
                case OBJECTIVE_TYPE.NONE:
                    return false;
                case OBJECTIVE_TYPE.FIND:
                case OBJECTIVE_TYPE.DESTROY:
                    return fileInteractable == _objectiveTarget;
                case OBJECTIVE_TYPE.MOVE:
                    return fileInteractable == _objectiveTarget && _roomManager.CurrentRoom == _roomTarget;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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