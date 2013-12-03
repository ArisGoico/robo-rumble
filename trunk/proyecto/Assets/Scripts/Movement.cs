using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	//debug variables
	public bool debug = false;
	public GUIText inputCapture;

	//player
	public int player = 1;
	public GameObject torso;

	//enemy player
	public GameObject enemy;

	//targeting mode
	public bool snapTarget = false;
	public float targetSpeed = 0.5f;
	private bool lockOn = false;

	//movement variables
	public float speed;
	public float dashSpeed = 1.0f;
	public float dashDelay = 1.0f;
	public float idleDrag = 5.0f;
	public float hoverDrag = 0.75f;
		
	//delay and activation variables
	private bool hovering = false;
	private bool dashing = false;


	// Use this for initialization
	void Start () {
		//idea para el futuro: rigidbody.mass = suma de masa de los componentes, mass influye en los rigidbody addforce.
	
	}
	
	// Update is called once per frame
	void Update () {


		//hovering 
		Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"+player)*speed,0, Input.GetAxis("Vertical"+player)*speed);
		if (debug) {
			Debug.DrawRay(transform.position, transform.forward, Color.red);
			Debug.DrawRay(transform.position, moveDir, Color.green);
		}
		transform.rigidbody.AddForce(moveDir);
		hovering = Input.GetAxisRaw ("hover"+player) == 1 && !dashing;

		if (hovering) {
			transform.rigidbody.drag = hoverDrag;
			inputCapture.text = "hovering";
		} else { 
			transform.rigidbody.drag = idleDrag;
			inputCapture.text = "walking";
		}

		//dashing
		if (Input.GetAxisRaw ("dash"+player) == 1 && !dashing) {
			dashing = true;
			//genera movimiento casi instantaneo en la direccion pulsada o hacia adelante en caso de no indicar direccion)
			transform.rigidbody.drag = idleDrag;
			Vector3 dashDir = (new Vector3(Input.GetAxis("Horizontal"+player),0, Input.GetAxis("Vertical"+player))).normalized;
			if (dashDir.Equals (Vector3.zero)) {
				//TODO: revisar, puede que el forward del transform no sea el punto donde mira el robot. Seria torso.transform.forward;
				dashDir = this.transform.forward;
			}
			//aplicar la fuerza
			if (debug) Debug.DrawRay(transform.position, dashDir*5.0f, Color.yellow);
			transform.rigidbody.AddForce(dashDir*dashSpeed, ForceMode.Impulse);
			inputCapture.text = "dash";
			//dashing = false;
			StartCoroutine(waitForDash(dashDelay));
		} 

		//targeting
		if (snapTarget){
			if (Input.GetAxisRaw("lockOn"+player) == 1) {
				lockOn = true;
			}
			else { 
				lockOn = false;
			}
			if (lockOn) {
				//Look at and dampen the rotation
				Quaternion rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * targetSpeed);
			}
		} else { 
			if (Input.GetAxisRaw("lockOn"+player) == 1) {
				//Look at and dampen the rotation
				Quaternion rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * targetSpeed);
			}
		}		
	}
	
	IEnumerator waitForDash (float time) {
		yield return new WaitForSeconds(time);
		dashing = false;
	}
}
