using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    /// <summary>
    /// OnMouseDown
    /// </summary>
    private void OnMouseDown()
    {
        LevelManaer.Instance.BombDown(this);
    }
}
