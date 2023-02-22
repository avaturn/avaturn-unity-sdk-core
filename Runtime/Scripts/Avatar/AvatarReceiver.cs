using System;
using UnityEngine;
using UnityEngine.Events;

public class AvatarReceiver : MonoBehaviour
{
    [Serializable] class OnReceived : UnityEvent<string> {}
    
    [SerializeField] private OnReceived received;

    private UniWebView webView;

    public void SetWebView(UniWebView webView)
    {
        this.webView = webView;
        webView.OnMessageReceived += ReceiveLinkAsUniwebViewMessage;
    }
    
    /// <summary>
    /// This method is invoked for Mobile Iframe controller. Only works for httpURL
    /// </summary>
    private void ReceiveLinkAsUniwebViewMessage(UniWebView webview, UniWebViewMessage message)
    {   
        string parameterKey = "avatar_link";
        var split = message.RawMessage.Split(new string[] {$"{parameterKey}="}, System.StringSplitOptions.None);
        if (split.Length != 2)
        {
            Debug.Log($"Argument {parameterKey} is empty");
            return;
        }
        var url = split[1];
        url =  System.Uri.UnescapeDataString(url);

        if (url.StartsWith("error://")) {
            Debug.Log($"Error when receiving data from Avaturn: {url.Substring(8)}");
            return;
        }

        received?.Invoke(url);
    }

    /// <summary>
    /// This method is invoked for WebGL controller. URL can be either dataURL or httpURL
    /// </summary>
    public void ReceiveAvatarLink(string url)
    {   
        received?.Invoke(url);
    }
}
