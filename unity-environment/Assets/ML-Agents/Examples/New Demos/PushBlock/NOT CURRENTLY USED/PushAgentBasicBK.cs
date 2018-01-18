//Put on your cube agent and make it a child of the pushblock area

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAgentBasicBK : Agent
{
	// [HideInInspector]
	// public PushArea area; //the pushblock area
	public float agentRunSpeed = 3; //magnitude of the run velocity
	public float agentTurningTorque = 3; //the torque to use when turning the agent
	public GameObject ground; //ground game object. we will use the area bounds to spawn the blocks
	[HideInInspector]
	public Bounds areaBounds; //the bounds of the pushblock area



	public GameObject goal; //goal to push the block to
    public GameObject block; 
	[HideInInspector]
	public GoalDetect goalDetect; //this script detects when the block touches the goal
	public LayerMask groundLayer; //layer the ground is on. used for raycasts to detect when we've fallen off the edge of the platform. If we fall off we will penalize the agent
	// public LayerMask blockLayer;
	// Vector3 blockPos;
	// [HideInInspector]
	public Rigidbody blockRB;  //set on initialization
	// [HideInInspector]
	public Rigidbody agentRB;  //set on initialization
	public float spawnAreaMarginMultiplier; //ex: .9 means 90% of spawn area will be used.... .1 margin will be left (so players don't spawn off of the edge)
	// bool canResetBlock;
	// public float blockIdleTime; //will be used to detect if block has been sitting in one spot for too long
	// public float maxBlockIdleTimeAllowed; //the maximum amount of time a block should sit idle. If the block sits idle too long we ant to penalize the agent. This should help motivate it to push the block
	// public float minWaitTimeToRespawnBlock;
    public Material goalScoredMaterial; //when a goal is scored the ground will use this material for a few seconds.
    public Material failMaterial; //when fail, the ground will use this material for a few seconds. 
	Material groundMaterial;
	Renderer groundRenderer;

	// public float ledgeRaycastDist; //this will be used for ledge detection to learn where ledges are
	// // Vector3 edgeDetectDirForward;
	// // Vector3 edgeDetectDirRight;
	// // Vector3 groundCheckDirLeft;
	// // Vector3 groundCheckDirBack;
	// public bool DrawDebugRays;
	// public bool noGroundFront;
	// public bool noGroundBack;
	// public bool noGroundLeft;
	// public bool noGroundRight;
	// public bool jumping;
	// public float jumpTime;
	// public float fallingForce; //this is a downward force applied when falling to make jumps look less floaty
	// public Collider[] hitGroundColliders = new Collider[1]; //used for groundchecks
	// public bool visualizeGroundCheckSphere;
	// public bool grounded;
	// public bool performingGroundCheck;
	// public float groundCheckRadius; //the radius from transform.position to check
	// public float groundCheckFrequency; //perform a groundcheck every x sec. ex: .5 will do a groundcheck every .5 sec.

	// Vector3 jumpTargetPos; //target this position during jump. it will be 
	// public float jumpHeight = 1; //how high should we jump?
	// public float jumpVelocity = 500; //higher number will result in a quicker jump
	// public float jumpVelocityMaxChange = 10; // don't let the velocity change more than this every tick. this helps smooth out the motion. lower number == slower/more controlled movement. I typically use 10-20 for this value.
	// public float turnTorque; //will be applied to the agent to turn it.

	// Camera camera;
    // [HideInInspector]
	// public Vector3 cameraForward;
    // [HideInInspector]
	// public Vector3 cameraRight;
    // Vector3 lastCameraPos;
    // Quaternion lastCameraRot;


	void Awake()
	{
		//The action size can be set manually in the inspector, but we're setting it in Awake for convenience.
		if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
			brain.brainParameters.actionSize = 3;
		}
		else if (brain.brainParameters.actionSpaceType == StateType.discrete)
		{
			brain.brainParameters.actionSize = 7;
		}
	}
    public override void InitializeAgent()
    {
		base.InitializeAgent();
		// canResetBlock = true;
		// blockIdleTime = 0;
		goalDetect = block.GetComponent<GoalDetect>();
		// goalDetect.agent = this;
		agentRB	= GetComponent<Rigidbody>(); //cache the agent rigidbody
		blockRB	= block.GetComponent<Rigidbody>(); //cache the block rigidbody
		areaBounds = ground.GetComponent<Collider>().bounds; //get the ground's bounds


		// camera = Camera.main;
        // lastCameraPos = camera.transform.position;
        // lastCameraRot = camera.transform.rotation;



        // UpdateCameraDir();

		groundRenderer = ground.GetComponent<Renderer>();
		groundMaterial = groundRenderer.material;



    }



	//Takes a Vector3 and adds it to the state list
    void CollectVector3State(List<float> state, Vector3 v)
    {
        state.Add(v.x);
        state.Add(v.y);
        state.Add(v.z);
    }
    void CollectRotationState(List<float> state, Transform t)
    {
        // state.Add(q.x);
        // state.Add(q.y);
        // state.Add(q.z);
		state.Add(t.rotation.eulerAngles.x/180.0f-1.0f);
		state.Add(t.rotation.eulerAngles.y/180.0f-1.0f);
		state.Add(t.rotation.eulerAngles.z/180.0f-1.0f);
    }

    // void CollectRotationState(List<float> state, Quaternion q)
    // {
    //     state.Add(q.x);
    //     state.Add(q.y);
    //     state.Add(q.z);
    // }

	// //we can only collect floats in CollecState so we need to convert bools to floats
	// float ConvertBoolToFloat(bool b)
	// {
	// 	float f = b == true? 1 : 0;
	// 	return f;
	// }

	// void CameraMoveCheck()
    // {
    //     if (camera.transform.position != lastCameraPos || camera.transform.rotation != lastCameraRot)
    //     {
    //         UpdateCameraDir();
    //     }
    // }

    // public void UpdateCameraDir()
	// {
	// 	cameraForward = camera.transform.forward;
	// 	cameraForward.y = 0;
	// 	cameraForward.Normalize();
	// 	cameraRight = camera.transform.right;
	// 	cameraRight.Normalize();
	// }


	public override List<float> CollectState()
	{
		List<float> state = new List<float>();

		Vector3 agentPosRelToGoal = agentRB.position - goal.transform.position; //vector to agent from goal
		Vector3 blockPosRelToGoal = blockRB.position - goal.transform.position; //vector to blockRB from goal
		Vector3 blockPosRelToAgent = blockRB.position - agentRB.position; //vector to blockRB from agent


		//COLLECTING 15 STATES
		// CollectVector3State(state, agentPosRelToGoal); 
		// CollectVector3State(state, blockPosRelToGoal); 
		// CollectVector3State(state, blockPosRelToAgent); 
		CollectVector3State(state, agentPosRelToGoal.normalized); 
		CollectVector3State(state, blockPosRelToGoal.normalized); 
		CollectVector3State(state, blockPosRelToAgent.normalized); 
		CollectVector3State(state, blockRB.velocity); //block's vel
		CollectVector3State(state, agentRB.velocity); //agent's vel
		// CollectVector3State(state, agentRB.angularVelocity); //agent's ang vel
		CollectRotationState(state, agentRB.transform); //agent's rotation
		// CollectRotationState(state, agentRB.rotation); //agent's rotation

		// //Are the Edge Detection Raycasts hitting or not
		// state.Add(ConvertBoolToFloat(noGroundFront));
		// state.Add(ConvertBoolToFloat(noGroundBack));
		// state.Add(ConvertBoolToFloat(noGroundLeft));
		// state.Add(ConvertBoolToFloat(noGroundRight));



		// state.Add(transform.rotation.y);
		// state.Add(transform.rotation.z);

        // state.Add(block.transform.localScale.x);
        // state.Add(goal.transform.localScale.x);

		// Vector3 randomDirForwardRight = (transform.forward + transform.right/Random.Range(1,5)).normalized; //get dir at random that is roughly forward + right
		// Vector3 randomDirForwardLeft = (transform.forward - transform.right/Random.Range(1,5)).normalized; //get dir at random that is roughly forward + right
		// Debug.DrawRay(transform.position, transform.forward + (transform.right * Random.Range(-1, 1)) * 3, Color.cyan, 0f, true);
		
		
		// Debug.DrawRay(transform.position, randomDirForwardRight * 5, Color.magenta, 0f, true);
		// Debug.DrawRay(transform.position, randomDirForwardLeft * 5, Color.green, 0f, true);
		// Debug.DrawRay(transform.position, transform.forward * 5, Color.green, 0f, true);



		// Debug.DrawRay(transform.position, transform.forward + transform.right * 3, Color.green, 0f, true);
		// Debug.DrawRay(transform.position, transform.forward - transform.right * 3, Color.green, 0f, true);
		// Vector3 dir = transform.forward * 
        // float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
 		// foreach (float angle in rayAngles)
        // {
		// 	Debug.DrawRay(transform.position, dir, Color.green, 0f, true);

		// }


		return state;
	}


	// //Always face the block. WE ARE SETTING THE PLAYER'S ROTATION, NOT ML AGENTS
	// void RotateTowards(Vector3 pos)
	// {
	// 	Vector3 dir = pos - transform.position; //get dir to block
    //     dir.y = 0; //to prevent the player from rotating up or down we need to zero out the y
	// 	Quaternion targetRotation = Quaternion.LookRotation(dir); //get our needed rotation
	// 	agentRB.MoveRotation(Quaternion.Lerp( agentRB.transform.rotation, targetRotation, Time.deltaTime * 15)); //ROTATE BRO
	// }




    public Vector3 GetRandomSpawnPos()
    {
        Vector3 randomSpawnPos = Vector3.zero;
        float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
        // float posY = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
        float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier);
        randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 2, randomPosZ );
        return randomSpawnPos;
    }

	public void IScoredAGoal()
	{
		// print("the block touched the goal");
		reward += 1; //you get a point
		done = true; //ya done
		StartCoroutine(GoalScoredSwapGroundMaterial(goalScoredMaterial, 2));
		// ResetBlock();

	}

	IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
	{
		groundRenderer.material = mat;
		yield return new WaitForSeconds(time); //wait for 2 sec
		groundRenderer.material = groundMaterial;
	}

	// IEnumerator SwapGroundColorForXSec(float sec)
	// {

	// }


	// //moves a rigidbody towards a position with a smooth controlled movement.
	// void MoveTowards(Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
	// {
	// 		Vector3 moveToPos = targetPos - rb.worldCenterOfMass;  //cube needs to go to the standard Pos
	// 		Vector3 velocityTarget = moveToPos * targetVel * Time.deltaTime; //not sure of the logic here, but it modifies velTarget
	// 		if (float.IsNaN(velocityTarget.x) == false) //sanity check. if the velocity is NaN that means it's going way too fast. this check isn't needed for slow moving objs
	// 		{
	// 			rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
	// 		}
		
	// }

	public void MoveAgent(float[] act) {
	
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
        else
        {
			//With Discrete control we are hard coding the action. We are setting what it means to go right. 
			
			//first we start off by getting the latest action & converting it into an int. We'll use this int similar to a switch statement
            int movement = Mathf.FloorToInt(act[0]);

        	Vector3 dirToGo = Vector3.zero; //Start with a zero Vector
			if (movement == 1){ dirToGo = Vector3.left;} //go left in world space
            if (movement == 2){ dirToGo = Vector3.right;} //go right in world space
            if (movement == 3){ dirToGo = Vector3.forward;} //go forward in world space
            if (movement == 4){ dirToGo = - Vector3.forward;} //go back in world space
            if (movement == 5){ agentRB.AddTorque(transform.up * agentTurningTorque, ForceMode.VelocityChange);} //turn right
            if (movement == 6){ agentRB.AddTorque(transform.up * -agentTurningTorque, ForceMode.VelocityChange );} //turn left
            if (movement == 7) //do nothing
			{ 
				//not a thing
			} 
			agentRB.AddForce(dirToGo * agentRunSpeed, ForceMode.VelocityChange); //GO
            // if (movement == 7 && !jumping && grounded) 
			// { 
			// 	StartCoroutine(Jump()); //jump
			// 	// dirToGo = transform.up; 
			// } 
        	// agentRB.AddForce(dirToGo * 50f, ForceMode.Acceleration);

			//Example: 
			// Debug.DrawRay(transform.position, groundCheckDirForward, Color.green, 0f, true);
			// Debug.DrawRay(transform.position, groundCheckDirBack, Color.green, 0f, true);
			// Debug.DrawRay(transform.position, groundCheckDirLeft, Color.green, 0f, true);
			// Debug.DrawRay(transform.position, groundCheckDirRight, Color.green, 0f, true);
			// Debug.DrawRay(transform.position, transform.forward * 2, Color.red, 0f, true);
        	// agentRB.AddForce(new Vector3(dirToGo.x * 50f, dirToGo.y * 300f, dirToGo.z * 50f), ForceMode.Acceleration);
        	// Vector3 rotationDir = Vector3.zero; //Start with a zero Vector

			//THE AGENT CAN CHOOSE THESE 7 AVAILABLE ACTIONS
            // if (movement == 1) //go left
            // { 
            //     dirToGo = -transform.right; 
            // } 
            // if (movement == 2)//go right
            // { 
            //     dirToGo = transform.right;
            //  } 
            // if (movement == 3)//go forward
            // { 
            //     dirToGo = transform.forward;
            // } 
            // if (movement == 4)//go back
            // { 
            //     dirToGo = -transform.forward;
			// } 



			// if (movement == 1) //go left
            // { 
			// 		dirToGo = -cameraRight;
            // } 
            // if (movement == 2)//go right
            // { 
			// 		dirToGo = cameraRight;
            //  } 
            // if (movement == 3)//go forward
            // { 
			// 		dirToGo = cameraForward;
            // } 
            // if (movement == 4)//go back
            // { 
			// 		dirToGo = -cameraForward;
			// } 
        }

 
        if (agentRB.velocity.sqrMagnitude > 25f) //if we're going higher than this vel...slow the agent down
        {
            agentRB.velocity *= 0.95f;
        }
    }

	
	// void UpdateRaycastInfo()
	// {
	// 	edgeDetectDirForward = transform.forward - transform.up;
	// 	groundCheckDirBack = -transform.forward - transform.up;
	// 	groundCheckDirLeft = -transform.right - transform.up;
	// 	edgeDetectDirRight = transform.right - transform.up;
	// 	Vector3 rayOrigin = agentRB.position + transform.up; //start ray from "head"
	// 	// Vector3 rayOrigin = agentRB.position + (transform.up * 2); //start ray from "head"
	// 	if(DrawDebugRays)
	// 	{
	// 		Debug.DrawRay(rayOrigin, edgeDetectDirForward * ledgeRaycastDist, Color.green, 0f, true);
	// 		Debug.DrawRay(rayOrigin, groundCheckDirBack * ledgeRaycastDist, Color.green, 0f, true);
	// 		Debug.DrawRay(rayOrigin, groundCheckDirLeft * ledgeRaycastDist, Color.green, 0f, true);
	// 		Debug.DrawRay(rayOrigin, edgeDetectDirRight * ledgeRaycastDist, Color.green, 0f, true);
	// 	}
	// 	noGroundFront = Physics.Raycast(rayOrigin, edgeDetectDirForward, ledgeRaycastDist, groundLayer);
	// 	noGroundBack = Physics.Raycast(rayOrigin, groundCheckDirBack, ledgeRaycastDist, groundLayer);
	// 	noGroundLeft = Physics.Raycast(rayOrigin, groundCheckDirLeft, ledgeRaycastDist, groundLayer);
	// 	noGroundRight = Physics.Raycast(rayOrigin, edgeDetectDirRight, ledgeRaycastDist, groundLayer);
	// 	// if(noGroundBack || noGr oundFront || noGroundLeft || noGroundRight){reward -= .005f;}

	// 	// if(!Physics.Raycast(transform.up, groundCheckDirForward, 2f, groundLayer)){}
	// }

	// void Update()
	// {
	// 	// CameraMoveCheck();

	// 	if(blockRB.velocity.sqrMagnitude < .1f) //ball is almost completely stopped
	// 	{
	// 		blockIdleTime += Time.deltaTime;
	// 	} 
	// }

	public override void AgentStep(float[] act)
	{
        // reward -= 0.001f;
		// agentRB.angularVelocity = Vector3.zero; //kill any residual angular vel (so we don't keep spinning)
		// agentRB.angularVelocity *= .99f; //we want the agent's vel to return to zero on reset

		// UpdateRaycastInfo(); //perform edge detection raycasts

        MoveAgent(act); //perform agent actions
		// RotateTowards(block.transform.position); //rotate towards the block
		bool fail = false;  // did the agent or block get pushed off the edge?
		if (!Physics.Raycast(agentRB.position, Vector3.down, 3, groundLayer)) //if the agent has gone over the edge, we done.
		{
			fail = true; //fell off bro
			reward -= 1f; // BAD AGENT
			transform.position =  GetRandomSpawnPos();
			done = true; //done

		}
		if (!Physics.Raycast(blockRB.position, Vector3.down, 3, groundLayer)) //if the block has gone over the edge, we done.
		{
			fail = true; //fell off bro
			reward -= 1f; // BAD AGENT
			ResetBlock();
			done = true; //done

		}

		// if(blockIdleTime > maxBlockIdleTimeAllowed)
		// {
		// 	fail = true;
		// 	reward -= .1f; //lets see some hustle out there
		// 	done = true; //done
		// }
		if (fail)
		{
			StartCoroutine(GoalScoredSwapGroundMaterial(failMaterial, .5f)); //failure
		}
	}

    // IEnumerator ResetBlockTimer()
    // {
    //     canResetBlock = false;
    //     yield return new WaitForSeconds(minWaitTimeToRespawnBlock);
    //     canResetBlock = true;
    // }

	void ResetBlock()
	{
		block.transform.position = GetRandomSpawnPos();
        blockRB.velocity = Vector3.zero;
        blockRB.angularVelocity = Vector3.zero;
		// blockIdleTime = 0; //reset the idle timer
		// StartCoroutine(ResetBlockTimer());

	}

	public override void AgentReset()
	{

		// transform.position =  area.GetRandomSpawnPos();
		// transform.position =  GetRandomSpawnPos();
		// agentRB.velocity = Vector3.zero; //we want the agent's vel to return to zero on reset
		ResetBlock();
        // area.ResetArea();
	}



	// public override void AgentOnDone()
	// {

	// }
}
