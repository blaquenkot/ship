using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class NetworkingController : MonoBehaviour
{
    private string Version;
    private string Session = "";

    void Awake()
    {
        Version = Application.version;
    }
    
    public void NewSession(Action<JSONNode> callback)
    {
        StartCoroutine(PostRequest("https://api.clank.kotzi.dev/sessions", "", (response) => { 
            if(response["session"] != null)
            {
                Session = response["session"];
            }
            callback(response);
        }));
    }

    public void SaveScore(string name, int score, int time, Action<JSONNode> callback)
    {
        string json = $"{{\"session\": \"{Session}\",\"score\": {score},\"time\": {time},\"version\": \"{Version}\",\"name\": \"{name}\"}}";
        StartCoroutine(PostRequest("https://api.clank.kotzi.dev/scores", json, callback));
    }

    public void GetHighscores(Action<JSONNode> callback)
    {
        StartCoroutine(GetRequest($"https://api.clank.kotzi.dev/scores?version={Version}", callback));
    }

    IEnumerator GetRequest(string uri, Action<JSONNode> callback)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            callback(JSON.Parse(uwr.downloadHandler.text));
        }
    }

    IEnumerator PostRequest(string url, string json, Action<JSONNode> callback)
    {
        var uwr = new UnityWebRequest(url, "POST");
        if(json != "")
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        }
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            callback("");
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            callback(JSON.Parse(uwr.downloadHandler.text));
        }
    }
}