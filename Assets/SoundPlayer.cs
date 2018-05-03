using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
	public AudioClip[] clips;
	public bool playOnAwake = true;
	public float volume = 1f;
	public float basePitch = 1f;
	public float pitchVariation = 0f;

	void Start ()
	{
		if (playOnAwake) {
			Play ();
		}
	}

	public void Play ()
	{
        AudioClip clip = clips[Random.Range(0, clips.Length)];
		var pitch = basePitch + (Random.Range (-pitchVariation, pitchVariation) + Random.Range (-pitchVariation, pitchVariation)) * 0.5f;
		SoundManager.instance.Play (clip, transform.position, pitch, volume);
	}
}
