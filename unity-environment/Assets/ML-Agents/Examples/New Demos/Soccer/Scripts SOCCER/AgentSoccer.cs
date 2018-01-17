using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSoccer : Agent
{
	public enum Team
    {
        red, blue
    }
	public enum AgentRole
    {
        striker, defender, goalie
    }
	public Team team;
	public AgentRole agentRole;
	public float teamFloat;
	public float playerID;
	public int playerIndex;
	public SoccerFieldArea area;
	[HideInInspector]
	public Rigidbody agentRB;
	[HideInInspector]
	public Vector3 startingPos;

	public List<float> myState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
	SoccerAcademy academy;

    void Awake()
    {
		academy = FindObjectOfType<SoccerAcademy>(); //get the academy


		//we need to set up each player. most of this is unused right now but will be useful when we start collecting other player's states
        if(team == Team.red)
        {
			brain = agentRole == AgentRole.striker? academy.redBrainStriker: agentRole == AgentRole.defender? academy.redBrainDefender: academy.redBrainGoalie;
			PlayerState playerState = new PlayerState();
			playerState.teamFloat = 0;
			teamFloat = 0;
			playerState.agentRoleFloat = agentRole == AgentRole.striker? 0: agentRole == AgentRole.defender? 1: 2;
			maxStep = academy.maxAgentSteps;
			playerState.playerID = area.redPlayers.Count; //float id used to id individual
			playerID = playerState.playerID;
			playerState.agentRB = GetComponent<Rigidbody>(); //cache the RB
			agentRB = GetComponent<Rigidbody>(); //cache the RB
			playerState.startingPos = transform.position;
			playerState.agentScript = this;
            area.redPlayers.Add(playerState);
            area.playerStates.Add(playerState);
			playerIndex = area.playerStates.IndexOf(playerState);
			playerState.playerIndex = playerIndex;

        }
        else if(team == Team.blue)
        {
			brain = agentRole == AgentRole.striker? academy.blueBrainStriker: agentRole == AgentRole.defender? academy.blueBrainDefender: academy.blueBrainGoalie;
			PlayerState playerState = new PlayerState();
			playerState.teamFloat = 1;
			teamFloat = 1;
			playerState.agentRoleFloat = agentRole == AgentRole.striker? 0: agentRole == AgentRole.defender? 1: 2;
			maxStep = academy.maxAgentSteps;
			playerState.playerID = area.bluePlayers.Count; //float id used to id individual
			playerID = playerState.playerID;
			playerState.agentRB = GetComponent<Rigidbody>(); //cache the RB
			agentRB = GetComponent<Rigidbody>(); //cache the RB
			playerState.startingPos = transform.position;
			playerState.agentScript = this;
            area.bluePlayers.Add(playerState);
            area.playerStates.Add(playerState);
			playerIndex = area.playerStates.IndexOf(playerState);
			playerState.playerIndex = playerIndex;
        }

        startingPos = transform.position; //cache the starting pos in case we want to spawn players back at their startingpos
    }

    public override void InitializeAgent()
    {
		base.InitializeAgent();
    }

  	public override List<float> CollectState()
    {
		myState = area.playerStates[playerIndex].state; //states for all players are collected in the SoccerFieldArea script. we can pull this player's state by index
		state.AddRange(myState);
		return state;
	}

	public void MoveAgent(float[] act) {

        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {

			Vector3 directionX = Vector3.right * Mathf.Clamp(act[0], -1f, 1f);  //go left or right in world space
            Vector3 directionZ = Vector3.forward * Mathf.Clamp(act[1], -1f, 1f); //go forward or back in world space
        	Vector3 dirToGo = directionX + directionZ; //the dir we want to go
			agentRB.AddForce(dirToGo * academy.agentRunSpeed, ForceMode.VelocityChange); //GO

			agentRB.AddTorque(transform.up * act[2] * academy.agentRotationSpeed, ForceMode.VelocityChange); //turn right or left

        }
    }

	public override void AgentStep(float[] act)
	{
        MoveAgent(act); //perform agent actions
	}

	public override void AgentReset()
	{
		transform.position =  area.GetRandomSpawnPos();
		agentRB.velocity = Vector3.zero; //we want the agent's vel to return to zero on reset
	}


	public override void AgentOnDone()
	{

	}
}
