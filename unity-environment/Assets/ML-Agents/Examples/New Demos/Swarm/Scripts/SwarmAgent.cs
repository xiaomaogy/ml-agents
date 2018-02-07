//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmAgent : Agent
{

	Rigidbody agentRB;  //cached on initialization
	SwarmAcademy academy;

	void Awake()
	{
		academy = FindObjectOfType<SwarmAcademy>();
	}

    public override void InitializeAgent()
    {
		base.InitializeAgent();
		agentRB	= GetComponent<Rigidbody>(); //cache the agent rigidbody

    }


	public override List<float> CollectState()
	{

		Vector3 vectorToSeed = academy.seed.transform.position - agentRB.position; //vector to agent from goal

		//COLLECTING STATES
		MLAgentsHelpers.CollectVector3State(state, vectorToSeed);  //pos of goal rel to ground
		MLAgentsHelpers.CollectRotationState(state, agentRB.transform); //agent's rotation


		RaycastHit hit;
		float didWeHitSomething = 0; //1 if yes, 0 if no
		float hitDistance = academy.agentRaycastDist; //how far away was it. if nothing was hit then this will return our max raycast dist (which is 10 right now)
		
		//forward
		if (Physics.Raycast(agentRB.position, transform.forward, out hit, academy.agentRaycastDist)) // raycast forward to look for walls
		{
			if(hit.collider.CompareTag("agent"))
			{
				// reward += .001f;
				didWeHitSomething = 1;
				hitDistance = hit.distance;
			}
		}
		else
		{
			didWeHitSomething = 0;
		}
		state.Add(didWeHitSomething);
		state.Add(hitDistance);

		//back
		if (Physics.Raycast(agentRB.position, -transform.forward, out hit, academy.agentRaycastDist)) // raycast forward to look for walls
		{
			if(hit.collider.CompareTag("agent"))
			{
				// reward += .001f;
				didWeHitSomething = 1;
				hitDistance = hit.distance;
			}
		}
		else
		{
			didWeHitSomething = 0;
		}
		state.Add(didWeHitSomething);
		state.Add(hitDistance);

		//left
		if (Physics.Raycast(agentRB.position, -transform.right, out hit, academy.agentRaycastDist)) // raycast forward to look for walls
		{
			if(hit.collider.CompareTag("agent"))
			{
				// reward += .001f;
				didWeHitSomething = 1;
				hitDistance = hit.distance;
			}
		}
		else
		{
			didWeHitSomething = 0;
		}
		state.Add(didWeHitSomething);
		state.Add(hitDistance);

		//right
		if (Physics.Raycast(agentRB.position, transform.right, out hit, academy.agentRaycastDist)) // raycast forward to look for walls
		{
			if(hit.collider.CompareTag("agent"))
			{
				// reward += .001f;
				didWeHitSomething = 1;
				hitDistance = hit.distance;
			}
		}
		else
		{
			didWeHitSomething = 0;
		}
		state.Add(didWeHitSomething);
		state.Add(hitDistance);

		//up
		if (Physics.Raycast(agentRB.position, transform.up, out hit, academy.agentRaycastDist)) // raycast forward to look for walls
		{
			if(hit.collider.CompareTag("agent"))
			{
				// reward += .001f;
				didWeHitSomething = 1;
				hitDistance = hit.distance;
			}
		}
		else
		{
			didWeHitSomething = 0;
		}
		state.Add(didWeHitSomething);
		state.Add(hitDistance);

		//down
		if (Physics.Raycast(agentRB.position, transform.up, out hit, academy.agentRaycastDist)) // raycast forward to look for walls
		{
			if(hit.collider.CompareTag("agent"))
			{
				// reward += .001f;
				didWeHitSomething = 1;
				hitDistance = hit.distance;
			}
		}
		else
		{
			didWeHitSomething = 0;
		}
		state.Add(didWeHitSomething);
		state.Add(hitDistance);







		return state;
	}



	// //use the ground's bounds to pick a random spawn pos
    // public Vector3 GetRandomGoalPos()
    // {
    //     Vector3 randomGoalPos = Vector3.zero;
    //     float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
    //     float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier);
    //     // float randomPosX = Random.Range(-areaBounds.extents.x, areaBounds.extents.x);
    //     // float randomPosZ = Random.Range(-areaBounds.extents.z, areaBounds.extents.z);
    //     randomGoalPos = ground.transform.position + new Vector3(randomPosX, goalStartingPos.y - ground.transform.position.y, randomPosZ ); //kind of a dumb way to do this. fix it later.
    //     return randomGoalPos;
    // }

	// //use the ground's bounds to pick a random spawn pos
    // public Vector3 GetRandomSpawnPos()
    // {
    //     Vector3 randomSpawnPos = Vector3.zero;
    //     float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
    //     float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier);
    //     randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1.5f, randomPosZ );
    //     return randomSpawnPos;
    // }

	// //woot
	// public void IScoredAGoal()
	// {
	// 	reward += 1; //you get a point
	// 	done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
	// 	StartCoroutine(GoalScoredSwapGroundMaterial(goalScoredMaterial, 2)); //swap ground material for a bit to indicate we scored.

	// }

	// //swap ground material, wait time seconds, then swap back to the regular ground material.
	// IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
	// {
	// 	groundRenderer.material = mat;
	// 	yield return new WaitForSeconds(time); //wait for 2 sec
	// 	groundRenderer.material = groundMaterial;
	// }


	public void MoveAgent(float[] act) 
	{
	
        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
			reward -= Mathf.Abs(act[0])/1000;
			reward -= Mathf.Abs(act[1])/1000;
			reward -= Mathf.Abs(act[2])/1000;
			// reward -= Mathf.Abs(act[3])/100;
			// reward -= Mathf.Abs(act[4])/100;
			// reward -= Mathf.Abs(act[5])/100;
			Vector3 directionX = Vector3.right * Mathf.Clamp(act[0], -1f, 1f) * academy.agentMoveSpeed;  //go left or right in world space
            Vector3 directionY = Vector3.up * Mathf.Clamp(act[1], -1f, 1f) * academy.agentMoveSpeed; //go forward or back in world space
            Vector3 directionZ = Vector3.forward * Mathf.Clamp(act[2], -1f, 1f) * academy.agentMoveSpeed; //go forward or back in world space
			// Vector3 directionX = Vector3.right * act[0] * academy.agentMoveSpeed;  //go left or right in world space
            // Vector3 directionY = Vector3.up * act[1] * academy.agentMoveSpeed; //go forward or back in world space
            // Vector3 directionZ = Vector3.forward * act[2] * academy.agentMoveSpeed; //go forward or back in world space
        	Vector3 dirToGo = directionX + directionY + directionZ; 
			// agentRB.AddForce(dirToGo, ForceMode.Acceleration); //GO
			agentRB.AddForce(dirToGo, ForceMode.VelocityChange); //GO

			// agentRB.AddTorque(transform.forward * act[3] * academy.agentRotationSpeed, ForceMode.Acceleration); //turn right or left
			// agentRB.AddTorque(transform.right * act[4] * academy.agentRotationSpeed, ForceMode.Acceleration); //turn right or left
			// agentRB.AddTorque(transform.up * act[5] * academy.agentRotationSpeed, ForceMode.Acceleration); //turn right or left
			// agentRB.AddTorque(transform.forward * act[3] * academy.agentRotationSpeed, ForceMode.VelocityChange); //turn right or left
			// agentRB.AddTorque(transform.right * act[4] * academy.agentRotationSpeed, ForceMode.VelocityChange); //turn right or left
			// agentRB.AddTorque(transform.up * act[5] * academy.agentRotationSpeed, ForceMode.VelocityChange); //turn right or left

        }
    }

	public override void AgentStep(float[] act)
	{

        MoveAgent(act); //perform agent actions
		// reward -= (academy.seed.transform.position - agentRB.position).sqrMagnitude/1000;
		float proxReward = 1 - (academy.seed.transform.position - agentRB.position).sqrMagnitude/100;
		// print(proxReward);
		reward += proxReward;

		// reward -= .001f; // don't waste time
		// bool fail = false;  // did the agent or block get pushed off the edge?

		// // if (!Physics.Raycast(agentRB.position, Vector3.down, 3, groundLayer)) //if the agent has gone over the edge, we done.
		// if (!Physics.Raycast(agentRB.position, Vector3.down, 3)) //if the agent has gone over the edge, we done.
		// {
		// 	fail = true; //fell off bro
		// 	reward -= 1f; // BAD AGENT
		// 	transform.position =  GetRandomSpawnPos();
		// 	done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
		// }

		// // if (!Physics.Raycast(blockRB.position, Vector3.down, 3, groundLayer)) //if the block has gone over the edge, we done.
		// if (!Physics.Raycast(blockRB.position, Vector3.down, 3)) //if the block has gone over the edge, we done.
		// {
		// 	fail = true; //fell off bro
		// 	reward -= 1f; // BAD AGENT
		// 	ResetBlock(); //reset block pos
		// 	done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
		// }

		// if (fail)
		// {
		// 	StartCoroutine(GoalScoredSwapGroundMaterial(failMaterial, .5f)); //swap ground material to indicate fail
		// }
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.CompareTag("seed"))
		{
			reward -= .1f;
			done = true;
		}
		if(col.gameObject.CompareTag("agent"))
		{
			reward -= .1f;
			done = true;
		}
	}
	
	// void ResetBlock()
	// {
	// 	agentRB.transform.position = academy.seed.transform.position + Random.insideUnitSphere * 10;
	// 	// block.transform.position = GetRandomSpawnPos(); //get a random pos
    //     // blockRB.velocity = Vector3.zero; //reset vel back to zero
    //     // blockRB.angularVelocity = Vector3.zero; //reset angVel back to zero
	// }


	//In the editor, if "Reset On Done" is checked then AgentReset() will be called automatically anytime we mark done = true in an agent script.
	public override void AgentReset()
	{
		agentRB.transform.position = academy.seed.transform.position + Random.insideUnitSphere * 10;
		// ResetBlock();
		// goal.transform.position = GetRandomGoalPos();
	}
}

