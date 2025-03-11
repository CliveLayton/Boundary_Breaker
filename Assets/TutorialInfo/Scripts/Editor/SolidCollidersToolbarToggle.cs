using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

//the visual element used in the overlay toolbar
[EditorToolbarElement(SolidCollidersToolbarToggle.id, typeof(SceneView))]
internal sealed class SolidCollidersToolbarToggle : EditorToolbarToggle
{
    public const string id = "Colliders/BoxCollidersAttribute";

    public SolidCollidersToolbarToggle() : base()
    {
        text = "Draw Solid Colliders";
        tooltip = "Toggles whether colliders should be drawn as solid or as wireframe.";
        icon = (Texture2D)EditorGUIUtility.Load("d_BoxCollider Icon");
        this.SetValueWithoutNotify(LBGEditorPreferences.instance.GetSolidValue());
        this.RegisterValueChangedCallback<bool>(HandleToggleValue);
    }

    private void HandleToggleValue(ChangeEvent<bool> value)
    {
        LBGEditorPreferences.instance.ModifySolid(value.newValue);
    }
}

//the visual element used in the overlay toolbar
[EditorToolbarElement(AlwaysShowCollidersToolbarToggle.id, typeof(SceneView))]
internal sealed class AlwaysShowCollidersToolbarToggle : EditorToolbarToggle
{
    public const string id = "Colliders/AlwaysShowColliders";

    public AlwaysShowCollidersToolbarToggle() : base()
    {
        text = "Always Show Colliders";
        tooltip = "Toggles whether colliders should always be visable or only on selected";
        icon = (Texture2D)EditorGUIUtility.Load("d_BoxCollider Icon");
        this.SetValueWithoutNotify(LBGEditorPreferences.instance.GetAlwaysShowValue());
        this.RegisterValueChangedCallback<bool>(HandleToggleValue);
    }

    private void HandleToggleValue(ChangeEvent<bool> value)
    {
        LBGEditorPreferences.instance.ModifyAlwaysShow(value.newValue);
    }
}


[Overlay(typeof(SceneView), "Collider Tools")]
internal sealed class ColliderToolbar : ToolbarOverlay
{
    public ColliderToolbar() : base(SolidCollidersToolbarToggle.id, AlwaysShowCollidersToolbarToggle.id)
    {
        
    }
}


internal static class ColliderGizmosDrawer
{
    [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NonSelected, typeof(BoxCollider))]
    public static void HandleDrawBoxColliderGizmo(BoxCollider collider, GizmoType gizmoType)
    {
        if (!LBGEditorPreferences.instance.GetAlwaysShowValue() && gizmoType.HasFlag(GizmoType.NotInSelectionHierarchy))
        {
            return;
        }

        bool solid = LBGEditorPreferences.instance.GetSolidValue();
        
        //get the per-object color from the helper component
        BoxColliderColor colorComponent = collider.GetComponent<BoxColliderColor>();
        Color c = colorComponent != null ? colorComponent.boxColor : Color.green;
        DrawGizmosUtilities.DrawBoxColliderGizmos(collider, c, solid);
    }
}

public static class DrawGizmosUtilities
{
    public static void DrawBoxColliderGizmos(BoxCollider box, Color color, bool drawSolid)
    {
        Gizmos.matrix = box.transform.localToWorldMatrix;
        Gizmos.color = color;

        Vector3 offset = box.center;
        Vector3 size = box.size;

        if (drawSolid)
        {
            Gizmos.DrawCube(offset, size);
        }
        else
        {
            Gizmos.DrawWireCube(offset, size);
        }
        
        Gizmos.matrix = Matrix4x4.identity;
    }
}
