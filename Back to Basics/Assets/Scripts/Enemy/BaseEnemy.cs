using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Slider EnemyHealthSlider;
    public AudioSource firstClip;
    public AudioSource secondClip;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        EnemyHealthSlider.value = currentHealth;
        firstClip.Play();
        if (currentHealth <= 0)
        {

            currentHealth = 0;
            EnemyHealthSlider.value = 0;
            secondClip.Play();
            //Kill the enemy
            Destroy(this.gameObject);
            //Drop Loot at enemy Position
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {

    }
}
