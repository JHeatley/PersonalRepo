using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffPauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    private void Start()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        TurnOffPause();
    }

    public void TurnOffPause()
    {
        pauseMenu.SetActive(false);
    }
}
