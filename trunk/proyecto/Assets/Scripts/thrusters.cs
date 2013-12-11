using UnityEngine;
using System.Collections;

public class thrusters : MonoBehaviour {

	private int player = 1;
	public GameObject movingObject;
	private Vector3 moveDir;
	public Transform[] models;
	private float power = 1f;
	private bool dashing;


	// Use this for initialization
	void Start () {
		player = movingObject.GetComponent<Movement> ().player;
		
	}
	
	// Update is called once per frame
	void Update () {
		dashing = movingObject.GetComponent<Movement> ().getDashing ();
		power = dashing ? 2f : 1f; 
		moveDir = movingObject.GetComponent<Movement> ().getMoveDir ();
		for (int i = 0; i< models.Length; i++) {
			models [i].localScale = Vector3.Lerp (new Vector3(0.1f,0.1f,0.1f), new Vector3(1f, power, 1f), Vector3.Dot (moveDir.normalized, models [i].transform.localEulerAngles));
		}
	}
}
