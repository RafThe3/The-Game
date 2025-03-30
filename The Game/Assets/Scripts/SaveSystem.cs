using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private bool test = false;
    //[SerializeField] private TextMeshProUGUI progressText;
    //[SerializeField] private float textDuration = 1;

    private static readonly string keyWord = "password";
    private string file;
    private GameObject player;

    private void Awake()
    {
        file = $"{Application.persistentDataPath}/{name}.json";

        player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        //progressText.enabled = false;
    }

    private void Update()
    {
        if (!test)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Delete();
        }
    }

    public void Save()
    {
        //Save player values
        SaveData myData = new()
        {
            x = player.transform.position.x,
            y = player.transform.position.y,
            z = player.transform.position.z,
        };
        //

        //Important - DO NOT DELETE
        string myDataString = JsonUtility.ToJson(myData);
        myDataString = EncryptDecryptData(myDataString);
        File.WriteAllText(file, myDataString);
        //

        /*
        progressText.text = "Progress saved!";
        StartCoroutine(ShowProgressText(textDuration));
        */
    }

    public void Load()
    {
        //Important - DO NOT DELETE
        if (File.Exists(file))
        {
            string jsonData = File.ReadAllText(file);
            jsonData = EncryptDecryptData(jsonData);
            SaveData myData = JsonUtility.FromJson<SaveData>(jsonData);
            //

            //Load player values
            player.transform.position = new Vector3(myData.x, myData.y, myData.z);

            /*
            progressText.text = "Progress loaded!";
            StartCoroutine(ShowProgressText(textDuration));
            */
        }
    }

    public string EncryptDecryptData(string data)
    {
        string result = string.Empty;

        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ (keyWord[i % keyWord.Length]));
        }

        return result;
    }

    public void Delete()
    {
        File.Delete(file);
    }

    private IEnumerator ShowProgressText(float duration)
    {
        //progressText.enabled = true;

        yield return new WaitForSeconds(duration);

        //progressText.enabled = false;
    }
}

[Serializable]
public class SaveData
{
    public float x, y, z;

    /*
    public int currentXP, currentLevelUpXP, currentLevel;
    public int currentHealth, maxHealth;
    public int areasConquered;
    */

    public int scene;
}
