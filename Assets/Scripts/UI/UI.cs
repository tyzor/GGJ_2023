using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GGJ.Player;
using TMPro;
public class UI : MonoBehaviour
{

    private PlayerHealth playerHealth;
    public Image UIFill;
    public TMP_Text text; 
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
        text.SetText((100*(1f - playerHealth.currentHealthValue)).ToString() + '%');    }
}
