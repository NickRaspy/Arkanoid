using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    private UnityEvent<int, Brick> onDestroyed = new();
    public UnityEvent<int, Brick> OnDestroyed 
    { 
        get {  return onDestroyed; }
        set { onDestroyed = value; }
    }

    private BrickValues brickValues;
    public BrickValues BrickValues
    {
        get { return brickValues; }
        set 
        { 
            brickValues = value;

            var renderer = GetComponent<Renderer>();
            MaterialPropertyBlock block = new();
            block.SetColor("_BaseColor", brickValues.blockColor);
            renderer.SetPropertyBlock(block);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        OnDestroyed.Invoke(brickValues.value, this);
        Destroy(gameObject);
    }
}
[Serializable]
public class BrickValues
{
    public int value;
    public Color blockColor;
}
