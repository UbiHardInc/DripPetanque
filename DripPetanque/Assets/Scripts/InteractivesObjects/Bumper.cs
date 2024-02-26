using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public int bumperForce = 800;
    private GameObject m_ball;

    void Start ()
    {
        m_ball = GameObject.FindGameObjectWithTag ("Player");
    }

    public void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject == m_ball) {
            m_ball.GetComponent<Rigidbody>().AddExplosionForce(bumperForce, collision.contacts[0].point, 5);
            Debug.Log("Bumped!");
        }
    }
}