using UnityEngine;
using System.Collections;

public class HullLogic : MonoBehaviour {

	//Hull Integrity / Salud 
	public GameObject lifeBar;							//GUI Life graphic representation
	public int hullIntegrityCurrent			= 100;		//Current hull integrity / Current health
	public int hullIntegrityMax				= 100;		//Max hull integrity / Max health
	public float hullDensity				= 1f;		//Percentage of each hit damage that goes through (0 - 1)
	public float hullPlating				= 0f;		//Min damage that goes through
	
	//Energy
	public GameObject energyBar;						//GUI Energy graphic representation
	public GUIText energyLabel;							//GUI Energy text representation
	public float energyCurrent				= 100f;		//Current energy
	public float energyMax					= 100f;		//Max energy
	public float energyRegenRate			= 10f;		//Energy regenerated per second
	public float energyRegenDelay			= 0f;		//Number of seconds (/10) to start regenerating
	
	private float energyLastUsed			= 0f;		//Last time energy was used.
	
	
	
	void Start() {
		hullIntegrityCurrent = hullIntegrityMax;
		energyCurrent = energyMax;
		energyLabel.text = Mathf.FloorToInt(energyCurrent).ToString ();
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
		
}
