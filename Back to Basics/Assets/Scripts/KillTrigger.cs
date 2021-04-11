using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private GameObject player;
    public HealthManager healthManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        healthManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HealthManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            healthManager.SetHealth(0);
        }
    }
}
