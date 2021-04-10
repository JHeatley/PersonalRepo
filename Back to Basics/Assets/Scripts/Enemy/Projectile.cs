using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float timeAlive;
    public int projectileDamage;

    private Transform player;
    private Vector3 target;

    public HealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthManager = player.GetComponent<HealthManager>();
        target = new Vector3(player.position.x, player.position.y, player.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position.x == target.x && transform.position.y == target.y && transform.position.z == target.z)
            DestroyProjectile();

        timeAlive -= Time.deltaTime;

        if (timeAlive <= 0)
            DestroyProjectile();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (player)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject playerBody = GameObject.FindGameObjectWithTag("Player");
        if (playerBody)
        {
            Debug.Log("This should be taking away Player Health");
            healthManager.TakeDamage(projectileDamage);
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
