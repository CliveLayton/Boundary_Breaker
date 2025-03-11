using UnityEditor;
using UnityEngine;

[FilePath("Editor/StateFile.foo", FilePathAttribute.Location.PreferencesFolder)]
public class LBGEditorPreferences : ScriptableSingleton<LBGEditorPreferences>
{
    private const string SolidCollider = "Solid_Collider";
    private const string AlwaysShowCollider = "Always_Show_Collider";

    public void ModifySolid(bool newValue)
    {
        EditorPrefs.SetBool(SolidCollider, newValue);
    }

    public bool GetSolidValue()
    {
        return EditorPrefs.GetBool(SolidCollider, false);
    }

    public void ModifyAlwaysShow(bool newValue)
    {
        EditorPrefs.SetBool(AlwaysShowCollider, newValue);
    }

    public bool GetAlwaysShowValue()
    {
        return EditorPrefs.GetBool(AlwaysShowCollider, false);
    }
}
