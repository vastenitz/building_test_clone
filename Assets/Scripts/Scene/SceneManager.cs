using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;

    private void Awake()
    {
        instance = this;

        this.Init();
    }

    private void Init()
    {
        this.EnterNormalMode();
    }

    public void EnterNormalMode()
    {
        //StartCoroutine(DownloadImage("https://freepngimg.com/thumb/building/28149-1-building.png"));
    }

    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {

        }
        else
        {
            /*Material m = testObject.GetComponent<MeshRenderer>().material;
            m.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;*/
        }
    }
}
