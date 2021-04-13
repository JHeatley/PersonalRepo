using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int healHealthAmount;
    public int damageAmount;
    public HealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        healthManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HealthManager>();
    }

    private void Update()
    {
        transform.Rotate(0, -45 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && gameObject.tag == "HealthPickup")
        {
            healthManager.HealDamage(healHealthAmount);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Player") && gameObject.tag == "DamagePickup")
        {
            healthManager.TakeDamage(damageAmount);
            Destroy(gameObject);
        }
        
    }
}
