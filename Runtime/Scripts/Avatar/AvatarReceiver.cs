using System;
using UnityEngine;
using UnityEngine.Events;

namespace Avaturn
{
    public class AvatarReceiver : MonoBehaviour
    {
        [Serializable]
        class OnReceived : UnityEvent<AvatarReceivedEventArgs> { }

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
            string url = Uri.UnescapeDataString(message.Args["url"]);
            string urlType = Uri.UnescapeDataString(message.Args["urlType"]);
            string bodyId = Uri.UnescapeDataString(message.Args["bodyId"]);
            string gender = Uri.UnescapeDataString(message.Args["gender"]);
            string avatarId = Uri.UnescapeDataString(message.Args["avatarId"]);

            received?.Invoke(new AvatarReceivedEventArgs(url, urlType, bodyId, gender, avatarId));
        }

        /// <summary>
        /// This method is invoked for WebGL controller. URL can be either dataURL or httpURL
        /// </summary>
        public void ReceiveAvatarLink(string url)
        {
            received?.Invoke(new AvatarReceivedEventArgs(url, "", "", "", ""));
        }
    }
    public class AvatarReceivedEventArgs
    {
        public string url;
        public string urlType;
        public string bodyId;
        public string gender;
        public string avatarId;


        public AvatarReceivedEventArgs(string url, string urlType, string bodyId, string gender, string avatarId)
        {
            this.url = url;
            this.urlType = urlType;
            this.bodyId = bodyId;
            this.gender = gender;
            this.avatarId = avatarId;
        }
    }
}