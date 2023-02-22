using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class IframeControllerWebGL : MonoBehaviour
{   
    // This class is used to control Avaturn iframe for WebGL platform.
    // IframeControllerMobile can also be used for these purposes
    // but this class supports dataURLs which are generated much faster than httpURLs
    // If you are building cross-platform application and use httpURLs, you may not need this class.

    [SerializeField] string subdomain;
    [SerializeField] string linkFromAPI = "";

    private static bool is_setup = false;
    private static bool is_open = false;
    
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetupAvaturnIframeJS(string subdomain);
    
    [DllImport("__Internal")]
    private static extern void ShowAvaturnIframeJS();
    
    [DllImport("__Internal")]
    private static extern void HideAvaturnIFrameJS();
#endif


    public void ChangeVisibility() { 
        if (is_open) {
            Hide();
        } else { 
            Show();
        }
    }

    public void Show()
    {               

#if !UNITY_EDITOR && UNITY_WEBGL
        SetupIframe();
        ShowAvaturnIframeJS();
        is_open = true;
#else
        Debug.Log("Iframe can't be open in editor. Build the project for WebGL to open iframe.");
#endif
    }

    public void Hide()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        HideAvaturnIFrameJS();
        is_open = false;
#else
        Debug.Log("Iframe does not work in editor. Build the project for WebGL to open iframe.");
#endif
    }

    public void SetupIframe()
    {   
        if(is_setup){
            return;
        }
        
        string link =  linkFromAPI == "" ? $"https://{subdomain}.avaturn.dev/iframe" : linkFromAPI; 
#if !UNITY_EDITOR && UNITY_WEBGL
        SetupAvaturnIframeJS(link);
#endif
        is_setup = true;
    }
}
