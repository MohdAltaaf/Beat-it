using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class SongSelectUI : MonoBehaviour
{
    public GameObject songButtonPrefab;
    public Transform contentContainer;
    public GameObject mainMenuPanel;
    public GameObject songSelectPanel;
    public string gameplaySceneName = "Gameplay"; // must match the exact scene name, see below

    private string songsFolder;

    void Awake()
    {
        songsFolder = Path.Combine(Application.persistentDataPath, "Songs");
        if (!Directory.Exists(songsFolder))
            Directory.CreateDirectory(songsFolder);
    }

    public void OpenSongSelect()
    {
        mainMenuPanel.SetActive(false);
        songSelectPanel.SetActive(true);
        PopulateSongList();
    }

    void PopulateSongList()
    {
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);

        string[] files = Directory.GetFiles(songsFolder, "*.mp3");
        if (files.Length == 0)
        {
            Debug.Log($"No songs found in {songsFolder}");
            return;
        }

        foreach (string filePath in files)
        {
            GameObject btnObj = Instantiate(songButtonPrefab, contentContainer);
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = fileName;

            string capturedPath = filePath;
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectSong(capturedPath));
        }
    }

    void SelectSong(string path)
    {
        GameSession.SelectedSongPath = path;
        SceneManager.LoadScene(gameplaySceneName); // hands off to the gameplay scene entirely
    }

    public void OpenSongsFolder() => Application.OpenURL("file://" + songsFolder);
    public void Quit() => Application.Quit();
}

public static class GameSession
{
    public static string SelectedSongPath;
}