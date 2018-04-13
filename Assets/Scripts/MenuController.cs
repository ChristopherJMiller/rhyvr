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

	void Play()
    {
        Debug.Log("Play");
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

    void UpdateMenuSelection()
    {
        foreach (GameObject item in menuItems)
        {
            item.GetComponent<Image>().color = Color.white;
        }
        menuItems[menuIndex].GetComponent<Image>().color = Color.gray;
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
        LoadMenu();
    }
}
