using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float timeAlive;
    public int projectileDamage;
    public AudioSource audioClip;
    GameObject playerBody;

    private Transform player;
    private Vector3 target;

    public HealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GameObject.FindGameObjectWithTag("Player");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HealthManager>();
        target = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y, playerBody.transform.position.z);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerBody)
        {
            Debug.Log("This should be taking away Player Health");
            healthManager.TakeDamage(projectileDamage);
            DestroyProjectile();
        }

        else if (other.gameObject.isStatic)
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
