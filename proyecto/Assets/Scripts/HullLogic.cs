using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class HullLogic : MonoBehaviour {

	public bool debug						= false;
	private int player 						= 0;
	private GamePadState state;

	//Hull Integrity / Salud 
	private float initialMass;
	public int hullCurrent					= 100;		//Current hull integrity / Current health
	public int hullMax						= 100;		//Max hull integrity / Max health
	public float hullDensity				= 1f;		//Percentage of each hit damage that goes through (0 - 1)
	public float hullPlating				= 0f;		//Min damage that goes through
	public float terrainPlatingFact			= 2f;		//Factor of hullPlating that affects terrain hits

	//Energy
	public float energyCurrent				= 100f;		//Current energy
	public float energyMax					= 100f;		//Max energy
	public float energyRegenRate			= 10f;		//Energy regenerated per second
	public float energyRegenDelay			= 0f;		//Number of seconds (/10) to start regenerating
	private bool disabled 					= false;	//lets or prevents the player do anything
	
	private float energyLastUsed			= 0f;		//Last time energy was used.
	
	//GUI
	public GameObject spriteLifeBar;					//GUI Life sprite representation
	public GUITexture lifeBar;							//GUI Life graphic representation
	public GUIText lifeLabel;							//GUI Life text representation
	public GUITexture energyBar;						//GUI Energy graphic representation
	public GUIText energyLabel;							//GUI Energy text representation

	//Hit Sounds and effects
	public AudioClip[] torsoHitSFX;
	public AudioClip[] blockHitSFX;
	public AudioClip[] softHitSFX;
	public AudioClip[] sandHitSFX;
	private AudioSource SFXAudio;
	private bool strongHitSoundPlaying		= false;
	private float lastStrongSoundPlayed		= 0f;
	public float strongSoundDelay			= 0.3f;

	public GameObject spark;
	
	void Start() {
		player = transform.parent.GetComponent<Movement> ().player;
		initialMass = transform.parent.rigidbody.mass;
		hullCurrent = hullMax;
		energyCurrent = energyMax;
		SFXAudio = this.GetComponent<AudioSource>();
	}
	
	void Update() {

		float life = (float)hullCurrent/(float)hullMax;
		spriteLifeBar.GetComponent<lifeBarSprite>().ChangeLifeBar(life);
		lifeBar.pixelInset = new Rect (lifeBar.pixelInset.x, lifeBar.pixelInset.y, Mathf.Floor ((float)hullCurrent* 100f / (float)hullMax) * 2f, lifeBar.pixelInset.height);
		lifeLabel.text = Mathf.FloorToInt(hullCurrent).ToString ();
		energyLabel.text = Mathf.FloorToInt(energyCurrent).ToString ();
		energyBar.pixelInset = new Rect (energyBar.pixelInset.x, energyBar.pixelInset.y, Mathf.Floor ((float)energyCurrent* 100f / (float)energyMax ) * 2f, energyBar.pixelInset.height);

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
		if (!disabled){
			float temp = hullCurrent;
			temp -= (quantity - hullPlating) * hullDensity;
			if (temp < hullCurrent) {
				hullCurrent = Mathf.RoundToInt(temp);
				return true;
			} else 
				return false;
		} else 
			return false;
	}

	public void restartHull() {
		transform.GetComponent<Attack> () .relaxArms (false);
		transform.parent.rigidbody.mass = initialMass;
		hullCurrent = hullMax;
		energyCurrent = energyMax;
		energyLastUsed = 0f;
		lastStrongSoundPlayed = 0f;
		disabled = false;
	}

	public bool isDisabled() {
		return disabled;
	}
	
	public void disable () {
		disabled = true;
		hullCurrent = 0;
		//efectos de disabled
		transform.parent.rigidbody.mass = 100;
		transform.GetComponent<Attack> () .relaxArms (true);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.transform.IsChildOf(this.transform.parent)) {
			return;
		}
		AudioClip audioTemp;
		if (collision.collider.tag == "Terrain") {
			if (collision.relativeVelocity.magnitude > (hullPlating * terrainPlatingFact)) {
				//Cuanto daño se manda exactamente? 
				float temp = collision.relativeVelocity.magnitude;
				if (damageHull(temp)) {
					audioTemp = torsoHitSFX[Random.Range(0, torsoHitSFX.Length)];
					strongHitSoundPlaying = true;
					Instantiate(spark, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
					StartCoroutine(rumbleForSecs(player, 1-Vector3.Dot(collision.contacts[0].normal, -transform.right), 1-Vector3.Dot(collision.contacts[0].normal, transform.right) , 0.5f));
				}
				else {
					audioTemp = sandHitSFX[Random.Range(0, sandHitSFX.Length)];
					strongHitSoundPlaying = false;
				}
			}
			else {
				audioTemp = sandHitSFX[Random.Range(0, sandHitSFX.Length)];
				strongHitSoundPlaying = false;
			}
		}
		else {
			if (collision.relativeVelocity.magnitude > hullPlating) {
				//Cuanto daño se manda exactamente? 
				float temp = collision.relativeVelocity.magnitude;
				if (damageHull(temp)) {
					audioTemp = torsoHitSFX[Random.Range(0, torsoHitSFX.Length)];
					strongHitSoundPlaying = true;
					Instantiate(spark, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
					StartCoroutine(rumbleForSecs(player, 1-Vector3.Dot(collision.contacts[0].normal, -transform.right), 1-Vector3.Dot(collision.contacts[0].normal, transform.right) , 0.5f));
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
		}
		if ((strongHitSoundPlaying || !SFXAudio.isPlaying) && (lastStrongSoundPlayed + strongSoundDelay) < Time.time) {
			SFXAudio.clip = audioTemp;
			SFXAudio.Play();
			if (strongHitSoundPlaying)
				lastStrongSoundPlayed = Time.time;
		}
	}

	IEnumerator rumbleForSecs(int player, float left, float right, float duration) {
		GamePad.SetVibration((PlayerIndex)player, left, right);
		yield return new WaitForSeconds(duration);
		GamePad.SetVibration((PlayerIndex)player, 0f, 0f);

	}		
}
