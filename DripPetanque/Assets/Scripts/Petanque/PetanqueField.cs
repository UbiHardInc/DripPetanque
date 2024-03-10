using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetanqueField : MonoBehaviour
{
    public Transform JackPosition => m_jackPosition;

    [SerializeField] private Transform m_jackPosition;
}
