// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class AgentSoccerBlue : Agent
// {
// 	public enum Team
//     {
//         red, blue
//     }
// 	public bool addOtherPlayerStates; //if this is not checked then we will only collect states for this players
// 	public Team team;
// 	public float teamFloat;
// 	public float playerID;
// 	public int playerIndex;
// 	public SoccerFieldArea area;
// 	// [HideInInspector]
// 	// public SoccerStateCollector stateCollector;

// 	// // public bool agentMustFindObjs;
// 	// GameObject targetGoal;
// 	// GameObject defendGoal;
// 	// public bool foundGoal;
//     // public GameObject ball;
// 	// public AgentSoccer teamMate;
// 	// public bool foundBlock;
// 	// Rigidbody blockRB;
// 	[HideInInspector]
// 	public Rigidbody agentRB;
// 	// public LayerMask groundLayer;
 
// 	// Vector3 groundCheckDirForward;
// 	// Vector3 groundCheckDirRight;
// 	// Vector3 groundCheckDirLeft;
// 	// Vector3 groundCheckDirBack;
// 	// public float visionDistance;
// 	// public float sphereCastRadius = 1;
// 	// public float turnPlayerTorque = 15;
// 	// public float walkSpeed = 50;
// 	// float teamFloat; //1 if blue, 0 if red
// 	// public bool useCameraRelMovement;
// 	// public float rotationSpeed;
// 	// Camera camera;
// 	// Vector3 cameraForward;
// 	// Vector3 cameraRight;
// 	// [HideInInspector]
// 	// public Vector3 startingPos;

// 	// Vector3 agentPosRelToGoal;
// 	// Vector3 blockPosRelToGoal;
// 	// Vector3 blockPosRelToAgent;
	
// 	// public RaycastHit[] sphereCastHits = new RaycastHit[5]; 
// 	// public int playerID; //this is the index of the player in the team list. ex: playerID = 1 & teamFloat 1 means they are at index 1 in area.bluePlayers List.

// 	public bool collectMyState;
// 	public bool collectMyTeamsState;
// 	public bool collectOtherTeamsState;

// 	public List<float> myState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// 	public List<float> myTeamsState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// 	public List<float> otherTeamsState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// 	public List<float> stateToSubmitToBrain = new List<float>(); //list for state data. to be updated every FixedUpdate in this script

//     void Awake()
//     {
//         // if(team == Team.red)
//         // {
// 		// 	brain = area.redBrain;
// 		// 	PlayerState playerState = new PlayerState();
// 		// 	playerState.teamFloat = 0;
// 		// 	teamFloat = 0;
// 		// 	playerState.playerIndex = area.redPlayers.Count; //indexed pos in playerStates
// 		// 	playerState.playerID = area.redPlayers.Count; //float id used to id individual
// 		// 	playerID = playerState.playerID;
// 		// 	playerIndex = playerState.playerIndex;
// 		// 	playerState.defendGoal = area.redGoal;
// 		// 	playerState.targetGoal = area.blueGoal;
// 		// 	playerState.agentRB = GetComponent<Rigidbody>(); //cache the RB
// 		// 	agentRB = GetComponent<Rigidbody>(); //cache the RB
// 		// 	playerState.startingPos = transform.position;
// 		// 	// playerState.agentScript = this;
// 		// 	playerState.agentScriptRed = this;
//         //     area.redPlayers.Add(playerState);
//         //     area.playerStates.Add(playerState);

//         // }
//         // else if(team == Team.blue)
//         // {
// 			brain = area.blueBrain;
// 			PlayerState playerState = new PlayerState();
// 			playerState.teamFloat = 1;
// 			teamFloat = 1;
// 			playerState.playerIndex = area.bluePlayers.Count; //indexed pos in playerStates
// 			playerState.playerID = area.bluePlayers.Count; //float id used to id individual
// 			playerID = playerState.playerID;
// 			playerIndex = playerState.playerIndex;
// 			playerState.defendGoal = area.blueGoal;
// 			playerState.targetGoal = area.redGoal;
// 			playerState.agentRB = GetComponent<Rigidbody>(); //cache the RB
// 			agentRB = GetComponent<Rigidbody>(); //cache the RB
// 			playerState.startingPos = transform.position;
// 			// playerState.agentScript = this;
// 			playerState.agentScriptBlue = this;
//             area.bluePlayers.Add(playerState);
//             area.playerStates.Add(playerState);

// 			// teamFloat = 1;
// 			// defendGoal = area.blueGoal;
// 			// targetGoal = area.redGoal;
//         // }

// 		// agentRB = GetComponent<Rigidbody>(); //cache the RB
// 		// ballRB = area.ball.GetComponent<Rigidbody>(); //cache the RB
//         // startingPos = transform.position;
//         // targetGoal.opponentAgent = this; //let the goal know I am 
//     }

// 	// void Start()
// 	// {
// 	// 	agentRB	= GetComponent<Rigidbody>(); //cache the agent rigidbody
// 	// 	// blockRB = ball.GetComponent<Rigidbody>();
// 	// 	// camera = Camera.main;
// 	// 	// area.GetCameraDir();
// 	// }

// 	//if cam is fixed you shouldn't need to use this.
// 	// void GetCameraDir()
// 	// {
// 	// 	cameraForward = camera.transform.forward;
// 	// 	cameraForward.y = 0;
// 	// 	cameraForward.Normalize();
// 	// 	cameraRight = camera.transform.right;
// 	// 	cameraRight.Normalize();
// 	// }

//     public override void InitializeAgent()
//     {
// 		// if(agentMustFindObjs)
// 		// {
// 		// 	foundBlock = false;
// 		// 	foundGoal = false;
// 		// }
// 		// else
// 		// {
// 		// 	foundBlock = true;
// 		// 	foundGoal = true;
// 		// }
// 		base.InitializeAgent();
//     }

// 	// //Add the x,y,z components of this Vector3 to the state list
//     // void CollectVector3State(List<float> state, Vector3 v)
//     // {
//     //     state.Add(v.x);
//     //     state.Add(v.y);
//     //     state.Add(v.z);
//     // }

// 	// void CollectRotationState(List<float> state, Quaternion q)
//     // {
//     //     state.Add(q.x);
//     //     state.Add(q.y);
//     //     state.Add(q.z);
//     // }

// 	// //we can only collect floats in CollecState so we need to convert bools to floats
// 	// float ConvertBoolToFloat(bool b)
// 	// {
// 	// 	float f = b == true? 1 : 0;
// 	// 	return f;
// 	// }

// 	//put whatever angles you want checked in here
// 	Vector3[] GetRayCastDirs()
// 	{
// 		Vector3 randomDirForwardRight = (transform.forward + transform.right/Random.Range(1,5)).normalized; //get dir at random that is roughly forward + right
// 		// Debug.DrawRay(transform.position, randomDirForwardRight * 5, Color.magenta, 0f, true);
// 		Vector3 randomDirForwardLeft = (transform.forward - transform.right/Random.Range(1,5)).normalized; //get dir at random that is roughly forward + left
// 		// Debug.DrawRay(transform.position, randomDirForwardLeft * 5, Color.green, 0f, true);

// 		// Debug.DrawRay(transform.position, transform.forward * 5, Color.green, 0f, true);

// 		Vector3[] sphereCastDirs = {randomDirForwardLeft, randomDirForwardRight, transform.forward};
// 		return sphereCastDirs;

// 	}

// 	// void LookForThangs()
// 	// {
// 	// 	foreach(Vector3 dir in GetRayCastDirs())
// 	// 	{
// 	// 		Debug.DrawRay(transform.position + transform.forward, dir * visionDistance, Color.green, 0f, true);
// 	// 		int hitObjs = Physics.SphereCastNonAlloc(transform.position + transform.forward, sphereCastRadius, dir, sphereCastHits, visionDistance);
// 	// 		if(hitObjs > 0) //found the block associated with this area
// 	// 		{
// 	// 			foreach(RaycastHit hit in sphereCastHits)
// 	// 			{
// 	// 				if(hit.collider != null)
// 	// 				{
// 	// 					if(hit.collider.gameObject == goal)
// 	// 					{
// 	// 						foundGoal = true;
// 	// 					}
// 	// 					if(hit.collider.gameObject == block)
// 	// 					{
// 	// 						foundBlock = true;
// 	// 						blockRB = hit.collider.gameObject.GetComponent<Rigidbody>();
// 	// 					}
// 	// 				}
// 	// 			}
// 	// 		}
// 	// 	}
// 	// }


//   	public override List<float> CollectState()
//     {
// 		//instead of new lists each tick we will reuse these
// 		// myState.Clear();
// 		myTeamsState.Clear();
// 		otherTeamsState.Clear();
// 		stateToSubmitToBrain.Clear();

// 		myState = area.playerStates[playerIndex].state;
// 		stateToSubmitToBrain.AddRange(myState); //26 per player
// 		if(addOtherPlayerStates)
// 		{
// 			//	MAY NEED TO COLLECT STATES FOR WHEN GOAL IS SCORED. <team, player>
// 			if(team == Team.red)
// 			{
// 				//collect our players. 26 states for each teammate (THIS EXCLUDES THIS PLAYER)
// 				foreach (PlayerState ps in area.redPlayers)
// 				{
// 					if(ps.playerID != playerID) //not me
// 					{
// 						myTeamsState.AddRange(ps.state);
// 					}

// 					// print("Adding team mate " + ps.agentScript.gameObject.name + " to myTeamsState: " + ps.state);
// 				}
// 				stateToSubmitToBrain.AddRange(myTeamsState);
// 				//collect other teams 26 states for each player on other team

// 				foreach (PlayerState ps in area.bluePlayers)
// 				{
// 					// print("Adding opponent " + ps.agentScript.gameObject.name + " to otherTeamsState: " + ps.state);
// 					otherTeamsState.AddRange(ps.state);
// 				}
// 				stateToSubmitToBrain.AddRange(otherTeamsState);
// 			}


// 			if(team == Team.blue)
// 			{
// 				//collect our players. 26 states for each teammate (THIS EXCLUDES THIS PLAYER)
// 				foreach (PlayerState ps in area.bluePlayers)
// 				{
// 					if(ps.playerID != playerID) //not me
// 					{
// 						myTeamsState.AddRange(ps.state);
// 					}

// 				}
// 				stateToSubmitToBrain.AddRange(myTeamsState);
// 				//collect other teams 26 states for each player on other team
// 				foreach (PlayerState ps in area.redPlayers)
// 				{
// 					otherTeamsState.AddRange(ps.state);
// 				}
// 				stateToSubmitToBrain.AddRange(otherTeamsState);
// 			}
// 		}

		
// 		// print ("stateToSubmitToBrain: " + stateToSubmitToBrain);
// 		return stateToSubmitToBrain;

// 	}
//   	// public override List<float> CollectState()
//     // {
//     //     List<float> state = new List<float>();


// 	// 	//*NOTE:  (dir & velocity defaults to [0,0,0] if not found) */
		
// 	// 	// state.Add(team); //what team are we on
// 	// 	CollectVector3State(state, agentRB.velocity);
// 	// 	// CollectVector3State(state, agentRB.angularVelocity);
// 	// 	CollectRotationState(state, transform.rotation); //agent's rotation

// 	// 	// CollectVector3State(state, teamMate.agentRB.velocity);
// 	// 	// CollectRotationState(state, teamMate.transform.rotation); //agent's rotation
// 	// 	// if(agentMustFindObjs)
// 	// 	// {
// 	// 	// 	if(!foundGoal || !foundBlock)
// 	// 	// 	{
// 	// 	// 		LookForThangs();
// 	// 	// 	}
// 	// 	// }

// 	// 	// if(!foundGoal)
// 	// 	// {
// 	// 	// 	float[] goalSubList = { 0f,0f,0f,0f};  // 4 STATES. NOTHING FOUND. [0] = found?, [1,2,3] = dir goal to player x/y/z.
// 	// 	// 	state.AddRange(goalSubList);
// 	// 	// }
// 	// 	// else
// 	// 	// {
// 	// 		agentPosRelToGoal = transform.position - targetGoal.transform.position;
// 	// 		// Vector3 teamMateAgentPosRelToGoal = teamMate.transform.position - goal.transform.position;
// 	// 		// state.Add(1f); //found
// 	// 		// float teamFloat;

// 	// 		state.Add(teamFloat); //which team
// 	// 		CollectVector3State(state, agentPosRelToGoal);
// 	// 		// CollectVector3State(state, teamMateAgentPosRelToGoal);


// 	// 		// float[] goalSubList = { 1f, team, agentPosRelToGoal.x, agentPosRelToGoal.y,agentPosRelToGoal.z};  //5 STATES WE KNOW WHERE THE GOAL IS NOW [0] = found?, [1,2,3] = dir goal to player x/y/z.
// 	// 		// state.AddRange(goalSubList);
// 	// 	// }
// 	// 	// if(!foundBlock)
// 	// 	// {
// 	// 	// 	float[] blockSubList = { 0f,0f,0f,0f,0f,0f,0f}; //7 STATES. [0] = found?, [1,2,3] = dir player to block, [4,5,6] = vel of block
// 	// 	// 	state.AddRange(blockSubList);
// 	// 	// }
// 	// 	// else
// 	// 	// {
// 	// 		blockPosRelToGoal = area.ballRB.position - targetGoal.transform.position;
// 	// 		blockPosRelToAgent = area.ballRB.position - agentRB.position;
// 	// 		// state.Add(1f); //found
// 	// 		CollectVector3State(state, blockPosRelToGoal);
// 	// 		CollectVector3State(state, blockPosRelToAgent);
// 	// 		CollectVector3State(state, area.ballRB.velocity);
// 	// 		// CollectVector3State(state, blockRB.angularVelocity);
// 	// 		// CollectRotationState(state, blockRB.rotation);

// 	// 		// float[] blockSubList = {1f,blockPosRelToAgent.x, blockPosRelToAgent.y, blockPosRelToAgent.z, blockRB.velocity.x, blockRB.velocity.y, blockRB.velocity.z};  // 7 STATES. WE KNOW WHERE THE BLOCK IS NOW //[0] = found?, [1,2,3] = dir player to block, [4,5,6] = vel of block.
// 	// 		// float[] blockSubList = {1f,blockPosRelToAgent.x, blockPosRelToAgent.y, blockPosRelToAgent.z, blockRB.velocity.x, blockRB.velocity.y, blockRB.velocity.z, blockPosRelToGoal.x, blockPosRelToGoal.y, blockPosRelToGoal.z};  // 10 STATES. WE KNOW WHERE THE BLOCK IS NOW //[0] = found?, [1,2,3] = dir player to block, [4,5,6] = vel of block.
// 	// 		// state.AddRange(blockSubList);
// 	// 	// }
				
//     //     return state;
//     // }

// 	//Always face the block. WE ARE SETTING THE PLAYER'S ROTATION, NOT ML AGENTS
// 	void RotateTowards(Vector3 pos)
// 	{
// 		Vector3 dir = pos - transform.position; //get dir to block
//         dir.y = 0; //to prevent the player from rotating up or down we need to zero out the y
// 		Quaternion targetRotation = Quaternion.LookRotation(dir); //get our needed rotation
// 		agentRB.MoveRotation(Quaternion.Lerp( agentRB.transform.rotation, targetRotation, Time.deltaTime * area.agentRotationSpeed)); //ROTATE BRO
// 	}


// 	public void MoveAgent(float[] act) {

//         if (brain.brainParameters.actionSpaceType == StateType.continuous)
//         {

// 			// Vector3 directionX = transform.right * Mathf.Clamp(act[0], -1f, 1f);
//             // // Vector3 directionY = transform.up * Mathf.Clamp(act[1], -1f, 1f);
//             // Vector3 directionZ = transform.forward * Mathf.Clamp(act[2], -1f, 1f);
//             // // if (agentRB.velocity.y > 0) { directionY = 0f; }
//             // // agentRB.AddForce(directionX + directionZ, ForceMode.Acceleration);
//         	// agentRB.AddForce((directionX + directionZ) * area.agentRunSpeed, ForceMode.VelocityChange);

// 			float directionX = 0;
// 			float directionZ = 0;
// 			float directionY = 0;

//             directionX = Mathf.Clamp(act[0], -1f, 1f);
//             directionZ = Mathf.Clamp(act[1], -1f, 1f);
//             directionY = Mathf.Clamp(act[2], -1f, 1f);
//             if (agentRB.velocity.y > 0) { directionY = 0f; }
//         	agentRB.AddForce(new Vector3(directionX * 40f, directionY * 300f, directionZ * 40f));
//         }
//         else
//         {
// 			// Debug.DrawRay(transform.position, groundCheckDirForward, Color.green, 0f, true);
// 			// Debug.DrawRay(transform.position, groundCheckDirBack, Color.green, 0f, true);
// 			// Debug.DrawRay(transform.position, groundCheckDirLeft, Color.green, 0f, true);
// 			// Debug.DrawRay(transform.position, groundCheckDirRight, Color.green, 0f, true);
// 			// Debug.DrawRay(transform.position, transform.forward * 2, Color.red, 0f, true);

// 			// if(useCameraRelMovement){GetCameraDir();} //if the players should move rel to camera we need to update our camera dirs 

//         	Vector3 dirToGo = Vector3.zero; //Start with a zero Vector
//             int movement = Mathf.FloorToInt(act[0]);
//             if (movement == 1) //go left
//             { 
// 					dirToGo = -area.cameraRight;
// 					// reward -= .005f;
//             } 
//             if (movement == 2)//go right
//             { 
// 					dirToGo = area.cameraRight;
// 					// reward -= .005f;
//              } 
//             if (movement == 3)//go forward
//             { 
// 					dirToGo = area.cameraForward;
// 					// reward -= .005f;
//             } 
//             if (movement == 4)//go back
//             { 
// 					dirToGo = -area.cameraForward;
// 					// reward -= .005f;
// 			} 
//             if (movement == 5)//rotate left
//             { 
// 					RotateTowards(agentRB.position - area.cameraRight);
// 					// reward -= .005f;
// 			} 
//             if (movement == 6)//rotate right
//             { 
// 					RotateTowards(agentRB.position + area.cameraRight);
// 					// reward -= .005f;
// 			} 
//             if (movement == 7)//rotate away from cam
//             { 
// 					RotateTowards(agentRB.position - area.cameraForward);
// 					// reward -= .005f;
// 			} 
//             if (movement == 8)//rotate towards cam
//             { 
// 					RotateTowards(agentRB.position + area.cameraForward);
// 					// reward -= .005f;
// 			} 
//             if (movement == 9)//do nothing
//             { 
// 				//not a thing
// 			} 
//             // if (movement == 10) //go leftForward
//             // { 
// 			// 		dirToGo = -area.cameraRight + area.cameraForward;
// 			// 		// reward -= .005f;
// 			// 		// print("leftforward: " + (-area.cameraRight + area.cameraForward));
// 			// 		// print("forward: "  + area.cameraForward);
//             // } 
//             // if (movement == 11)//go leftBack
//             // { 
// 			// 		dirToGo = -area.cameraRight - area.cameraForward;
// 			// 		// reward -= .005f;
//             //  } 
//             // if (movement == 12)//go rightforward
//             // { 
// 			// 		dirToGo = area.cameraRight + area.cameraForward;
// 			// 		// reward -= .005f;
//             // } 
//             // if (movement == 13)//go rightback
//             // { 
// 			// 		dirToGo = area.cameraRight - area.cameraForward;
// 			// 		// reward -= .005f;
// 			// } 
//         	agentRB.AddForce(dirToGo * area.agentRunSpeed, ForceMode.VelocityChange);
//         	// agentRB.AddForce(dirToGo * area.agentRunSpeed, ForceMode.Acceleration);

//             // if (movement == 5 && agentRB.velocity.y <= 0) { dirToGo = transform.up; } //go up (jump)
//         	// agentRB.AddForce(new Vector3(dirToGo.x * 40f, dirToGo.y * 300f, dirToGo.z * 40f), ForceMode.Acceleration);
//         }

 
//         if (agentRB.velocity.sqrMagnitude > 25f) //if we're going higher than this vel...slow the agent down
//         {
//             agentRB.velocity *= 0.95f;
//         }
//     }

// 	// void UpdateRaycastInfo()
// 	// {
// 	// 	groundCheckDirForward = transform.forward - transform.up;
// 	// 	groundCheckDirBack = -transform.forward - transform.up;
// 	// 	groundCheckDirLeft = -transform.right - transform.up;
// 	// 	groundCheckDirRight = transform.right - transform.up;
// 	// }

// 	// void GroundCheckPlayerAndBall()
// 	// {

// 	// }



// 	public override void AgentStep(float[] act)
// 	{
//         // reward -= 0.001f; //don't waste time
//         // reward -= 0.005f; //don't waste time

// 		// // print(blockRB.velocity.sqrMagnitude);



// 		// if(area.ballRB.velocity.sqrMagnitude < 1)//ball isn't being kicked
// 		// {  
// 		// 	// float punishAmount = 1 - area.ballRB.velocity.sqrMagnitude;// the slower it goes the more punishment
// 		// 	// reward -= punishAmount;

//         // 	reward -= .01f; //lets see some hustle out there
// 			if(area.ballRB.velocity.sqrMagnitude < .1f) //ball is almost completely stopped
// 			{
// 				if(area.canResetBall)
// 				{
// 					// print("resetting ball");
//         			// reward -= .01f; //lets see some hustle out there * i think this was the good one?
//         			reward -= .1f; //lets see some hustle out there
//         			// reward -= 1f; //lets see some hustle out there
// 					area.ResetBall();
// 					done = true; //done
// 				}
//         	// reward = -1f; //lets see some hustle out there
// 			}
// 		// }





// 		// if(area.ballRB.velocity.sqrMagnitude < .01)//ball isn't being kicked
// 		// {
// 		// 	area.ResetBall();
// 		// 	done = true; //done
//         // 	reward = -0.01f; //lets see some hustle out there
// 		// }

// 		// UpdateRaycastInfo();
//         MoveAgent(act); //perform agent actions
// 		// print(agentRB.angularVelocity.sqrMagnitude);
// 		// if(foundBlock && foundGoal)
// 		// {
// 		// 	RotateTowards(block.transform.position); //rotate towards the block
// 		// }

// 		// if (!Physics.Raycast(agentRB.position, Vector3.down, 7, area.groundLayer)) //if the agent or the block has gone over the edge, we done.
// 		// {
// 		// 	done = true; //done
// 		// 	// reward -= 1f; // BAD AGENT
// 		// }
// 	}


// 	// void OnCollisionEnter(Collision col)
// 	// {
// 	// 	// if(col.gameObject.CompareTag("ball")) //stay away from the wall
// 	// 	// {
// 	// 	// 	if(col.relativeVelocity.sqrMagnitude/1000 > .75f)
// 	// 	// 	{
// 	// 	// 		reward += .25f;
// 	// 	// 	}
// 	// 	// 	// print(col.relativeVelocity.sqrMagnitude/1000);
// 	// 	// 	// reward -= 1f; // BAD AGENT
// 	// 	// 	// done = true; //done
// 	// 	// }
// 	// 	if(col.gameObject.CompareTag("wall")) //stay away from the wall
// 	// 	{
// 	// 		reward -= 1f; // BAD AGENT
// 	// 		// done = true; //done
// 	// 	}
// 	// }

// 	public override void AgentReset()
// 	{
// 		// if(agentMustFindObjs)
// 		// {
// 		// 	foundBlock = false;
// 		// 	foundGoal = false;
// 		// }
// 		// else
// 		// {
// 		// 	foundBlock = true;
// 		// 	foundGoal = true;
// 		// }
// 		// lookingForBlock = false;
// 		// foundBlock = false;
// 		// needToFindGoal = true;
// 		// lookingForGoal = false;
// 		// foundGoal = false;
// 		transform.position =  area.GetRandomSpawnPos();
// 		agentRB.velocity = Vector3.zero; //we want the agent's vel to return to zero on reset
// 		// needToFindBlock = true;
//         // area.ResetArea();
// 	}

// 	public override void AgentOnDone()
// 	{

// 	}
// }
