using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public int cameraMode = 0;
	public float focusSpeed = 5.0f;
	public float minFocusDis = 5.0f;
	public float maxFocusDis = 15.0f;
	public float minFov = 15.0f;
	public float maxFov = 60.0f;

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
				//cemara ortho
				Camera.main.orthographic = true;
				medianPoint = (player1.transform.position + player2.transform.position)/2.0f;
				transform.position = Vector3.Lerp(transform.position, initialPos + medianPoint, Time.deltaTime * focusSpeed);
				transform.camera.orthographicSize = Mathf.Lerp (minFocusDis, maxFocusDis, Hermit(Vector3.Distance(player1.transform.position, player2.transform.position)/10.0f ));
				break;
			case 2:
				//camera persp
				Camera.main.orthographic = false;
				medianPoint = (player1.transform.position + player2.transform.position)/2.0f;
				transform.position = Vector3.Lerp(transform.position, initialPos + medianPoint, Time.deltaTime * focusSpeed);
				transform.camera.fieldOfView = Mathf.Lerp (minFov, maxFov, Hermit(Vector3.Distance(player1.transform.position, player2.transform.position)/10.0f ));
			break;
			default:
				//classic
				transform.rotation = initialRot;
				transform.position = initialPos;														
			break;
		}
	}

	float Hermit(float t)
	{
		return 3 * t - 2*t;
	}
}
