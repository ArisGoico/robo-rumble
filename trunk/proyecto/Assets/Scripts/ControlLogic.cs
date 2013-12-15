using UnityEngine;
using System.Collections;

public class ControlLogic : MonoBehaviour {

	public GUIText textOpening;
	public GUIText textEnding;
	public GUIText textEndingSubtitle;

	public GameObject[] players;
	public bool[] remainingPlayers;
	public int activePlayers;
	private HullLogic[] playersHull;
	public Transform[] spawnpoints;

	private enum State {beginning, battle, win, lose};
	private State state;

	// Use this for initialization
	void Start () {
		remainingPlayers = new bool[players.Length];
		playersHull = new HullLogic[players.Length];
		activePlayers = players.Length;
		for (int i = 0; i < players.Length; i++) {
			playersHull [i] = players [i].GetComponentInChildren<HullLogic> ();
			players[i].transform.position = spawnpoints[i].position;
			remainingPlayers[i] = true;
		}
		state = State.battle;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.beginning:
			textEndingSubtitle.text = "Press start to battle!";
			if (Input.GetAxisRaw ("start0") > 0  || Input.GetAxisRaw ("start1") > 0) {
				activePlayers = players.Length;
				textEndingSubtitle.text = "";
				for (int i = 0; i < playersHull.Length; i++){
					playersHull[i].restartHull();
					players[i].transform.position = spawnpoints[i].position;
					remainingPlayers[i] = true;
				}
				state = State.battle;
			}
			break;
		case State.battle:
			string winner;
			for (int i = 0; i < playersHull.Length; i++){
				if (playersHull[i].hullIntegrityCurrent < 0 ){
					playersHull[i].disable();
					remainingPlayers[i] = false;
					activePlayers--;
				}
				if (activePlayers == 1) {
					int j = 0;
					while (j < remainingPlayers.Length && !remainingPlayers[j]){
						j++;
					}
					winner = ((int)j+1).ToString();
					textEnding.text = string.Format ("Player {0} wins!", winner);
					textEndingSubtitle.text = "Press start for next round";
					state = State.win;
				}
			}
			break;
		case State.lose:
			//TODO No se usa esto porque no hay red, pero se deja por si en el futuro la hay... ;)
			break;
		case State.win:
			if (Input.GetAxisRaw ("start0") > 0 || Input.GetAxis ("start1") > 0) {
				activePlayers = players.Length;
				for (int i = 0; i < playersHull.Length; i++){
					playersHull[i].restartHull();
					players[i].transform.position = spawnpoints[i].position;
					remainingPlayers[i] = true;
				}
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
