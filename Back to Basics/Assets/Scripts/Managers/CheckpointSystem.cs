using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSystem : MonoBehaviour
{
    public GameObject player;
    public GameObject lastCheckPointPos;
    public Scene scene;

    void Start()
    {
        scene = SceneManager.GetActiveScene();
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponentInChildren<CharacterController>().enabled = false;
        player.transform.position = lastCheckPointPos.transform.position;
        player.GetComponentInChildren<CharacterController>().enabled = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (player)
        {
            lastCheckPointPos.transform.position = transform.position;
        }
    }
}
