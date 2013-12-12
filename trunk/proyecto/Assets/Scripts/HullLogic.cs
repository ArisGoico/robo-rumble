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
	public AudioClip[] softHitSFX;
	private AudioSource SFXAudio;
	private bool strongHitSoundPlaying		= false;
	private float lastStrongSoundPlayed		= 0f;
	public float strongSoundDelay			= 0.3f;
	
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

		if (!SFXAudio.isPlaying)
			strongHitSoundPlaying = false;
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

		AudioClip audioTemp;
		if (collision.relativeVelocity.magnitude > 10f) {
			//Cuanto daño se manda exactamente? 
			float temp = collision.relativeVelocity.magnitude;
			if (damageHull(temp)) {
				audioTemp = torsoHitSFX[Random.Range(0, torsoHitSFX.Length)];
				strongHitSoundPlaying = true;
			}
			else {
				audioTemp = softHitSFX[Random.Range(0, softHitSFX.Length)];
				strongHitSoundPlaying = false;
			}
		}
		else {
			audioTemp = softHitSFX[Random.Range(0, softHitSFX.Length)];
			strongHitSoundPlaying = false;
		}
		if ((strongHitSoundPlaying || !SFXAudio.isPlaying) && (lastStrongSoundPlayed + strongSoundDelay) < Time.time) {
			SFXAudio.clip = audioTemp;
			SFXAudio.Play();
			if (strongHitSoundPlaying)
				lastStrongSoundPlayed = Time.time;
		}
	}

}
