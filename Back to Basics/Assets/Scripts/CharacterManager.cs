using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public HealthManager healthManager;
    public Transform respawnPosition;

    void Start()
    {
        transform.position = respawnPosition.position;
        currentHealth = maxHealth;
        healthManager.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (healthManager.healthSlider.value <= 0)
        {
            currentHealth = 0;
            transform.position = respawnPosition.position;
            healthManager.SetHealth(maxHealth);
            currentHealth = maxHealth;
        }
    }
}
