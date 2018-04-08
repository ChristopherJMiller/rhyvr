using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicDrum : MonoBehaviour
{
    public GameObject recipient;
    public string message;

    void OnTriggerEnter(Collider collided)
    {
        if (collided.tag == "Mallet")
        {
            recipient.SendMessage(message);
        }
    }
}
