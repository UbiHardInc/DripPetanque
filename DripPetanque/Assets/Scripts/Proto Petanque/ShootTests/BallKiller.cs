using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BallKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider ball)
    {
        Destroy(ball.gameObject);
    }
}
