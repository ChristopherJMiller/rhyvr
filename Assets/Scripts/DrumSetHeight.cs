using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSetHeight : MonoBehaviour {

	public GameObject hmd;

    IEnumerator LateStart(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        transform.position = new Vector3(transform.position.x, hmd.transform.position.y / 2, hmd.transform.position.z + 0.5f);
    }

    public void Start()
    {
		StartCoroutine(LateStart(1));
    }
}
