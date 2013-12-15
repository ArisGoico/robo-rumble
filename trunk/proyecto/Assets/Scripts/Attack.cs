using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Attack : MonoBehaviour {

	public 	bool 				debug 				= false;
	private int 				player 				= 1;
	public 	float 				force;
	public 	GameObject 			leftArm;
	public 	GameObject 			rightArm;
	public 	GameObject 			leftShoulder;
	public 	GameObject 			rightShoulder;
	public 	float 				punchConsume 		= 5f;
	public 	float 				blockConsume 		= 0.3f;
	public 	float 				blockTickConsume 	= 1.5f;

	private HullLogic 			Energy;

	//XInput variables
	GamePadState state;
	GamePadState prevState;

	//delays
	private 	bool 				punchingR 			= false;
	private 	bool 				punchingL 			= false;
	private 	bool 				punchingRlow 		= false;
	private 	bool 				punchingLlow 		= false;
	private 	bool 				blocking 			= false;
	private 	float 				punchDelay 			= 0.5f;

	//joints
	private ConfigurableJoint 	leftCJ;
	private ConfigurableJoint 	rightCJ;
	private CharacterJoint		leftHJ;
	private	CharacterJoint		rightHJ;
	private SoftJointLimit 		jointRelaxed;
	private SoftJointLimit 		jointConstrained;
	private SoftJointLimit 		jointBlockingLimit;
	private SoftJointLimit 		jointBlockingLimitInv;
	public 	float 				relaxedLimit 		= 0.5f;
	private float	 			constrainedLimit 	= 0f;

	// Use this for initialization
	void Start () {
		player = transform.parent.GetComponent<Movement> ().player;
		Energy = transform.GetComponent<HullLogic> ();
		leftCJ = leftArm.GetComponent<ConfigurableJoint>();
		rightCJ = rightArm.GetComponent<ConfigurableJoint>();
		leftHJ = leftShoulder.GetComponent<CharacterJoint> ();
		rightHJ = rightShoulder.GetComponent<CharacterJoint> ();

		jointRelaxed = new SoftJointLimit();
		jointConstrained = new SoftJointLimit();
		jointBlockingLimit = new SoftJointLimit ();

		jointRelaxed.limit = relaxedLimit;
		jointConstrained.limit = constrainedLimit;
		jointBlockingLimit.limit = 90f;
		jointBlockingLimitInv.limit = -90f;
	}
	
	// Update is called once per frame
	void Update () {



		state = GamePad.GetState((PlayerIndex)(player-1));

		Vector3 lArmDirection = transform.position + transform.forward - leftArm.transform.position;
		Vector3 rArmDirection = transform.position + transform.forward - rightArm.transform.position;

		//blocking

		if (debug){
			Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);
			Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue);
			Debug.DrawLine(transform.position, transform.position + transform.right, Color.red);

			Debug.DrawLine(leftShoulder.transform.position, leftShoulder.transform.position + leftShoulder.transform.up*0.5f, Color.green);
			Debug.DrawLine(leftShoulder.transform.position, leftShoulder.transform.position + leftShoulder.transform.forward*0.5f, Color.blue);
			Debug.DrawLine(leftShoulder.transform.position, leftShoulder.transform.position + leftShoulder.transform.right*0.5f, Color.red);

			//where to look with the arm
			Debug.DrawLine(transform.position, leftShoulder.transform.position + transform.up*0.5f ,Color.cyan);

		}


		if (( Input.GetAxisRaw ("punchL" + player) > 0 || state.Triggers.Left > 0 ) && ( Input.GetAxisRaw ("punchR" + player) > 0 || state.Triggers.Right > 0) && !punchingLlow && !punchingRlow) {
						if (debug) {
								Debug.Log ("Bloqueando" + Time.deltaTime);
						}
						blocking = true;

						setJointforBlock (leftHJ, true, true);
						setJointforBlock (rightHJ, true, false);

						//por Quaternions
						//rightArm.rigidbody.AddForce(torso.transform.forward+torso.transform.right*2f);
						//Quaternion rotationL = Quaternion.AngleAxis (70, transform.forward) * Quaternion.AngleAxis (5, transform.right); // funciona.
						//Quaternion rotationR = Quaternion.AngleAxis (10, transform.position - rightArm.transform.localPosition + transform.right); // Quaternion.AngleAxis (90, rightArm.transform.forward);

						//leftShoulder.transform.localRotation = Quaternion.Slerp (leftShoulder.transform.rotation, rotationL* leftShoulder.transform.forward, Time.deltaTime*5f);

						//por lookAts
						rightShoulder.transform.LookAt (rightShoulder.transform.position + transform.up, transform.right);
						leftShoulder.transform.LookAt (leftShoulder.transform.position + transform.up, -transform.right);

				} else if (blocking) {
						StartCoroutine (waitBlock (punchDelay));
				} else { //if not blocking
						setJointforBlock (leftHJ, false, false);
						setJointforBlock (rightHJ, false, false);
				}

		//punching
		//XInput version
		//if (Energy.energyCurrent - punchConsume > 0 && state.Triggers.Left > 0 && !punchingL && !punchingRlow && !blocking) {
		if (Energy.energyCurrent - punchConsume > 0 && (Input.GetAxisRaw ("punchL" + player) > 0 || state.Triggers.Left > 0 ) && !punchingL && !punchingRlow && !blocking) {
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


		if (Energy.energyCurrent - punchConsume > 0 && (Input.GetAxisRaw ("punchR" + player) > 0 || state.Triggers.Right > 0) && !punchingR && !punchingLlow  && !blocking) {
		//Input version if (Energy.energyCurrent - punchConsume > 0 && Input.GetAxisRaw ("punchR" + player) > 0 && !punchingR && !punchingLlow  && !blocking) {
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

		prevState = state;
	}

	private IEnumerator waitPunchR(float time) {
		yield return new WaitForSeconds(time*0.2f);
		punchingRlow = false;
		if (debug)
			Debug.Log ("deja de pegarme con la derecha");

		rightCJ.linearLimit = jointConstrained;
		rightCJ.angularYMotion = ConfigurableJointMotion.Locked;
		yield return new WaitForSeconds(time*0.8f);
		punchingR = false;
	}

	private IEnumerator waitPunchL(float time) {
		yield return new WaitForSeconds(time*0.2f);
		punchingLlow = false;
		if (debug)
			Debug.Log ("deja de pegarme con la izqda");

		leftCJ.linearLimit = jointConstrained;
		leftCJ.angularYMotion = ConfigurableJointMotion.Locked;
		yield return new WaitForSeconds(time*0.8f);
		punchingL = false;
	}

	private IEnumerator waitBlock(float time) {
		yield return new WaitForSeconds(time*0.1f);
		//constrain joints
		setJointforBlock(leftHJ, false, false);
		setJointforBlock(rightHJ, false, false);
		if (debug)
			Debug.Log ("deja de bloquear");
		yield return new WaitForSeconds(time*0.8f);
		blocking = false;
	}

	private void setJointforBlock(CharacterJoint joint, bool block, bool left){
		if (block) {
			//relax joint
			joint.swing2Limit = jointBlockingLimit;

			if (left){
				joint.highTwistLimit = jointBlockingLimit;
				joint.lowTwistLimit = new SoftJointLimit();
			}
			else { 
				joint.lowTwistLimit = jointBlockingLimitInv;
				joint.highTwistLimit = new SoftJointLimit();
			}

		} else {
			//constrain joint
			joint.highTwistLimit = new SoftJointLimit();
			joint.lowTwistLimit = new SoftJointLimit();
			joint.swing2Limit = new SoftJointLimit();
		}
	}

	public bool getBlocking () {
		return blocking;
	}
}
