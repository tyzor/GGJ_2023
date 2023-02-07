using System;
using UnityEngine;

namespace GGJ.Interactables
{
    [RequireComponent(typeof(MeshRenderer))]
    public class RecyclingBin : MonoBehaviour
    {
        
        
        private Bounds _bounds;
        
        //Unity Functions
        //============================================================================================================//
        
        private void OnEnable()
        {
            FileInteractable.OnDroppedFile += OnDroppedFile;
        }

        private void Start()
        {
            var collider = GetComponent<Collider>();
            _bounds = collider.bounds;
        }

        private void OnDisable()
        {
            FileInteractable.OnDroppedFile -= OnDroppedFile;
        }

        //============================================================================================================//
        private void OnDroppedFile(FileInteractable fileInteractable)
        {
            var fileBounds = fileInteractable.ColliderBounds;
            var fileBoundsCenter = fileBounds.center;
            fileBoundsCenter.y = _bounds.center.y;
            fileBounds.center = fileBoundsCenter;
            
            if (fileBounds.Intersects(_bounds) == false)
                return;
            
            Destroy(fileInteractable.gameObject);
        }
        //============================================================================================================//
    }
}