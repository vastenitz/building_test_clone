using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseItemScript : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(DownloadImage("https://freepngimg.com/thumb/building/28149-1-building.png"));
    }

    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        Debug.Log("request");
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("request fail " + request.error.ToString());
        }
        else
        {
            Material myNewMaterial = new Material(Shader.Find("Standard"));
            myNewMaterial.SetTexture("_MainTex", ((DownloadHandlerTexture)request.downloadHandler).texture);
            Vector3 defaultImgSize = new Vector3(2f, 2f, 2f);
            
            Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            float heightFactor = ((float)texture.height / (float)texture.width);

            this.gameObject.transform.localPosition = new Vector3(15, 0, 15);
            this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(45,45,0));
            this.gameObject.transform.localScale = new Vector3(defaultImgSize.x, defaultImgSize.x * heightFactor, 1);

            this.gameObject.transform.GetComponent<MeshRenderer>().material = myNewMaterial;
        }
    }


}
