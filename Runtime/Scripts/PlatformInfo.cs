using UnityEngine;

namespace Avaturn
{
    public class PlatformInfo : MonoBehaviour
    {
        void Start()
        {
#if !UNITY_EDITOR || !UNITY_STANDALONE_WIN
        gameObject.SetActive(false);
#endif
        }
    }
}
