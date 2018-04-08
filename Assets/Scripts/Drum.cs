using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Drum : MonoBehaviour {
    List<GameObject> notes = new List<GameObject>();
    public GameObject particles;
    public SongPlayer player;

    //  90% to 100% Accurate
    public GameObject fantastic;

    // 61% to 90% Accurate
    public GameObject excellent;

    // 31% to 60% Accurate
    public GameObject good;

    // 1% to 30% Accurate
    public GameObject poor;

    // Never Hit
    public GameObject miss;

    public GameObject multiplierBase;

    public GameObject hmd;

    public VRTK_ControllerEvents leftController;
    public VRTK_ControllerEvents rightController;

    public bool adjust;

    public void AddNote(GameObject note)
    {
        notes.Add(note);
    }

    public void Missed(GameObject note)
    {
        Instantiate(miss, transform.position, Quaternion.identity);
        notes.RemoveAt(0);
    }

    IEnumerator LateStart(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        transform.position = new Vector3(transform.position.x, hmd.transform.position.y / 2, hmd.transform.position.z + 0.5f);
    }

    public void Start()
    {
        if (adjust)
        {
            StartCoroutine(LateStart(1));
        }
    }

    void OnTriggerEnter(Collider collided)
    {
        Debug.Log(collided.name);
        if (collided.tag == "Mallet" && notes.Count > 0)
        {
            float amountOff = Vector3.Distance(notes[0].transform.position, transform.position);
            float percentAccurate = (0.6f - amountOff) / 0.6f;
            if (percentAccurate > 0)
            {
                Instantiate(particles, transform.position, particles.transform.rotation);
                Debug.Log(percentAccurate);
                if (percentAccurate > 0 && percentAccurate <= 0.3)
                {
                    Instantiate(poor, transform.position, Quaternion.identity);
                }
                else if (percentAccurate > 0.3 && percentAccurate <= 0.6)
                {
                    Instantiate(good, transform.position, Quaternion.identity);
                }
                else if (percentAccurate > 0.6 && percentAccurate <= 0.9)
                {
                    Instantiate(excellent, transform.position, Quaternion.identity);
                }
                else if (percentAccurate > 0.9)
                {
                    Instantiate(fantastic, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(miss, transform.position, Quaternion.identity);
                }
                int multiplier = 1;
                if (notes[0].GetComponent<Note>().type == 2)
                {

                    multiplier = Mathf.RoundToInt(Mathf.Max(leftController.GetVelocity().magnitude, rightController.GetVelocity().magnitude));
                    GameObject multiplierText = Instantiate(multiplierBase, transform.position, Quaternion.identity);
                    multiplierText.GetComponent<TextMesh>().text = multiplier + "x";
                    Debug.Log(multiplier);
                }
                player.Score(Mathf.FloorToInt(Mathf.Max(0, 50f - amountOff)) * multiplier);
                Destroy(notes[0]);
                notes.RemoveAt(0);
            }
        }
    }
}
