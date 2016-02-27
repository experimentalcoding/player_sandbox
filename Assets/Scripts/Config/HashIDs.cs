using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour
{
    [HideInInspector] public int angularSpeedFloat;
    [HideInInspector] public int speedFloat;
    [HideInInspector] public int attackingBool;

    void Awake()
    {
        angularSpeedFloat = Animator.StringToHash("AngularSpeed");
        speedFloat = Animator.StringToHash("Speed");
        attackingBool = Animator.StringToHash("Attacking");
    }
}