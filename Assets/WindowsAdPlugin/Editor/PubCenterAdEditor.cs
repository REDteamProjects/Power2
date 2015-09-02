using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PubCenterAd))]
public class PubCenterAdEditor : Editor {

    public override void OnInspectorGUI()
    {
        //Provide links to the test values
        EditorGUILayout.LabelField("Windows Phone Test Values");
        EditorGUILayout.TextArea("http://msdn.microsoft.com/en-us/library/advertising-mobile-windows-phone-test-mode-values(v=msads.20).aspx" );
        EditorGUILayout.LabelField("Windows Store Test Values");
        EditorGUILayout.TextArea("http://msdn.microsoft.com/en-us/library/advertising-windows-test-mode-values(v=msads.10).aspx ");

        DrawDefaultInspector();
    }
}
