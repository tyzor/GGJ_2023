using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GGJ.Player;
public class UI : MonoBehaviour
{

    private PlayerHealth playerHealth;
    public Image UIFill;
    private float health;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            return;
        }
        UIFill.fillAmount = 1f-playerHealth.currentHealthValue;
    }
}
