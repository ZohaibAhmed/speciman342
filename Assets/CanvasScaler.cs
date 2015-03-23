using UnityEngine;
using System.Collections;

public class CanvasScaler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void Awake(){
		RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
		RectTransform rectTrans = GetComponent<RectTransform>();
		Debug.Log(parentRect.sizeDelta);
		rectTrans.sizeDelta = new Vector2(parentRect.sizeDelta.x / 2.0f, parentRect.sizeDelta.y);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
