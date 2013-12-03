using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.rigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal")*speed,0, Input.GetAxis("Vertical")*speed));
	
	}
}
