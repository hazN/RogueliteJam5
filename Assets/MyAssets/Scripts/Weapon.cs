using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector2 damage;
    BoxCollider hitbox;
    public List<AudioClip> audioClips;
    [SerializeField][Range(0f, 2f)] public float volume = 1f;
    private bool isPlayer = false;
    public List<AttackSO> combo;
    List<Collider> targetsHit;
    void Start()
    {
        targetsHit = new List<Collider>();
        hitbox = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Make sure hitbox is enabled
        if (!hitbox.enabled)
            return;

        // Check if its a player or enemy
        bool isPlayer = transform.root.CompareTag("Player");
        bool isEnemy = gameObject.layer == 9;
        if (isPlayer && other.CompareTag("Enemy") && !targetsHit.Contains(other))
        {
            // Deal Damage
            float finalDamage = Random.Range(damage.x, damage.y);
            other.GetComponent<Health>().TakeDamage(finalDamage);
            targetsHit.Add(other);
        }
        else if (isEnemy && other.CompareTag("Player") && !targetsHit.Contains(other))
        {
            // Deal Damage
            float finalDamage = Random.Range(damage.x, damage.y);
            other.GetComponent<Health>().TakeDamage(finalDamage);
            // Add to list to avoid hitting the same target multiple times in one swing
            targetsHit.Add(other);
        }
    }

    public void EnableHitbox()
    {
        targetsHit.Clear();
        hitbox.enabled = true;
    }
    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }
}
 