using UnityEngine;

public class Hitbox : MonoBehaviour
{
    #region Variables

    public Vector3 hitboxSize = Vector3.one;
    public Vector3 hitboxOffset = Vector3.zero;
    public bool useSphere = false;
    public float radius = 0.5f;
    public Color colliderColor;
    
    private ColliderState state = ColliderState.Closed;
    private IHitboxResponder responder = null;
    private bool collided;

    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private Color inactiveColor = new Color(0.6f, 0.2f, 0.2f, 0.2f);
    [SerializeField] private Color collisionOpenColor = new Color(0.6f, 0.2f, 0.2f, 1);
    [SerializeField] private Color collidingColor = Color.red;

    public enum ColliderState
    {
        Closed,
        Open, 
        Colliding
    }

    #endregion

    #region Hitbox Methods

    public void HitBoxUpdate()
    {
        if (state == ColliderState.Closed || state == ColliderState.Colliding)
        {
            return;
        }
        
        Collider[] colliders;
        
        //by adding the offset we need to rotate the position correctly with the object 
        Vector3 hitboxBoxPosition = transform.position + transform.rotation * hitboxOffset;

        if (useSphere)
        {
            colliders = Physics.OverlapSphere(transform.position + hitboxOffset, radius, layerToCheck);
        }
        else
        {
            colliders = Physics.OverlapBox(hitboxBoxPosition, hitboxSize, transform.rotation, layerToCheck);
        }
        
        state = colliders.Length > 0 ? ColliderState.Colliding : ColliderState.Open;
        

        for (int i = 0; i < colliders.Length; i++)
        {
            if (i == 0)
            {
                Collider acollider = colliders[i];
                responder?.CollisionedWith(acollider);
                Debug.Log("got a hit");
            }
        }
        
    }

    //set a responder to send collison messages to
    public void SetResponder(IHitboxResponder aResponder)
    {
        responder = aResponder;
    }

    public void StartCheckingCollision()
    {
        state = ColliderState.Open;
    }

    public void StopCheckingCollision()
    {
        state = ColliderState.Closed;
    }
    
    /// <summary>
    /// Get the color for the gizmo
    /// </summary>
    /// <returns>colliderColor</returns>
    public Color CheckGizmoColor()
    {
        switch (state)
        {
            case ColliderState.Closed:
                colliderColor = inactiveColor;
                break;
            case ColliderState.Open:
                colliderColor = collisionOpenColor;
                break;
            case ColliderState.Colliding:
                colliderColor = collidingColor;
                break;
        }

        return colliderColor;
    }

    #endregion
    
}
