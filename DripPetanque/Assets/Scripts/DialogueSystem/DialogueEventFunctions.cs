using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventFunctions : MonoBehaviour
{
    public void AddDoubleJump()
    {
        ThirdPersonController.s_maxJumps = 2;
    }
}
