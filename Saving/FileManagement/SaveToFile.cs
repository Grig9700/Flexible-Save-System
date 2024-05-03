using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveToFile
{
    public static void Save(GameData data, string fileName, string profile, string fileDirectory, bool useEncryption)
    {
        string fullPath = Path.Combine(fileDirectory, profile, fileName);

        Debug.Log(fullPath);
        
        try
        {
            EnsureExistingDirectory(fullPath);

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                //Implement your choice of Encryption Here
            }
            
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not save data to file : {fullPath} \n {e}");
            throw;
        }
    }

    private static void EnsureExistingDirectory(string fullPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
    }
}
