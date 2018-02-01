using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerState
{
    //all of this gets setup in AgentSoccer.CS Awake()
    public int playerIndex; //index pos on the team
    // public float playerID; //index pos on the team
    // public float currentTeamFloat; //1 = blue, 0 = red
    public List<float> state = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
    public Rigidbody agentRB; //the agent's rb
    public Vector3 startingPos; //the agent's starting position
    public AgentSoccer agentScript; //this is the agent's script
    // public float agentRoleFloat; //for state
    // public Transform targetGoal;
    // public Transform defendGoal;
}

public class SoccerFieldArea : MonoBehaviour {


    public Transform redGoal;
    public Transform blueGoal;

    public AgentSoccer redStriker;
    public AgentSoccer blueStriker;
    public AgentSoccer redGoalie;
    public AgentSoccer blueGoalie;
    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRB;
    public GameObject ground; //to be used to determine spawn areas
    public GameObject centerPitch; //to be used to determine spawn areas
    SoccerBallController ballController;
    // public List<PlayerState> redPlayers = new List<PlayerState>();
    // public List<PlayerState> bluePlayers = new List<PlayerState>();
    public List<PlayerState> playerStates = new List<PlayerState>();
    // float blueTeamID; //1 if blue, 0 if red
	// float redTeamID; //1 if blue, 0 if red
    [HideInInspector]
    public Vector3 ballStartingPos;
    Bounds areaBounds;
    public bool drawSpawnAreaGizmo;
    Vector3 spawnAreaSize;
    public float goalScoreByTeamReward; //if red scores they get this reward & vice versa
    public float goalScoreAgainstTeamReward; //if red scores we deduct some from blue & vice versa
    public GameObject goalTextUI;
    public float totalPlayers;
    public float minWaitTimeToRespawnBall = 5; //the ball can a spawn every 5 sec
    [HideInInspector]
    public bool canResetBall;
    public bool useSpawnPoint;
    public Transform spawnPoint;
    Material groundMaterial;
	Renderer groundRenderer;
    SoccerAcademy academy;


    public IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
	{
		groundRenderer.material = mat;
		yield return new WaitForSeconds(time); //wait for 2 sec
		groundRenderer.material = groundMaterial;
	}


	void Awake () {
        academy  = FindObjectOfType<SoccerAcademy>();
        groundRenderer = centerPitch.GetComponent<Renderer>(); //get the ground renderer so we can change the material when a goal is scored
		groundMaterial = groundRenderer.material; //starting material
        canResetBall = true;
        if(goalTextUI){goalTextUI.SetActive(false);}
        ballRB = ball.GetComponent<Rigidbody>();
        ballController = ball.GetComponent<SoccerBallController>();
        ballController.area = this;
        ballStartingPos = ball.transform.position;
        Mesh mesh = ground.GetComponent<MeshFilter>().mesh;  //get the ground's mesh
        areaBounds = ground.GetComponent<Collider>().bounds; //get the ground's bounds
	}

    IEnumerator ShowGoalUI()
    {
        if(goalTextUI)goalTextUI.SetActive(true);
        yield return new WaitForSeconds(.25f);
        if(goalTextUI)goalTextUI.SetActive(false);
    }

    public void AllPlayersDone(float reward)
    {
        foreach(PlayerState ps in playerStates)
        {
            // if(ps.agentScript.gameObject.activeInHierarchy &&  ps.agentScript.agentRole == AgentSoccer.AgentRole.striker || ps.agentScript.agentRole == AgentSoccer.AgentRole.defender)
            if(ps.agentScript.gameObject.activeInHierarchy)
            {
                if(reward != 0)
                {
                    ps.agentScript.reward += reward;
                }
                ps.agentScript.done = true; 
            }

        }
    }




    // public void GoalScored(string goalTag)
    // {

    // }
    public void BlueGoalTouched() //ball touched the blue goal
    {
        foreach(PlayerState ps in playerStates)
        {
            if (ps.agentScript.team == AgentSoccer.Team.blue) //if currently on the blue team you suck
            {
                RewardOrPunishPlayer(ps, academy.strikerPunish, academy.defenderPunish, academy.goaliePunish);
            }
            else if(ps.agentScript.team == AgentSoccer.Team.red) //if currently on the red team you get a reward
            {
                RewardOrPunishPlayer(ps, academy.strikerReward, academy.defenderReward, academy.goalieReward);
            }

            if(academy.randomizePlayersTeamForTraining)
            {
                ps.agentScript.ChooseRandomTeam();
                // ps.currentTeamFloat = Random.Range(0,2); //return either a 0 or 1 * max is exclusive ex: Random.Range(0,10) will pick a int between 0-9
            }
        }

        if(academy.randomizeFieldOrientationForTraining)
        {
            // print("rotating field");
            ground.transform.Rotate(Vector3.up * Random.Range(0, 90));
            // this.transform.rotation *= Quaternion.Euler(0, Random.Range(0, 360), 0); //rotate by a random amount on the y axis.
        }


        StartCoroutine(GoalScoredSwapGroundMaterial(academy.redMaterial, 2));
        ResetBall();
        if(goalTextUI)
        {
            StartCoroutine(ShowGoalUI());
        }
    }
    

    public void RedGoalTouched() //ball touched the blue goal
    {
        foreach(PlayerState ps in playerStates)
        {
            if (ps.agentScript.team == AgentSoccer.Team.blue) //if currently on the blue team you get a reward
            {
                RewardOrPunishPlayer(ps, academy.strikerReward, academy.defenderReward, academy.goalieReward);
            }
            else if(ps.agentScript.team == AgentSoccer.Team.red) //if currently on the red team you suck
            {
                RewardOrPunishPlayer(ps, academy.strikerPunish, academy.defenderPunish, academy.goaliePunish);
            }
            if(academy.randomizePlayersTeamForTraining)
            {
                ps.agentScript.ChooseRandomTeam();
                // ps.currentTeamFloat = Random.Range(0,2); //return either a 0 or 1 * max is exclusive ex: Random.Range(0,10) will pick a int between 0-9
            }
        }

        if(academy.randomizeFieldOrientationForTraining)
        {
                        // print("rotating field");

            ground.transform.Rotate(Vector3.up * Random.Range(0, 90));

            // transform.Rotate(new Vector3(0, Random.Range(0, 360), 0), Space.Self);
            // this.transform.rotation *= Quaternion.Euler(0, Random.Range(0, 360), 0); //rotate by a random amount on the y axis.
        }
        StartCoroutine(GoalScoredSwapGroundMaterial(academy.blueMaterial, 2));
        ResetBall();
        if(goalTextUI)
        {
            StartCoroutine(ShowGoalUI());
        }
    }




    // public void RedScores() //ball touched the blue goal
    // {
    //     RewardOrPunishTeam(redPlayers, academy.strikerReward, academy.defenderReward, academy.goalieReward);
    //     RewardOrPunishTeam(bluePlayers, academy.strikerPunish, academy.defenderPunish, academy.goaliePunish);
	// 	StartCoroutine(GoalScoredSwapGroundMaterial(redScoredMaterial, 2));
    //     ResetBall();
    //     if(goalTextUI)
    //     {
    //         StartCoroutine(ShowGoalUI());
    //     }
    // }

    // public void BlueScores() //ball touched the red goal
    // {
    //     RewardOrPunishTeam(bluePlayers, academy.strikerReward, academy.defenderReward, academy.goalieReward);
    //     RewardOrPunishTeam(redPlayers, academy.strikerPunish, academy.defenderPunish, academy.goaliePunish);
	// 	StartCoroutine(GoalScoredSwapGroundMaterial(blueScoredMaterial, 2));
    //     ResetBall();
    //     if(goalTextUI)
    //     {
    //         StartCoroutine(ShowGoalUI());
    //     }
    // }



    public void RewardOrPunishPlayer(PlayerState ps, float striker, float defender, float goalie) //ball touched the red goal
    {
        // foreach(PlayerState ps in team)
        // {
            if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
            {
                ps.agentScript.reward += striker;
            }
            if(ps.agentScript.agentRole == AgentSoccer.AgentRole.defender)
            {
                ps.agentScript.reward += defender;
            }
            if(ps.agentScript.agentRole == AgentSoccer.AgentRole.goalie)
            {
                ps.agentScript.reward += goalie;
            }
            ps.agentScript.done = true;  //all agents need to be reset
        // }

    }


    // public void RewardOrPunishTeam(List<PlayerState> team, float striker, float defender, float goalie) //ball touched the red goal
    // {
    //     foreach(PlayerState ps in team)
    //     {
    //         if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
    //         {
    //             ps.agentScript.reward += striker;
    //         }
    //         if(ps.agentScript.agentRole == AgentSoccer.AgentRole.defender)
    //         {
    //             ps.agentScript.reward += defender;
    //         }
    //         if(ps.agentScript.agentRole == AgentSoccer.AgentRole.goalie)
    //         {
    //             ps.agentScript.reward += goalie;
    //         }
    //         ps.agentScript.done = true;  //all agents need to be reset
    //     }

    // }

    public void PlayerScoredWrongGoal(AgentSoccer player)
    {
        player.reward -= goalScoreAgainstTeamReward;
        AllPlayersDone(0);
        ResetBall();
    }

    public void PlayerScoredCorrectGoal(AgentSoccer player)
    {
        // AgentSoccer agent = player.GetComponent<AgentSoccer>();
        player.reward += goalScoreByTeamReward;
        AllPlayersDone(0);
        ResetBall();
    }

    
    //DEBUG AREA BOUNDS
    void OnDrawGizmos()
    {
        if(drawSpawnAreaGizmo)
        {
            // spawnAreaSize = areaBounds.size * spawnAreaMarginMultiplier;
            spawnAreaSize = areaBounds.extents;
            Gizmos.DrawWireCube(ground.transform.position, spawnAreaSize);
        }
    }

    public Vector3 GetRandomSpawnPos()
    {
        Vector3 randomSpawnPos = Vector3.zero;
        float randomPosX = Random.Range(-areaBounds.extents.x * academy.spawnAreaMarginMultiplier, areaBounds.extents.x * academy.spawnAreaMarginMultiplier);
        // float posY = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
        float randomPosZ = Random.Range(-areaBounds.extents.z * academy.spawnAreaMarginMultiplier, areaBounds.extents.z * academy.spawnAreaMarginMultiplier);
        if(useSpawnPoint)
        {
            randomSpawnPos = spawnPoint.position + new Vector3(randomPosX, 2, randomPosZ );
        }
        else
        {
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 2, randomPosZ );
        }
        return randomSpawnPos;
    }
	
    void SpawnObjAtPos(GameObject obj, Vector3 pos)
    {
        obj.transform.position = pos;
        // canUseThisPos = true;
    }

    IEnumerator ResetBallTimer()
    {
        canResetBall = false;
        yield return new WaitForSeconds(minWaitTimeToRespawnBall);
        canResetBall = true;
    }


    public void ResetBall()
    {
        
        ball.transform.position = GetRandomSpawnPos();
        StartCoroutine(ResetBallTimer());
    }


    //STATE COLLECTOR STUFF

    //Add the x,y,z components of this Vector3 to the state list
    void CollectVector3State(List<float> state, Vector3 v)
    {
        state.Add(v.x);
        state.Add(v.y);
        state.Add(v.z);
    }

    // void CollectRotationState(List<float> state, Quaternion q)
    // {
    //     state.Add(q.x);
    //     state.Add(q.y);
    //     state.Add(q.z);
    // }

    void CollectRotationState(List<float> state, Transform t)
    {
		state.Add(t.rotation.eulerAngles.x/180.0f-1.0f);
		state.Add(t.rotation.eulerAngles.y/180.0f-1.0f);
		state.Add(t.rotation.eulerAngles.z/180.0f-1.0f);
    }

    //we can only collect floats in CollecState so we need to convert bools to floats
    float ConvertBoolToFloat(bool b)
    {
        float f = b == true? 1 : 0;
        return f;
    }





    void CollectPlayerState(PlayerState ps)
    {

        Vector3 playerDirToTargetGoal = Vector3.zero; //set the target goal based on which team this player is currently on
        // var playerTeam = ps.tea
        // if(ps.currentTeamFloat == 0)//I'm on the red team
        if(ps.agentScript.team == AgentSoccer.Team.red)//I'm on the red team
        {
            playerDirToTargetGoal = blueGoal.position - ps.agentRB.position;
        }
        if(ps.agentScript.team == AgentSoccer.Team.blue)//I'm on the blue team
        {
            playerDirToTargetGoal = redGoal.position - ps.agentRB.position;
        }

        Vector3 playerDirToDefendGoal = Vector3.zero;//set the defend goal based on which team this player is currently on
        if(ps.agentScript.team == AgentSoccer.Team.red)//I'm on the red team
        {
            playerDirToDefendGoal = redGoal.position - ps.agentRB.position;
        }
        if(ps.agentScript.team == AgentSoccer.Team.blue)//I'm on the blue team
        {
            playerDirToDefendGoal = blueGoal.position - ps.agentRB.position;
        }
        // Vector3 playerDirToDefendGoal = ps.defendGoal.transform.position - ps.agentRB.position;
        Vector3 playerPos = ps.agentRB.position - ground.transform.position;
        Vector3 ballPos = ballRB.position - ground.transform.position;
        Vector3 playerDirToBall = ballRB.position - ps.agentRB.position;
        // Vector3 playerDirToRedGoal = redGoal.transform.position - ps.agentRB.position;
        // Vector3 playerDirToBlueGoal = blueGoal.transform.position - ps.agentRB.position;
        // Vector3 playerPos = ps.agentRB.position - ground.transform.position;
        // Vector3 playerDirToBall = ballRB.position - ps.agentRB.position;

        // float playerDistToBall = playerDirToBall.sqrMagnitude;
        // ps.state.Add(playerDistToBall);

        // Vector3 redGoalPosition = redGoal.transform.position - ground.transform.position;
        // Vector3 blueGoalPosition = blueGoal.transform.position - ground.transform.position;
        // Vector3 ballDirToRedGoal = redGoal.transform.position - ballRB.position;
        // Vector3 ballDirToBlueGoal = blueGoal.transform.position - ballRB.position;
        // Vector3 ballDirToTargetGoal = ps.targetGoal.transform.position - ballRB.position;
        // Vector3 ballDirToDefendGoal =  ps.defendGoal.transform.position - ballRB.position;

        ps.state.Clear(); //instead of creating a new list each tick we will reuse this one
        // ps.state.Add(ps.playerID); //whoami 
        // ps.state.Add(ps.currentTeamFloat); //which team 
        // ps.state.Add(ps.agentRoleFloat);
        CollectVector3State(ps.state, ps.agentRB.velocity); //agent's vel
        CollectRotationState(ps.state, ps.agentRB.transform); //agent's rotation
        CollectVector3State(ps.state, playerPos); //dir from player to red goal
        // CollectVector3State(ps.state, ballPos); //dir from player to red goal
        CollectVector3State(ps.state, playerDirToBall); //dir from agent to ball
        // CollectVector3State(ps.state, redGoalPosition);  //red goal abs position
        // CollectVector3State(ps.state, blueGoalPosition); //blue goal abs position
        CollectVector3State(ps.state, playerDirToTargetGoal); //dir from player to red goal
        CollectVector3State(ps.state, playerDirToDefendGoal); //dir from player to blue goal
        // CollectVector3State(ps.state, ballDirToTargetGoal); //dir from ball to target goal
        // CollectVector3State(ps.state, ballDirToDefendGoal); //dir from ball to defend goal


        // CollectVector3State(ps.state, ballDirToRedGoal); //dir from ball to target goal
        // CollectVector3State(ps.state, ballDirToBlueGoal); //dir from ball to defend goal
        // CollectVector3State(ps.state, playerDirToRedGoal); //dir from player to red goal
        // CollectVector3State(ps.state, playerDirToBlueGoal); //dir from player to blue goal
        // CollectVector3State(ps.state, ballDirToRedGoal); //dir from ball to target goal
        // CollectVector3State(ps.state, ballDirToBlueGoal); //dir from ball to defend goal


        // CollectVector3State(ps.state, ps.targetGoal.transform.position - ps.agentRB.position); //dir from player to target goal
        // CollectVector3State(ps.state, ps.defendGoal.transform.position - ps.agentRB.position); //dir from player to defend goal
        // CollectVector3State(ps.state, ps.targetGoal.transform.position - ballRB.position); //dir from ball to target goal
        // CollectVector3State(ps.state, ps.defendGoal.transform.position - ballRB.position); //dir from ball to defend goal
        CollectVector3State(ps.state, ballRB.velocity);



        
        // if(ps.teamFloat == 1) //blue team
        // {
        //     bluePlayers.Add(ps);
        // }
        // else if (ps.teamFloat == 0) //redteam
        // {
        //     redPlayers.Add(ps);
        // }


    }


    void FixedUpdate()
    {
        if(playerStates.Count > 0)
        {
            //UPDATE ALL PLAYERS FIRST
            foreach(PlayerState ps in playerStates)
            {
                CollectPlayerState(ps);
            }

        }
    }
}
