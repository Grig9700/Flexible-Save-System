using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentDataManager : MonoBehaviour
{
    [Header("Debug")] 
    [SerializeField] private bool newDataIfNull;
    
    public static PersistentDataManager Instance { get; private set; }

    private string _selectedProfile = "base";

    private FileDataHandler _dataHandler;
    private GameData _gameData;
    private List<IPersistentData> _persistentDataObjects;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _dataHandler = new FileDataHandler();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    private static List<IPersistentData> FindAllPersistentDataObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IPersistentData>().ToList(); //Has to be MonoBehavior to exist as an object in the scene
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _persistentDataObjects = FindAllPersistentDataObjects();
        
        if (_gameData == null && newDataIfNull)
            NewGame();
        
        if (_gameData == null)
            return;
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.LoadData(_gameData);
        }
    }
    
    private void OnSceneUnLoaded(Scene scene)
    {
        if (newDataIfNull && _gameData == null)
            NewGame();
        
        if (_gameData == null)
        {
            Debug.Log("No game data to save to");
            return;
        }
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.SaveData(ref _gameData);
        }
    }
    
    public void NewGame()
    {
        _gameData = new GameData();
        SaveGame("Auto Save");
    }

    public void LoadGame(string fileName)
    {
        if (newDataIfNull && _gameData == null)
            NewGame();
        
        _gameData = _dataHandler.Load(fileName, _selectedProfile);

        if (_gameData == null)
        {
            Debug.LogWarning($"No save data was found. Could not load game");
            return;
        }

        //SceneManager.LoadSceneAsync(_gameData.sceneName);
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.LoadData(_gameData);
        }
    }

    public void SaveGame(string fileName)
    {
        if (newDataIfNull && _gameData == null)
            NewGame();

        if (_gameData == null)
        {
            Debug.LogWarning($"No data container was found. Could not save game");
            return;
        }
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.SaveData(ref _gameData);
        }

        _dataHandler.Save(_gameData, fileName, _selectedProfile);
    }

    public void DeleteGame(string fileName)
    {
        _dataHandler.Delete(fileName, _selectedProfile);
    }
    
    // private void OnApplicationQuit()
    // {
    //     //Auto-saving on quit?
    // }

    public void GetAllSaveProfiles()
    {
        var profiles = _dataHandler.LoadAllProfiles();
    }

    public void ChangeSelectedProfile(string selectedProfile)
    {
        _selectedProfile = selectedProfile;
    }
}

[CustomEditor(typeof(PersistentDataManager))]
public class PersistentDataManagerEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PersistentDataManager script = (PersistentDataManager)target;
        if (GUILayout.Button("Delete Save")) {
            script.DeleteGame("Auto Save");
        }
    }
}