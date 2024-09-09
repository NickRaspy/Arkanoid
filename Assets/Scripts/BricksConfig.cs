using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bricks Config", menuName = "Scriptable Objects/Bricks Config")]
public class BricksConfig : ScriptableObject
{
    public List<BrickValues> bricks;
}
