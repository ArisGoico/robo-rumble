using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	public 	bool 				debug 				= false;
	private int 				player 				= 1;
	private GameObject 			torso;
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

	//joints
	private ConfigurableJoint 	leftCJ;
	private ConfigurableJoint 	rightCJ;
	private SoftJointLimit 		jointRelaxed;
	private SoftJointLimit 		jointConstrained;
	private SoftJointLimit 		jointBlockingLimit;
	public 	float 				relaxedLimit 		= 0.5f;
	public 	float 				constrainedLimit 	= 0f;

	// Use this for initialization
	void Start () {
		player = transform.parent.GetComponent<Movement> ().player;
		torso = transform.parent.GetComponent<Movement> ().torso;
		Energy = transform.GetComponent<HullLogic> ();
		leftCJ = leftArm.GetComponent<ConfigurableJoint>();
		rightCJ = rightArm.GetComponent<ConfigurableJoint>();	

		jointRelaxed = new SoftJointLimit();
		jointConstrained = new SoftJointLimit();
		jointBlockingLimit = new SoftJointLimit ();

		jointRelaxed.limit = relaxedLimit;
		jointConstrained.limit = constrainedLimit;
		jointBlockingLimit.limit = 90f;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 lArmDirection = transform.position + transform.forward - leftArm.transform.position;
		Vector3 rArmDirection = transform.position + transform.forward - rightArm.transform.position;

		if (debug) { 
			Debug.DrawLine (transform.position, transform.position + transform.forward, Color.blue);
			Debug.DrawLine (leftArm.transform.position, transform.position + transform.forward);
			Debug.DrawLine (rightArm.transform.position, transform.position + transform.forward);
			Debug.DrawLine (leftArm.transform.position, (leftArm.transform.position - leftArm.transform.up* 0.5f + leftArm.transform.right* 0.4f ) , Color.red);
			Debug.DrawLine (rightArm.transform.position, (rightArm.transform.position - rightArm.transform.up* 0.5f - rightArm.transform.right* 0.4f ) , Color.red);

		}


		//blocking
		if (!punchingL && !punchingR && /*Input.GetAxisRaw ("punchL" + player) > 0 && Input.GetAxisRaw ("punchR" + player)*/Input.GetAxisRaw("lockMode"+player) > 0) {
			if (debug) { Debug.Log ("Bloqueando" + Time.deltaTime); }
			blocking = true;

			//relax joints
			setJointforBlock(leftCJ, true);
			setJointforBlock(rightCJ, true);

			//rotate arm vertically
			leftArm.transform.rotation = Quaternion.Slerp(leftArm.transform.rotation, /*Quaternion.AngleAxis(90, transform.right) */ Quaternion.AngleAxis(90, transform.up), Time.deltaTime * 10f);
			rightArm.transform.rotation = Quaternion.Slerp(leftArm.transform.rotation, /*Quaternion.AngleAxis(90, transform.right) */ Quaternion.AngleAxis(90, transform.up), Time.deltaTime * 10f);

			/*leftArm.transform.position = Vector3.Lerp (	leftArm.transform.position, 
			                                           leftArm.transform.position + leftArm.transform.up* 0.5f - rightArm.transform.right* 0.4f,
			                                           Time.deltaTime * 10f);
			rightArm.transform.position = Vector3.Lerp ( rightArm.transform.position, 
			                                            rightArm.transform.position + rightArm.transform.up* 0.5f - rightArm.transform.right* 0.4f, 
			                                           Time.deltaTime * 10f);*/
		} else if (blocking) {

			//constrain joints
			setJointforBlock(leftCJ, false);
			setJointforBlock(rightCJ, false);
			StartCoroutine(waitBlock(punchDelay));
		}

		//punching
		if (Energy.energyCurrent - punchConsume > 0 && Input.GetAxisRaw ("punchL" + player) > 0 && !punchingL && !blocking) {
			punchingL = true;
			Energy.consumeEnergy (punchConsume);
			if (debug)
				Debug.Log ("pega con la iqda");
			leftArm.transform.LookAt (transform.position + transform.forward);
			leftCJ.linearLimit = jointRelaxed;  
			leftCJ.angularYMotion = ConfigurableJointMotion.Limited;
			leftArm.rigidbody.AddForce (lArmDirection * force, ForceMode.Impulse);
			StartCoroutine(waitPunchL(punchDelay));
		} 

		if (Energy.energyCurrent - punchConsume > 0 && Input.GetAxisRaw ("punchR" + player) > 0 && !punchingR && !blocking) {
			punchingR = true;
			Energy.consumeEnergy (punchConsume);
			rightArm.transform.LookAt (transform.position + transform.forward);
			if (debug)
				Debug.Log ("pega con la dcha");
			rightCJ.linearLimit = jointRelaxed;
			rightCJ.angularYMotion = ConfigurableJointMotion.Limited;
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
		rightCJ.angularYMotion = ConfigurableJointMotion.Locked;

	}

	private IEnumerator waitPunchL(float time) {
		yield return new WaitForSeconds(time);
		if (debug)
			Debug.Log ("deja de pegarme con la izqda");
		punchingL = false;
		leftCJ.linearLimit = jointConstrained;
		leftCJ.angularYMotion = ConfigurableJointMotion.Locked;
	}

	private IEnumerator waitBlock(float time) {
		yield return new WaitForSeconds(time);
		if (debug)
			Debug.Log ("deja de bloquear");
		blocking = false;
	}

	private void setJointforBlock(ConfigurableJoint joint, bool block){
		if (block) {
			//relax joint
			//joint.linearLimit = jointRelaxed;
			joint.xMotion = ConfigurableJointMotion.Limited;
			joint.yMotion = ConfigurableJointMotion.Locked;
			joint.zMotion = ConfigurableJointMotion.Limited;
			joint.angularXMotion = ConfigurableJointMotion.Limited;
			joint.angularYMotion = ConfigurableJointMotion.Limited;
			joint.highAngularXLimit = jointBlockingLimit;
			joint.angularYLimit = jointBlockingLimit;
		} else {
			//constrain joint
			joint.linearLimit = jointConstrained;
			joint.xMotion = ConfigurableJointMotion.Locked;
			joint.yMotion = ConfigurableJointMotion.Limited;
			joint.zMotion = ConfigurableJointMotion.Locked;
			joint.angularXMotion = ConfigurableJointMotion.Locked;
			joint.angularYMotion = ConfigurableJointMotion.Locked;
			joint.highAngularXLimit = jointConstrained;
			joint.angularYLimit = jointConstrained;
		}
	}

}
