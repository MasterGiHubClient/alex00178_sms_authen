using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class UsersManager : MonoBehaviour
{
    [SerializeField] UsersContainer usersContainer;

    private void Start()
    {
        StartCoroutine(DownloadDB(success => 
        {
            Debug.Log(success);
        }));
    }

    IEnumerator DownloadDB(Action<bool> success)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://api.github.com/repos/MasterGiHubClient/alex00178_sms_authen/contents/db.json");
        webRequest.SetRequestHeader("Authorization", "Bearer " + "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");
        yield return webRequest.SendWebRequest();

        if(webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            success.Invoke(false);
        }
        else
        {
            Responce responce = JsonUtility.FromJson<Responce>(webRequest.downloadHandler.text);

            webRequest = UnityWebRequest.Get(responce.download_url);
            yield return webRequest.SendWebRequest();
            
            if(webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(webRequest.error);
                success.Invoke(false);
            }
            else
            {
                usersContainer = JsonUtility.FromJson<UsersContainer>(webRequest.downloadHandler.text);
                success.Invoke(true);
            }
        }
    }

    [Serializable]
    public class Responce
    {
        public string download_url;
    }
}
