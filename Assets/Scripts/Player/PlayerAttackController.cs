using GGJ.Inputs;
using UnityEngine;

namespace GGJ.Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private void Start()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
        }

        private void OnAttackPressed(bool isPressed)
        {
            if (isPressed)
            {
                Debug.Log("Inside PlayerAttackController");
                //Start Attack Charge
            }
            else
            {
                //Determine how long we were pressing 
                //Do appropriate attack
                Debug.Log("Inside PlayerAttackController");
            }
        }
    }
}