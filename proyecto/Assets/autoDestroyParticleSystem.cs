using UnityEngine;
using System.Collections;

public class autoDestroyParticleSystem : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (particleSystem != null && particleSystem.particleCount == 0)
			Destroy(gameObject,0.5f);
	}
}
