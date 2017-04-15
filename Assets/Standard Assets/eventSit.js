#pragma strict
var scriptnavmesh: GameObject;
var elementporte: GameObject;
var elementlampe: GameObject;

function Start () {
}

function Update () {
}

function Go () {
	Debug.Log("Go");
	/*scriptnavmesh.GetComponent.<myPointDeVue>().GotoNextPoint();*/
}

function GoGo () {
	Debug.Log("GoGo");
	scriptnavmesh.GetComponent.<myPointDeVue>().GotoNextPoint();
}


function Hodor() {
	elementporte.GetComponent.<porte>().Hodor();
}

function rodoh() {
	elementporte.GetComponent.<porte>().rodoh();
}


function panne() {
	elementlampe.GetComponent.<panne>().pannelampe();
}

