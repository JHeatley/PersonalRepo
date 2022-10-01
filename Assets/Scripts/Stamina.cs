using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stamina : MonoBehaviour
{
    [SerializeField] int stamina;
    int maxStaminaAmount = 100;
    int staminaReductAmount = 4;

    float lastRegen;
    float staminaRegenSpeed = 0.7f;
    int staminaRegenAmount = 2;

    [SerializeField] TMP_Text staminaText;
    Sword oneHandedSword;

    // Start is called before the first frame update
    void Start()
    {
        oneHandedSword = GetComponentInChildren<Sword>();
        stamina = maxStaminaAmount;
        staminaText.text = stamina.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (PauseMenu.GamePaused == false)
            {
                StartCoroutine(UseStamina());
                oneHandedSword.PerformAttack();
            }
            else if (PauseMenu.GamePaused == true)
            {
                oneHandedSword.NoAttack();
            }
        }

        if (stamina < staminaReductAmount)
            oneHandedSword.NoAttack();

        if (stamina <= 0)
        {
            stamina = 0;
            staminaText.text = stamina.ToString();
        }

        if (stamina == 0)
            oneHandedSword.NoAttack();

        RegenStamina();

        if (stamina >= 100)
            stamina = maxStaminaAmount;
    }

    public IEnumerator UseStamina()
    {
        if (oneHandedSword.GetComponent<MeshRenderer>().enabled == true && stamina > staminaReductAmount)
        {
            GetComponentInChildren<Sword>().enabled = true;
            yield return new WaitForSeconds(0.1f);
            stamina -= staminaReductAmount;
            staminaText.text = stamina.ToString();
            yield return null;
        }
    }

    void RegenStamina()
    {
        if (Time.time - lastRegen > staminaRegenSpeed)
        {
                stamina += staminaRegenAmount;
                lastRegen = Time.time;
                staminaText.text = stamina.ToString();

            if (stamina >= maxStaminaAmount)
            {
                stamina = maxStaminaAmount;
                staminaText.text = stamina.ToString();
            }
        }
    }
}