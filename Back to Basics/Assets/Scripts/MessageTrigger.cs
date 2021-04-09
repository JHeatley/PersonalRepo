using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    public GameObject messageBox;

    // Start is called before the first frame update
    void Start()
    {
        // Get References
        messageBox.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Show Message
            messageBox.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Turn off Message
        messageBox.SetActive(false);
        Destroy(messageBox);
    }
}
