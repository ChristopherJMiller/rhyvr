using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text title, score, streakText;
    public SongPlayer player;

	void Update () {
        title.text = player.GetSongPlaying().name;
        score.text = player.GetScore().ToString();
        int streak = player.GetStreak();
        if (streak > 5) {
            streakText.text = streak.ToString() + "x";
        } else {
            streakText.text = "";
        }
	}
}
