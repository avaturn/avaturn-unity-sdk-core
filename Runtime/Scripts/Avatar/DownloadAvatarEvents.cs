using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AvaturnEvents/DownloadEvents")]
public class DownloadAvatarEvents : ScriptableObject
{
   public Action<Transform> OnSuccess;
}
