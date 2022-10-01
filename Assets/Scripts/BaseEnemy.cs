using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    public void TakeDamage(int dmg)
    {
        curHealth -= dmg;
        StartCoroutine(Damaged());
    }

    public IEnumerator Damaged()
    {
        yield return new WaitForSeconds(0.1f);
        yield return GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        yield return GetComponent<Renderer>().material.color = Color.white;
    }

    public IEnumerator Died()
    {
        yield return GetComponent<Renderer>().material.color = Color.black;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
