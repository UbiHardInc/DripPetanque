using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public int bumperForce = 800;
    private GameObject m_ball;

    public Color bumpColor;

    void Start ()
    {
        m_ball = GameObject.FindGameObjectWithTag ("Player");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == m_ball) {
            m_ball.GetComponent<Rigidbody>().AddExplosionForce(bumperForce, collision.contacts[0].point, 5);
            StartCoroutine(changeOnBump());
            SpriteRandom.Instance.FlashSprite();
            Debug.Log("Bumped!");
        }
    }

    private IEnumerator changeOnBump()
    {
        MeshRenderer meshCollider = gameObject.GetComponent<MeshRenderer>();
        MaterialPropertyBlock matPro = new MaterialPropertyBlock();
        var basecolor = "_BaseColor";
        //meshCollider.GetPropertyBlock(matPro, 0);
        Color lastColor;
        lastColor = matPro.HasColor(basecolor) ? matPro.GetColor(basecolor) : meshCollider.material.GetColor(basecolor);
        matPro.SetColor(basecolor,bumpColor);
        meshCollider.SetPropertyBlock(matPro);
        
        Debug.Log("LastColor : " + lastColor.ToString());
        
        yield return new WaitForSeconds(0.15f);
        
        matPro.SetColor(basecolor,lastColor);
        meshCollider.SetPropertyBlock(matPro);
    }
}