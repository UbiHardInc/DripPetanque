using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] private float m_force = 10.0f;
    [SerializeField] private float m_floorHeight = 0.0f;

    [SerializeField] private Rigidbody m_ball = null;
    private bool m_shot;

    private List<Vector3> m_ballPos = new List<Vector3>();

    public void Shoot()
    {
        m_shot = true;
        m_ball.isKinematic = false;
        m_ball.transform.position = transform.position;
        m_ball.gameObject.SetActive(true);
        m_ball.AddForce(transform.up * m_force);
        m_ballPos.Clear();
    }

    private void FixedUpdate()
    {
        if (m_shot)
        {
            if (m_ball.position.y < m_floorHeight)
            {
                m_shot = false;
                m_ball.isKinematic = true;
                m_ball.position = Vector3.zero;
                m_ball.gameObject.SetActive(false);
                return;
            }
            m_ballPos.Add(m_ball.position);

        }
    }

    private void OnDrawGizmos()
    {
        if (m_ballPos != null)
        {
            Color c = Gizmos.color;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < m_ballPos.Count - 1; i++) 
            {
                Gizmos.DrawLine(m_ballPos[i], m_ballPos[i + 1]);
            }
            Gizmos.color = c;
        }
    }
}
