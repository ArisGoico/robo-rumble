using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public int player = 1;
	public GUIText inputCapture;
	public float speed;
	public float dashSpeed = 1.0f;
	public float idleDrag = 5.0f;
	public float hoverDrag = 0.75f;

	// Use this for initialization
	void Start () {
		//idea para el futuro: rigidbody.mass = suma de masa de los componentes.
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.rigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal"+player)*speed,0, Input.GetAxis("Vertical"+player)*speed));
		if (Input.GetAxisRaw ("hover"+player) == 1) {
			transform.rigidbody.drag = hoverDrag;
			inputCapture.text = "hover";
		} else {
			transform.rigidbody.drag = idleDrag;
		}
		if (Input.GetAxisRaw ("dash"+player) == 1) {
			//genera movimiento casi instantaneo en la direccion pulsada o hacia adelante en caso de no indicar direccion)
			Vector3 direction = (new Vector3(Input.GetAxis("Horizontal"+player),0, Input.GetAxis("Vertical"+player))).normalized;
			if (direction.Equals (Vector3.zero)) {
				//TODO: revisar, puede que el forward del transform no sea el punto donde mira el robot. Seria torso.transform.forward;
				direction = this.transform.forward;
			}
			//aplicar la fuerza
			transform.rigidbody.AddForce(direction, ForceMode.Impulse);
			inputCapture.text = "dash";
		} 
	}
}
