using System;
using UnityEngine;
using UnityEngine.Android;

public class IframeControllerMobile : MonoBehaviour
{
    private UniWebView webView;
    [SerializeField] private GameObject webViewGameObject;
    [SerializeField] private RectTransform webViewFrame;
    [SerializeField] private AvatarReceiver _avatarReceiver;
    [SerializeField] string subdomain;
    [SerializeField] string linkFromAPI = "";
    
    void Start()
    {
        string domain, link;
        if ( linkFromAPI == "") {
            domain = $"{subdomain}.avaturn.dev";
            link = $"https://{domain}/iframe";
        }   else {
            domain = (new Uri(linkFromAPI)).Host;
            link = linkFromAPI;
        }

#if UNITY_IOS || UNITY_ANDROID
        Permission.RequestUserPermission(Permission.Camera);
        Permission.RequestUserPermission(Permission.Microphone);

        UniWebView.SetAllowAutoPlay(true);
        UniWebView.SetAllowInlinePlay(true);
        webView = webViewGameObject.AddComponent<UniWebView>();

        webView.Load(link);
        webView.ReferenceRectTransform = webViewFrame;

        _avatarReceiver.SetWebView(webView);
        webView.SetShowToolbar(true);
        webView.AddPermissionTrustDomain("scan.in3d.io");
        webView.AddPermissionTrustDomain(domain);
        webView.SetUserAgent("Mozilla/5.0 (Linux; Android 12) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.5359.128 Mobile Safari/537.36");
        webView.OnPageFinished += WebViewOnOnPageFinished;
#endif
    }

    private void WebViewOnOnPageFinished(UniWebView webview, int statuscode, string url)
    {
        //webView.OnPageFinished -= WebViewOnOnPageFinished;
        
        string jsCode = @"
        var time;
        if (typeof isListnerAttached === 'undefined') {
            window.addEventListener('message', subscribe);
            isListnerAttached = true;

            // Required overrides for mobile
            window.avaturnForceExportHttpUrl = true;
            window.avaturnFirebaseUseSignInWithRedirect = true;
        }

        function subscribe(event) {
            const json = parse(event);
            if (json?.source !== 'avaturn' || json?.eventName == null) {
                return;
            }
            if (json.eventName === 'v2.avatar.exported') {
                let url = json.data.url;

                if (json.data.urlType !== 'httpURL') { 
                    url = 'error://urlType is not httpURL';
                }
            
                location.href = 'uniwebview://action?avatar_link=' + encodeURIComponent(url);
            }

            function parse(event) {
                try {
                    return JSON.parse(event.data);
                } catch (error) {
                    return null;
                }
            }
        }
        ";
        
        webView.AddJavaScript(jsCode, (payload) => {
            if (payload.resultCode.Equals("0")) {
                print("Adding JavaScript Finished without error.");
            }
        });
    }

    public void ShowView(bool show)
    {
        if (show) webView.Show();
        else webView.Hide();
    }
}
