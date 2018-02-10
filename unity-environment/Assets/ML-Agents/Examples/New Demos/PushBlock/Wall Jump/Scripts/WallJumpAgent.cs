//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpAgent : Agent
{
	public GameObject ground; //ground game object. we will use the area bounds to spawn the blocks
	public GameObject spawnArea; //ground game object. we will use the area bounds to spawn the blocks
	public bool visualizeSpawnArea;
	// [HideInInspector]
	public Bounds spawnAreaBounds; //the bounds of the pushblock area
	public Bounds areaBounds; //the bounds of the pushblock area

	public GameObject goal; //goal to push the block to
    public GameObject shortBlock; //the orange block we are going to be pushing
    public GameObject mediumBlock; //the orange block we are going to be pushing
    public GameObject tallBlock; //the orange block we are going to be pushing
    public GameObject wall; //
	Rigidbody shortBlockRB;  //cached on initialization
	Rigidbody mediumBlockRB;  //cached on initialization
	Rigidbody tallBlockRB;  //cached on initialization
	Rigidbody agentRB;  //cached on initialization
	Material groundMaterial; //cached on Awake()
	Renderer groundRenderer;
	Vector3 goalStartingPos;
	WallJumpAcademy academy;


	//JUMPING STUFF
	public bool jumping;
	public float jumpTime;
	public float fallingForce; //this is a downward force applied when falling to make jumps look less floaty
	public Collider[] hitGroundColliders = new Collider[3]; //used for groundchecks
	public bool visualizeGroundCheckSphere;
	public bool grounded;
	public bool performingGroundCheck;
	public float groundCheckRadius; //the radius from transform.position to check
	public Vector3 groundCheckOffset; //offset groundcheck pos. useful for tweaking groundcheck box
	public float groundCheckFrequency; //perform a groundcheck every x sec. ex: .5 will do a groundcheck every .5 sec.
	Vector3 jumpTargetPos; //target this position during jump. it will be 
	Vector3 jumpStartingPos; //target this position during jump. it will be 
	public float jumpHeight = 1; //how high should we jump?
	public float jumpVelocity = 500; //higher number will result in a quicker jump
	public float jumpVelocityMaxChange = 10; // don't let the velocity change more than this every tick. this helps smooth out the motion. lower number == slower/more controlled movement. I typically use 10-20 for this value.
	
	
	void Awake()
	{
		academy = FindObjectOfType<WallJumpAcademy>();
		goalStartingPos = goal.transform.position; //cached goal starting Pos in case we want to remember that
		brain = FindObjectOfType<Brain>(); //only one brain in the scene so this should find our brain. BRAAAINS.
		wall.transform.localScale = new Vector3(wall.transform.localScale.x, academy.wallHeight, wall.transform.localScale.z); //set the wall height to match the slider
	}

    public override void InitializeAgent()
    {
		base.InitializeAgent();
		// goalDetect = block.GetComponent<GoalDetect>();
		// spawnArea.SetActive(false);
		// goalDetect.agent = this; 
		StartGroundCheck();

		agentRB	= GetComponent<Rigidbody>(); //cache the agent rigidbody
		shortBlockRB = shortBlock.GetComponent<Rigidbody>(); //cache the block rigidbody
		mediumBlockRB = mediumBlock.GetComponent<Rigidbody>(); //cache the block rigidbody
		tallBlockRB	= tallBlock.GetComponent<Rigidbody>(); //cache the block rigidbody
		areaBounds = ground.GetComponent<Collider>().bounds; //get the ground's bounds
		spawnAreaBounds = spawnArea.GetComponent<Collider>().bounds; //get the ground's bounds
		groundRenderer = ground.GetComponent<Renderer>(); //get the ground renderer so we can change the material when a goal is scored
		groundMaterial = groundRenderer.material; //starting material

		spawnArea.SetActive(false);
    }

	// //add some falling force (otherwise it looks floaty)
	// IEnumerator Falling()
	// {
	// 	while(!grounded)
	// 	{
	// 		agentRB.AddForce(Vector3.down * fallingForce, ForceMode.Acceleration);
	// 		yield return null;
	// 	}
	// }
	
	//put agent into the jumping state for the specified jumpTime
	IEnumerator Jump()
	{

		jumping = true;
		jumpStartingPos = agentRB.position;
		// jumpTargetPos = agentRB.position + Vector3.up * jumpHeight;
		yield return new WaitForSeconds(jumpTime);
		jumping = false;
		// StartCoroutine(Falling());//should be falling now
	}


	//GROUND CHECK STUFF (Used for jumping)
	public void StartGroundCheck()
	{
		if(!IsInvoking("DoGroundCheck"))
		{
			InvokeRepeating("DoGroundCheck", 0, groundCheckFrequency);
			performingGroundCheck = true;
		}
	}

	public void StopGroundCheck() 
	{
		CancelInvoke("DoGroundCheck"); //stop doing ground check;
		performingGroundCheck = false;
	}

	// GROUND CHECK
	public void DoGroundCheck()
	{

		Vector3 posToUse = agentRB.position + groundCheckOffset;
		int numberGroundCollidersHit = Physics.OverlapSphereNonAlloc(posToUse, groundCheckRadius, hitGroundColliders); //chose .6 radius because this should make a sphere a little bit bigger than our cube that is a scale of 1 unit. sphere will be 1.2 units. 
		if (numberGroundCollidersHit > 1 )
		{
			grounded = false;
			foreach(Collider col in hitGroundColliders)
			{
				if(col != null && col.transform != this.transform && col.CompareTag("walkableSurface"))
				{
					//build a random position to use for a groundcheck raycast
					Vector3 randomRaycastPos = agentRB.position;
					randomRaycastPos += agentRB.transform.forward * Random.Range(-.4f, .4f); // random forward/back
					randomRaycastPos += agentRB.transform.right * Random.Range(-.4f, .4f); // plus a random left/right
					if (Physics.Raycast(randomRaycastPos, Vector3.down, .6f)) //if we hit
					{
						grounded = true; //then we're grounded
						break;
					}
				}
			}
		}
		else
		{
			grounded = false;
		}
	}


	//debug
	void OnDrawGizmos()
	{
		if(visualizeGroundCheckSphere)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
		}
	}

	//moves a rigidbody towards a position with a smooth controlled movement.
	void MoveTowards(Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
	{
		Vector3 moveToPos = targetPos - rb.worldCenterOfMass;  //cube needs to go to the standard Pos
		Vector3 velocityTarget = moveToPos * targetVel * Time.deltaTime; //not sure of the logic here, but it modifies velTarget
		if (float.IsNaN(velocityTarget.x) == false) //sanity check. if the velocity is NaN that means it's going way too fast. this check isn't needed for slow moving objs
		{
			rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
		}
	}

	public override List<float> CollectState()
	{
		Vector3 goalPos = goal.transform.position - ground.transform.position;  //pos of goal rel to ground
		Vector3 shortBlockPos = shortBlockRB.transform.position - ground.transform.position;  //pos of goal rel to ground
		// Vector3 medBlockPos = mediumBlockRB.transform.position - ground.transform.position;  //pos of goal rel to ground
		// Vector3 tallBlockPos = tallBlockRB.transform.position - ground.transform.position;  //pos of goal rel to ground
		Vector3 agentPos = agentRB.position - ground.transform.position;  //pos of agent rel to ground

		//COLLECTIN STATES
		MLAgentsHelpers.CollectVector3State(state, agentPos);  //pos of agent rel to ground
		MLAgentsHelpers.CollectVector3State(state, goalPos);  //pos of goal rel to ground
		MLAgentsHelpers.CollectVector3State(state, shortBlockPos);  //pos of short block rel to ground
		// MLAgentsHelpers.CollectVector3State(state, medBlockPos);  //pos of med block rel to ground
		// MLAgentsHelpers.CollectVector3State(state, tallBlockPos);  //pos of tall block rel to ground
		MLAgentsHelpers.CollectVector3State(state, agentRB.velocity); //agent's vel
		MLAgentsHelpers.CollectRotationState(state, agentRB.transform); //agent's rotation

		RaycastHit hit;
		float didWeHitSomething = 0; //1 if yes, 0 if no
		float hitDistance = 10; //how far away was it. if nothing was hit then this will return our max raycast dist (which is 10 right now)
		if (Physics.Raycast(agentRB.position, transform.forward, out hit, 10)) // raycast forward to look for walls
		{
			if(hit.collider.CompareTag("walkableSurface"))
			{
				didWeHitSomething = 1;
				hitDistance = hit.distance;
				// print(hit.collider.name + hit.distance);
			}
		}

		state.Add(didWeHitSomething);
		state.Add(hitDistance);

		return state;
	}

	//use the ground's bounds to pick a random spawn pos
    public Vector3 GetRandomSpawnPos()
    {
        Vector3 randomSpawnPos = Vector3.zero;
        float randomPosX = Random.Range(-spawnAreaBounds.extents.x * academy.spawnAreaMarginMultiplier, spawnAreaBounds.extents.x * academy.spawnAreaMarginMultiplier);
        float randomPosZ = Random.Range(-spawnAreaBounds.extents.z * academy.spawnAreaMarginMultiplier, spawnAreaBounds.extents.z * academy.spawnAreaMarginMultiplier);
        randomSpawnPos = spawnArea.transform.position + new Vector3(randomPosX, 1.5f, randomPosZ );
        return randomSpawnPos;
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
			

			float speedX = 0;
			float speedZ = 0;
			if(act[0] != 0)
			{
				float energyConservationPentalty = Mathf.Abs(act[0])/1000;
				speedX = grounded? act[0]: act[0]/2; //if we are in the air, our move speed should be a fraction of normal speed.
				// print("act[0] = " + act[0]);
				reward -= energyConservationPentalty;
				// reward -= .0001f;
			}
			if(act[1] != 0)
			{
				float energyConservationPentalty = Mathf.Abs(act[1])/1000;
				speedZ= grounded? act[1]: act[1]/2; //if we are in the air, our move speed should be a fraction of normal speed.
				// print("act[1] = " + act[1]);
				reward -= energyConservationPentalty;
			}
			
			Vector3 directionX = Vector3.right * speedX;  //go left or right in world space
            Vector3 directionZ = Vector3.forward * speedZ; //go forward or back in world space
        	Vector3 dirToGo = directionX + directionZ; //the dir we want to go

			if(act[2] > 0 && !jumping && grounded)
			{
				//jump
				reward -= .005f; //energy conservation penalty
				StartCoroutine(Jump());
			}

			//add force
			agentRB.AddForce(dirToGo * academy.agentRunSpeed, ForceMode.VelocityChange); //GO
			//rotate the player forward
			if(dirToGo != Vector3.zero)
			{
				agentRB.rotation = Quaternion.Lerp(agentRB.rotation, Quaternion.LookRotation(dirToGo), Time.deltaTime * academy.agentRotationSpeed);
			}
        }
    }

	public override void AgentStep(float[] act)
	{

        MoveAgent(act); //perform agent actions
		bool fail = false;  // did the agent or block get pushed off the edge?

		//debug
		if(visualizeSpawnArea && !spawnArea.activeInHierarchy)
		{
			spawnArea.SetActive(true);
		}

		if(jumping)
		{
			// jumpTargetPos = transform.forward
			jumpTargetPos = new Vector3(agentRB.position.x,  jumpStartingPos.y + jumpHeight, agentRB.position.z) + transform.forward/4; 

			MoveTowards(jumpTargetPos, agentRB, jumpVelocity, jumpVelocityMaxChange);
			// agentRB.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
			// agentRB.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
		}

		if(!jumping && !grounded) //add some downward force so it's not floaty
		{
			agentRB.AddForce(Vector3.down * fallingForce, ForceMode.Acceleration);
		}

		if (!Physics.Raycast(agentRB.position, Vector3.down, 20)) //if the agent has gone over the edge, we done.
		{
			fail = true; //fell off bro
			reward -= 1f; // BAD AGENT
			transform.position =  GetRandomSpawnPos();
			done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
		}

		if (!Physics.Raycast(shortBlockRB.position, Vector3.down, 20)) //if the block has gone over the edge, we done.
		{
			fail = true; //fell off bro
			reward -= 1f; // BAD AGENT
			ResetBlock(shortBlockRB); //reset block pos
			done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
		}

		if (fail)
		{
			StartCoroutine(GoalScoredSwapGroundMaterial(academy.failMaterial, .5f)); //swap ground material to indicate fail
		}
	}

	// detect when we touch the goal
	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.CompareTag("goal")) //touched goal
		{
			reward += 1; //you get a point
			done = true; //if we mark an agent as done it will be reset automatically. AgentReset() will be called.
			StartCoroutine(GoalScoredSwapGroundMaterial(academy.goalScoredMaterial, 2)); //swap ground material for a bit to indicate we scored.
		}
	}
	
	
	//Reset the orange block position
	void ResetBlock(Rigidbody blockRB)
	{
		blockRB.transform.position = GetRandomSpawnPos(); //get a random pos
        blockRB.velocity = Vector3.zero; //reset vel back to zero
        blockRB.angularVelocity = Vector3.zero; //reset angVel back to zero
	}

	//In the editor, if "Reset On Done" is checked then AgentReset() will be called automatically anytime we mark done = true in an agent script.
	public override void AgentReset()
	{
		ResetBlock(shortBlockRB);
		ResetBlock(mediumBlockRB);
		ResetBlock(tallBlockRB);
		transform.position =  GetRandomSpawnPos();
		wall.transform.localScale = new Vector3(wall.transform.localScale.x, academy.wallHeight, wall.transform.localScale.z);
	}
}

