using UnityEngine;
using System.Collections;

public class gameOverScreen : MonoBehaviour {

	public float StartAlpha = 1.0f; // The transparency value to start fading from
	public float EndAlpha = 0.0f; // The transparency value to end fading at
	public float FadingSpeed = 1.0f; // The speed of the effect


	private float Timer = 0.0f; // The time passed since fading was enabled


	void Awake() {

	}

	void Update() {

	}

	public void FadeToBlack() {

		// TODO: maybe fade to black here.

	}
}
