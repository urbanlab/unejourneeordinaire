#pragma strict
var startrotation = false;
var duration = 0.0;
var direction = 1;

function Start () {
}

function Update () {
	if(startrotation && duration<2.5){
			transform.Rotate(Vector3.up, Time.deltaTime*40 * direction);
			duration += Time.deltaTime;
	}
}

function Hodor () {
	direction = 1;
	startrotation = true;
	duration = 0.0;
	Debug.Log("porte.js");
}

function rodoh () {
	direction = -1;
	startrotation = true;
	duration = 0.0;
	Debug.Log("porte.js");
}
