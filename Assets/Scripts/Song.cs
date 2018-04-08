using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Song {
    public string name;
    public string author;
    public int bpm;
    public int subdivision;
    public float offset;
    public int numberOfDrums;
    public List<Difficulty> difficulties;

    public Song(string n, string a, int b, List<Difficulty> diffs)
    {
        name = n;
        author = a;
        bpm = b;
        difficulties = diffs;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
