using System.Collections;
using UnityEngine;
using UnityUtility.CustomAttributes;

public class Bumper : MonoBehaviour
{
    [SerializeField, Layer] private int m_whatIsBall;
    public BumpManager.BumpersStrength bumperForce;

    public Color bumpColor;
    public ForceMode forceMode = ForceMode.VelocityChange;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == m_whatIsBall)
        {
            Rigidbody ballRigidBody = collision.transform.GetComponent<Rigidbody>();
            ballRigidBody.AddExplosionForce(BumpManager.Instance.GetBumperStrength(bumperForce), collision.GetContact(0).point, 5,0,forceMode);
            //_ = StartCoroutine(ChangeOnBump());
            SpriteRandom.Instance.FlashSprite();
            Debug.Log("Bumped!");
        }
    }

    private IEnumerator ChangeOnBump()
    {
        MeshRenderer meshCollider = gameObject.GetComponent<MeshRenderer>();
        MaterialPropertyBlock matPro = new MaterialPropertyBlock();
        var basecolor = "_BaseColor";
        meshCollider.GetPropertyBlock(matPro, 0);
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