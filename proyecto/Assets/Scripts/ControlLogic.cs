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

	private GameObject player1Spawn;
	private GameObject player2Spawn;
	private HullLogic player1Hull;
	private HullLogic player2Hull;

	private enum State {beginning, battle, win, lose};
	private State state;

	// Use this for initialization
	void Start () {
		player1Spawn = Instantiate(player1, spawnPoint1.position, spawnPoint1.rotation) as GameObject;
		player2Spawn = Instantiate(player2, spawnPoint2.position, spawnPoint2.rotation) as GameObject;
		player1Hull = player1Spawn.GetComponentInChildren<HullLogic>();
		player2Hull = player2Spawn.GetComponentInChildren<HullLogic>();
		player1.SetActive(false);
		player2.SetActive(false);
		state = State.battle;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.beginning:
			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
				Destroy(player1Spawn);
				Destroy(player2Spawn);
//				player1Spawn = Instantiate(player1, spawnPoint1.position, spawnPoint1.rotation) as GameObject;
//				player2Spawn = Instantiate(player2, spawnPoint2.position, spawnPoint2.rotation) as GameObject;
//				player1Hull = player1Spawn.GetComponentInChildren<HullLogic>();
//				player2Hull = player2Spawn.GetComponentInChildren<HullLogic>();
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
				Destroy(player1Spawn);
				Destroy(player2Spawn);
//				player1Spawn = Instantiate(player1, spawnPoint1.position, spawnPoint1.rotation) as GameObject;
//				player2Spawn = Instantiate(player2, spawnPoint2.position, spawnPoint2.rotation) as GameObject;
//				player1Hull = player1Spawn.GetComponentInChildren<HullLogic>();
//				player2Hull = player2Spawn.GetComponentInChildren<HullLogic>();
				state = State.battle;
			}
			break;
		}

		if (player1Spawn == null) {
			player1Spawn = Instantiate(player1, spawnPoint1.position, spawnPoint1.rotation) as GameObject;
			player1Hull = player1Spawn.GetComponentInChildren<HullLogic>();
		}
		if (player2Spawn == null) {
			player2Spawn = Instantiate(player2, spawnPoint2.position, spawnPoint2.rotation) as GameObject;
			player2Hull = player2Spawn.GetComponentInChildren<HullLogic>();
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
