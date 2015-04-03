using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour {

	AudioSource audioSource;
	public AudioClip[] tracks;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = tracks [Random.Range (0, tracks.Length)];
		audioSource.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
