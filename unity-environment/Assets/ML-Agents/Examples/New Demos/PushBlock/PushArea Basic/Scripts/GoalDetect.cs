using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetect : MonoBehaviour {
	// [HideInInspector]
	public PushAgentBasic agent;  //this will be set by the agent script on Initialization. Don't need to manually set

	void OnCollisionEnter(Collision col)
	{

		if(col.gameObject.CompareTag("goal")) //touched goal
		{
			agent.IScoredAGoal();
		}

		// //We don't HAVE to do this but it does improve learning. The agent quickly learns it should interact with the orange block
		// if(col.gameObject.CompareTag("agent")) //touched goal
		// {
		// 	agent.reward += .001f; //encourage to push the block
		// 	// agent.IScoredAGoal();
		// }
	}
}
