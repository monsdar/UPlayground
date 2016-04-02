using UnityEngine;
using System.Collections;

public class InitDistanceMarker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TextMesh textObj = GetComponentInChildren<TextMesh>();
        textObj.text = transform.position.x.ToString("0") + "m";
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
