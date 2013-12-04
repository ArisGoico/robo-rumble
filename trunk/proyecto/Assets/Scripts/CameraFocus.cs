using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public int cameraMode = 0;
	public float focusSpeed = 5.0f;
	public float minFocusDis = 5.0f;
	public float maxFocusDis = 15.0f;

	private Vector3 initialPos;
	private Vector3 medianPoint;
	private Quaternion initialRot;
	private Quaternion rotation;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
		initialRot = transform.rotation;
	
	}
	
	// Update is called once per frame
	void Update () {
		switch (cameraMode) {
			case 1:
				//camara distancia
				medianPoint = (player1.transform.position + player2.transform.position)/2.0f;
				transform.position = Vector3.Lerp(transform.position, initialPos + medianPoint, Time.deltaTime * focusSpeed);
				transform.camera.orthographicSize = Mathf.Lerp (minFocusDis, maxFocusDis, Vector3.Distance(player1.transform.position, player2.transform.position)/10.0f );
				break;
			case 2:
				//camara rotante
				medianPoint = (player1.transform.position + player2.transform.position)/2.0f;
				rotation = Quaternion.LookRotation(medianPoint - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * focusSpeed);
			break;
			case 3:
				//combi
				medianPoint = (player1.transform.position + player2.transform.position)/2.0f;
				transform.position = Vector3.Lerp(transform.position, initialPos + medianPoint, Time.deltaTime * focusSpeed);
				transform.camera.orthographicSize = Mathf.Lerp (minFocusDis, maxFocusDis, Vector3.Distance(player1.transform.position, player2.transform.position)/10.0f );
				rotation = Quaternion.LookRotation(medianPoint - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * focusSpeed);
			break;
			default:
				//classic
				transform.rotation = initialRot;
				transform.position = initialPos;														
			break;
		}
	}
}
