using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public float aggroDistance = 10f;
    public float attackDistance = 2f;
    [SerializeField] private NavMeshAgent agent;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Get speed and pass to animator
        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", speed);

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < aggroDistance)
        {
            agent.SetDestination(player.transform.position);
        }

        if (distanceToPlayer < attackDistance)
        {
            // Trigger attack animation
        }
    }

    void OnFootstep()
    {
        // Play footstep sound
    }
}
