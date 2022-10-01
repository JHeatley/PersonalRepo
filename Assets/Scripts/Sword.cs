using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Animator animator;
    bool disabledWeapon = false;

    public int swordDamageAmount;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(disabledWeapon == false)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SheaveWeapon();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                TakeOutWeapon();
            }
        }
    }

    public void PerformAttack()
    {
        animator.SetTrigger("Base_Attack");
    }

    public void NoAttack()
    {
        animator.ResetTrigger("Base_Attack");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Shooter")
        {
            collision.GetComponent<ShooterEnemy>().TakeDamage(swordDamageAmount);
        }
    }

    void SheaveWeapon()
    {
        Debug.Log("Putting away the sword.");
        NoAttack();
        GetComponent<MeshRenderer>().enabled = false;
        animator.enabled = false;
        disabledWeapon = true;
    }

    void TakeOutWeapon()
    {
        animator.enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        disabledWeapon = false;
    }
}
