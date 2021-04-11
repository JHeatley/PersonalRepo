using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public HealthManager healthManager;

    public CheckpointSystem checkPointSystem;

    void Start()
    {
        healthManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HealthManager>();
        currentHealth = maxHealth;
        healthManager.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (healthManager.healthSlider.value <= 0)
        {
            currentHealth = 0;
            transform.position = checkPointSystem.lastCheckPointPos.transform.position;
            healthManager.SetHealth(maxHealth);
            currentHealth = maxHealth;
        }
    }
}
