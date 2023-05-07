using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class PlayerCombat : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStats
    {
        public int health;
        public int damage;
        public int speed;
        public int coins;
    }

    public PlayerStats stats;

    public List<AttackSO> combo;
    private float lastClickTime;
    private float lastComboEnd;
    private int comboIndex;
    private Animator anim;
    [SerializeField] private GameObject WeaponHolster;
    private Weapon weapon;
    [SerializeField] private RuntimeAnimatorController defaultController;

    private void Start()
    {
        LoadPlayerStats();
        GetComponentInChildren<Health>().SetHealth(stats.health);
        anim = GetComponent<Animator>();
        // Get weapon script reference
        weapon = WeaponHolster.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weapon.gameObject.tag = "CinemachineTarget";
            weapon.gameObject.layer = 8;
            combo = WeaponHolster.GetComponentInChildren<Weapon>().combo;
        }
        InvokeRepeating("SavePlayerStats", 30.0f, 30.0f);
    }

    private void Update()
    {
        weapon = WeaponHolster.GetComponentInChildren<Weapon>();
        if (weapon == null)
            anim.runtimeAnimatorController = defaultController;
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
        ExitAttack();
    }

    private void Attack()
    {
        if (weapon == null) return;
        combo = WeaponHolster.GetComponentInChildren<Weapon>().combo;
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
                lastClickTime = Time.time;
                if (comboIndex >= combo.Count)
                    comboIndex = 0;
            }
        }
    }

    private void ExitAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    private void EndCombo()
    {
        comboIndex = 0;
        lastComboEnd = Time.time;
    }

    private void OnAttack()
    {
        if (weapon != null && weapon.audioClips.Count > comboIndex)
        {
            AudioSource.PlayClipAtPoint(weapon.audioClips[comboIndex], transform.position, weapon.volume);
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

    public void SavePlayerStats()
    {
        string filePath = "/playerStats.json";
        string json = JsonUtility.ToJson(stats);
        File.WriteAllText(filePath, json);
        Debug.Log(json);
    }

    public void LoadPlayerStats()
    {
        string filePath = "/playerStats.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            stats = JsonUtility.FromJson<PlayerStats>(json);
            GetComponent<Player>().addCoins(stats.coins);
            Debug.Log(json);
        }
        if (stats.health < 100)
            stats.health = 100;
        if (stats.damage < 1)
            stats.damage = 1;
        if (stats.speed < 1)
            stats.speed = 1;
    }
}