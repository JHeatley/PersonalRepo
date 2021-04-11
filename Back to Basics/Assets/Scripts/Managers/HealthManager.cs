using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Slider healthSlider;
    public AudioSource audioClip;

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }

    public void TakeDamage(int damage)
    {
        audioClip.Play();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<CharacterManager>().currentHealth -= damage;
        SetHealth(player.GetComponent<CharacterManager>().currentHealth);
    }

    public void HealDamage(int healAmount)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<CharacterManager>().currentHealth += healAmount;
        SetHealth(player.GetComponent<CharacterManager>().currentHealth);
    }
}
