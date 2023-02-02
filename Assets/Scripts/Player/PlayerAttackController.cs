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
            float intensity = 0;
            if (isPressed)
            {
                intensity +=0.1f;
                if (intensity >10)
                {
                    intensity = 10;
                }
            }
            else
            {
                //Determine how long we were pressing 
                //Do appropriate attack
            }

            Debug.Log(intensity);
        }
    }
}