#pragma strict
var walkAnimation;
var points: Transform[];
var destPoint: int = 0;
var currentgoal: int = 0;
var agent: UnityEngine.AI.NavMeshAgent;
var personnage: Animator;

function Start () {
    agent = GetComponent.<UnityEngine.AI.NavMeshAgent>();
}

function GotoNextPoint() {

  personnage.SetBool("choix"+currentgoal, true);
  personnage.SetBool("Moving", true);
    // Returns if no points have been set up
    if (points.Length == 0)
        return;

    // Set the agent to go to the currently selected destination.
    agent.destination = points[destPoint].position;

    // Choose the next point in the array as the destination,
    // cycling to the start if necessary.
    destPoint = (destPoint + 1) % points.Length;
    currentgoal ++;
    personnage.SetInteger("goal", currentgoal);
}

function Update () {
	// if (Input.GetKey (KeyCode.Space)){
  //   personnage.SetBool("Moving", true);
  //   agent.Resume();
	// }
	// else {
  //   personnage.SetBool("Moving", false);
  //   agent.Stop();
	// }

  if(Input.GetKeyDown (KeyCode.A)){
    GotoNextPoint();
  }

  if (agent.remainingDistance == 0) {
    personnage.SetBool("Moving", false);

    if(Input.GetKeyDown (KeyCode.Space)){
      personnage.SetBool("choix"+currentgoal, true);
      GotoNextPoint();
    }
  }
}

function start () {
  currentgoal = personnage.GetInteger("goal");
  GotoNextPoint();
}

function portejs () {

}