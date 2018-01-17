using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerAcademy : Academy {

    public Brain redBrainStriker;
    public Brain redBrainDefender;
    public Brain redBrainGoalie;
    public Brain blueBrainStriker;
    public Brain blueBrainDefender;
    public Brain blueBrainGoalie;
	public float spawnAreaMarginMultiplier; //ex: .9 means 90% of spawn area will be used.... .1 margin will be left

	public int maxAgentSteps; //max sim steps for agents. this is here so we only have to set it once and all agents can reference it

    public float agentRunSpeed; //we will be using AddForce ForceMode.VelocityChange so this should be between 1-4. Anything higher than 4 will be too fast
    public float agentRotationSpeed; //something between 10-20 should work well.


    //Rewards
    public float strikerPunish; //if opponents scores, the striker gets this neg reward (-1)
    public float strikerReward; //if team scores a goal they get a reward (+1)
    public float defenderPunish; //not currently used
    public float defenderReward; //not currently used
    public float goaliePunish; //if opponents score, goalie gets this neg reward (-1)
    public float goalieReward; //if team scores, goalie gets this reward (currently 0...no reward. can play with this later)

	public override void AcademyReset()
	{
	}

	public override void AcademyStep()
	{

	}

}
