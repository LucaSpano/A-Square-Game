using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
	public float stereoWith = 1f;

	Queue<AudioSource> sources = new Queue<AudioSource>();

	public static SoundManager instance;
	
	void Awake ()
	{
		instance = this;
	}
	

	public void Play (AudioClip clip, Vector3 pos, float pitch = 1f, float volume = 1f)
	{
		Play (clip, Camera.main.WorldToViewportPoint (pos).x * stereoWith * 2 - stereoWith, pitch, volume);
	}

	public void Play (AudioClip clip)
	{
		Play (clip, 0f);
	}

	public void Play (AudioClip clip, float pan, float pitch = 1f, float volume = 1f)
	{
		if (!clip) {
			Debug.LogWarning("Missing clip");
			return;
		}
		
		var source = GetSource ();
		source.clip = clip;
		source.volume = volume;
		source.Play ();
		source.panStereo = pan;
		source.pitch = pitch;
		//source.loop = loop;
		source.name = clip.name + " (Playing)";
		StartCoroutine ("OnStop", source);
		sources.Enqueue (source);
	}

	IEnumerator OnStop (AudioSource audioSource)
	{
		yield return new WaitForSeconds (audioSource.clip.length);
		audioSource.name = "Audio (Ready)";

	}

	AudioSource GetSource ()
	{
		// Trouver une source
		for (int i = 0; i < sources.Count; i++)
		{
			var activeSource = sources.Dequeue ();
			if(!activeSource.isPlaying) {
				return activeSource;
			}
			else
			{
				sources.Enqueue (activeSource);
			}
		}

		// Si rien n'est trouvé, on crée une nouvelle source
		return NewSource ();
	}


	AudioSource NewSource ()
	{
		GameObject obj = new GameObject ("Audio");
		obj.transform.parent = transform;
		var newSource = obj.AddComponent <AudioSource> ();
		return newSource;
	}

}
