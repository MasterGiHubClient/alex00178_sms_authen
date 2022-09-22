using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Text;

public class UsersManager : MonoBehaviour
{
    private static UsersManager instance;
    public static UsersManager Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<UsersManager>();
            }

            return instance;
        }
    }

    string sha;
    [SerializeField] UsersContainer usersContainer;

    public bool AlreadyExist
    {
        get => usersContainer.IsContains(AuthManager.PhoneNumber);
    }

    private void Start()
    {
        StartCoroutine(DownloadDB(status => 
        {
            Debug.Log($"db download status: {status}");
        }));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(UpdateDB(status =>
            {
                Debug.Log($"db update status: {status}");
            }));
        }
    }

    public void TryAddNewUser(string name, string phoneNumber, List<string> visits)
    {
        usersContainer.AddUser(name, phoneNumber, visits, out bool added);
        if(!added)
        {
            return;
        }

        StartCoroutine(UpdateDB(status =>
        {
            Debug.Log($"db update status: {status}");
        }));
    }

    IEnumerator DownloadDB(Action<bool> success)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://api.github.com/repos/MasterGiHubClient/alex00178_sms_authen/contents/db.json");

        yield return webRequest.SendWebRequest();

        if(webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(webRequest.error);
            success.Invoke(false);
        }
        else
        {
            Responce responce = JsonUtility.FromJson<Responce>(webRequest.downloadHandler.text);
            usersContainer = JsonUtility.FromJson<UsersContainer>(Encoding.UTF8.GetString(Convert.FromBase64String(responce.content)));
            sha = responce.sha;
        }
    }

    IEnumerator UpdateDB(Action<bool> success)
    {          
        UnityWebRequest webRequest = new UnityWebRequest("https://api.github.com/repos/MasterGiHubClient/alex00178_sms_authen/contents/db.json", "PUT");
        webRequest.SetRequestHeader("Authorization", "Bearer " + "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");

        string dbJson = JsonUtility.ToJson(usersContainer);
        byte[] dbBytes = Encoding.UTF8.GetBytes(dbJson);
        string db64String = Convert.ToBase64String(dbBytes);

        string commitMsg = $"update from {AuthManager.PhoneNumber}";
        Payload.Committer committer = new Payload.Committer("botname", "bot@gmail.com");

        Payload payload = new Payload(commitMsg, committer, sha, db64String);
        string payloadString = JsonUtility.ToJson(payload);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(payloadString);

        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
            success.Invoke(false);
        }
        else
        {
            success.Invoke(true);
        }
    }

    [Serializable]
    public class Responce
    {
        public string content;
        public string download_url;
        public string sha;
    }
}
