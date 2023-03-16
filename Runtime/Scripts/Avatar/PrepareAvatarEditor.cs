using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrepareAvatar))]
public class PrepareAvatarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PrepareAvatar prepareAvatar = (PrepareAvatar) target;
        prepareAvatar.SetAnimator();

        if (GUILayout.Button("Save Avatar Skeleton"))
        {
            string path = EditorUtility.SaveFilePanel("Save to this location", "Assets/", "newAvatar", "asset");
            path = path.Substring(path.IndexOf("Assets/", StringComparison.Ordinal));
            prepareAvatar.SaveAvatar(path);
        }
    }
}