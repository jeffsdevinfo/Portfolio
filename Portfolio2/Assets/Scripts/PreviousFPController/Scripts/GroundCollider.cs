using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GroundCollider - class used to check if player is grounded. 
///     Uses Trigger Collider
/// </summary>
public class GroundCollider : MonoBehaviour
{
    //[SerializeField] GameObject GroundCheckPoint; // point used as a reference to cast the sphere from
    [SerializeField] LayerMask surfaceMask; // mask used to filter out what to hit
    [SerializeField] public bool IsGrounded = false;
    int numGroundsOn;

    private void OnTriggerEnter(Collider other)
    {
        if ((surfaceMask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            Debug.Log($"OnTriggerEnter other:{other.gameObject.name}");
            numGroundsOn++;
            IsGrounded = numGroundsOn > 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((surfaceMask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            Debug.Log($"OnTriggerExit other:{other.gameObject.name}");
            numGroundsOn--;
            IsGrounded = numGroundsOn > 0;
        }
    }
}
