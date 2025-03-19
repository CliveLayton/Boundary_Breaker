using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AutoAddBoxColliderColorEditor
{
    static AutoAddBoxColliderColorEditor()
    {
        //runs on editor load and scene changes
        EditorApplication.hierarchyChanged += AutoAttachScript;
    }

    private static void AutoAttachScript()
    {
        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            //check if the object has a BoxCollider but no AutoAddBoxColliderColor
            if (obj.GetComponent<BoxCollider>() != null & obj.GetComponent<BoxColliderColor>() == null)
            {
                obj.AddComponent<BoxColliderColor>();
                EditorUtility.SetDirty(obj); //marks object as modified
            }
        }
    }
}
