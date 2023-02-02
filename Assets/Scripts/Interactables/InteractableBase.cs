using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class InteractableBase : MonoBehaviour
    {
        private Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            Assert.IsTrue(_collider.isTrigger);
        }

        private void OnTriggerEnter(Collider other)
        {
            //TODO Look for the player
        }
    }
}