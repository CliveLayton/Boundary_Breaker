using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HoloWall : MonoBehaviour
{
    [SerializeField] private GameReferee referee;
    [SerializeField] private float damage;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float hitStopDuration = 0.3f;
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private Vector2 attackForce = new Vector2(1.5f,-2f);
    [SerializeField] private bool applyKnockDown;

    [SerializeField] private Vector3 position1;
    [SerializeField] private Vector3 position2;
    [SerializeField] private Vector3 position3;
    [SerializeField] private bool isRightWall;

    [Header("Impact Values")] 
    [SerializeField] private float shakeStrength = 0.03f;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float rippleCooldown = 0.4f;

    private Material material;
    private float rippleTime = 100f;
    private float desolveValue = -1.2f;
    private Coroutine shakeRoutine;
    private Vector3 originalPosition;
    private bool wallIsBroken;

    private PlayerStateMachine player1;
    private PlayerStateMachine player2;
    
    private MeshCollider col;
    private bool isOnWall1, isOnWall2;

    private void Awake()
    {
        col = GetComponent<MeshCollider>();
        material = GetComponent<Renderer>().material;
        material.DisableKeyword("_USE_DESOLVE");
        GameStateManager.Instance.onStateChanged += GetPlayers;
        SetupWall();
    }

    private void Update()
    {
        rippleTime += Time.deltaTime;
        material.SetFloat("_Ripple_Time", rippleTime);
    }

    private void OnDisable()
    {
        GameStateManager.Instance.onStateChanged -= GetPlayers;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player1.gameObject == other.gameObject)
            {
                Player1OnWall();
            }
            else if (player2.gameObject == other.gameObject)
            {
                Player2OnWall();
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (player1.gameObject == other.gameObject && !player1.InHitStun)
            {
                isOnWall1 = false;
            }
            else if (player2.gameObject == other.gameObject & !player2.InHitStun)
            {
                isOnWall2 = false;
            }
        }
    }

    public void GetImpact(RaycastHit hit)
    {
        if (rippleTime < rippleCooldown)
        {
            return;
        }
        
        material.SetVector("_Ripple_Origin", hit.textureCoord);
        if (wallIsBroken)
        {
            desolveValue = -1.1f;
            material.SetFloat("_Desolve_Value", -1.1f);
            material.EnableKeyword("_USE_DESOLVE");
            StartCoroutine(LerpDesolve());
        }
        rippleTime = material.GetFloat("_Ripple_Thickness") * -2f;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            transform.position = originalPosition;
        }

        originalPosition = transform.position;
        shakeRoutine = StartCoroutine(Shake(hit));
    }

    private IEnumerator Shake(RaycastHit hit)
    {
        for (float t = 0f; t < shakeDuration; t += Time.deltaTime)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeStrength;
            yield return null;
        }

        transform.position = originalPosition;
    }

    private IEnumerator LerpDesolve()
    {
        float elapsed = 0f;

        while (elapsed < hitStopDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / hitStopDuration;
            desolveValue = Mathf.Lerp(-1.1f, -0.7f, t);
            material.SetFloat("_Desolve_Value", desolveValue);

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < stunDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / stunDuration;
            desolveValue = Mathf.Lerp(-0.7f, 0.4f, t);
            material.SetFloat("_Desolve_Value", desolveValue);

            yield return null;
        }

        desolveValue = 0.4f;
    }

    private void Player1OnWall()
    {
        if (player1.InHitStun)
        {
            return;
        }

        if (player1.CombinedForce.magnitude > 10f && !isOnWall1)
        {
            wallIsBroken = true;
            isOnWall1 = true;
            player1.Damage(damage, 0.5f, 0.5f, 
                new Vector2(3,0.8f), 0.4f, false, false, false, false, false);
            col.enabled = false;
            WallBreak(1);
        }
        else if(!isOnWall1)
        {
            isOnWall1 = true;
            player1.Damage(damage, stunDuration, hitStopDuration, 
                attackForce, knockBackTime, false, false, true, false, applyKnockDown);
        }
    }

    private void Player2OnWall()
    {
        if (player2.InHitStun)
        {
            return;
        }

        if (player2.CombinedForce.magnitude > 10f && !isOnWall2)
        {
            wallIsBroken = true;
            isOnWall2 = true;
            player2.Damage(damage, 0.5f, 0.5f, 
                new Vector2(3,0.8f), 0.4f, false, false, false, false, false);
            col.enabled = false;
            WallBreak(0);
        }
        else if(!isOnWall2)
        {
            isOnWall2 = true;
            player2.Damage(damage, stunDuration, hitStopDuration, 
                attackForce, knockBackTime, false, false, true, false, applyKnockDown);
        }
    }

    private void SetupWall()
    {
        if (isRightWall)
        {
            switch (GameStateManager.Instance.wallBreakCountR)
            {
                case 0:
                    transform.position = position1;
                    break;
                case 1:
                    transform.position = position2;
                    break;
                case 2:
                    transform.position = position3;
                    break;
            }
        }
        else
        {
            switch (GameStateManager.Instance.wallBreakCountL)
            {
                case 0:
                    transform.position = position1;
                    break;
                case 1:
                    transform.position = position2;
                    break;
                case 2:
                    transform.position = position3;
                    break;
            }
        }
    }
    
    private void GetPlayers(GameStateManager.GameState newState)
    {
        if (newState == GameStateManager.GameState.InGame)
        {
            var characterArray = FindObjectsByType<PlayerStateMachine>(FindObjectsSortMode.None);
            for (int i = 0; i < characterArray.Length; i++)
            {
                if (characterArray[i].PlayerIndex == 0)
                {
                    player1 = characterArray[i];
                }

                if (characterArray[i].PlayerIndex == 1)
                {
                    player2 = characterArray[i];
                }
            }
        }
    }

    private void WallBreak(int index)
    {
        if (isRightWall)
        {
            GameStateManager.Instance.wallBreakCountR += 1;
        }
        else
        {
            GameStateManager.Instance.wallBreakCountL += 1;
        }

        StartCoroutine(referee.RestartGame(index));
    }
}
