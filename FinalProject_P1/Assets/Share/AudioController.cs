using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {

    [SerializeField] AudioClip[] clips;
    [SerializeField] float defaultDelayBetweenClips;

    AudioSource source;
    bool canPlay;
    AudioClip clip;
    
	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        canPlay = true;
	}
	
	public void Play (float delayBetweenClips) {
        if (!canPlay)
            return;
        canPlay = false;
        GameManager.GetInstance().GetTimer().add(() => { canPlay = true; }, delayBetweenClips);
        clip = clips[Random.Range(0, clips.Length)];
        source.PlayOneShot(clip);
    }

    public void Play()
    {
        if (!canPlay)
            return;
        canPlay = false;
        GameManager.GetInstance().GetTimer().add(() => { canPlay = true; }, defaultDelayBetweenClips);
        clip = clips[Random.Range(0, clips.Length)];
        source.PlayOneShot(clip);
    }
}
