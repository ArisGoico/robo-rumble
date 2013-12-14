using UnityEngine;
using System.Collections;

public class thrusters : MonoBehaviour {

	public GameObject movingObject;
	private Vector3 moveDir;
	public Transform[] models;
	private float power = 1f;
	private bool dashing;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		dashing = movingObject.GetComponent<Movement> ().getDashing ();
		power = dashing ? 2.5f : 1f; 
		moveDir = movingObject.GetComponent<Movement> ().getMoveDir ();
		for (int i = 0; i< models.Length; i++) {
			models [i].localScale = Vector3.Lerp (Vector3.zero, new Vector3(1f, power, 1f), Vector3.Dot (moveDir.normalized, models [i].transform.up) * moveDir.magnitude);
		}
	}
}
