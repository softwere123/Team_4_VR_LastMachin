
//ObjectNameChanger Copyright(c) 2021 ParanPado.All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectNameChanger : EditorWindow
{
    private string[] tabNames = { "Append", "Replace" };
    private int curTabIdx = 0;

    private GameObject rootObject = null;   // 이름바꿀 최상위 오브젝트
    public string PrefixText { get; private set; } = string.Empty;  // 앞에 붙일 스트링
    public string SuffixText { get; private set; } = string.Empty;  // 뒤에 붙일 스트링

    public string FromText { get; private set; } = string.Empty;  // 찾을 스트링
    public string ToText { get; private set; } = string.Empty;  // 바꿀 스트링

    [MenuItem("ParanPado/ObjectNameChanger")]
    static void Open()
    {
        var window = EditorWindow.GetWindow<ObjectNameChanger>();
        window.Setup();
    }

    private void Setup()
    {
    }

    private void OnGUI()
    {
        using(new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            curTabIdx = GUILayout.Toolbar(curTabIdx, tabNames, new GUIStyle(EditorStyles.toolbarButton), GUI.ToolbarButtonSize.FitToContents);
        }

        if(curTabIdx == 0) // 확장
        {
            //GUILayout.BeginHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Append string front or back", EditorStyles.boldLabel);
            GUILayout.Space(20);

            EditorGUILayout.LabelField("RootObject");
            rootObject = EditorGUILayout.ObjectField(rootObject, typeof(GameObject), true) as GameObject;
            //GUILayout.EndHorizontal();

            GUILayout.Space(20);

            PrefixText = EditorGUILayout.TextField("Prefix Text", PrefixText);
            SuffixText = EditorGUILayout.TextField("Suffix Text", SuffixText);

            GUILayout.Space(20);

            if (GUILayout.Button("Go!"))
            {
                BtnAppendOK();
            }
        }
        else if(curTabIdx == 1) // 교체
        {
            //GUILayout.BeginHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Replace string", EditorStyles.boldLabel);
            GUILayout.Space(20);

            EditorGUILayout.LabelField("RootObject");
            rootObject = EditorGUILayout.ObjectField(rootObject, typeof(GameObject), true) as GameObject;
            //GUILayout.EndHorizontal();

            GUILayout.Space(20);

            FromText = EditorGUILayout.TextField("From", FromText);
            ToText = EditorGUILayout.TextField("To", ToText);

            GUILayout.Space(20);

            if (GUILayout.Button("Go!"))
            {
                BtnReplaceOK();
            }
        }
    }

    void BtnAppendOK()
    {
        if(rootObject != null)
        {
            if(PrefixText.Length > 0 || SuffixText.Length > 0)  // PrefixText나 SuffixText에 뭔가 있을때만 동작
            {
                // 뒤로가기를 하기위해 등록
                string unDoName = string.Format("{0}{1}{2}", PrefixText, rootObject.name, SuffixText);
                Undo.RegisterFullObjectHierarchyUndo(rootObject, unDoName);

                Transform[] allChildren = rootObject.transform.GetComponentsInChildren<Transform>();
                foreach (var child in allChildren)
                {
                    child.name = string.Format("{0}{1}{2}", PrefixText, child.name, SuffixText);
                }

                rootObject = EditorGUILayout.ObjectField(null, typeof(GameObject), true) as GameObject;
                PrefixText = EditorGUILayout.TextField("Prefix Text", "");
                SuffixText = EditorGUILayout.TextField("Suffix Text", "");
            }
        }
    }

    void BtnReplaceOK()
    {
        if (rootObject != null)
        {
            if (FromText.Length > 0)  // FromText(찾을 문자열)이 있는경우에만 실행
            {
                // 뒤로가기를 하기위해 등록
                string unDoName = string.Format("{0}", rootObject.name);
                Undo.RegisterFullObjectHierarchyUndo(rootObject, unDoName);

                Transform[] allChildren = rootObject.transform.GetComponentsInChildren<Transform>();
                foreach (var child in allChildren)
                {
                    child.name = child.name.Replace(FromText, ToText);
                }

                rootObject = EditorGUILayout.ObjectField(null, typeof(GameObject), true) as GameObject;
                FromText = EditorGUILayout.TextField("From", "");
                ToText = EditorGUILayout.TextField("To", "");
            }
        }
    }
}
