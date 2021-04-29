using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[RequireComponent(typeof(WebViewObject))]
public class SmsCertificationWebview : MonoBehaviour
{
    [SerializeField] private string htmlFilePathFromStrimingAssets;
    [SerializeField] public UnityEvent onSuccess;
    [SerializeField] public UnityEvent onCloseOrFailed;

    private WebViewObject web;
    private string url;
    private bool isVisible;

    private void Awake()
    {
        web = gameObject.GetComponent<WebViewObject>();
        
        if (web == null)
        {
            web = gameObject.AddComponent<WebViewObject>();
        }
    }

    private IEnumerator Start()
    {
        web.Init(Callback
#if UNITY_EDITOR
            , separated: true
#endif
        );
        // web.enabled = false;
        isVisible = false;

        string fileName = htmlFilePathFromStrimingAssets;//"index.html";
        string filepath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        string storagePath = Application.persistentDataPath + "/" + fileName;

        // Checks if it's an Android file in the jar. 
        if (filepath.Contains("://"))
        {
            using (var w = UnityWebRequest.Get(filepath))
            {
                yield return w.SendWebRequest();
                
                System.IO.File.WriteAllBytes(storagePath, w.downloadHandler.data);
                url = ("file://" + storagePath);
            }
        }
        else
        {
            // Everything else
            url = filepath;
        }

        Debug.Log("초기화 됨.");
    }

    private void Update()
    {
        if (Input.GetButtonUp("Cancel") && isVisible)
        {
            web.SetVisibility(false);
            onCloseOrFailed.Invoke();
            // web.enabled = false;
            isVisible = false;
            Debug.Log("인증 실패!");
        }
    }

    private void Callback(string message)
    {
        Debug.Log($"CallFromJS[{message}]");
        switch (message)
        {
            case "/onSuccess":
                Debug.Log("인증 성공!");
                
                web.SetVisibility(false);
                web.enabled = false;
                
                onSuccess.Invoke();
                break;
                
            case "/onFailed":
            case "/close":
                Debug.Log("인증 실패!");
                
                web.SetVisibility(false);
                web.enabled = false;
                
                onCloseOrFailed.Invoke();
                break;
        }
    }

    public void Open()
    {
        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        // web.enabled = true;
        isVisible = true;

        web.LoadURL(url);
        web.SetVisibility(true);
    }
}
