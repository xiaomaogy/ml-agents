using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAgentRaycaster : Agent
{
	public PushArea area;
	public bool agentMustFindObjs;
	public GameObject goal;
	public bool foundGoal;
    public GameObject block;
	public bool foundBlock;
	Rigidbody blockRB;
	Rigidbody agentRB;
	public LayerMask groundLayer;
 
	Vector3 groundCheckDirForward;
	Vector3 groundCheckDirRight;
	Vector3 groundCheckDirLeft;
	Vector3 groundCheckDirBack;
	public float visionDistance;
	public float sphereCastRadius = 1;
	public float turnPlayerTorque = 15;
	public float walkSpeed = 50;
	// public float team; //1 if blue, 0 if red
	
	public RaycastHit[] sphereCastHits = new RaycastHit[5]; 
    public override void InitializeAgent()
    {
		agentRB	= GetComponent<Rigidbody>(); //cache the agent rigidbody
		blockRB = block.GetComponent<Rigidbody>();
		if(agentMustFindObjs)
		{
			foundBlock = false;
			foundGoal = false;
		}
		else
		{
			foundBlock = true;
			foundGoal = true;
		}
		base.InitializeAgent();
    }

	//Add the x,y,z components of this Vector3 to the state list
    void CollectVector3State(List<float> state, Vector3 v)
    {
        state.Add(v.x);
        state.Add(v.y);
        state.Add(v.z);
    }

	void CollectRotationState(List<float> state, Quaternion q)
    {
        state.Add(q.x);
        state.Add(q.y);
        state.Add(q.z);
    }

	//we can only collect floats in CollecState so we need to convert bools to floats
	float ConvertBoolToFloat(bool b)
	{
		float f = b == true? 1 : 0;
		return f;
	}

	//put whatever angles you want checked in here
	Vector3[] GetRayCastDirs()
	{
		Vector3 randomDirForwardRight = (transform.forward + transform.right/Random.Range(1,5)).normalized; //get dir at random that is roughly forward + right
		// Debug.DrawRay(transform.position, randomDirForwardRight * 5, Color.magenta, 0f, true);
		Vector3 randomDirForwardLeft = (transform.forward - transform.right/Random.Range(1,5)).normalized; //get dir at random that is roughly forward + left
		// Debug.DrawRay(transform.position, randomDirForwardLeft * 5, Color.green, 0f, true);

		// Debug.DrawRay(transform.position, transform.forward * 5, Color.green, 0f, true);

		Vector3[] sphereCastDirs = {randomDirForwardLeft, randomDirForwardRight, transform.forward};
		return sphereCastDirs;

	}

	void LookForThangs()
	{
		foreach(Vector3 dir in GetRayCastDirs())
		{
			Debug.DrawRay(transform.position + transform.forward, dir * visionDistance, Color.green, 0f, true);
			int hitObjs = Physics.SphereCastNonAlloc(transform.position + transform.forward, sphereCastRadius, dir, sphereCastHits, visionDistance);
			if(hitObjs > 0) //found the block associated with this area
			{
				foreach(RaycastHit hit in sphereCastHits)
				{
					if(hit.collider != null)
					{
						if(hit.collider.gameObject == goal)
						{
							foundGoal = true;
						}
						if(hit.collider.gameObject == block)
						{
							foundBlock = true;
							blockRB = hit.collider.gameObject.GetComponent<Rigidbody>();
						}
					}
				}
			}
		}
	}


  	public override List<float> CollectState()
    {
        List<float> state = new List<float>();


		//*NOTE:  (dir & velocity defaults to [0,0,0] if not found) */
		
		CollectVector3State(state, agentRB.velocity);
		CollectVector3State(state, agentRB.angularVelocity);
		CollectRotationState(state, transform.rotation); //agent's rotation
		if(agentMustFindObjs)
		{
			if(!foundGoal || !foundBlock)
			{
				LookForThangs();
			}
		}

		if(!foundGoal)
		{
			float[] goalSubList = { 0f,0f,0f,0f};  // 4 STATES. NOTHING FOUND. [0] = found?, [1,2,3] = dir goal to player x/y/z.
			state.AddRange(goalSubList);
		}
		else
		{
			Vector3 agentPosRelToGoal = transform.position - goal.transform.position;
			float[] goalSubList = { 1f,agentPosRelToGoal.x, agentPosRelToGoal.y,agentPosRelToGoal.z};  //4 STATES WE KNOW WHERE THE GOAL IS NOW [0] = found?, [1,2,3] = dir goal to player x/y/z.
			state.AddRange(goalSubList);
		}
		if(!foundBlock)
		{
			float[] blockSubList = { 0f,0f,0f,0f,0f,0f,0f}; //7 STATES. [0] = found?, [1,2,3] = dir player to block, [4,5,6] = vel of block
			state.AddRange(blockSubList);
		}
		else
		{
			Vector3 blockPosRelToGoal = blockRB.position - goal.transform.position;
			Vector3 blockPosRelToAgent = blockRB.position - transform.position;
			// float[] blockSubList = {1f,blockPosRelToAgent.x, blockPosRelToAgent.y, blockPosRelToAgent.z, blockRB.velocity.x, blockRB.velocity.y, blockRB.velocity.z};  // 7 STATES. WE KNOW WHERE THE BLOCK IS NOW //[0] = found?, [1,2,3] = dir player to block, [4,5,6] = vel of block.
			float[] blockSubList = {1f,blockPosRelToAgent.x, blockPosRelToAgent.y, blockPosRelToAgent.z, blockRB.velocity.x, blockRB.velocity.y, blockRB.velocity.z, blockPosRelToGoal.x, blockPosRelToGoal.y, blockPosRelToGoal.z};  // 7 STATES. WE KNOW WHERE THE BLOCK IS NOW //[0] = found?, [1,2,3] = dir player to block, [4,5,6] = vel of block.
			state.AddRange(blockSubList);
		}
				
        return state;
    }

	//Always face the block. WE ARE SETTING THE PLAYER'S ROTATION, NOT ML AGENTS
	void RotateTowards(Vector3 pos)
	{
		Vector3 dirToBox = pos - transform.position; //get dir to block
        dirToBox.y = 0; //to prevent the player from rotating up or down we need to zero out the y
		Quaternion targetRotation = Quaternion.LookRotation(dirToBox); //get our needed rotation
		agentRB.MoveRotation(Quaternion.Lerp( agentRB.transform.rotation, targetRotation, Time.deltaTime * 15)); //ROTATE BRO
	}


	public void MoveAgent(float[] act) {

        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {

			// Vector3 directionX = transform.right * Mathf.Clamp(act[0], -1f, 1f);
            // Vector3 directionY = transform.up * Mathf.Clamp(act[1], -1f, 1f);
            // Vector3 directionZ = transform.forward * Mathf.Clamp(act[2], -1f, 1f);
            // if (agentRB.velocity.y > 0) { directionY = 0f; }
            // agentRB.AddForce(directionX + directionZ, ForceMode.Acceleration);

			float directionX = 0;
			float directionZ = 0;
			float directionY = 0;

            directionX = Mathf.Clamp(act[0], -1f, 1f);
            directionZ = Mathf.Clamp(act[1], -1f, 1f);
            directionY = Mathf.Clamp(act[2], -1f, 1f);
            if (agentRB.velocity.y > 0) { directionY = 0f; }
        	agentRB.AddForce(new Vector3(directionX * 40f, directionY * 300f, directionZ * 40f));
        }
        else
        {
			Debug.DrawRay(transform.position, groundCheckDirForward, Color.green, 0f, true);
			Debug.DrawRay(transform.position, groundCheckDirBack, Color.green, 0f, true);
			Debug.DrawRay(transform.position, groundCheckDirLeft, Color.green, 0f, true);
			Debug.DrawRay(transform.position, groundCheckDirRight, Color.green, 0f, true);
			// Debug.DrawRay(transform.position, transform.forward * 2, Color.red, 0f, true);

        	Vector3 dirToGo = Vector3.zero; //Start with a zero Vector
            int movement = Mathf.FloorToInt(act[0]);
            if (movement == 1) //go left
            { 
                dirToGo = -transform.right; 
                // RotateTowards(-transform.right * 5f);
            } 
            if (movement == 2)//go right
            { 
                dirToGo = transform.right;
                // RotateTowards(transform.right * 5f);
             } 
            if (movement == 3)//go forward
            { 
                dirToGo = transform.forward;
                // RotateTowards(transform.forward * 5f);
            } 
            if (movement == 4)//go back
            { 
                dirToGo = -transform.forward;
                // RotateTowards(-transform.forward * 5f);
			} 
            if (movement == 5)//go back
            { 
				if(!foundGoal || !foundBlock)
				{
					agentRB.AddTorque(transform.up * turnPlayerTorque, ForceMode.VelocityChange);
					// print("turn left");
					//MAYBE PUT A TORQUE PENALTY
				}
			} 
            if (movement == 6)//go back
            { 
				if(!foundGoal || !foundBlock)
				{
					agentRB.AddTorque(transform.up * -turnPlayerTorque, ForceMode.VelocityChange);
					// print("turn left");
					//MAYBE PUT A TORQUE PENALTY
				}
			} 
        	agentRB.AddForce(dirToGo * walkSpeed, ForceMode.Acceleration);

            // if (movement == 5 && agentRB.velocity.y <= 0) { dirToGo = transform.up; } //go up (jump)
        	// agentRB.AddForce(new Vector3(dirToGo.x * 40f, dirToGo.y * 300f, dirToGo.z * 40f), ForceMode.Acceleration);
        }

 
        if (agentRB.velocity.sqrMagnitude > 25f) //if we're going higher than this vel...slow the agent down
        {
            agentRB.velocity *= 0.95f;
        }
    }

	// void UpdateRaycastInfo()
	// {
	// 	groundCheckDirForward = transform.forward - transform.up;
	// 	groundCheckDirBack = -transform.forward - transform.up;
	// 	groundCheckDirLeft = -transform.right - transform.up;
	// 	groundCheckDirRight = transform.right - transform.up;
	// }
	public override void AgentStep(float[] act)
	{
        reward = -0.004f;
		// UpdateRaycastInfo();
        MoveAgent(act); //perform agent actions
		if(foundBlock && foundGoal)
		{
			RotateTowards(block.transform.position); //rotate towards the block
		}

		if (!Physics.Raycast(agentRB.position, Vector3.down, 7, groundLayer) || !Physics.Raycast(blockRB.position, Vector3.down, 7, groundLayer)) //if the agent or the block has gone over the edge, we done.
		{
			done = true; //done
			reward -= 1f; // BAD AGENT
		}
	}

	public override void AgentReset()
	{
		if(agentMustFindObjs)
		{
			foundBlock = false;
			foundGoal = false;
		}
		// else
		// {
		// 	foundBlock = true;
		// 	foundGoal = true;
		// }
		// lookingForBlock = false;
		// foundBlock = false;
		// needToFindGoal = true;
		// lookingForGoal = false;
		// foundGoal = false;
		// transform.position =  area.GetRandomSpawnPos();
		agentRB.velocity = Vector3.zero; //we want the agent's vel to return to zero on reset
		// needToFindBlock = true;
        area.ResetArea();
	}

	public override void AgentOnDone()
	{

	}
}
