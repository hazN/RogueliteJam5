using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal Attack")]
public class AttackSO : ScriptableObject
{
    public AnimatorOverrideController animatorOV;
    [SerializeField] private float SwingSpeed = 1.0f;

    public float getSwingSpeed()
    {
        return SwingSpeed;
    }
}
