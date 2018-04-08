using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyText : MonoBehaviour {

    float timeAlive = 0.5f;
    float currentTime = 0;
    public float multiplier = 1;
	void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * multiplier);
        currentTime += Time.deltaTime;
        if (currentTime >= timeAlive)
        {
            Destroy(gameObject);
        }
    }
}
