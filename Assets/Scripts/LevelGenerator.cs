using UnityEngine;
using UnityEditor;
public class LevelGenerator : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    string s = "";

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Level Generator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelGenerator window = (LevelGenerator)EditorWindow.GetWindow(typeof(LevelGenerator));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Generate Level")) {
            GameObject obj =  (GameObject)EditorUtility.InstanceIDToObject(Selection.activeObject.GetInstanceID());
            Debug.Log(obj.transform);
        }
    }
}