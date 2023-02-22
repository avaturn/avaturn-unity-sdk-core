using System;
using System.Threading.Tasks;
using UnityEngine;

using GLTFast;
using GLTFast.Loading;
using UnityEngine.Scripting;

/// <summary>
/// This example of loading avatar model.
/// </summary>
[RequireComponent(typeof(GltfAsset))]
public class DownloadAvatar : MonoBehaviour
{
    [SerializeField] private DownloadAvatarEvents _events;

    [Tooltip("Use this for debug along with 'start url' to load avatar at runtime on start.")]
    [SerializeField] private bool _downloadOnStart;
    [SerializeField] private string _startUrl;

    private void Start()
    {
        if(_downloadOnStart)
            Download(_startUrl);
    }

    public async void Download(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("Fail to download: url is empty");
            return;
        }
        Debug.Log("Start download...");
        
        // Loading via GltFast loader
        var asset = GetComponent<GltfAsset>();
        asset.ClearScenes();
        var success = await asset.Load(url, new AvaturnDownloadProvider());
        // Optional for animations
        if (success)
        {
            _events.OnSuccess?.Invoke(transform);
        }
        else
        {
            Debug.LogError($"Fail to download");
        }
    }
    
    public async  Task<IDownload> Request(Uri url) {
        var req = new AvaturnAwaitableDownload(url);
        await req.WaitAsync();
        return req;
    }
}