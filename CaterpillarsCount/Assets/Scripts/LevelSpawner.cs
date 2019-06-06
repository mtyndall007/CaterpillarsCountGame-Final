using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    //Calculate number of levels
    static string absolutePath = Application.dataPath;

    static DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath + "/Scenes");
    private static int levelCount = 6; //dirInfo.GetFiles().Length;


    public static string[] SpawnScenes()
    {
        Debug.Log("DirInfo " + dirInfo);
        Debug.Log("Level count " + levelCount);

        //Array for scene names. One array entry for each level
        string[] returnArray = new string[levelCount];
        List<string> tmp = new List<string>();

        for (int j = 1; j <= levelCount; j++)
        {
            //Find scenes in the corresponding level folder
            DirectoryInfo dir = new DirectoryInfo(dirInfo + "/Demo" + j.ToString());
            FileInfo[] info = dir.GetFiles("*.unity");

            //Select a random scene from each level
            int ran = Random.Range(0, info.Length);
            string sceneName = Path.GetFileNameWithoutExtension(info[ran].ToString());
            tmp.Add(sceneName);
        }
        return tmp.ToArray();

    }
}
