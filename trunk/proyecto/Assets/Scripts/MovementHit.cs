using UnityEngine;
using System.Collections;

public class MovementHit : MonoBehaviour {

	public float speed;
	public float force;
	public GameObject leftArm;
	public GameObject rightArm;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.rigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed));
		
		if (Input.GetKeyDown(KeyCode.Space)) {
			//TODO Bloqueo
		}
		
		if (Input.GetMouseButtonDown(0)) {
			//TODO Golpe con la izquierda
			leftArm.rigidbody.AddForce(Vector3.right * force, ForceMode.Impulse);
		}
		
		if (Input.GetMouseButtonDown(1)) {
			//TODO Golpe con la derecha
			rightArm.rigidbody.AddForce(Vector3.right * force, ForceMode.Impulse);
		}
	}
}
