using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector2 damage;
    BoxCollider hitbox;
    public List<AudioClip> audioClips;

    void Start()
    {
        hitbox = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Deal Damage      
            float finalDamage = Random.Range(damage.x, damage.y);
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
 