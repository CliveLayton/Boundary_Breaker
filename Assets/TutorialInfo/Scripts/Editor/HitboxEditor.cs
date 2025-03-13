using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Hitbox))]
public class HitboxEditor : Editor
{
    private void OnSceneGUI()
    {
        Hitbox hitbox = (Hitbox)target;
        
        //convert local position to world position
        Vector3 worldHitBoxPosition = hitbox.transform.position +  hitbox.transform.rotation * hitbox.hitboxOffset;
        
        //draw a visible marker at the hitbox position
        Handles.color = Color.black;
        Handles.SphereHandleCap(0, worldHitBoxPosition, Quaternion.identity, 0.1f, EventType.Repaint);
        
        //draw a handle at the hitbox position
        EditorGUI.BeginChangeCheck();
        Vector3 newWorldPosition = Handles.PositionHandle(worldHitBoxPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(hitbox, "Move Hitbox");
            hitbox.hitboxOffset = Quaternion.Inverse(hitbox.transform.rotation) *
                                  (newWorldPosition - hitbox.transform.position);
            EditorUtility.SetDirty(hitbox); //mark scene as changed
        }
    }
}
