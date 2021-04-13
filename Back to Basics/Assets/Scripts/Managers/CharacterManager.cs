using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public HealthManager healthManager;
    public AudioSource audioToPlay;

    public CheckpointSystem checkPointSystem;

    void Start()
    {
        healthManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HealthManager>();
        currentHealth = maxHealth;
        healthManager.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

        if (healthManager.healthSlider.value <= 0)
        {
            currentHealth = 0;
            audioToPlay.Play();
            GetComponent<CharacterController>().enabled = false;
            transform.position = checkPointSystem.lastCheckPointPos.transform.position;
            transform.rotation = checkPointSystem.lastCheckPointPos.transform.rotation;
            GetComponent<CharacterController>().enabled = true;
            healthManager.SetHealth(maxHealth);
            currentHealth = maxHealth;
        }
    }
}