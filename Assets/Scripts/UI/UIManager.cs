using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject SceneEnteringWindow;

    public GameObject WindowContainer;

    public RawImage image;

    private void Awake()
    {
        instance = this;
    }


    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {

        } else
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public void ShowWindow(GameObject prefab)
    {
        WindowScript window = Utilities.CreateInstance(prefab, this.WindowContainer, true).GetComponent<WindowScript>();
        window.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void ShowSceneEnteringWindow()
    {
        StartCoroutine(DownloadImage("https://freepngimg.com/thumb/building/28149-1-building.png"));
    }
}
