using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class MenuController : MonoBehaviour {

    public GameObject songMenuItem;
    public GameObject listParent;
    int menuIndex = 0;
    public List<GameObject> menuItems = new List<GameObject>();
    public List<SongPair> menuItemSongs = new List<SongPair>();
    int maxIndex;
    public GameObject messenger;

    AudioSource audioSource;

    public float volumeRampMulti;

	void Play()
    {
        GameObject messengerObj = (GameObject)Instantiate(messenger, Vector3.zero, Quaternion.identity);
        messengerObj.GetComponent<Messenger>().songToPlay = menuItemSongs[menuIndex];
        messengerObj.name = "Messenger";
        DontDestroyOnLoad(messengerObj);

        SceneManager.LoadScene(1);
    }

    void MenuUp()
    {
        menuIndex--;
        if (menuIndex < 0)
        {
            menuIndex = maxIndex;
        }
        UpdateMenuSelection();
    }

    void MenuDown()
    {
        menuIndex++;
        if (menuIndex > maxIndex)
        {
            menuIndex = 0;
        }
        UpdateMenuSelection();
    }

    IEnumerator RampVolume() {
        float volume = 0;
        float currentTime = 0;
        while(audioSource.volume != 1) {
            audioSource.volume = volume;
            volume = Mathf.Sqrt(currentTime / 4);
            yield return new WaitForEndOfFrame();
            currentTime += (Time.deltaTime / volumeRampMulti);
        }
    }

    void UpdateMenuSelection()
    {
        audioSource.Stop();
        foreach (GameObject item in menuItems)
        {
            item.GetComponent<Image>().color = Color.white;
        }
        menuItems[menuIndex].GetComponent<Image>().color = Color.gray;
        
        audioSource.clip = menuItemSongs[menuIndex].clip;
        float startingPos = (Random.value * 0.6f) * audioSource.clip.length;
        audioSource.time = startingPos;
        audioSource.volume = 0;
        audioSource.Play();
        StopCoroutine(RampVolume());
        StartCoroutine(RampVolume());
    }

    async void LoadMenu()
    {
        foreach (string file in Directory.GetFiles(Application.dataPath + "/Songs"))
        {
            if (file.Contains(".rhyvr"))
            {
                string fileExtension = file.Substring(file.IndexOf(".rhyvr"), file.Length - file.IndexOf(".rhyvr"));
                if (fileExtension == ".rhyvr")
                {
                    SongPair song = await SongManager.Load(file);
                    GameObject listItem = (GameObject)Instantiate(songMenuItem, Vector3.zero, Quaternion.identity);
                    listItem.transform.parent = listParent.transform;
                    listItem.transform.localPosition = Vector3.zero;
                    listItem.transform.localScale = Vector3.one;
                    listItem.GetComponentInChildren<Text>().text = song.song.name;
                    menuItems.Add(listItem);
                    menuItemSongs.Add(song);
                }
            }
        }
        maxIndex = menuItems.Count - 1;
        UpdateMenuSelection();
    }

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        Random.InitState(Mathf.RoundToInt(Time.time * Time.deltaTime));
        LoadMenu();
    }
}
