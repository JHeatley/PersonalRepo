using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private GameObject player;
    public HealthManager healthManager;
    public CharacterManager characterManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            healthManager.SetHealth(0);
            
        }
    }
}
