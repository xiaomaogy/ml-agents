// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

//     public enum Team
//     {
//         red, blue
//     }
// // public class PushAgent : AreaAgent
// public class SoccerAgent : Agent
// {
// 	public SoccerFieldArea area;
// 	public SoccerGoal targetGoal; //this is the goal we want to kick the ball towards(the opponent's)
//     // GameObject ball;
//     public SoccerAgent opponent;
//     [HideInInspector]
// 	public Rigidbody agentRB;
//     [HideInInspector]
// 	public Rigidbody ballRB;
// 	public float turnSpeed;
// 	public float xForce;
// 	public float yForce;
// 	public float zForce;

//     // public float previousDistFromBallToGoalSqrMagnitude;
//     // public float previousDistFromAgentToBallSqrMagnitude;
//     // public float distFromBallToGoalSqrMagnitude;
//     // public float distFromAgentToBallSqrMagnitude;
//     // public float distFromAgentToGoalSqrMagnitude;
//     // public Vector3 dirFromBallToGoal;
//     // public Vector3 dirFromAgentToBall;
//     // public Vector3 dirFromAgentToOpponent;
//     // public Vector3 dirFromAgentToGoal;
//     public Vector3 startingPos;
//     public Team team;

//     public float strengthContinuous = 100;
//     void Awake()
//     {
//         if(team == Team.red)
//         {
//             area.redPlayers.Add(this);
//         }
//         else if(team == Team.blue)
//         {
//             area.bluePlayers.Add(this);
//         }

// 		agentRB = GetComponent<Rigidbody>(); //cache the RB
// 		ballRB = area.ball.GetComponent<Rigidbody>(); //cache the RB
//         startingPos = transform.position;
//         // targetGoal.opponentAgent = this; //let the goal know I am 
//     }
    
//     public override void InitializeAgent()
//     {
// 		base.InitializeAgent();
//     }

//     void CollectVector3State(List<float> state, Vector3 v)
//     {
//         state.Add(v.x);
//         state.Add(v.y);
//         state.Add(v.z);
//     }
// 	public override List<float> CollectState()
// 	{
// 		List<float> state = new List<float>();
//         //get dirs
//         Vector3 dirFromBallToTargetGoal = Vector3.zero;
//         Vector3 dirFromBallToOwnGoal = Vector3.zero;

//         Vector3 dirFromAgentToBall = ballRB.transform.position - agentRB.transform.position;
//         // Vector3 dirFromAgentToOpponent = opponent.agentRB.transform.position - agentRB.transform.position;

//         Vector3 dirFromAgentToTargetGoal = Vector3.zero;
//         Vector3 dirFromAgentToOwnGoal = Vector3.zero;
//         if(team == Team.red)
//         {
//             dirFromBallToTargetGoal = area.blueGoal.transform.position - ballRB.transform.position;
//             dirFromBallToOwnGoal = area.redGoal.transform.position - ballRB.transform.position;
//             dirFromAgentToTargetGoal = area.blueGoal.transform.position - agentRB.transform.position;
//             dirFromAgentToOwnGoal = area.redGoal.transform.position - agentRB.transform.position;
//         }
//         else if(team == Team.blue)
//         {
//             dirFromBallToTargetGoal = area.redGoal.transform.position - ballRB.transform.position;
//             dirFromBallToOwnGoal = area.blueGoal.transform.position - ballRB.transform.position;
//             dirFromAgentToTargetGoal = area.redGoal.transform.position - agentRB.transform.position;
//             dirFromAgentToOwnGoal = area.blueGoal.transform.position - agentRB.transform.position;
//         }
//         dirFromBallToTargetGoal.y = 0;
//         dirFromBallToOwnGoal.y = 0;

//         Vector3 dirFromAreaCenterToAgent = transform.position - area.transform.position;
//         // Vector3 dirFromAreaCenterToOpponent = opponent.transform.position - area.transform.position;
//         // Vector3 dirFromAreaCenterToTargetGoal = opponent.transform.position - area.transform.position;
//         Vector3 dirFromAreaCenterToBall = ballRB.transform.position - area.transform.position;
//         // dirFromAgentToGoal = goalHolder.transform.position - agentRB.transform.position;


//         CollectVector3State(state, dirFromBallToTargetGoal);
//         CollectVector3State(state, dirFromBallToOwnGoal);
//         CollectVector3State(state, dirFromAgentToBall);
//         // CollectVector3State(state, dirFromAgentToOpponent);
//         CollectVector3State(state, dirFromAgentToTargetGoal);
//         CollectVector3State(state, dirFromAgentToOwnGoal);

//         CollectVector3State(state, dirFromAreaCenterToAgent);
//         // CollectVector3State(state, dirFromAreaCenterToOpponent);
//         CollectVector3State(state, dirFromAreaCenterToBall);
//         CollectVector3State(state, agentRB.velocity);
//         CollectVector3State(state, ballRB.velocity);
//         // CollectVector3State(state, opponent.agentRB.velocity);

//         //get magnitudes
//         // distFromBallToGoalSqrMagnitude = dirFromBallToGoal.sqrMagnitude;
//         // distFromAgentToBallSqrMagnitude = dirFromAgentToBall.sqrMagnitude;
//         // distFromAgentToGoalSqrMagnitude = dirFromAgentToGoal.sqrMagnitude;
      
//         // //add magnitudes
//         // state.Add(distFromBallToGoalSqrMagnitude);
//         // state.Add(distFromAgentToBallSqrMagnitude);
//         // state.Add(distFromAgentToGoalSqrMagnitude);

//         // Vector3 plus5 = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5);
//         // Debug.DrawRay(plus5,  transform.up, Color.green, 1f);


//         // //add positions relative to the area
//         // state.Add(dirFromAreaCenterToAgent.x);
//         // state.Add(dirFromAreaCenterToAgent.y);
//         // state.Add(dirFromAreaCenterToAgent.z);

//         // //get opponents positions relative to the area
//         // state.Add(dirFromAreaCenterToOpponent.x);
//         // state.Add(dirFromAreaCenterToOpponent.y);
//         // state.Add(dirFromAreaCenterToOpponent.z);

//         // state.Add(dirFromAreaCenterToTargetGoal.x);
//         // state.Add(dirFromAreaCenterToTargetGoal.y);
//         // state.Add(dirFromAreaCenterToTargetGoal.z);

//         // state.Add(dirFromAreaCenterToBall.x);
//         // state.Add(dirFromAreaCenterToBall.y);
//         // state.Add(dirFromAreaCenterToBall.z);


//         // state.Add((ballRB.velocity.x));
//         // state.Add((ballRB.velocity.y));
//         // state.Add((ballRB.velocity.z));

//         // state.Add((agentRB.velocity.x));
//         // state.Add((agentRB.velocity.y));
//         // state.Add((agentRB.velocity.z));

//         // state.Add((opponent.agentRB.velocity.x));
//         // state.Add((opponent.agentRB.velocity.y));
//         // state.Add((opponent.agentRB.velocity.z));


//         // state.Add((block.transform.position.z + 5 - area.transform.position.z));


//         // //add dirs
//         // state.Add(dirFromBallToGoal.x);
//         // state.Add(dirFromBallToGoal.y);
//         // state.Add(dirFromBallToGoal.z);

//         // state.Add(dirFromAgentToBanana.x);
//         // state.Add(dirFromAgentToBanana.y);
//         // state.Add(dirFromAgentToBanana.z);

//         // state.Add(dirFromAgentToGoal.x);
//         // state.Add(dirFromAgentToGoal.y);
//         // state.Add(dirFromAgentToGoal.z);

//         // //add agent rotation
//         // state.Add((agentRB.rotation.eulerAngles.x));
//         // state.Add((agentRB.rotation.eulerAngles.y));
//         // state.Add((agentRB.rotation.eulerAngles.z));

//         // //add monster rotation
//         // state.Add((blockRB.rotation.eulerAngles.x));
//         // state.Add((blockRB.rotation.eulerAngles.y));
//         // state.Add((blockRB.rotation.eulerAngles.z));

// 		// state.Add(agentRB.velocity.x);
// 		// state.Add(agentRB.velocity.y);
// 		// state.Add(agentRB.velocity.z);

// 		// state.Add(blockRB.velocity.x);
// 		// state.Add(blockRB.velocity.y);
// 		// state.Add(blockRB.velocity.z);

//         // state.Add(block.transform.localScale.x);
//         // state.Add(goalHolder.transform.localScale.x);


// 		return state;
// 	}


// 	// public override List<float> CollectState()
// 	// {
// 	// 	List<float> state = new List<float>();
//     //     //get dirs
//     //     dirFromBallToGoal = targetGoal.transform.position - ballRB.transform.position;
//     //     dirFromBallToGoal.y = 0;

//     //     dirFromAgentToBall = ballRB.transform.position - agentRB.transform.position;
//     //     // dirFromAgentToGoal = goalHolder.transform.position - agentRB.transform.position;

//     //     //get magnitudes
//     //     // distFromBallToGoalSqrMagnitude = dirFromBallToGoal.sqrMagnitude;
//     //     // distFromAgentToBallSqrMagnitude = dirFromAgentToBall.sqrMagnitude;
//     //     // distFromAgentToGoalSqrMagnitude = dirFromAgentToGoal.sqrMagnitude;
      
//     //     // //add magnitudes
//     //     // state.Add(distFromBallToGoalSqrMagnitude);
//     //     // state.Add(distFromAgentToBallSqrMagnitude);
//     //     // state.Add(distFromAgentToGoalSqrMagnitude);

//     //     // Vector3 plus5 = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5);
//     //     // Debug.DrawRay(plus5,  transform.up, Color.green, 1f);
//     //     //add positions relative to the area
//     //     state.Add((transform.position.x - area.transform.position.x));
//     //     state.Add((transform.position.y - area.transform.position.y));
//     //     state.Add((transform.position.z - area.transform.position.z)); //dir from ground center to agent
//     //     // state.Add((transform.position.z + 5 - area.transform.position.z)); //dir from ground center to agent

//     //     //get opponents positions relative to the area
//     //     state.Add((opponent.transform.position.x - area.transform.position.x));
//     //     state.Add((opponent.transform.position.y - area.transform.position.y));
//     //     state.Add((opponent.transform.position.z - area.transform.position.z)); //dir from ground center to agent

//     //     state.Add((targetGoal.transform.position.x - area.transform.position.x));
//     //     state.Add((targetGoal.transform.position.y - area.transform.position.y));
//     //     state.Add((targetGoal.transform.position.z - area.transform.position.z));
//     //     // state.Add((goalHolder.transform.position.z + 5 - area.transform.position.z));

//     //     state.Add((ballRB.transform.position.x - area.transform.position.x));
//     //     state.Add((ballRB.transform.position.y - area.transform.position.y));
//     //     state.Add((ballRB.transform.position.z - area.transform.position.z));


//     //     state.Add((ballRB.velocity.x));
//     //     state.Add((ballRB.velocity.y));
//     //     state.Add((ballRB.velocity.z));

//     //     state.Add((agentRB.velocity.x));
//     //     state.Add((agentRB.velocity.y));
//     //     state.Add((agentRB.velocity.z));

//     //     state.Add((opponent.agentRB.velocity.x));
//     //     state.Add((opponent.agentRB.velocity.y));
//     //     state.Add((opponent.agentRB.velocity.z));


//     //     // state.Add((block.transform.position.z + 5 - area.transform.position.z));


//     //     // //add dirs
//     //     // state.Add(dirFromBallToGoal.x);
//     //     // state.Add(dirFromBallToGoal.y);
//     //     // state.Add(dirFromBallToGoal.z);

//     //     // state.Add(dirFromAgentToBanana.x);
//     //     // state.Add(dirFromAgentToBanana.y);
//     //     // state.Add(dirFromAgentToBanana.z);

//     //     // state.Add(dirFromAgentToGoal.x);
//     //     // state.Add(dirFromAgentToGoal.y);
//     //     // state.Add(dirFromAgentToGoal.z);

//     //     // //add agent rotation
//     //     // state.Add((agentRB.rotation.eulerAngles.x));
//     //     // state.Add((agentRB.rotation.eulerAngles.y));
//     //     // state.Add((agentRB.rotation.eulerAngles.z));

//     //     // //add monster rotation
//     //     // state.Add((blockRB.rotation.eulerAngles.x));
//     //     // state.Add((blockRB.rotation.eulerAngles.y));
//     //     // state.Add((blockRB.rotation.eulerAngles.z));

// 	// 	// state.Add(agentRB.velocity.x);
// 	// 	// state.Add(agentRB.velocity.y);
// 	// 	// state.Add(agentRB.velocity.z);

// 	// 	// state.Add(blockRB.velocity.x);
// 	// 	// state.Add(blockRB.velocity.y);
// 	// 	// state.Add(blockRB.velocity.z);

//     //     // state.Add(block.transform.localScale.x);
//     //     // state.Add(goalHolder.transform.localScale.x);


// 	// 	return state;
// 	// }


// 	    public void MoveAgent(float[] act) {

        
//         if (brain.brainParameters.actionSpaceType == StateType.continuous)
//         {
//             Vector3 directionX = transform.right * act[0] * strengthContinuous;
//             Vector3 directionY = transform.up * act[1] * strengthContinuous;
//             Vector3 directionZ = transform.forward * act[2] * strengthContinuous;

//             agentRB.AddForce(directionX + directionZ, ForceMode.Acceleration);
//             // agentRB.AddForce(directionX + directionY + directionZ, ForceMode.Acceleration);
// 		    RotateTowards(ballRB.position);
//         }
//         else
//         {
//             Vector3 dirToGo = Vector3.zero;
//             int movement = Mathf.FloorToInt(act[0]);
//             // print(movement);
//             // print(transform.right);
//             if (movement == 1) { dirToGo = -transform.right; } //go left
//             if (movement == 2) { dirToGo = transform.right; } //go right
//             if (movement == 3) { dirToGo = transform.forward; } //go forward
//             if (movement == 4) { dirToGo = -transform.forward; } //go back
//             if (movement == 5 && agentRB.velocity.y <= 0) { dirToGo = transform.up; } //go up (jump)
//             agentRB.AddForce(new Vector3(dirToGo.x, dirToGo.y, dirToGo.z) * strengthContinuous, ForceMode.Acceleration);
//             // agentRB.AddForce(new Vector3(dirToGo.x * xForce, dirToGo.y * yForce, dirToGo.z * zForce), ForceMode.Acceleration);
// 		    RotateTowards(ballRB.position);
//         }

//         // agentRB.AddForce(new Vector3(directionX * 40f, directionY * 300f, directionZ * 40f));
//         // if (agentRB.velocity.sqrMagnitude > 25f) //slow it down
//         // {
//         //     agentRB.velocity *= 0.95f;
//         // }
//     }

// 	void RotateTowards(Vector3 pos)
// 	{
// 		Vector3 dirToBox = pos - transform.position; //get dir
//         dirToBox.y = 0;
// 		Quaternion targetRotation = Quaternion.LookRotation(dirToBox); //get our needed rotation
// 		agentRB.MoveRotation(Quaternion.Lerp(agentRB.transform.rotation, targetRotation, Time.deltaTime * turnSpeed)); 
// 	}

// 	public override void AgentStep(float[] act)
// 	{
//         // if(agentRB.velocity.sqrMagnitude < 1)
//         // {
//         //     reward -= 0.005f;
//         // }
//         if(ballRB.velocity.sqrMagnitude < 1)
//         {
//             reward -= 0.005f;
//         }
//         // reward -= 0.01f;
//         MoveAgent(act);

//         // //if ball closer to goal you get a reward
//         // if(distFromBananaToGoalSqrMagnitude < previousDistFromBananaToGoalSqrMagnitude){ reward += .01f;}
//         // previousDistFromBananaToGoalSqrMagnitude = distFromBananaToGoalSqrMagnitude;

//         // //if player closer to nanner you get a reward
//         // if(distFromAgentToBananaSqrMagnitude < previousDistFromAgentToBananaSqrMagnitude){reward += .001f;}
//         // previousDistFromAgentToBananaSqrMagnitude = distFromAgentToBananaSqrMagnitude;


//         // // Debug.DrawRay(agentRB.position,  -transform.up, Color.green, 1f);
//         // if (!Physics.Raycast(ballRB.position, -transform.up, 2))
//         // {
// 		// 	done = true;
// 		// 	reward += 1f; //REWARDED FOR PUSHING BANANA OFF THE SIDE
//         // }
//         // Debug.DrawRay(agentRB.position,  -transform.up, Color.green, 1f);
//         // if (!Physics.Raycast(agentRB.position, -transform.up, 2)) //we've gone over the edge
//         // {
// 		// 	done = true;
// 		// 	reward -= 1f;
//         // }


//         // if (gameObject.transform.position.y < 0.0f || Mathf.Abs(gameObject.transform.position.x - area.transform.position.x) > 8f ||
//         //     Mathf.Abs(gameObject.transform.position.z + 5 - area.transform.position.z) > 8)
// 		// {
// 		// 	done = true;
// 		// 	reward = -1f;
// 		// }
// 	}

// 	public override void AgentReset()
// 	{
//         // float xVariation = GameObject.Find("Academy").GetComponent<PushAcademy>().xVariation;
//         // transform.position = new Vector3(Random.Range(-xVariation, xVariation), 1.1f, -4f) + area.transform.position;
//         // transform.position = new Vector3(Random.Range(-xVariation, xVariation), 1.1f, -8f) + area.transform.position;
//         // transform.position = area.transform.position;
// 		// agentRB.velocity = new Vector3(0f, 0f, 0f);
// 		// agentRB.velocity = new Vector3(0f, 0f, 0f);
// 		// agentRB.velocity = Vector3.zero;
// 		// ballRB.velocity = Vector3.zero;
//         // transform.position = startingPos;
//         // opponent.transform.position = opponent.startingPos;

//         area.ResetArea();
// 	}

// 	public override void AgentOnDone()
// 	{

// 	}
// }
