using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathZone : MonoBehaviour
{
    [NonSerialized] public UnityEvent onEntered;

    private void Start() => onEntered = new();
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Bullet>() != null) return;
        onEntered.Invoke();
    }
}
