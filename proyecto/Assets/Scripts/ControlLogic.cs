using UnityEngine;
using System.Collections;

public class ControlLogic : MonoBehaviour {

	public GUIText textOpening;
	public GUIText playerWins;
	public GUIText playerWinsSubtitle;

	public Transform spawnPoint1;
	public Transform spawnPoint2;

	public GameObject player1;
	public GameObject player2;

	private enum State {beginning, battle, win, lose};
	private State state;

	// Use this for initialization
	void Start () {
		Instantiate(player1, spawnPoint1.position, spawnPoint1.rotation);
		Instantiate(player2, spawnPoint2.position, spawnPoint2.rotation);
		state = State.beginning;
	}
	
	// Update is called once per frame
	void Update () {
		//TODO Falta controlar la vida, avanzar de estados, etc.
	}

	void OnGUI() {
		//Antes de la batalla
		if (state == State.beginning) {
			textOpening.enabled = true;
		}
		else {
			textOpening.enabled = false;
		}

		//Durante la batalla
		if (state == State.battle) {
			textOpening.enabled = true;
		}
		else {
			textOpening.enabled = false;
		}

		//Despues (perdido)
		if (state == State.lose) {
			textOpening.enabled = true;
		}
		else {
			textOpening.enabled = false;
		}

		//Despues (ganado)
		if (state == State.win) {
			textOpening.enabled = true;
		}
		else {
			textOpening.enabled = false;
		}
	}
}
