using System;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

#if UNITY_IOS
using UnityEngine.iOS;
#endif


namespace Avaturn
{
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
                link = $"https://{domain}?sdk=true";
            }   else {
                domain = new Uri(linkFromAPI).Host;
                link = linkFromAPI + "&sdk=true";
            }

#if UNITY_ANDROID
            Permission.RequestUserPermission(Permission.Camera);
            Permission.RequestUserPermission(Permission.Microphone);
#elif UNITY_IOS 
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
#endif

#if UNITY_ANDROID || UNITY_IOS
            UniWebView.SetAllowAutoPlay(true);
            UniWebView.SetAllowInlinePlay(true);
            webView = webViewGameObject.AddComponent<UniWebView>();
    
            webView.Load(link);
            webView.ReferenceRectTransform = webViewFrame;
    
            _avatarReceiver.SetWebView(webView);
            webView.SetAcceptThirdPartyCookies(true);
            webView.SetShowToolbar(true);
            webView.AddPermissionTrustDomain(domain);
            webView.SetUserAgent("Mozilla/5.0 (Linux; Android 12) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.5359.128 Mobile Safari/537.36");
            webView.OnPageFinished += WebViewOnOnPageFinished;
#endif
        }

        private void WebViewOnOnPageFinished(UniWebView webview, int statuscode, string url)
        {     
            string jsCode = @"
function loadAvaturn() {
  
  // Required overrides for mobile
  window.avaturnForceExportHttpUrl = true;
  window.avaturnFirebaseUseSignInWithRedirect = true;

  // Init SDK and callback
  window.avaturnSDKEnvironment = JSON.stringify({ engine: 'Unity', version: '__VERSION__', platform: '__PLATFORM__' });
  window.avaturnSDK.init(null, {})
    .then(() => window.avaturnSDK.on('export',
      (data) => {
        const params = new URLSearchParams();

        ['avatarId', 'avatarSupportsFaceAnimations', 'bodyId', 'gender', 'sessionId', 'url', 'urlType'].forEach( (p) => {
          params.append(p, data[p] || '');
        })
          
        location.href = 'uniwebview://action?' + params.toString();
      })
    );
}

// Start Avaturn on page load 
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', loadAvaturn);
} else {
  loadAvaturn();
}";
            jsCode = jsCode.Replace("__VERSION__", Application.unityVersion);
            jsCode = jsCode.Replace("__PLATFORM__", GetPlatformString());
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
        private string GetPlatformString()
        {
            #if UNITY_EDITOR
                return "editor";
            #elif UNITY_IOS
                return "ios";
            #elif UNITY_ANDROID
                return "android";
            #elif UNITY_WEBGL
                return "webgl";
            #elif UNITY_STANDALONE_WIN
                return "windows";
            #elif UNITY_STANDALONE_OSX
                return "mac";
            #elif UNITY_STANDALONE_LINUX
                return "linux";
            #else
                return "unknown";
            #endif
        }
    }
    
    
}
