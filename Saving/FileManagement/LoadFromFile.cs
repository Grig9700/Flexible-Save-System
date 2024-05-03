using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LoadFromFile
{
    public static GameData Load(string fileName, string profile, string fileDirectory, bool useEncryption)
    {
        string fullPath = Path.Combine(fileDirectory, profile, fileName);
        
        GameData gameData = null;

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"No save by this name was found: {fullPath}");
            return gameData;
        }

        try
        {
            string dataToLoad = "";
            
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
            
            if (useEncryption)
            {
                //Implement your choice of matching Decryption Here
            }

            gameData = JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured when trying to load from file: {fullPath} \n {e}");
            throw;
        }
        
        return gameData;
    }
}
