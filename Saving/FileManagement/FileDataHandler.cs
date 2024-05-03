using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FileDataHandler
{
    private readonly string _dataDirectoryPath;
    private readonly bool _useEncryption;

    public FileDataHandler(bool useEncryption = false)
    {
        _dataDirectoryPath = Application.persistentDataPath;
        _useEncryption = useEncryption;
    }
    
    public GameData Load(string fileName, string profile) => LoadFromFile.Load(fileName, profile, _dataDirectoryPath, _useEncryption);
    
    public GameData Continue()
    {
        var mainDirInfo = new DirectoryInfo(_dataDirectoryPath).EnumerateDirectories();

        FileInfo fileInfo = null;
        var profile = "";
        foreach (var directory in mainDirInfo)
        {
            var info = GetLastModified(Path.Combine(_dataDirectoryPath, directory.Name));
            
            if (info == null)
                continue;

            var profileData = LoadTest(info.Name, directory.Name);
            
            if (profileData == null)
                continue;
            
            if (fileInfo == null)
            {
                profile = directory.Name;
                fileInfo = info;
                continue;
            }

            if (DateTime.Compare(fileInfo.LastWriteTime, info.LastWriteTime) >= 0) 
                continue;
            
            profile = directory.Name;
            fileInfo = info;
        }

        if (profile == "" || fileInfo == null)
            return null;
        
        return Load(fileInfo.Name, profile);
    }
    
    public void Save(GameData gameData, string fileName, string profile) => SaveToFile.Save(gameData, fileName, profile, _dataDirectoryPath, _useEncryption);

    public void Delete(string fileName, string profile) => DeleteSaveFile.Delete(fileName, profile, _dataDirectoryPath);

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        var profileDictionary = new Dictionary<string, GameData>();

        var mainDirInfo = new DirectoryInfo(_dataDirectoryPath).EnumerateDirectories();

        foreach (var directory in mainDirInfo)
        {
            var profile = directory.Name;
            
            var directoryPath = Path.Combine(_dataDirectoryPath, profile);

            var files = Directory.GetFiles(directoryPath);

            if (!files.Any())
            {
                Debug.LogWarning($"Skipped the following directory as it doesn't contain data: {profile}");
                continue;
            }

            var lastModified = GetLastModified(directoryPath);
            
            var profileData = LoadTest(lastModified.Name, profile);
            
            if (profileData == null)
                continue;
            
            profileDictionary.Add(profile, profileData);
        }

        return profileDictionary;
    }
    
    public Dictionary<string, GameData> LoadAllSavesInProfile(string profile)
    {
        var savesDictionary = new Dictionary<string, GameData>();

        var directoryPath = Path.Combine(_dataDirectoryPath, profile);
        
        var thisDirectoryInfo = new DirectoryInfo(directoryPath);
        
        foreach (var file in thisDirectoryInfo.GetFiles())
        {
            var profileData = LoadTest(file.Name, profile);
            
            if (profileData == null)
                continue;
            
            savesDictionary.Add(file.Name, profileData);
        }

        return savesDictionary;
    }

    private static FileInfo GetLastModified(string path)
    {
        var thisDirectoryInfo = new DirectoryInfo(path);

        var lastModified =
            (from file in thisDirectoryInfo.GetFiles()
                orderby file.LastWriteTime descending
                select file).First();

        return lastModified;
    }

    private GameData LoadTest(string fileName, string profile)
    {
        var profileData = Load(fileName, profile);

        if (profileData != null) 
            return profileData;
        
        Debug.LogWarning($"Skipped the following file as data could not be loaded: {fileName}");
        return null;
    }
}
