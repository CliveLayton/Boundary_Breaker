using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Vector3 hitboxSize = Vector3.one;
    public Vector3 hitboxOffset = Vector3.zero;
    public bool useSphere = false;
    public float radius = 0.5f;

    [SerializeField] private bool drawSolid = true;
    [SerializeField] private Color colliderColor = Color.red;
    [SerializeField] private LayerMask layerToCheck;

    private void Update()
    {
        Collider[] colliders;
        
        Vector3 overlapBoxPosition = transform.position + transform.rotation * hitboxOffset;

        if (useSphere)
        {
            colliders = Physics.OverlapSphere(transform.position + hitboxOffset, radius, layerToCheck);
        }
        else
        {
            colliders = Physics.OverlapBox(overlapBoxPosition, hitboxSize, transform.rotation, layerToCheck);
        }
        

        if (colliders.Length > 0)
        {
            Debug.Log("got a hit");
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 worldHitBoxPosition = transform.position + transform.rotation * hitboxOffset; 
        
        Gizmos.color = colliderColor;
        Gizmos.matrix = Matrix4x4.TRS(worldHitBoxPosition, transform.rotation, transform.localScale);

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
            Gizmos.DrawSphere(Vector3.zero, radius);
        }
        else if(!drawSolid && useSphere)
        {
            Gizmos.DrawWireSphere(Vector3.zero, radius);
        }

    }
}
