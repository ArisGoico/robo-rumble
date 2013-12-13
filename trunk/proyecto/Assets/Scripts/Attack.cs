using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	public 	bool 				debug 				= false;
	private int 				player 				= 1;
	public 	float 				force;
	public 	GameObject 			leftArm;
	public 	GameObject 			rightArm;
	public 	float 				punchConsume 		= 5f;
	public 	float 				blockConsume 		= 0.3f;
	public 	float 				blockTickConsume 	= 1.5f;

	private HullLogic 			Energy;

	//delays
	public 	bool 				punchingR 			= false;
	public 	bool 				punchingL 			= false;
	public 	bool 				blocking 			= false;
	public 	float 				punchDelay 			= 0.5f;
	public 	float 				blockDelay 			= 0.5f;

	//joints
	private ConfigurableJoint 	leftCJ;
	private ConfigurableJoint 	rightCJ;
	private SoftJointLimit 		jointRelaxed;
	private SoftJointLimit 		jointConstrained;
	public 	float 				relaxedLimit 		= 0.5f;
	public 	float 				constrainedLimit 	= 0f;

	// Use this for initialization
	void Start () {
		player = transform.parent.GetComponent<Movement> ().player;
		Energy = transform.GetComponent<HullLogic> ();
		leftCJ = leftArm.GetComponent<ConfigurableJoint>();
		rightCJ = rightArm.GetComponent<ConfigurableJoint>();	

		jointRelaxed = new SoftJointLimit();
		jointConstrained = new SoftJointLimit();

		jointRelaxed.limit = relaxedLimit;
		jointConstrained.limit = constrainedLimit;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 lArmDirection = transform.position + transform.forward - leftArm.transform.position;
		Vector3 rArmDirection = transform.position + transform.forward - rightArm.transform.position;

		if (debug) { 
			Debug.DrawLine (transform.position, transform.position + transform.forward, Color.blue);
			Debug.DrawLine (leftArm.transform.position, transform.position + transform.forward);
			Debug.DrawLine (rightArm.transform.position, transform.position + transform.forward);
		}

		if (Input.GetAxisRaw ("punchL" + player) > 0 && Input.GetAxisRaw ("punchL" + player) > 0 && !blocking) {
			//TODO Bloqueo
			Debug.Log ("Bloqueando" + Time.deltaTime);
			blocking = true;
		} else if (!blocking) {
			StartCoroutine(waitBlock(0.5f));
		}

		if (Energy.energyCurrent - punchConsume > 0 && Input.GetAxisRaw ("punchL" + player) > 0 && !punchingL && !blocking) {
			punchingL = true;
			Energy.consumeEnergy (punchConsume);
			if (debug)
				Debug.Log ("pega con la iqda");
			leftArm.transform.LookAt (transform.position + transform.forward);
			leftCJ.linearLimit = jointRelaxed;  //con esto los brazos irian al cuerpo rapido si no esta pulsado, pero... delay y volver a linearLimit.limit= 0;
			leftArm.rigidbody.AddForce (lArmDirection * force, ForceMode.Impulse);
			StartCoroutine(waitPunchL(punchDelay));
		}
		
		if (Energy.energyCurrent - punchConsume > 0 && Input.GetAxisRaw ("punchR"+player) > 0 && !punchingR && !blocking) {
			punchingR = true;
			Energy.consumeEnergy (punchConsume);
			rightArm.transform.LookAt (transform.position + transform.forward);
			if (debug)
				Debug.Log ("pega con la dcha");
			rightCJ.linearLimit = jointRelaxed;
			rightArm.rigidbody.AddForce(rArmDirection*force, ForceMode.Impulse);
			StartCoroutine(waitPunchR(punchDelay));
		}
	}

	private IEnumerator waitPunchR(float time) {
		yield return new WaitForSeconds(time);
		if (debug)
			Debug.Log ("deja de pegarme con la derecha");
		punchingR = false;
		rightCJ.linearLimit = jointConstrained;
	}

	private IEnumerator waitPunchL(float time) {
		yield return new WaitForSeconds(time);
		if (debug)
			Debug.Log ("deja de pegarme con la izqda");
		punchingL = false;
		leftCJ.linearLimit = jointConstrained;
	}

	private IEnumerator waitBlock(float time) {
		yield return new WaitForSeconds(time);
		if (debug)
			Debug.Log ("deja de bloquear");
		blocking = false;
	}

}
