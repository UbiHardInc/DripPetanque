using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityUtility.CustomAttributes;
using DG.Tweening;
using UnityUtility.Utils;
using System.Linq;
using System.Collections.Generic;

public class Bumper : MonoBehaviour
{
    [SerializeField] private SpriteRandom m_spriteRandom;
    [SerializeField, Layer] private int m_whatIsBall;
    public BumpManager.BumpersStrength bumperForce;
    public Color bumpColor;
    public ForceMode forceMode = ForceMode.VelocityChange;
    [Title ("Visual effect")]
    [SerializeField] private VisualEffect m_vfxBump;
    [SerializeField] private Transform[] transformsToShake;
    public float shakeDuration;
    public Vector3 shakeStrenght;
    public int shakeVibrato;
    private IEnumerable<Tween> m_shakes;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == m_whatIsBall)
        {
            Rigidbody ballRigidBody = collision.transform.GetComponent<Rigidbody>();
            ballRigidBody.AddExplosionForce(BumpManager.Instance.GetBumperStrength(bumperForce), collision.GetContact(0).point, 5,0,forceMode);
            //_ = StartCoroutine(ChangeOnBump());
            m_vfxBump.transform.position = collision.transform.position;
            m_vfxBump.SendEvent("PlayBumpEffect");
            ShakeTransforms();
            m_spriteRandom.FlashSprite();
            SoundManager.Instance.PlayUISFX("bumper");
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

    private void ShakeTransforms()
    {
        if (m_shakes != null)
        {
            m_shakes.ForEach(tween => { tween?.Complete(); });
        }
        m_shakes = transformsToShake.Select(transform => transform.DOShakeScale(shakeDuration, shakeStrenght, shakeVibrato, 90f, true)).ToArray();
        
    }
}