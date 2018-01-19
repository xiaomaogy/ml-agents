//Detect when the orange block has touched the goal. 
//Put this script onto the orange block. There's nothing you need to set in the editor.
//Make sure the goal is tagged with "goal" in the editor.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetect : MonoBehaviour {
	[HideInInspector]
	public PushAgentBasic agent;  //this will be set by the agent script on Initialization. Don't need to manually set

	void OnCollisionEnter(Collision col)
	{

		if(col.gameObject.CompareTag("goal")) //touched goal
		{
			agent.IScoredAGoal();
		}
	}
}
