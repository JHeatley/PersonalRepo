using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //Kill the enemy
            Destroy(this.gameObject);
            //Drop Loot at enemy Position
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerWeapon")
        {
            //currentHealth -= Player weapon damage amount;
        }
    }
}
