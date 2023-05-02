using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector2 damage;
    BoxCollider hitbox;
    public List<AudioClip> audioClips;
    [SerializeField][Range(0f, 2f)] public float volume = 1f;
    private bool isPlayer = false;
    public List<AttackSO> combo;

    void Start()
    {
        hitbox = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        isPlayer = transform.root.CompareTag("Player") ? true : false;
        if (isPlayer)
        {
            if (other.CompareTag("Enemy"))
            {
                // Deal Damage      
                float finalDamage = Random.Range(damage.x, damage.y);
                other.GetComponent<Health>().TakeDamage(finalDamage);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                // Deal Damage      
                float finalDamage = Random.Range(damage.x, damage.y);
                other.GetComponent<Health>().TakeDamage(finalDamage);
            }
        }
    }
    public void EnableHitbox()
    {
        hitbox.enabled = true;
    }
    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }
}
 