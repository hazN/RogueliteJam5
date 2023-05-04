using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldOrb : MonoBehaviour
{
    [SerializeField] private int goldValue = 1;
    public List<AudioClip> audioClips;
    [SerializeField][Range(0f, 2f)] public float volume = 1f;
    private int index = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player)
            {
                player.addCoins(goldValue);
                index = (index + 1) % audioClips.Count;
                AudioSource.PlayClipAtPoint(audioClips[index], transform.position, volume);
                Destroy(gameObject);
            }
        }
    }
}
