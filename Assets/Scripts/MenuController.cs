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

    public GameObject leftContainer, difficultyConainter, difficultyItem;
    int menuIndex = 0;

    int difficultyIndex = 0;
    public List<GameObject> menuItems = new List<GameObject>();
    public List<SongPair> menuItemSongs = new List<SongPair>();

    public List<GameObject> difficultyContainers = new List<GameObject>();
    int maxIndex;
    public GameObject messenger;

    AudioSource audioSource;

    public float volumeRampMulti;

	void Play()
    {
        GameObject messengerObj = (GameObject)Instantiate(messenger, Vector3.zero, Quaternion.identity);
        messengerObj.GetComponent<Messenger>().songToPlay = menuItemSongs[menuIndex];
        messengerObj.GetComponent<Messenger>().difficultySelected = difficultyIndex;
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

    void UpdateDifficultySelection() {
        int i = 0;
        foreach (Transform item in difficultyContainers[menuIndex].transform)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(menuItemSongs[menuIndex].song.difficulties[i].color, out color)) {
                item.GetComponent<Image>().color = color;
            } else {
                item.GetComponent<Image>().color = Color.white;
            }
            i++;
        }
        difficultyContainers[menuIndex].transform.GetChild(difficultyIndex).GetComponent<Image>().color = Color.gray;        
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


        foreach (GameObject difficulty in difficultyContainers)
        {
            difficulty.SetActive(false);
        }
        difficultyContainers[menuIndex].SetActive(true);

        difficultyIndex = 0;
        UpdateDifficultySelection();
        
        audioSource.clip = menuItemSongs[menuIndex].clip;
        float startingPos = (Random.value * 0.6f) * audioSource.clip.length;
        audioSource.time = startingPos;
        audioSource.volume = 0;
        audioSource.Play();
        StopCoroutine(RampVolume());
        StartCoroutine(RampVolume());
    }

    void BuildDifficultyContainer(Song song) {
        GameObject difficultyContainerPanel = (GameObject)Instantiate(difficultyConainter, leftContainer.transform.position, leftContainer.transform.rotation, leftContainer.transform);

        foreach (Difficulty diff in song.difficulties) {
            GameObject listItem = (GameObject)Instantiate(difficultyItem, Vector3.zero, leftContainer.transform.rotation);
            listItem.transform.parent = difficultyContainerPanel.transform;
            listItem.transform.localPosition = Vector3.zero;
            listItem.transform.localScale = Vector3.one;

            listItem.transform.GetChild(0).GetComponent<Text>().text = diff.name;
            listItem.transform.GetChild(1).GetComponent<Text>().text = diff.level.ToString();
            Color color;
            if (ColorUtility.TryParseHtmlString(diff.color, out color)) {
                listItem.GetComponent<Image>().color = color;
            }
        } 

        difficultyContainerPanel.SetActive(false);
        difficultyContainers.Add(difficultyContainerPanel);
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
                    BuildDifficultyContainer(song.song);
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
