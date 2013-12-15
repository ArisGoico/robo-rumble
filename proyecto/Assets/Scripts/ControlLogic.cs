using UnityEngine;
using System.Collections;

public class ControlLogic : MonoBehaviour {

	public GUIText textOpening;
	public GUIText textEnding;
	public GUIText textEndingSubtitle;

	public Transform spawnPoint1;
	public Transform spawnPoint2;

	public GameObject player1;
	public GameObject player2;
	
	private HullLogic player1Hull;
	private HullLogic player2Hull;

	private enum State {beginning, battle, win, lose};
	private State state;

	// Use this for initialization
	void Start () {
		player1Hull = player1.GetComponentInChildren<HullLogic>();
		player2Hull = player2.GetComponentInChildren<HullLogic>();
		player1.transform.position = spawnPoint1.position;
		player2.transform.position = spawnPoint2.position;
		state = State.battle;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.beginning:
			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
				player1Hull.restartHull();
				player1.transform.position = spawnPoint1.position;
				player2Hull.restartHull();
				player2.transform.position = spawnPoint2.position;
				state = State.battle;
			}
			break;
		case State.battle:
			if (player1Hull.hullIntegrityCurrent < 0f) {
				textEnding.text = "Player 2 wins!";
				textEndingSubtitle.text = "Press enter to start again...";
				state = State.win;
			}
			else if (player2Hull.hullIntegrityCurrent < 0f){
				textEnding.text = "Player 1 wins!";
				textEndingSubtitle.text = "Press enter to start again...";
				state = State.win;
			}

			break;
		case State.lose:
			//TODO No se usa esto porque no hay red, pero se deja por si en el futuro la hay... ;)
			break;
		case State.win:
			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
				player1Hull.restartHull();
				player1.transform.position = spawnPoint1.position;
				player2Hull.restartHull();
				player2.transform.position = spawnPoint2.position;
				state = State.battle;
			}
			break;
		}


	}

	void OnGUI() {
		//Antes de la batalla
		if (state == State.beginning) {
			textOpening.enabled = true;
		}
		else {
			textOpening.enabled = false;
		}

		//Despues 
		if (state == State.lose || state == State.win) {
			textEnding.enabled = true;
			textEndingSubtitle.enabled = true;
		}
		else {
			textEnding.enabled = false;
			textEndingSubtitle.enabled = false;
		}


	}
}
