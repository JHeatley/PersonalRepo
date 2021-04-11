using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public Animator animator;
    public int attackStrength;
    public Transform attackPoint;
    public float attackRange;
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    public AudioSource swordSlashAudioClip;

    public LayerMask enemyLayers;

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButton(0))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        swordSlashAudioClip.Play();

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider enemy in hitEnemies)
        {
            enemy.GetComponent<RangedEnemy>().TakeDamage(attackStrength);
            Debug.Log(enemy.name + " is taking " + attackStrength + " damage.");
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Enemy"))
    //    {
    //        other.gameObject.GetComponent<RangedEnemy>().TakeDamage(attackStrength);
    //        Debug.Log(other.gameObject.name + " is taking " + attackStrength + " damage.");
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
