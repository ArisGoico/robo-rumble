using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	//debug variables
	public bool debug = false;
	public GUIText inputCapture;

	//player
	public int player = 1;
	public GameObject torso;
	private HullLogic Energy;

	//enemy player
	public GameObject enemy;

	//targeting mode
	public bool snapTarget 					= false;
	public float targetSpeed 				= 0.5f;
	private float effectiveTargetSpeed 		= 0.1f;
	private float blocking 					= 0f;
	private float snapping 					= 0f;
	private float lockTime 					= 0.25f;
	private bool lockOn 					= false;
	private bool lockingTarget				= false;
	public GUIText lockOnGUIText;
	public GUIText lockModeGUIText;

	//movement variables
	public float speed;
	public float dashSpeed 					= 1.0f;
	public float dashDelay 					= 1.0f;
	public float idleDrag 					= 5.0f;
	public float hoverDrag 					= 0.75f;
	public float hoverConsume 				= 1f;
	public float dashConsume 				= 5f;
		
	//delay and activation variables
	private bool hovering = false;
	private bool dashing = false;

	// Use this for initialization
	void Start () {
		//idea para el futuro: rigidbody.mass = suma de masa de los componentes, mass influye en los rigidbody addforce.	
		Energy = torso.GetComponent<HullLogic> ();
	}
	
	// Update is called once per frame
	void Update () {

		blocking = torso.GetComponent<Attack> ().blocking ? 2f : 0.1f;
		snapping = snapTarget ? 2f : 0.1f;

		effectiveTargetSpeed = targetSpeed / (blocking + snapping);

		if (Input.GetAxis("Horizontal"+player)!= 0 ||  Input.GetAxis("Vertical"+player)!= 0 ||  Input.GetAxis("hover"+player)!= 0  || dashing) {
			//hovering 
			Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"+player)*speed,0, Input.GetAxis("Vertical"+player)*speed);
			if (moveDir.Equals (Vector3.zero)) {
				moveDir = this.transform.forward*speed;
			}
			if (debug) {
				Debug.DrawRay(transform.position, transform.forward, Color.red);
				Debug.DrawRay(transform.position, moveDir, Color.green);
			}
			transform.rigidbody.AddForce(moveDir);
			torso.transform.rigidbody.AddForce(moveDir);
			hovering = (Energy.energyCurrent - dashConsume > 0) && Input.GetAxisRaw ("hover"+player) == 1 && !dashing;

			if (hovering) {
				//animation.CrossFade ("hover");
				Energy.consumeEnergy(hoverConsume);
				transform.rigidbody.drag = hoverDrag;
				inputCapture.text = "hovering";
			} else {
				if (dashing){
					//animation.CrossFade ("dashing");
					inputCapture.text = "dashing";
				} else {
					//animation.CrossFade("walking");
					inputCapture.text = "walking";
				}
				transform.rigidbody.drag = idleDrag;
			}
		} else { 
			inputCapture.text = "idle";
			//animation.CrossFade("idle");
		}

		//dashing
		if (Energy.energyCurrent - dashConsume > 0 && Input.GetAxisRaw ("dash"+player) == 1 && !dashing) {
			dashing = true;
			Energy.consumeEnergy(dashConsume);
			//genera movimiento casi instantaneo en la direccion pulsada o hacia adelante en caso de no indicar direccion)
			transform.rigidbody.drag = idleDrag;
			Vector3 dashDir = (new Vector3(Input.GetAxis("Horizontal"+player),0, Input.GetAxis("Vertical"+player))).normalized;
			if (dashDir.Equals (Vector3.zero)) {
				dashDir = this.transform.forward;
			}
			//aplicar la fuerza
			if (debug) Debug.DrawRay(transform.position, dashDir*5.0f, Color.yellow);
			transform.rigidbody.AddForce(dashDir*dashSpeed, ForceMode.Impulse);
			torso.transform.LookAt (transform.position + transform.forward);
			torso.transform.rigidbody.AddForce(dashDir*dashSpeed, ForceMode.Impulse);
			inputCapture.text = "dash";
			//dashing = false;
			StartCoroutine(waitForDash(dashDelay));
		} 
		

		//targeting
		if (snapTarget){
			if (Input.GetAxisRaw("lockOn"+player) == 1 && !lockingTarget) {
				lockingTarget = true;
				lockOn = !lockOn;
				StartCoroutine(waitForLock(lockTime));
			}
			if (lockOn) {
				lockOnGUIText.text = "Target Set";
				//Look at and dampen the rotation
				Quaternion rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * effectiveTargetSpeed);
			} else { lockOnGUIText.text = "Select Target"; }
			lockModeGUIText.text = "Locking mode: Auto";
		} else { 
			if (Input.GetAxisRaw("lockOn"+player) == 1) {
				lockOnGUIText.text = "Target Set";
				//Look at and dampen the rotation
				Quaternion rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * effectiveTargetSpeed);
			} else { lockOnGUIText.text = "Select Target"; }
			lockModeGUIText.text = "Locking mode: Manual";
		}
		//changing LockMode
		if (Input.GetAxisRaw("lockMode"+player) == 1 && !lockingTarget) {
			lockingTarget = true;
			snapTarget = !snapTarget;
			lockOn = snapTarget;
			StartCoroutine(waitForLock(lockTime));
		}
	}
	
	IEnumerator waitForDash (float time) {
		yield return new WaitForSeconds(time);
		dashing = false;
	}

	IEnumerator waitForLock (float time) {
		yield return new WaitForSeconds(time);
		lockingTarget = false;
	}
}
