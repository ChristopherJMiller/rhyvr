using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

    public GameObject spawn;
    public GameObject target;
    public int type;

    private float approachRate = 2;
    private float currentTime = 0;

	void FixedUpdate () {
        transform.position = new Vector3(Mathf.LerpUnclamped(spawn.transform.position.x, target.transform.position.x, currentTime), Mathf.LerpUnclamped(spawn.transform.position.y, target.transform.position.y, currentTime), Mathf.LerpUnclamped(spawn.transform.position.z, target.transform.position.z, currentTime));
        currentTime += Time.fixedDeltaTime / approachRate;
        if (currentTime > 1.1f)
        {
            target.GetComponent<Drum>().Missed(gameObject);
            Destroy(gameObject);
        }
	}
}
