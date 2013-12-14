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
	private AudioSource Audio;
	public float audioVolumeMin				= 0.2f;

	//enemy player
	public GameObject enemy;

	//targeting mode
	public float targetSpeed 				= 0.5f;
	private float effectiveTargetSpeed 		= 0.1f;
	private float blocking 					= 0f;
	private float lockTime 					= 0.25f;
	private bool lockOn 					= false;
	private bool lockingTarget				= false;
	public GUIText lockOnGUIText;

	//movement variables
	public float speed;
	public float dashSpeed 					= 1.0f;
	public float dashDelay 					= 1.0f;
	public float idleDrag 					= 5.0f;
	public float hoverDrag 					= 0.75f;
	public float hoverConsume 				= 1f;
	public float dashConsume 				= 5f;
	private Vector3 moveDir;
	private Vector3 lookDir;
	public GameObject[] flares;
		
	//delay and activation variables
	private bool hovering = false;
	private bool dashing = false;

	// Use this for initialization
	void Start () {
		//idea para el futuro: rigidbody.mass = suma de masa de los componentes, mass influye en los rigidbody addforce.	
		Energy = torso.GetComponent<HullLogic> ();
		Audio = this.GetComponent<AudioSource>();
		Audio.volume = audioVolumeMin;
	}
	
	// Update is called once per frame
	void Update () {

		lookDir = new Vector3(Input.GetAxis("lookH"+player),0, Input.GetAxis("lookV"+player));

		if (Input.GetAxis("Horizontal"+player)!= 0 ||  Input.GetAxis("Vertical"+player)!= 0 ||  Input.GetAxis("hover"+player)!= 0  || dashing) {
			//hovering 
			moveDir = new Vector3(Input.GetAxis("Horizontal"+player)*speed,0, Input.GetAxis("Vertical"+player)*speed);
			//lookDir = new Vector3(Input.GetAxis("lookH"+player),0, Input.GetAxis("lookV"+player));
			if (moveDir.Equals (Vector3.zero)) {
				moveDir = this.transform.forward*speed;
			}
			if (debug) {
				Debug.DrawRay(transform.position, transform.forward, Color.red);
				Debug.DrawRay(transform.position, lookDir*3, Color.yellow);
				Debug.DrawRay(transform.position, moveDir, Color.green);
			}
			transform.rigidbody.AddForce(moveDir);
			torso.transform.rigidbody.AddForce(moveDir);
			hovering = (Energy.energyCurrent - dashConsume > 0) && Input.GetAxisRaw ("hover"+player) == 1 && !dashing;

			if (hovering) {
				//Efectos

				Energy.consumeEnergy(hoverConsume);
				transform.rigidbody.drag = hoverDrag;
				//TODO: Vo-la-re! 
				transform.rigidbody.AddForce (98f*transform.up);
				inputCapture.text = "hovering";
				Audio.pitch = 2.5f;
			} else {
				if (dashing){
					inputCapture.text = "dashing";
				} else {
					inputCapture.text = "walking";
				}
				transform.rigidbody.drag = idleDrag;
				Audio.pitch = 1.0f;
			}
		} else { 
			inputCapture.text = "idle";
			moveDir = Vector3.zero;
			//TODO: animation.CrossFade("idle");
		}

		for (int i = 0; i < flares.Length; i++) {
			flares[i].transform.localScale = Vector3.Lerp (flares[i].transform.localScale, 
			                                               new Vector3(Input.GetAxis("hover"+player), Input.GetAxis("hover"+player), Input.GetAxis("hover"+player)),
			                                               Time.deltaTime*20f);
		}
		
		
		if (lookDir.Equals (Vector3.zero)) {
			if (moveDir.Equals (Vector3.zero)) {
				lookDir = transform.forward;
			} else {
				lookDir = moveDir;
			}
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
			torso.transform.rigidbody.AddForce(dashDir*dashSpeed*torso.transform.rigidbody.mass/transform.rigidbody.mass, ForceMode.Impulse);
			inputCapture.text = "dash";
			//dashing = false;
			StartCoroutine(waitForDash(dashDelay));
		} 
		

		//targeting
		blocking = torso.GetComponent<Attack> ().getBlocking() ? 2f : 0.1f;
		effectiveTargetSpeed = targetSpeed / blocking;

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
		} else { 
			lockOnGUIText.text = "Select Target";
			Quaternion rotation = Quaternion.LookRotation(lookDir);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * effectiveTargetSpeed * 2f);
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

	public Vector3 getMoveDir() {
		return moveDir;
	}
	public bool getDashing() {
		return dashing;
	}
}
