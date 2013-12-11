using UnityEngine;
using System.Collections;

public class HullLogic : MonoBehaviour {

	public bool debug						= false;
	//Hull Integrity / Salud 
	public GameObject lifeBar;							//GUI Life graphic representation
	public int hullIntegrityCurrent			= 100;		//Current hull integrity / Current health
	public int hullIntegrityMax				= 100;		//Max hull integrity / Max health
	public float hullDensity				= 1f;		//Percentage of each hit damage that goes through (0 - 1)
	public float hullPlating				= 0f;		//Min damage that goes through
	private float totalMass					= 0f;		//The total mass of the robot
	
	//Energy
	public float energyCurrent				= 100f;		//Current energy
	public float energyMax					= 100f;		//Max energy
	public float energyRegenRate			= 10f;		//Energy regenerated per second
	public float energyRegenDelay			= 0f;		//Number of seconds (/10) to start regenerating
	
	private float energyLastUsed			= 0f;		//Last time energy was used.
	
	//GUI
	public GameObject energyBar;						//GUI Energy graphic representation
	public GUIText energyLabel;							//GUI Energy text representation

	//Hit Sounds
	public AudioClip[] torsoHitSFX;
	public AudioClip[] blockHitSFX;
	private AudioSource SFXAudio;
	
	void Start() {
		hullIntegrityCurrent = hullIntegrityMax;
		energyCurrent = energyMax;
		energyLabel.text = Mathf.FloorToInt(energyCurrent).ToString ();
		SFXAudio = this.GetComponent<AudioSource>();
		Rigidbody[] rigidBodies = this.transform.parent.GetComponentsInChildren<Rigidbody>();
		totalMass = 0f;
		foreach (Rigidbody rigBod in rigidBodies) {
			totalMass += rigBod.mass;
		}
	}
	
	void Update() {
		energyLabel.text = Mathf.FloorToInt(energyCurrent).ToString ();
		if (energyCurrent < energyMax && (energyLastUsed + ((float)energyRegenDelay / 10)) < Time.time) {
			energyCurrent += (energyRegenRate * Time.deltaTime);
		}
		if (energyCurrent > energyMax) {
			energyCurrent = energyMax;
		}
	}
	
	public bool consumeEnergy(float quantity) {
		if (energyCurrent > quantity) {
			energyCurrent -= quantity;
			energyLastUsed = Time.time;
			return true;
		}
		else {
			return false;
		}
	}
	
	public bool damageHull(float quantity) {
		float temp = hullIntegrityCurrent;
		temp -= (quantity - hullPlating) * hullDensity;
		if (temp < hullIntegrityCurrent) {
			hullIntegrityCurrent = Mathf.RoundToInt(temp);
			return true;
		}
		else 
			return false;
	}

	/*OPTION 
	 * Este bloque de OnCollisionEnter funciona por velocidades unicamente, 
	 * sin tener en cuenta la masa de ninguno de los dos objetos.
	 * */

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.tag == "Fists") {		//Si el golpe es producido por un puñetazo...
			if (collision.relativeVelocity.magnitude > 10f) {
				//Cuanto daño se manda exactamente? 
				float temp = collision.relativeVelocity.magnitude;
				if (damageHull(temp)) {
					SFXAudio.clip = torsoHitSFX[Random.Range(0, torsoHitSFX.Length)];
				}
				else if (!SFXAudio.isPlaying){
					SFXAudio.clip = blockHitSFX[Random.Range(0, blockHitSFX.Length)];
				}


			}
			else if (!SFXAudio.isPlaying)
				SFXAudio.clip = blockHitSFX[Random.Range(0, blockHitSFX.Length)];
			SFXAudio.Play();
			if (debug) {
				Debug.Log("Relative velocity: " + collision.relativeVelocity);
				Debug.Log("Magnitude: " + collision.relativeVelocity.magnitude);
			}
		}
	}


	/*OPTION 
	 * Este bloque de OnTriggerEnter funciona por velocidades y masas, 
	 * multiplicando a cada velocidad la masa de su objeto (torso para golpeado
	 * y robot entero para el que golpea).
	 * */
	/*
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Fists") {		//Si el golpe es producido por un puñetazo...
			Vector3 dirVector = Vector3.zero;
			dirVector = (other.rigidbody.velocity * other.rigidbody.mass) + (this.rigidbody.velocity * totalMass);

			if (dirVector.magnitude > other.rigidbody.mass) {
				//Cuanto daño se manda exactamente? 
				float temp = dirVector.magnitude;
				if (damageHull(temp)) {
					SFXAudio.clip = torsoHitSFX[Random.Range(0, torsoHitSFX.Length)];
				}
				else if (!SFXAudio.isPlaying){
					SFXAudio.clip = blockHitSFX[Random.Range(0, blockHitSFX.Length)];
				}
				
				
			}
			else if (!SFXAudio.isPlaying)
				SFXAudio.clip = blockHitSFX[Random.Range(0, blockHitSFX.Length)];
			SFXAudio.Play();
			if (debug) {
				Debug.Log("Collider " + other.name + " velocity: " + other.rigidbody.velocity);
				Debug.Log("Collider " + other.name + " total mass: " + totalMass);
				Debug.Log("Collider " + this.collider.name + " velocity: " + this.rigidbody.velocity);
				Debug.Log("Collider " + this.collider.name + " mass: " + this.rigidbody.mass);
				Debug.Log("Weighted Relative velocity: " + dirVector);
				Debug.Log("Weighted Magnitude: " + dirVector.magnitude);
			}
		}
	}*/
	
}
