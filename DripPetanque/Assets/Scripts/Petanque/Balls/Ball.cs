using System;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;
using UnityUtility.Timer;

public class Ball : MonoBehaviour, IPoolOperationCallbackReciever
{
    public event Action<Ball> OnBallStopped;

    public Rigidbody Rigidbody => m_rigidbody;

    public BasePetanquePlayer BallOwner { get => m_ballOwner; set => m_ballOwner = value; }

    [SerializeField, Layer] private int m_groundLayer;

    [Title("Ball Stop")]
    [SerializeField] private float m_speedToStop = 0.01f;
    [SerializeField] private Timer m_timerToStop;
    [SerializeField] private float m_stopSoundThreshold = 0.5f;


    [NonSerialized] protected bool m_ballStopped = false;
    [NonSerialized] protected bool m_touchedGround = false;
    [NonSerialized] protected bool m_grounded = false;

    [NonSerialized] protected Rigidbody m_rigidbody;

    [NonSerialized] private BasePetanquePlayer m_ballOwner;
    
    [NonSerialized] private bool m_alreadyTouchedTheGround = false;
    [NonSerialized] private bool m_ballSoundStopped = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == this.gameObject.layer)
        {
            if (m_touchedGround)
            {
                SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.ball);
            }
            else
            {
                SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.ballAir);
            }
        }
        
        if (collision.gameObject.layer == m_groundLayer)
        {
            //if not this, the call is made twice
            if (!m_alreadyTouchedTheGround)
            {
                SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.ground);
                m_alreadyTouchedTheGround = true;
            }
            OnGroundTouched();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (m_ballStopped)
        {
            return;
        }
        
        if (m_rigidbody.velocity.magnitude < m_stopSoundThreshold && !m_ballSoundStopped && m_timerToStop.IsRunning)
        {
            SoundManager.Instance.StopBallRolling();
            m_ballSoundStopped = true;
        }

        if (m_touchedGround)
        {
            // Checks if the ball is slow enough for enough time
            if (m_rigidbody.velocity.sqrMagnitude < m_speedToStop * m_speedToStop)
            {
                if (!m_timerToStop.IsRunning)
                {
                    m_timerToStop.Start();
                }
                if (m_timerToStop.Update(Time.fixedDeltaTime))
                {
                    m_timerToStop.Stop();
                    StopBall();
                }
            }
            else
            {
                m_timerToStop.Stop();
            }
        }
    }

    protected virtual void OnGroundTouched()
    {
        m_grounded = true;
        m_touchedGround = true;
    }

    private void StopBall()
    {
        m_ballStopped = true;
        OnBallStopped?.Invoke(this);
    }

    public virtual void ResetBall()
    {
        m_touchedGround = false;
        m_ballStopped = false;
        m_ballSoundStopped = false;
    }

    #region IPoolOperationCallbackReciever Implementation
    public void OnObjectReleased()
    {
        ResetBall();
    }

    public void OnObjectRequested()
    {
        ResetBall();
        m_rigidbody ??= GetComponent<Rigidbody>();
        m_rigidbody.maxAngularVelocity = 100000;
    }
    #endregion
}
