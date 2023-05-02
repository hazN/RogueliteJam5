using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combo;
    float lastClickTime;
    float lastComboEnd;
    int comboIndex;

    Animator anim;
    [SerializeField] GameObject WeaponHolster;
    Weapon weapon;
    void Start()
    {
        anim = GetComponent<Animator>();
        // Get weapon script reference
        weapon = WeaponHolster.GetComponentInChildren<Weapon>();
        if (weapon!= null)
        {
            weapon.gameObject.tag = "CinemachineTarget";
            weapon.gameObject.layer = 8;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
        ExitAttack();
    }
    void Attack()
    {
        weapon = WeaponHolster.GetComponentInChildren<Weapon>();
        if (weapon == null)
            return;

        weapon.gameObject.tag = "CinemachineTarget";
        weapon.gameObject.layer = 8;
        if (Time.time - lastClickTime > 0.5f && comboIndex < combo.Count)
        {
            CancelInvoke("EndCombo");
            float animationLength = anim.runtimeAnimatorController.animationClips[0].length;
            // Check if its 70% complete
            if (Time.time - lastClickTime >= animationLength * 0.5f)
            {
                anim.runtimeAnimatorController = combo[comboIndex].animatorOV;
                anim.Play("Attack", 0, 0);
                comboIndex++;
                lastClickTime= Time.time;

                if ( comboIndex >= combo.Count)
                {
                    comboIndex = 0;
                }
            }
        }
    }
    void ExitAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }
    void EndCombo()
    {
        comboIndex = 0;
        lastComboEnd= Time.time;
    }

    private void OnAttack()
    {
        if (weapon != null && weapon.audioClips.Count > comboIndex)
        {
            AudioSource.PlayClipAtPoint(weapon.audioClips[comboIndex], transform.position);
        }
    }
    private void OnAttackStart()
    {
        weapon.EnableHitbox();
    }
    private void OnAttackEnd()
    {
        weapon.DisableHitbox();
    }
}
