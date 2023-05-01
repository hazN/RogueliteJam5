using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    Weapon weaponScript;

    public float aggroDistance = 10f;
    public float attackDistance = 1.1f;

    [SerializeField] private NavMeshAgent agent;
    private Animator anim;

    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    public AudioClip[] TakeDamageAudioClips;
    [Range(0, 1)] public float TakeDamageAudioVolume = 0.5f;
    public AudioClip[] AttackAudioClips;
    [Range(0, 1)] public float AttackAudioVolume = 0.5f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        weaponScript = GetComponentInChildren<Weapon>();
    }

    private void Update()
    {
        // Get speed and pass to animator
        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", speed);

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < attackDistance)
        {
            // Check if attack isn't already playing
            if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                anim.SetTrigger("attack");
            }
        }
        else if (distanceToPlayer < aggroDistance)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    private void OnFootstep()
    {
        if (FootstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
        }
    }

    public void TakeDamage()
    {
        if (TakeDamageAudioClips.Length > 0)
        {
            var index = Random.Range(0, TakeDamageAudioClips.Length);
            AudioSource.PlayClipAtPoint(TakeDamageAudioClips[index], transform.position, TakeDamageAudioVolume);
        }
    }
    
    private void OnAttack()
    {
        if (AttackAudioClips.Length > 0)
        {
            var index = Random.Range(0, AttackAudioClips.Length);
            AudioSource.PlayClipAtPoint(AttackAudioClips[index], transform.position, AttackAudioVolume);
        }
    }
    private void OnAttackStart()
    {
        weaponScript.EnableHitbox();
    }
    private void OnAttackEnd()
    {
        weaponScript.DisableHitbox();
    }
}