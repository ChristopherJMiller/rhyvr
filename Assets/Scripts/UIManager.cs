using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text title;
    public Text score;
    public SongPlayer player;

	void Update () {
        title.text = player.GetSongPlaying().name;
        score.text = player.GetScore().ToString();
	}
}
