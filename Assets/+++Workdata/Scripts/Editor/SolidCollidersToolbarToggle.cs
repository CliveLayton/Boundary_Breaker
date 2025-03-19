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

//get the informations for the gizmo and set when to render them
internal static class HitBoxGizmosDrawer
{
    [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NonSelected, typeof(Hitbox))]
    public static void HandleDrawHitboxGizmo(Hitbox hitbox, GizmoType gizmoType)
    {
        if (!LBGEditorPreferences.instance.GetAlwaysShowValue() && gizmoType.HasFlag(GizmoType.NotInSelectionHierarchy))
        {
            return;
        }

        bool solid = LBGEditorPreferences.instance.GetSolidValue();
        bool useSphere = hitbox.useSphere;

        Vector3 worldHitboxPosition = hitbox.transform.position + hitbox.transform.rotation * hitbox.hitboxOffset;
        Vector3 hitboxSize = hitbox.hitboxSize;
        Quaternion hitboxRotation = hitbox.transform.rotation;
        Vector3 hitboxLocalScale = hitbox.transform.localScale;
        float hitboxRadius = hitbox.radius;
        Color c = hitbox.CheckGizmoColor();
        DrawGizmosUtilities.DrawHitboxGizmos(worldHitboxPosition, hitboxSize, hitboxRotation, hitboxLocalScale, hitboxRadius, c,solid, useSphere);
    }
}

//actually drawing the gizmos with the informations
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

    public static void DrawHitboxGizmos(Vector3 worldHitboxPosition, Vector3 hitboxSize, Quaternion hitboxRotation,Vector3 hitboxLocalScale,float hitboxRadius,
        Color color, bool drawSolid, bool useSphere)
    {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(worldHitboxPosition, hitboxRotation, hitboxLocalScale);
        
        if (drawSolid && !useSphere)
        {
            Gizmos.DrawCube(Vector3.zero, new Vector3(hitboxSize.x *2, hitboxSize.y *2, hitboxSize.z *2)); // *2 because size is half extents
        }
        else if(!drawSolid && !useSphere)
        {
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(hitboxSize.x *2, hitboxSize.y *2, hitboxSize.z *2)); // *2 because size is half extents
        }

        if (drawSolid && useSphere)
        {
            Gizmos.DrawSphere(Vector3.zero, hitboxRadius);
        }
        else if(!drawSolid && useSphere)
        {
            Gizmos.DrawWireSphere(Vector3.zero, hitboxRadius);
        }
    }
}
