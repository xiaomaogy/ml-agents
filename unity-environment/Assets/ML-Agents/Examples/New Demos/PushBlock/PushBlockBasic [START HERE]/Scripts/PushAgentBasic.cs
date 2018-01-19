//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAgentBasic : Agent
{
	public float agentRunSpeed = 3; //magnitude of the run velocity
	public float agentTurningTorque = 3; //the torque to use when turning the agent
	public GameObject ground; //ground game object. we will use the area bounds to spawn the blocks
	[HideInInspector]
	public Bounds areaBounds; //the bounds of the pushblock area

	public GameObject goal; //goal to push the block to
    public GameObject block; //the orange block we are going to be pushing
	[HideInInspector]
	public GoalDetect goalDetect; //this script detects when the block touches the goal
	// public LayerMask groundLayer; //layer the ground is on. used for raycasts to detect when we've fallen off the edge of the platform. If we fall off we will penalize the agent
	Rigidbody blockRB;  //cached on initialization
	Rigidbody agentRB;  //cached on initialization
	public float spawnAreaMarginMultiplier; //ex: .9 means 90% of spawn area will be used.... .1 margin will be left (so players don't spawn off of the edge). the higher this value, the longer training time required
    public Material goalScoredMaterial; //when a goal is scored the ground will use this material for a few seconds.
    public Material failMaterial; //when fail, the ground will use this material for a few seconds. 
	Material groundMaterial; //cached on Awake()
	Renderer groundRenderer;
	Vector3 goalStartingPos;

	void Awake()
	{
		goalStartingPos = goal.transform.position; //cached goal starting Pos in case we want to remember that
		brain = FindObjectOfType<Brain>(); //only one brain in the scene so this should find our brain. BRAAAINS.
		//The action size can be set manually in the inspector, but we're setting it in Awake for convenience.
		if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
			brain.brainParameters.actionSize = 3;
		}
		// else if (brain.brainParameters.actionSpaceType == StateType.discrete) //not used since we're 
		// {
		// 	brain.brainParameters.actionSize = 7;
		// }
	}

    public override void InitializeAgent()
    {
		base.InitializeAgent();
		goalDetect = block.GetComponent<GoalDetect>();
		goalDetect.agent = this; 
		agentRB	= GetComponent<Rigidbody>(); //cache the agent rigidbody
		blockRB	= block.GetComponent<Rigidbody>(); //cache the block rigidbody
		areaBounds = ground.GetComponent<Collider>().bounds; //get the ground's bounds
		groundRenderer = ground.GetComponent<Renderer>(); //get the ground renderer so we can change the material when a goal is scored
		groundMaterial = groundRenderer.material; //starting material

    }


	//Takes a Vector3 and adds it to the state list
    void CollectVector3State(List<float> state, Vector3 v)
    {
        state.Add(v.x);
        state.Add(v.y);
        state.Add(v.z);
    }

	//Takes a Transform and adds the rotation to the state list
    void CollectRotationState(List<float> state, Transform t)
    {
		state.Add(t.rotation.eulerAngles.x/180.0f-1.0f);
		state.Add(t.rotation.eulerAngles.y/180.0f-1.0f);
		state.Add(t.rotation.eulerAngles.z/180.0f-1.0f);
    }

	public override List<float> CollectState()
	{
		// List<float> state = new List<float>();

		Vector3 agentPosRelToGoal = agentRB.position - goal.transform.position; //vector to agent from goal
		Vector3 blockPosRelToGoal = blockRB.position - goal.transform.position; //vector to blockRB from goal
		Vector3 blockPosRelToAgent = blockRB.position - agentRB.position; //vector to blockRB from agent
		Vector3 goalPos = goal.transform.position - ground.transform.position;  //pos of goal rel to ground
		Vector3 blockPos = blockRB.transform.position - ground.transform.position;  //pos of goal rel to ground
		// Vector3 agentPos = agentRB.position - ground.transform.position;  //pos of agent rel to ground

		//COLLECTING 18 STATES
		// CollectVector3State(state, agentPos);  //pos of agent rel to ground
		CollectVector3State(state, goalPos);  //pos of goal rel to ground
		CollectVector3State(state, blockPos);  //pos of goal rel to ground
		CollectVector3State(state, agentPosRelToGoal);  //vector to agent from goal
		CollectVector3State(state, blockPosRelToGoal); //vector to blockRB from goal
		CollectVector3State(state, blockPosRelToAgent);  //vector to blockRB from agent
		CollectVector3State(state, blockRB.velocity); //block's vel
		CollectVector3State(state, agentRB.velocity); //agent's vel
		CollectRotationState(state, agentRB.transform); //agent's rotation
		// state.Add(blockPosRelToGoal.sqrMagnitude);


		return state;
		// CollectVector3State(state, agentPosRelToGoal.normalized); 
		// CollectVector3State(state, blockPosRelToGoal.normalized); 
		// CollectVector3State(state, blockPosRelToAgent.normalized); 
	}



	//use the ground's bounds to pick a random spawn pos
    public Vector3 GetRandomGoalPos()
    {
        Vector3 randomGoalPos = Vector3.zero;
        float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
        float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier);
        // float randomPosX = Random.Range(-areaBounds.extents.x, areaBounds.extents.x);
        // float randomPosZ = Random.Range(-areaBounds.extents.z, areaBounds.extents.z);
        randomGoalPos = ground.transform.position + new Vector3(randomPosX, goalStartingPos.y - ground.transform.position.y, randomPosZ ); //kind of a dumb way to do this. fix it later.
        return randomGoalPos;
    }

	//use the ground's bounds to pick a random spawn pos
    public Vector3 GetRandomSpawnPos()
    {
        Vector3 randomSpawnPos = Vector3.zero;
        float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
        float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier);
        randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1.5f, randomPosZ );
        return randomSpawnPos;
    }

	//woot
	public void IScoredAGoal()
	{
		reward += 1; //you get a point
		done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
		StartCoroutine(GoalScoredSwapGroundMaterial(goalScoredMaterial, 2)); //swap ground material for a bit to indicate we scored.

	}

	//swap ground material, wait time seconds, then swap back to the regular ground material.
	IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
	{
		groundRenderer.material = mat;
		yield return new WaitForSeconds(time); //wait for 2 sec
		groundRenderer.material = groundMaterial;
	}


	public void MoveAgent(float[] act) 
	{
	
		//AGENT ACTIONS
		// this is where we define the actions our agent can use...stuff like "go left", "go forward", "turn" ...etc.

		//Continuous control Vs. Discrete control

		//If we're using Continuous control you will need to change the Action
        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
			//Continuous control means we are letting the neural network set the direction on a sliding scale. 
			//We will define the number of "slots" we want to use here. In this example we need 3 "slots" to define:
				//right/left movement (act[0])
				//forward/back movement (act[1])
				//rotate right/left movement (act[2])
				
				//Example: One agent action is that the agent can go right or left. It is defined in this line:
					//Vector3 directionX = Vector3.right * Mathf.Clamp(act[0], -1f, 1f);

					//The neural network is setting the act[0] value using a float in between -1 & 1. 
					//If it chooses 1 then the agent will go right. 
					//If it chooses -1 the agent will go left. 
					//If it chooses .42 then it will go a little bit right
					//If it chooses -.8 then it will go left (well...80% left)
			
			Vector3 directionX = Vector3.right * Mathf.Clamp(act[0], -1f, 1f);  //go left or right in world space
            Vector3 directionZ = Vector3.forward * Mathf.Clamp(act[1], -1f, 1f); //go forward or back in world space
        	Vector3 dirToGo = directionX + directionZ; 
			agentRB.AddForce(dirToGo * agentRunSpeed, ForceMode.VelocityChange); //GO

			agentRB.AddTorque(transform.up * act[2] * agentTurningTorque, ForceMode.VelocityChange); //turn right or left

        }
        // else
        // {
		// 	//With Discrete control we are hard coding the action. We are setting what it means to go right. 
			
		// 	//first we start off by getting the latest action & converting it into an int. We'll use this int similar to a switch statement
        //     int movement = Mathf.FloorToInt(act[0]);

        // 	Vector3 dirToGo = Vector3.zero; //Start with a zero Vector
		// 	if (movement == 1){ dirToGo = Vector3.left;} //go left in world space
        //     if (movement == 2){ dirToGo = Vector3.right;} //go right in world space
        //     if (movement == 3){ dirToGo = Vector3.forward;} //go forward in world space
        //     if (movement == 4){ dirToGo = - Vector3.forward;} //go back in world space
        //     if (movement == 5){ agentRB.AddTorque(transform.up * agentTurningTorque, ForceMode.VelocityChange);} //turn right
        //     if (movement == 6){ agentRB.AddTorque(transform.up * -agentTurningTorque, ForceMode.VelocityChange );} //turn left
        //     if (movement == 7) //do nothing
		// 	{ 
		// 		//not a thing
		// 	} 
		// 	agentRB.AddForce(dirToGo * agentRunSpeed, ForceMode.VelocityChange); //GO
           
        // }

 
        // if (agentRB.velocity.sqrMagnitude > 25f) //if we're going higher than this vel...slow the agent down
        // {
        //     agentRB.velocity *= 0.95f;
        // }
    }

	public override void AgentStep(float[] act)
	{

        MoveAgent(act); //perform agent actions
		reward -= .001f; // don't waste time
		bool fail = false;  // did the agent or block get pushed off the edge?

		// if (!Physics.Raycast(agentRB.position, Vector3.down, 3, groundLayer)) //if the agent has gone over the edge, we done.
		if (!Physics.Raycast(agentRB.position, Vector3.down, 3)) //if the agent has gone over the edge, we done.
		{
			fail = true; //fell off bro
			reward -= 1f; // BAD AGENT
			transform.position =  GetRandomSpawnPos();
			done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
		}

		// if (!Physics.Raycast(blockRB.position, Vector3.down, 3, groundLayer)) //if the block has gone over the edge, we done.
		if (!Physics.Raycast(blockRB.position, Vector3.down, 3)) //if the block has gone over the edge, we done.
		{
			fail = true; //fell off bro
			reward -= 1f; // BAD AGENT
			ResetBlock(); //reset block pos
			done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
		}

		if (fail)
		{
			StartCoroutine(GoalScoredSwapGroundMaterial(failMaterial, .5f)); //swap ground material to indicate fail
		}
	}

	
	void ResetBlock()
	{
		block.transform.position = GetRandomSpawnPos(); //get a random pos
        blockRB.velocity = Vector3.zero; //reset vel back to zero
        blockRB.angularVelocity = Vector3.zero; //reset angVel back to zero
	}


	//In the editor, if "Reset On Done" is checked then AgentReset() will be called automatically anytime we mark done = true in an agent script.
	public override void AgentReset()
	{
		ResetBlock();
		goal.transform.position = GetRandomGoalPos();
	}
}

