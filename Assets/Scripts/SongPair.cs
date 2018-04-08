using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongPair {

    public Song song;
    public AudioClip clip;

    public SongPair(Song s, AudioClip c) {
        song = s;
        clip = c;
    }
}
