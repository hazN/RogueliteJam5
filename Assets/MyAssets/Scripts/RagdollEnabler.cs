using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RagdollEnabler : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Transform RagdollRoot;
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private bool StartRagdoll = false;
    private Rigidbody[] Rigidbodies;
    private CharacterJoint[] Joints;

    private void Start()
    {
        // Get all the ragdoll components
        Rigidbodies = GetComponentsInChildren<Rigidbody>().Where(rb => rb.transform.parent.name != "EnemyHolster").ToArray();
        Joints = RagdollRoot.GetComponentsInChildren<CharacterJoint>();
        if (StartRagdoll)
            EnableRagdoll();
        else
            EnableAnimator();
    }

    public void EnableRagdoll()
    {
        Animator.enabled = false;
        Agent.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Enemy>().enabled = false;
        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = true;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
        }
    }

    public void DisableAllRigidbodies()
    {
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.detectCollisions = false;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }
    }

    public void EnableAnimator()
    {
        Animator.enabled = true;
        Agent.enabled = true;
        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = false;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }
    }
}