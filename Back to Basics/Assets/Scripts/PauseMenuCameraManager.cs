using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuCameraManager : MonoBehaviour
{
    PlayerController playerControllerScript;

    private void Start()
    {
        playerControllerScript = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            playerControllerScript.enabled = false;

        else
            playerControllerScript.enabled = true;
    }
}
