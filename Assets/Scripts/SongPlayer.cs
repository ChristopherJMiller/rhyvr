using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public class SongPlayer : MonoBehaviour {

    public string songPath;
    SongPair songPair;
    public SongManager manager;
    float secsBetweenNotes;
    public GameObject noteObj;
    List<Transform> spawnLocations = new List<Transform>();
    public GameObject spawnLocationObj;
    public float approachRate = 5f;
    List<GameObject> drums = new List<GameObject>();
    public GameObject drumObj;
    public int points = 0;
    public int selectDifficulty = 0;

    public GameObject cameraHead;
    public VRTK_ControllerEvents left;
    public VRTK_ControllerEvents right;

    public Material[] noteSubDivisionMaterials;

    public Material[] noteAccentMaterials;

    void SetUpDrums()
    {
        int drumCount = songPair.song.numberOfDrums;
        if (drumCount % 2 == 0)
        {
            float startingX = ((drumCount / 2) * -0.4f) + 0.2f;
            for (int drumNum = 0; drumNum < drumCount; drumNum++)
            {
                GameObject drum = (GameObject)Instantiate(drumObj, new Vector3(startingX + (0.4f * drumNum), 0, -10), Quaternion.Euler(-90, 0 ,0));
                drum.GetComponent<Drum>().player = this;
                drum.GetComponent<Drum>().hmd = cameraHead;
                drum.GetComponent<Drum>().leftController = left;
                drum.GetComponent<Drum>().rightController = right;
                drums.Add(drum);

                GameObject spawner = (GameObject)Instantiate(spawnLocationObj, new Vector3(startingX + (0.4f * drumNum), 0, approachRate), Quaternion.identity);
                spawnLocations.Add(spawner.transform);
            }
        }

    }

    void Start () {
        GameObject messenger = GameObject.Find("Messenger");
        songPair = messenger.GetComponent<Messenger>().songToPlay;
        Destroy(messenger);
        Debug.Log(songPair.song.bpm);
        Debug.Log(songPair.song.subdivision);
        secsBetweenNotes = 60f / (float)(songPair.song.bpm * songPair.song.subdivision);
        SetUpDrums();
        Invoke("PlayMusic", songPair.song.offset);
	}

    void PlayMusic()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(songPair.clip);
    }

    public void Score(int amount)
    {
        points += amount;
    }

    public int GetScore()
    {
        return points;
    }

    public Song GetSongPlaying()
    {
        return songPair.song;
    }

    void BuildRow(NoteRow row, int rowNumber)
    {
        int noteNum = 0;
        foreach(int note in row.row)
        {
            if (note > 0)
            {
                GameObject obj = Instantiate(noteObj, spawnLocations[noteNum].position, Quaternion.identity);
                obj.GetComponent<Note>().target = drums[noteNum];
                obj.GetComponent<Note>().spawn = spawnLocations[noteNum].gameObject;
                obj.GetComponent<Note>().type = note;
                switch(note)
                {
                    case 1:
                        for (int i = 0; i < songPair.song.subdivision; i++)
                        {
                            if ((rowNumber + 1) % (i + 1) == 0)
                            {
                                obj.GetComponent<MeshRenderer>().material = noteSubDivisionMaterials[i];
                            }
                        }
                        break;
                    case 2:
                        for (int i = 0; i < songPair.song.subdivision; i++)
                        {
                            if ((rowNumber + 1) % (i + 1) == 0)
                            {
                                obj.GetComponent<MeshRenderer>().material = noteAccentMaterials[i];
                            }
                        }
                        break;
                }

                drums[noteNum].GetComponent<Drum>().AddNote(obj);
            }
            noteNum++;
        }
    }

    float internalClock;
    int currentRow = 0;
	void FixedUpdate () {
        internalClock += Time.fixedDeltaTime;
        if (internalClock >= secsBetweenNotes && currentRow <= songPair.song.difficulties[selectDifficulty].noteRow.Count - 1)
        {
            BuildRow(songPair.song.difficulties[selectDifficulty].noteRow[currentRow], currentRow);
            internalClock = 0;
            currentRow++;
        }
	}
}
