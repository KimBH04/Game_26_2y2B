using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BasicMain : MonoBehaviour
{
    public Button Hello;
    public string host;
    public string port;
    public string route;

    public void Start()
    {
        Hello.onClick.AddListener(() =>
        {
            var url = $"{host}:{port}/{route}";
            Debug.Log(url);

            StartCoroutine(GetBasic(url, (raw) =>
            {
                Debug.Log($"{raw}");
            }));
        });
    }

    private IEnumerator GetBasic(string url, System.Action<string> callback)
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogWarning("네트워크 통신 에러");
        }
        else
        {
            callback?.Invoke(webRequest.downloadHandler.text);
        }
    }
}
