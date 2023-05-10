using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    [SerializeField] private AudioSource[] tracks;
    [SerializeField][Range(0f, 2f)] private float volume = 1f;
    private int currentTrack = 0;
    private bool isPaused = false;

    void Start()
    {
        NextTrack();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if all tracks are done playing
        bool allDone = true;
        foreach (AudioSource track in tracks)
        {
            if (track.isPlaying)
            {
                allDone = false;
                break;
            }
        }
        // Goto next track if all are done
        if (allDone && !isPaused)
        {
            NextTrack();
        }
    }

    public void NextTrack()
    {
        // Make sure current track is stopped
        tracks[currentTrack].Stop();
        // Randomize next track until its a new one
        int lastTrack = currentTrack;
        while (lastTrack == currentTrack)
            currentTrack = Random.Range(0, tracks.Length - 1);
        // Play next track
        tracks[currentTrack].volume = volume;
        tracks[currentTrack].Play();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            // Resume game
            Time.timeScale = 1f;
            isPaused = false;
        }
        else
        {
            // Pause game
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
}
