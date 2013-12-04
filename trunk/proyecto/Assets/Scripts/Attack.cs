using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	public int player = 1;
	public float force;
	public GameObject leftArm;
	public GameObject rightArm;
	private ConfigurableJoint leftCJ;
	private ConfigurableJoint rightCJ;

	// Use this for initialization
	void Start () {
		leftCJ = leftArm.GetComponent<ConfigurableJoint>();
		rightCJ = rightArm.GetComponent<ConfigurableJoint>();
	
	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetKeyDown(KeyCode.Space)) {
			//TODO Bloqueo
		}
		
		if (Input.GetAxisRaw ("punch"+player) >0) {
			//TODO Golpe con la izquierda
			leftArm.rigidbody.AddForce(leftArm.transform.forward*force);
		}
		
		if (Input.GetAxisRaw ("punch"+player) <0) {
			//TODO Golpe con la derecha
			rightArm.rigidbody.AddForce(transform.forward*force);
		}
	}
}
