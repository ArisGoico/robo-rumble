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

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.tag == "Fists") {
			if (collision.relativeVelocity.magnitude > 10f) {		//Si el golpe es producido por un puñetazo...
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
	
}
