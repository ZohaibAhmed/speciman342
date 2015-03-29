using UnityEngine;
using System.Collections;

public class WaterFountainTimer : MonoBehaviour {

	public float effectDuration = 10f;
	float totalTime;
	ParticleEmitter emitter;
	float originalY;

	// Use this for initialization
	void Start () {
	
	}

	void Awake(){
		emitter = GetComponent<ParticleEmitter>();
		originalY = emitter.localVelocity.y;
		totalTime = effectDuration;
	}
	
	// Update is called once per frame
	void Update () {
		effectDuration -= Time.deltaTime;
		emitter.localVelocity = new Vector3(0, originalY - Mathf.Lerp(originalY, 0, effectDuration / totalTime), 0);

		if (effectDuration <= 0){
			DestroyObject (gameObject);
		}
	}
}
