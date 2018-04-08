using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SongEditorManager : MonoBehaviour {

	public GameObject rowPanel, rowTemplate, checkboxTemplate;
	SongPair songPair;

	public InputField title, author, bpm, subdivision, offset, numberOfDrums;

	int selectDifficulty;
	
	List<GameObject> activeNoteRows = new List<GameObject>();


	void PlayMusic()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(songPair.clip);
    }

	float internalClock;
    int currentRow = 0;
	float secsBetweenNotes;
	bool play = false;
	void FixedUpdate () {
		if (play) {
			internalClock += Time.fixedDeltaTime;
			if (internalClock >= secsBetweenNotes && currentRow <= songPair.song.difficulties[selectDifficulty].noteRow.Count - 1)
			{
				activeNoteRows[currentRow].GetComponent<Image>().color = Color.red;
				if ((currentRow * 30) > (GetComponent<RectTransform>().rect.height / 2f)) {
					//rowPanel.GetComponent<RectTransform>().rect = new Rect(rowPanel.GetComponent<RectTransform>().rect.x, 30 * currentRow, rowPanel.GetComponent<RectTransform>().rect.width, rowPanel.GetComponent<RectTransform>().rect.height);
				}
				if (currentRow != 0) {
					activeNoteRows[currentRow - 1].GetComponent<Image>().color = Color.white;
				}
				internalClock = 0;
				currentRow++;
			}
		}
	}

	public void PlayCurrentSong() {
		secsBetweenNotes = (60f / (float)(songPair.song.bpm * songPair.song.subdivision)) - (songPair.song.offset / (float)activeNoteRows.Count);
		currentRow = 1;
		internalClock = 0;
		play = true;
		PlayMusic();
	}

	IEnumerator LoadDifficulty(int index) {
		selectDifficulty = index;
		int num = 1;
		int waitForFrameCounter = 0;
		foreach(NoteRow noteRow in songPair.song.difficulties[index].noteRow) {
			GameObject row = Instantiate(rowTemplate, rowTemplate.transform.position, Quaternion.identity);
			row.transform.SetParent(rowPanel.transform);
			row.transform.localScale = Vector3.one;
			row.GetComponentInChildren<Text>().text = num.ToString();
			for(int i = 0; i < songPair.song.numberOfDrums; i++) {
				GameObject note = Instantiate(checkboxTemplate, checkboxTemplate.transform.position, Quaternion.identity);
				note.transform.SetParent(row.transform.GetChild(1));
				note.transform.localScale = Vector3.one;
				note.GetComponentInChildren<Toggle>().isOn = noteRow.row[i] > 0;
			}
			activeNoteRows.Add(row);
			num++;
			waitForFrameCounter++;
			if (waitForFrameCounter >= 10) {
				yield return new WaitForEndOfFrame();
				waitForFrameCounter = 0;
			}
		}
		Vector2 size = rowPanel.GetComponent<RectTransform>().sizeDelta;
		size.y = 30 * songPair.song.difficulties[index].noteRow.Count;
		rowPanel.GetComponent<RectTransform>().sizeDelta = size;
	}

	void InitializeUI() {

		title.text = songPair.song.name;
		author.text = songPair.song.author;
		bpm.text = songPair.song.bpm.ToString();
		subdivision.text = songPair.song.subdivision.ToString();
		offset.text = songPair.song.offset.ToString();
		numberOfDrums.text = songPair.song.numberOfDrums.ToString();

		StartCoroutine(LoadDifficulty(0));
	}

	// Use this for initialization
	void Start () {
		songPair = SongManager.Load(Application.dataPath + "/Songs/Distant.rhyvr");
		Debug.Log(songPair.song.name);
		InitializeUI();
		//Load Side Info
		//Set up difficulties
		//Load difficulty with chosen to
	}
}
