using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IMatchTarget
{
    Collider _targetCollider;

    private void Awake()
    {
        TryGetComponent(out _targetCollider);
    }

    public Vector3 TargetPosition => _targetCollider.ClosestPoint(transform.position);
}
