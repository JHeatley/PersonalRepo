using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] int health;

    [SerializeField] TMP_Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        healthText.text = health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            health = 0;

        if (health == 0)
            health = 100;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            health -= collision.gameObject.GetComponent<Projectile>().projectileDamage;
            healthText.text = health.ToString();
        }
    }
}
