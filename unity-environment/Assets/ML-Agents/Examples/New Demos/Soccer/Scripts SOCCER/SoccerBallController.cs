using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBallController : MonoBehaviour {

	[HideInInspector]
	public SoccerFieldArea area;
	Rigidbody rb;
	public AgentSoccer lastTouchedBy; //who was the last to touch the ball
	// public string wallTag; //will be used to check if collided with wall
	public string agentTag; //will be used to check if collided with a agent
	public string redGoalTag; //will be used to check if collided with red goal
	public string blueGoalTag; //will be used to check if collided with blue goal

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		// print (rb.velocity.sqrMagnitude);

		if(rb.velocity.sqrMagnitude < .05f) //ball is almost completely stopped
		{
			if(area.canResetBall)
			{
				// area.AllPlayersDone(0);
				// area.AllPlayersDone(-.1f);
				area.AllPlayersDone(-.5f);
				area.ResetBall();
			}
		// reward = -1f; //lets see some hustle out there
		}
	}

	void OnCollisionEnter(Collision col)
	{
		// // //who touched the ball last
		// if(col.gameObject.CompareTag(agentTag) && col.relativeVelocity.sqrMagnitude/1000 > 4)
		// {
		// 	lastTouchedBy = col.gameObject.GetComponent<AgentSoccer>();
		// 	lastTouchedBy.reward += .001f;//encourage touching the ball
		// 	// print("ball touched by " + lastTouchedBy.name);
		// }

		// if(col.gameObject.CompareTag(agentTag))
		// {
		// 	lastTouchedBy = col.gameObject.GetComponent<AgentSoccer>();
		// 	var colReward = col.relativeVelocity.sqrMagnitude/10000;
		// 	lastTouchedBy.reward += colReward;//encourage touching the ball
		// 	// lastTouchedBy.reward += .001f;//encourage touching the ball
		// 	// // print("ball touched by " + lastTouchedBy.name);
		// }

		// if(col.gameObject.CompareTag(redGoalTag) || col.gameObject.CompareTag(blueGoalTag)) //ball touched red goal
		// {
		// 	area.GoalScored();
		// }
		if(col.gameObject.CompareTag(redGoalTag)) //ball touched red goal
		{
			area.RedGoalTouched();
		}
		if(col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
		{
			area.BlueGoalTouched();
		}
		// if(col.gameObject.CompareTag(redGoalTag)) //ball touched red goal
		// {
		// 	area.BlueScores();
		// }
		// if(col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
		// {
		// 	area.RedScores();
		// }
	}
}
