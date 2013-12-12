using UnityEngine;
using System.Collections;

public class FistImpact : MonoBehaviour {

	//Hit Sounds
	public AudioClip[] torsoHitSFX;
	public AudioClip[] blockHitSFX;
	public AudioClip[] softHitSFX;
	private AudioSource SFXAudio;
	private float lastStrongSoundPlayed		= 0f;
	public float strongSoundDelay			= 0.3f;

	void Start() {
		SFXAudio = this.GetComponent<AudioSource>();
	}

	void OnCollisionEnter(Collision collision) {
		AudioClip audioTemp;
		if (collision.collider.transform.IsChildOf(this.transform.parent.parent)) {
			return;
		}
		if (collision.relativeVelocity.magnitude > 10f) { 
			//Cuanto daño se manda exactamente? 
			if (collision.collider.tag == "Fists") {
				audioTemp = blockHitSFX[Random.Range(0, blockHitSFX.Length)];
			}
			else {
				audioTemp = torsoHitSFX[Random.Range(0, torsoHitSFX.Length)];
			}
		}
		else {
			audioTemp = softHitSFX[Random.Range(0, softHitSFX.Length)];
		}
		if (!SFXAudio.isPlaying && (lastStrongSoundPlayed + strongSoundDelay) < Time.time) {
			SFXAudio.clip = audioTemp;
			SFXAudio.Play();
			lastStrongSoundPlayed = Time.time;
		}
	}
}
