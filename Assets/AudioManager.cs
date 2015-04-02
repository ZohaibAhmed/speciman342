using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void PlayAudio(AudioClip sound,  float volumeScale = 1.0F){
		volumeScale = Mathf.Clamp(volumeScale, 0f, 1f);
		audio.PlayOneShot(sound, volumeScale);
	}
}
