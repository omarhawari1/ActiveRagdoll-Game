using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Mirror;

public class headLookAt : NetworkBehaviour
{
    [SerializeField] private float distance = 5f;  // Radius around the player to look at
    [SerializeField] private Transform player;     // Reference to the player's transform
    [SerializeField] private MultiAimConstraint[] multiAimConstraints;
    [SerializeField] private float weightTransitionSpeed;

    private Camera playerCam;

    [SyncVar(hook = nameof(onWeightChanged))]
    private float ConstraintWeight;

    private Coroutine weightCoroutine;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerCam = Camera.main;
    }

    private void Update()
    {
        if(!isLocalPlayer) { return; }
        Vector3 dir = playerCam.transform.forward * distance;
        Vector3 calculatedTargetPosition = player.position + dir;

        if (Vector3.Distance(player.position, calculatedTargetPosition) > distance)
        {
            calculatedTargetPosition = player.position + dir.normalized * distance;
        }

        Vector3 toTarget = calculatedTargetPosition - player.position;
        float dotProduct = Vector3.Dot(player.forward, toTarget.normalized);

        float newWeight = (dotProduct < 0) ? 1 : 0f;
        CmdChangeWeight(newWeight);
        CmdSetTarget(calculatedTargetPosition);
    }
    [Command]
    private void CmdSetTarget(Vector3 calculatedTargetPosition)
    {
        transform.position = calculatedTargetPosition;
    }

    private void onWeightChanged(float newWeight, float oldWeight)
    {
        if (weightCoroutine != null)
        {
            StopCoroutine(weightCoroutine);
        }
        weightCoroutine = StartCoroutine(gradualyChangeConstraintWeight(newWeight));
    }

    private IEnumerator gradualyChangeConstraintWeight(float newWeight)
    {
        float elapsedTime = 0f;
        while (elapsedTime < weightTransitionSpeed)
        {
            
            foreach (MultiAimConstraint constraint in multiAimConstraints)
            {
                float currentWeight = Mathf.Lerp(constraint.weight, newWeight, elapsedTime / weightTransitionSpeed);
                constraint.weight = currentWeight;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final weight is set correctly
        foreach (MultiAimConstraint constraint in multiAimConstraints)
        {
            constraint.weight = newWeight;
        }

        // End the coroutine
        weightCoroutine = null;
    }

    [Command]
    private void CmdChangeWeight(float newWeight)
    {
        ConstraintWeight = newWeight;
    }
}
