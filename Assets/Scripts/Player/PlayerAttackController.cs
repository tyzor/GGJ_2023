using GGJ.Inputs;
using UnityEngine;

namespace GGJ.Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        public float Maxintensity = 10;
        
        private float startTime =0;
        private float endTime = 0;
        private void Start()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
        }

        private void OnAttackPressed(bool isPressed)
        {

            if (isPressed)
            {
                startTime= Time.time;
                // intensity +=0.1f;
                // if (intensity >Maxintensity)
                // {
                //     intensity = Maxintensity;
                // }
                //Debug.Log(intensity);
            }
            else
            {
                endTime =  Time.time - startTime;
                Debug.Log(endTime);

                switch (endTime)
                {
                    case float i when i > 0 && i< 0.5f:
                        Debug.Log(0);
                        break;
                    case float i when i > 0.5 && i< 1.0f:
                        Debug.Log(1);
                        break;
                    case float i when i > 1.0 && i< 1.5f:
                        Debug.Log(2);
                        break;
                    case float i when i > 1.5 && i< 2.0f:
                        Debug.Log(3);
                        break;
                }
                //intensity =0;
                //Determine how long we were pressing 
                //Do appropriate attack
            }

            
        }
    }
}