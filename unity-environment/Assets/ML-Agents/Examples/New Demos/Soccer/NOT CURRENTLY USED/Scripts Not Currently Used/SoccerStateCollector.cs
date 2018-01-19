// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// 	public class PlayerSate
// 	{
// 		public int playerID;
// 		public float teamFloat;
// 		public List<float> stateData = new List<float>();

// 	}
// public class SoccerStateCollector : MonoBehaviour
// {
// 	public GameObject blueGoal;
// 	public GameObject redGoal;
//     public GameObject ball;
// 	public List<AgentSoccer> bluePlayers = new List<AgentSoccer>();
// 	public List<AgentSoccer> redPlayers = new List<AgentSoccer>();
// 	Rigidbody ballRB;
// 	public float blueTeamID; //1 if blue, 0 if red
// 	public float redTeamID; //1 if blue, 0 if red

// 	void Awake()
// 	{
// 		foreach(AgentSoccer agent in bluePlayers){agent.stateCollector = this;}
// 		foreach(AgentSoccer agent in redPlayers){agent.stateCollector = this;}
// 		ballRB = ball.GetComponent<Rigidbody>();
// 	}


// 	//Add the x,y,z components of this Vector3 to the state list
//     void CollectVector3State(List<float> state, Vector3 v)
//     {
//         state.Add(v.x);
//         state.Add(v.y);
//         state.Add(v.z);
//     }

// 	void CollectRotationState(List<float> state, Quaternion q)
//     {
//         state.Add(q.x);
//         state.Add(q.y);
//         state.Add(q.z);
//     }

// 	//we can only collect floats in CollecState so we need to convert bools to floats
// 	float ConvertBoolToFloat(bool b)
// 	{
// 		float f = b == true? 1 : 0;
// 		return f;
// 	}

// 	void CollectStateList(List<float> state, List<AgentSoccer> list, GameObject goal)
// 	{
// 		foreach(AgentSoccer p in list)
// 		{
// 			CollectVector3State(state, p.agentRB.velocity);
// 			CollectRotationState(state, p.agentRB.rotation); //agent's rotation
// 			Vector3 agentPosRelToGoal = p.agentRB.position - goal.transform.position;
// 			CollectVector3State(state, agentPosRelToGoal);
// 		}
// 		Vector3 ballPosRelToGoal = ballRB.position - goal.transform.position;
// 		Vector3 ballPosRelToAgent = ballRB.position - transform.position;
// 		// state.Add(1f); //found
// 		CollectVector3State(state, ballPosRelToGoal);
// 		CollectVector3State(state, ballPosRelToGoal);
// 		CollectVector3State(state, ballRB.velocity);
// 	}

// 	void FixedUpdate()
// 	{
//         List<float> state = new List<float>();

// 		if(bluePlayers.Count > 0)
// 		{
// 			state.Add(blueTeamID); //which team
// 			CollectStateList(state, bluePlayers, redGoal); //blue players want to target the red goal
// 		}

// 		if(redPlayers.Count > 0)
// 		{
// 			state.Add(redTeamID); //which team
// 			CollectStateList(state, redPlayers, blueGoal);
// 		}
// 	}
// }
