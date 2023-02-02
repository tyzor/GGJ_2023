using System.Collections;
using System.Collections.Generic;
using GGJ.Inputs;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputDelegator.OnAttackPressed += OnAttackPressed;
    }

    private void OnAttackPressed(bool isPressed)
    {
        if (isPressed)
        {
            //Start Attack Charge
        }
        else
        {
            //Determine how long we were pressing 
            //Do appropriate attack
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
