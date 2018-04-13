using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class SongPlayer : MonoBehaviour {

    public string songPath;
    SongPair songPair;
    public SongManager manager;
    double secsBetweenNotes;
    public GameObject noteObj;
    List<Transform> spawnLocations = new List<Transform>();
    public GameObject spawnLocationObj;
    public float approachRate = 5f;
    List<GameObject> drums = new List<GameObject>();
    public GameObject drumObj;
    public int points = 0;

    int noteStreak = 0;
    int maxStreak = 0;
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
        secsBetweenNotes = 60f / (float)(songPair.song.bpm * songPair.song.subdivision);
        SetUpDrums();
        Invoke("PlayMusic", songPair.song.offset);
        InvokeRepeating("BuildRow", 0f, (float)secsBetweenNotes);
	}

    void PlayMusic()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(songPair.clip);
    }

    public void Score(int amount)
    {
        points += amount;
        if (amount == 0) {
            noteStreak = 0;
        } else {
            noteStreak++;
            if (noteStreak > maxStreak) {
                maxStreak = noteStreak;
            }
        }
    }

    public int GetScore()
    {
        return points;
    }

    public Song GetSongPlaying()
    {
        return songPair.song;
    }
    
    public int GetStreak()
    {
        return noteStreak;
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

    void SongDone() {
        SceneManager.LoadScene(0);
    }

    int currentRow = 0;
    void BuildRow () {
        if (currentRow < songPair.song.difficulties[selectDifficulty].noteRow.Count) {
            BuildRow(songPair.song.difficulties[selectDifficulty].noteRow[currentRow], currentRow);
            currentRow++;
        } else {
            SongDone();
        }
    }
}
