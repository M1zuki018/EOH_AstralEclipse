using System;
using UnityEngine;

public class TriggerControl : MonoBehaviour
{
    public event Action<Collider> OnTrigger; 
    
    private void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(other);
    }
}
