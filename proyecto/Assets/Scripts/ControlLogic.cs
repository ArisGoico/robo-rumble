using UnityEngine;
using System.Collections;

public class ControlLogic : MonoBehaviour {

	public GUIText textOpening;
	public GUIText textEnding;
	public GUIText textEndingSubtitle;
	public GUITexture background;

	public GameObject[] players;
	public bool[] remainingPlayers;
	public int activePlayers;
	private HullLogic[] playersHull;
	public Transform[] spawnpoints;

	private enum State {beginning, battle, win, lose};
	private State state;

	public float timeRound;
	private float roundFinish;
	private float roundStart;
	private int[] scorePlayers;

	public GameObject battleZone;
	public float maxTimeOutside			= 5f;
	private float[] timeOutsidePlayers;

	// Use this for initialization
	void Start () {
		hideGUIBox ();
		remainingPlayers = new bool[players.Length];
		playersHull = new HullLogic[players.Length];
		activePlayers = players.Length;
		scorePlayers = new int[players.Length];
		timeOutsidePlayers = new float[players.Length];
		for (int i = 0; i < players.Length; i++) {
			playersHull [i] = players [i].GetComponentInChildren<HullLogic> ();
			players[i].transform.position = spawnpoints[i].position;
			remainingPlayers[i] = true;
			scorePlayers[i] = 0;
			timeOutsidePlayers[i] = 0f;
		}
		state = State.battle;
		roundStart = Time.time;
		roundFinish = roundStart + timeRound;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.beginning:
			showGuiBox("GET READY", "The battle begins!", "Press start");
			if (Input.GetAxisRaw ("start0") > 0  || Input.GetAxisRaw ("start1") > 0) {
				activePlayers = players.Length;
				textEndingSubtitle.text = "";
				for (int i = 0; i < playersHull.Length; i++){
					playersHull[i].restartHull();
					players[i].transform.position = spawnpoints[i].position;
					remainingPlayers[i] = true;
				}
				roundStart = Time.time;
				roundFinish = roundStart + timeRound;
				state = State.battle;
			}
			break;
		case State.battle:
			hideGUIBox();
			hullCheck();
			zoneCheck();
			victoryCheck();
			timeCheck();
			break;
		case State.lose:
			//TODO No se usa esto porque no hay red, pero se deja por si en el futuro la hay... ;)
			break;
		case State.win:
			if (Input.GetAxisRaw ("start0") > 0 || Input.GetAxis ("start1") > 0) {
				hideGUIBox();
				activePlayers = players.Length;
				for (int i = 0; i < playersHull.Length; i++){
					playersHull[i].restartHull();
					players[i].transform.position = spawnpoints[i].position;
					remainingPlayers[i] = true;
				}
				roundStart = Time.time;
				roundFinish = roundStart + timeRound;
				state = State.battle;
			}
			break;
		}


	}

	private void hullCheck() {
		for (int i = 0; i < playersHull.Length; i++){
			if (playersHull[i].hullCurrent < 0 ){
				if (!playersHull[i].isDisabled()) {
					remainingPlayers[i] = false;
					activePlayers--;
					playersHull[i].disable();
				}
			}
		}
	}

	private void timeCheck() {
		if (Time.time > roundFinish) {
			int playerWin = 0;
			int maxHull = -1;
			for (int i = 0; i < playersHull.Length; i++){
				if (playersHull[i].hullCurrent > maxHull) {
					playerWin = i;
					maxHull = playersHull[i].hullCurrent;
				}
			}
			for (int i = 0; i < playersHull.Length; i++){
				playersHull[i].disable();
			}
			
			showGuiBox("BATTLE REPORT", string.Format ("Player {0} wins!", (playerWin + 1).ToString()), "Press start for next round");
			scorePlayers[playerWin] = scorePlayers[playerWin] + 1;
			state = State.win;
		}
	}

	private void zoneCheck() {
		for (int i = 0; i < players.Length; i++){
			if (!battleZone.collider.bounds.Contains(players[i].transform.position)) {
				if (timeOutsidePlayers[i] != 0f) {
					if (Time.time > timeOutsidePlayers[i]) {
						if (!playersHull[i].isDisabled()) {
							remainingPlayers[i] = false;
							activePlayers--;
							playersHull[i].disable();
						}
					}
					//TODO Poner un aviso en la GUI para este jugador
				}
				else {
					timeOutsidePlayers[i] = Time.time + maxTimeOutside;
				}
			}
			else {
				timeOutsidePlayers[i] = 0f;
			}
		}
	}

	private void victoryCheck() {
		string winner;
		if (activePlayers < 2) {
			if (activePlayers == 1){ 
				int j = 0;
				while (j < remainingPlayers.Length && !remainingPlayers[j]){
					j++;
				}
				winner = ((int)j+1).ToString();
				showGuiBox("BATTLE REPORT", string.Format ("Player {0} wins!", winner.ToString()), "Press start for next round");
				scorePlayers[j] = scorePlayers[j] + 1;
			} else {
				showGuiBox("BATTLE REPORT", "The match was a draw!", "Press start for next round");
			}
			state = State.win;
		}
	}

	public void showGuiBox(string super, string message, string subtitle) {
		textOpening.enabled = true;
		textEnding.enabled = true;
		textEndingSubtitle.enabled = true;
		background.enabled = true;

		textOpening.text = super;
		textEnding.text = message;
		textEndingSubtitle.text = subtitle;
	}

	public void hideGUIBox() {
		textOpening.enabled = false;
		textEnding.enabled = false;
		textEndingSubtitle.enabled = false;
		background.enabled = false;
	}

}
