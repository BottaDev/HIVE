using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class KamCustomEditor : Editor
{
    public enum EditorTypes
    {
        GameDesigner, Programmer
    }
    
    private EditorTypes current;
    private bool listeningForGuiChanges;
    private bool guiChanged;

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Custom Inspector Change");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current View Type", EditorStyles.boldLabel);
        current = (EditorTypes) EditorGUILayout.EnumPopup(current);
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        switch (current)
        {
            case EditorTypes.Programmer:
                EditorGUILayout.HelpBox(
                    "This is a full view of every inspector variable, you should not touch any of this if you're not who programmed this or haven't consulted with them first!",
                    MessageType.Warning);
                EditorGUILayout.Space();
                base.OnInspectorGUI();
                break;

            case EditorTypes.GameDesigner:
                GameDesignerInspector();
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            guiChanged = true;
            
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(target);
        }
    }

    public virtual void GameDesignerInspector()
    {
        EditorGUILayout.HelpBox("There is nothing here for you to modify. Which means this is a programmer only script.", MessageType.Info);
    }
}
#endif