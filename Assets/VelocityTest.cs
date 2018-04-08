using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VelocityTest : MonoBehaviour {

    public VRTK_ControllerEvents left;
    public VRTK_ControllerEvents right;

	void Update () {
        Debug.Log(left.GetVelocity().magnitude);
        Debug.Log(right.GetVelocity().magnitude);
    }
}
