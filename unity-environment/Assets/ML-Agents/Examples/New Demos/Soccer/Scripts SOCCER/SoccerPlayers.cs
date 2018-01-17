// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// [System.Serializable]
// public class PlayerState
// {
//     //all of this gets setup in AgentSoccer.CS Awake()
//     public int playerIndex; //index pos on the team
//     public float playerID; //index pos on the team
//     public float teamFloat; //1 = blue, 0 = red
//     public List<float> state = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
//     // public GameObject defendGoal; //goal we need to defend
//     // public GameObject targetGoal; //goal we need to attack
//     public Rigidbody agentRB; //the agent's rb
//     public Vector3 startingPos; //the agent's starting position
//     public AgentSoccer agentScript; //this is the agent's script
//     public float agentRoleFloat; //for state
//     // public AgentSoccerRed agentScriptRed; //this is the agent's script
//     // public AgentSoccerBlue agentScriptBlue; //this is the agent's script
//     // public List<float> myState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// 	// public List<float> myTeamsState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// 	// public List<float> otherTeamsState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// 	// public List<float> stateToSubmitToBrain = new List<float>(); //list for state data. to be updated every FixedUpdate in this script

// }

// public class SoccerFieldArea : MonoBehaviour {

//     // void SortPlayerStates()
//     // {
//     //     if(playerStates.Count > 0)
//     //     {
//     //         foreach(Player)
//     //     }
//     // }

//     public Brain redBrain;
//     public Brain redBrainStriker;
//     public Brain redBrainDefender;
//     public Brain redBrainGoalie;
//     public Brain blueBrain;
//     public Brain blueBrainStriker;
//     public Brain blueBrainDefender;
//     public Brain blueBrainGoalie;
//     public GameObject redGoal;
//     public GameObject blueGoal;
//     public Material redScoredMaterial;
//     public Material blueScoredMaterial;
//     public AgentSoccer redGoalie;
//     public AgentSoccer blueGoalie;
//     public GameObject ball;
//     [HideInInspector]
//     public Rigidbody ballRB;
//     public GameObject ground; //to be used to determine spawn areas
//     // public LayerMask groundLayer;
//     SoccerBallController ballController;
//     public List<PlayerState> redPlayers = new List<PlayerState>();
//     public List<PlayerState> bluePlayers = new List<PlayerState>();
//     public List<PlayerState> playerStates = new List<PlayerState>();
//     float blueTeamID; //1 if blue, 0 if red
// 	float redTeamID; //1 if blue, 0 if red
//     // [HideInInspector]
//     [HideInInspector]
//     public Vector3 ballStartingPos;

//     Bounds areaBounds;
//     public bool drawSpawnAreaGizmo;
//     Vector3 spawnAreaSize;
//     public float spawnAreaMarginMultiplier; //ex: .9 means 90% of spawn area will be used.... .1 margin will be left (so players don't spawn off of the edge)
//     // public List<Collider> cannotSpawnOverTheseColliders = new List<Collider>(); //to prevent ball from spawning in the goal
//     // bool canUseThisPos = false;
//     // public GameObject goal1Holder;
//     // public GameObject goal2Holder;
//     // public GameObject academy;


//     public float agentRunSpeed;
//     public float agentRotationSpeed;

//     public float strikerPunish;
//     public float strikerReward;
//     public float defenderPunish;
//     public float defenderReward;
//     public float goaliePunish;
//     public float goalieReward;
//     // public Vector2 strikerGoalRewards; //x is negative reward, y 
//     // public Vector2 defenderGoalRewards; //
//     // public Vector2 goalieGoalRewards; //
//     public float goalScoreByTeamReward; //if red scores they get this reward & vice versa
//     public float goalScoreAgainstTeamReward; //if red scores we deduct some from blue & vice versa
// 	// Camera camera;
//     // [HideInInspector]
// 	// public Vector3 cameraForward;
//     // [HideInInspector]
// 	// public Vector3 cameraRight;
//     // Vector3 lastCameraPos;
//     // Quaternion lastCameraRot;

//     public GameObject goalTextUI;
//     public float totalPlayers;
//     public float minWaitTimeToRespawnBall = 5; //the ball can a spawn every 5 sec
//     [HideInInspector]
//     public bool canResetBall;
//     public bool useSpawnPoint;
//     public Transform spawnPoint;
//     Material groundMaterial;
// 	Renderer groundRenderer;

    
//     // void CameraMoveCheck()
//     // {
//     //     if (camera.transform.position != lastCameraPos || camera.transform.rotation != lastCameraRot)
//     //     {
//     //         UpdateCameraDir();
//     //     }
//     // }
//     // public void UpdateCameraDir()
// 	// {
// 	// 	cameraForward = camera.transform.forward;
// 	// 	cameraForward.y = 0;
// 	// 	cameraForward.Normalize();
// 	// 	cameraRight = camera.transform.right;
// 	// 	cameraRight.Normalize();
// 	// }

//     // [HideInInspector]
//     // public float redTeamFloat; //0 (for state capture)
//     // public float blueTeamFloat; //1 (for state capture)
// 	// Use this for initialization

//     public IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
// 	{
// 		groundRenderer.material = mat;
// 		yield return new WaitForSeconds(time); //wait for 2 sec
// 		groundRenderer.material = groundMaterial;
// 	}


// 	void Start () {
//         groundRenderer = ground.GetComponent<Renderer>(); //get the ground renderer so we can change the material when a goal is scored
// 		groundMaterial = groundRenderer.material; //starting material
//         canResetBall = true;
//         // camera = Camera.main;
//         // lastCameraPos = camera.transform.position;
//         // lastCameraRot = camera.transform.rotation;

//         // UpdateCameraDir();

//         if(goalTextUI){goalTextUI.SetActive(false);}
        
//         ballRB = ball.GetComponent<Rigidbody>();
//         ballController = ball.GetComponent<SoccerBallController>();
//         ballStartingPos = ball.transform.position;
//         Mesh mesh = ground.GetComponent<MeshFilter>().mesh;  //get the ground's mesh
//         // Vector3[] vertices = mesh.vertices;
//         // Vector2[] uvs = new Vector2[vertices.Length];
//         areaBounds = ground.GetComponent<Collider>().bounds; //get the ground's bounds
// 		// academy = GameObject.Find("Academy");
// 	}

//     // void Update()
//     // {
//     //     CameraMoveCheck();
//     //     if (!Physics.Raycast(ballRB.position, Vector3.down, 3, groundLayer)) //if the agent or the block has gone over the edge, we done.
// 	// 	{
//     //         ResetBall();
// 	// 	}
//     // }
//     IEnumerator ShowGoalUI()
//     {
//         if(goalTextUI)goalTextUI.SetActive(true);
//         yield return new WaitForSeconds(.25f);
//         if(goalTextUI)goalTextUI.SetActive(false);

//     }


//     // public void RedScores()  //called if the blue goal is touched with the ball
//     // {
//     //     foreach(PlayerState ps in redPlayers)
//     //     {
//     //         ps.agentScriptRed.reward += goalScoreByTeamReward;
//     //         ps.agentScriptRed.done = true; //is this necessary? can we just reset the area without marking the agent done?
//     //     }
//     //     foreach(PlayerState ps in bluePlayers)
//     //     {
//     //         ps.agentScriptBlue.reward += goalScoreAgainstTeamReward; //small penalty for oppoosing team
//     //         ps.agentScriptBlue.done = true; //is this necessary? can we just reset the area without marking the agent done?
//     //     }

//     //     ResetBall();
//     //     AllPlayersDone();

//     //     if(goalTextUI)
//     //     {
//     //         StartCoroutine(ShowGoalUI());
//     //     }
//     // }

//     // public void AllPlayersDone()
//     // {
//     //     foreach(PlayerState ps in redPlayers)
//     //     {
//     //         ps.agentScriptRed.done = true; 
//     //     }
//     //     foreach(PlayerState ps in bluePlayers)
//     //     {
//     //         ps.agentScriptBlue.done = true;
//     //     }

//     // }
//     // public void BlueScores() //called if the red goal is touched with the ball
//     // {
//     //     foreach(PlayerState ps in redPlayers)
//     //     {
//     //         ps.agentScriptRed.reward += goalScoreAgainstTeamReward; //small penalty for oppoosing team
//     //         ps.agentScriptRed.done = true; //is this necessary? can we just reset the area without marking the agent done?
//     //     }
//     //     foreach(PlayerState ps in bluePlayers)
//     //     {
//     //         ps.agentScriptBlue.reward += goalScoreByTeamReward;
//     //         ps.agentScriptBlue.done = true; //is this necessary? can we just reset the area without marking the agent done?
//     //     }

//     //     AllPlayersDone();
//     //     ResetBall();

//     //     if(goalTextUI)
//     //     {
//     //         StartCoroutine(ShowGoalUI());
//     //     }
//     // }

//     public void AllPlayersDone(float reward)
//     {
//         foreach(PlayerState ps in playerStates)
//         {
//             if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
//             {
//                 if(reward != 0)
//                 {
//                     ps.agentScript.reward += reward;
//                 }
//                 ps.agentScript.done = true; 
//             }

//         }
//         // foreach(PlayerState ps in bluePlayers)
//         // {
//         //     if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
//         //     {
//         //         if(reward != 0)
//         //         {
//         //             ps.agentScript.reward += reward;
//         //         }
//         //     }
//         //         ps.agentScript.done = true;
//         // }

//     }
//     // public void AllPlayersDone(float reward)
//     // {
//     //     foreach(PlayerState ps in play)
//     //     {
//     //         if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
//     //         {
//     //             if(reward != 0)
//     //             {
//     //                 ps.agentScript.reward += reward;
//     //             }
//     //         }
//     //             ps.agentScript.done = true; 

//     //     }
//     //     foreach(PlayerState ps in bluePlayers)
//     //     {
//     //         if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
//     //         {
//     //             if(reward != 0)
//     //             {
//     //                 ps.agentScript.reward += reward;
//     //             }
//     //         }
//     //             ps.agentScript.done = true;
//     //     }

//     // }

//     public void RedScores() //ball touched the blue goal
//     {
//         // if(blueGoalie && blueGoalie.gameObject.activeInHierarchy)
//         // {

//         //     blueGoalie.reward -= 1; //do your job bro
//         //     // blueGoalie.reward += goaliePunish; //do your job bro
//         //     blueGoalie.done = true; //do your job bro
//         //     redGoalie.done = true; //reset
//         //                 print("penalize blue goalie" + blueGoalie + blueGoalie.reward);
//         // }
//         RewardOrPunishRedTeam(strikerReward, defenderReward, goalieReward);
//         RewardOrPunishBlueTeam(strikerPunish, defenderPunish, goaliePunish);
//         // foreach(PlayerState ps in redPlayers)
//         // {
//         //     // print("blue goal touched reward red player " + ps.agentScript.gameObject.name);
//         //     ps.agentScript.reward += goalScoreByTeamReward;
//         //     ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
//         // }
//         // foreach(PlayerState ps in bluePlayers)
//         // {
//         //     // print("blue goal touched punish blue player " + ps.agentScript.gameObject.name);
//         //     ps.agentScript.reward += goalScoreAgainstTeamReward; //small penalty for oppoosing team
//         //     ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
//         // }

// 		StartCoroutine(GoalScoredSwapGroundMaterial(redScoredMaterial, 2));
//         ResetBall();
//         // AllPlayersDone();

//         if(goalTextUI)
//         {
//             StartCoroutine(ShowGoalUI());
//         }
//     }

//     public void BlueScores() //ball touched the red goal
//     {
//         // if(redGoalie && redGoalie.gameObject.activeInHierarchy)
//         // {
//         //     // redGoalie.reward += goaliePunish; //do your job bro
//         //     redGoalie.reward -= 1; //do your job bro
//         //     redGoalie.done = true; //do your job bro
//         //     blueGoalie.done = true; //reset
//         //     print("penalize red goalie" + redGoalie.reward);
//         // }
//         RewardOrPunishBlueTeam(strikerReward, defenderReward, goalieReward);
//         RewardOrPunishRedTeam(strikerPunish, defenderPunish, goaliePunish);


//         // foreach(PlayerState ps in redPlayers)
//         // {
//         //     // print("red goal touched punish red player " + ps.agentScript.gameObject.name);
//         //     ps.agentScript.reward += goalScoreAgainstTeamReward; //small penalty for oppoosing team
//         //     ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
//         // }
//         // foreach(PlayerState ps in bluePlayers)
//         // {
//         //     // print("red goal touched reward blue player " + ps.agentScript.gameObject.name);
//         //     ps.agentScript.reward += goalScoreByTeamReward;
//         //     ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
//         // }
// 		StartCoroutine(GoalScoredSwapGroundMaterial(blueScoredMaterial, 2));

//         // AllPlayersDone();
//         ResetBall();

//         if(goalTextUI)
//         {
//             StartCoroutine(ShowGoalUI());
//         }
//     }



//     public void RewardOrPunishBlueTeam(float striker, float defender, float goalie) //ball touched the red goal
//     {
//         foreach(PlayerState ps in bluePlayers)
//         {
//             if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
//             {
//                 ps.agentScript.reward += striker;
//                 ps.agentScript.done = true;
//             }
//             if(ps.agentScript.agentRole == AgentSoccer.AgentRole.defender)
//             {
//                 ps.agentScript.reward += defender;
//                 ps.agentScript.done = true;
//             }
//             if(ps.agentScript.agentRole == AgentSoccer.AgentRole.goalie)
//             {
//                 ps.agentScript.reward += goalie;
//                 ps.agentScript.done = true;
//             }
//         }

//     }

//     public void RewardOrPunishRedTeam(float striker, float defender, float goalie) //ball touched the red goal
//     {
//         foreach(PlayerState ps in redPlayers)
//         {
//             if(ps.agentScript.agentRole == AgentSoccer.AgentRole.striker)
//             {
//                 ps.agentScript.reward += striker;
//                 ps.agentScript.done = true;
//             }
//             if(ps.agentScript.agentRole == AgentSoccer.AgentRole.defender)
//             {
//                 ps.agentScript.reward += defender;
//                 ps.agentScript.done = true;
//             }
//             if(ps.agentScript.agentRole == AgentSoccer.AgentRole.goalie)
//             {
//                 ps.agentScript.reward += goalie;
//                 ps.agentScript.done = true;
//             }
//         }
//     }

//     // public void PlayerScoredWrongGoal(AgentSoccer player)
//     // {
//     //     player.reward -= 1;
//     //     AllPlayersDone();
//     //     ResetBall();
//     // }
//     public void PlayerScoredWrongGoal(AgentSoccer player)
//     {
//         player.reward -= goalScoreAgainstTeamReward;
//         AllPlayersDone(0);
//         ResetBall();
//     }

//     public void PlayerScoredCorrectGoal(AgentSoccer player)
//     {
//         // AgentSoccer agent = player.GetComponent<AgentSoccer>();
//         player.reward += goalScoreByTeamReward;
//         AllPlayersDone(0);
//         ResetBall();
//     }


//     void OnDrawGizmos()
//     {
//         if(drawSpawnAreaGizmo)
//         {
//             // spawnAreaSize = areaBounds.size * spawnAreaMarginMultiplier;
//             spawnAreaSize = areaBounds.extents;
//             Gizmos.DrawWireCube(ground.transform.position, spawnAreaSize);
//         }
//     }

//     public Vector3 GetRandomSpawnPos()
//     {
//         Vector3 randomSpawnPos = Vector3.zero;
//         float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
//         // float posY = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
//         float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier);
//         if(useSpawnPoint)
//         {
//             randomSpawnPos = spawnPoint.position + new Vector3(randomPosX, 2, randomPosZ );
//         }
//         else
//         {
//             randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 2, randomPosZ );
//         }
//         return randomSpawnPos;
//     }
	
//     void SpawnObjAtPos(GameObject obj, Vector3 pos)
//     {
//         obj.transform.position = pos;
//         // canUseThisPos = true;
//     }

// 	// // Update is called once per frame
// 	// void Update () {

//     //     // }
// 	// }
//     IEnumerator ResetBallTimer()
//     {
//         canResetBall = false;
//         yield return new WaitForSeconds(minWaitTimeToRespawnBall);
//         canResetBall = true;
//     }
//     public void ResetBall()
//     {
        
//         ballRB.velocity = Vector3.zero;
//         ballRB.angularVelocity = Vector3.zero;
//         ball.transform.position = GetRandomSpawnPos();
//         // ballController.lastTouchedBy = null;
//         // if(canResetBall)
//         // {

//         // }
//         StartCoroutine(ResetBallTimer());
//         // randomPos = GetRandomSpawnPos(); //get a random pos
//         // if (!Physics.CheckSphere(GetRandomSpawnPos(), .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
//         // {
//         //     SpawnObjAtPos(ball, GetRandomSpawnPos());
//         // }
//         // else //otherwise let's use the default spawnLocation
//         // {
//         //     SpawnObjAtPos(ball, ballStartingPos);
//         // } 
//     }
// 	public void ResetAllPlayers()
// 	{

//         Vector3 randomPos;

//         foreach(PlayerState ps in playerStates)
//         {
//              randomPos = GetRandomSpawnPos(); //get a random pos

//             if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
//             {
//                 SpawnObjAtPos(ps.agentScript.gameObject, randomPos);
//             }
//             else //otherwise let's use the default spawnLocation
//             {
//                 SpawnObjAtPos(ps.agentScript.gameObject, ps.startingPos);
//             }        
//         }


//         // foreach(PlayerState ps in redPlayers)
//         // {
//         //      randomPos = GetRandomSpawnPos(); //get a random pos

//         //     if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
//         //     {
//         //         SpawnObjAtPos(ps.agentScriptRed.gameObject, randomPos);
//         //     }
//         //     else //otherwise let's use the default spawnLocation
//         //     {
//         //         SpawnObjAtPos(ps.agentScriptRed.gameObject, ps.startingPos);
//         //     }        
//         // }
//         // foreach(PlayerState ps in bluePlayers)
//         // {
//         //      randomPos = GetRandomSpawnPos(); //get a random pos

//         //     if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
//         //     {
//         //         SpawnObjAtPos(ps.agentScriptBlue.gameObject, randomPos);
//         //     }
//         //     else //otherwise let's use the default spawnLocation
//         //     {
//         //         SpawnObjAtPos(ps.agentScriptBlue.gameObject, ps.startingPos);
//         //     }        
//         // }




//         // foreach(AgentSoccer player in redPlayers)
//         // {
//         //     // player.agentRB.velocity = Vector3.zero;
//         //     randomPos = GetRandomSpawnPos(); //get a random pos

//         //     if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
//         //     {
//         //         SpawnObjAtPos(player.gameObject, randomPos);
//         //     }
//         //     else //otherwise let's use the default spawnLocation
//         //     {
//         //         SpawnObjAtPos(player.gameObject, player.startingPos);
//         //     }
//         // }

//         // foreach(AgentSoccer player in bluePlayers)
//         // {
//         //     // player.agentRB.velocity = Vector3.zero;
//         //     randomPos = GetRandomSpawnPos(); //get a random pos

//         //     if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
//         //     {
//         //         SpawnObjAtPos(player.gameObject, randomPos);
//         //     }
//         //     else //otherwise let's use the default spawnLocation
//         //     {
//         //         SpawnObjAtPos(player.gameObject, player.startingPos);
//         //     }
//         // }
// 	}



//     //STATE COLLECTER STUFF

//     //Add the x,y,z components of this Vector3 to the state list
//     void CollectVector3State(List<float> state, Vector3 v)
//     {
//         state.Add(v.x);
//         state.Add(v.y);
//         state.Add(v.z);
//     }

//     // void CollectRotationState(List<float> state, Quaternion q)
//     // {
//     //     state.Add(q.x);
//     //     state.Add(q.y);
//     //     state.Add(q.z);
//     // }

//     void CollectRotationState(List<float> state, Transform t)
//     {
// 		state.Add(t.rotation.eulerAngles.x/180.0f-1.0f);
// 		state.Add(t.rotation.eulerAngles.y/180.0f-1.0f);
// 		state.Add(t.rotation.eulerAngles.z/180.0f-1.0f);
//     }

//     //we can only collect floats in CollecState so we need to convert bools to floats
//     float ConvertBoolToFloat(bool b)
//     {
//         float f = b == true? 1 : 0;
//         return f;
//     }

//     void CollectPlayerState(PlayerState ps)
//     {

//         // Vector3 playerDirToTargetGoal = ps.targetGoal.transform.position - ps.agentRB.position;
//         // Vector3 playerDirToDefendGoal = ps.defendGoal.transform.position - ps.agentRB.position;
//         Vector3 playerPos = ps.agentRB.position - ground.transform.position;
//         Vector3 ballPos = ballRB.position - ground.transform.position;
//         Vector3 playerDirToBall = ballRB.position - ps.agentRB.position;
//         // Vector3 playerDirToRedGoal = redGoal.transform.position - ps.agentRB.position;
//         // Vector3 playerDirToBlueGoal = blueGoal.transform.position - ps.agentRB.position;
//         // Vector3 playerPos = ps.agentRB.position - ground.transform.position;
//         // Vector3 playerDirToBall = ballRB.position - ps.agentRB.position;

//         // float playerDistToBall = playerDirToBall.sqrMagnitude;
//         // ps.state.Add(playerDistToBall);

//         Vector3 redGoalPosition = redGoal.transform.position - ground.transform.position;
//         Vector3 blueGoalPosition = blueGoal.transform.position - ground.transform.position;
//         // Vector3 ballDirToRedGoal = redGoal.transform.position - ballRB.position;
//         // Vector3 ballDirToBlueGoal = blueGoal.transform.position - ballRB.position;
//         // Vector3 ballDirToTargetGoal = ps.targetGoal.transform.position - ballRB.position;
//         // Vector3 ballDirToDefendGoal =  ps.defendGoal.transform.position - ballRB.position;

//         //35 total per player
//         ps.state.Clear(); //instead of creating a new list each tick we will reuse this one
//         // ps.state.Add(ps.playerID); //whoami 
//         // ps.state.Add(ps.teamFloat); //which team 
//         // ps.state.Add(ps.agentRoleFloat);
//         CollectVector3State(ps.state, ps.agentRB.velocity); //agent's vel
//         CollectRotationState(ps.state, ps.agentRB.transform); //agent's rotation
//         CollectVector3State(ps.state, playerPos); //dir from player to red goal
//         CollectVector3State(ps.state, ballPos); //dir from player to red goal
//         CollectVector3State(ps.state, playerDirToBall); //dir from agent to ball
//         CollectVector3State(ps.state, redGoalPosition);  //red goal abs position
//         CollectVector3State(ps.state, blueGoalPosition); //blue goal abs position
//         // CollectVector3State(ps.state, playerDirToTargetGoal); //dir from player to red goal
//         // CollectVector3State(ps.state, playerDirToDefendGoal); //dir from player to blue goal
//         // CollectVector3State(ps.state, ballDirToTargetGoal); //dir from ball to target goal
//         // CollectVector3State(ps.state, ballDirToDefendGoal); //dir from ball to defend goal


//         // CollectVector3State(ps.state, ballDirToRedGoal); //dir from ball to target goal
//         // CollectVector3State(ps.state, ballDirToBlueGoal); //dir from ball to defend goal
//         // CollectVector3State(ps.state, playerDirToRedGoal); //dir from player to red goal
//         // CollectVector3State(ps.state, playerDirToBlueGoal); //dir from player to blue goal
//         // CollectVector3State(ps.state, ballDirToRedGoal); //dir from ball to target goal
//         // CollectVector3State(ps.state, ballDirToBlueGoal); //dir from ball to defend goal


//         // CollectVector3State(ps.state, ps.targetGoal.transform.position - ps.agentRB.position); //dir from player to target goal
//         // CollectVector3State(ps.state, ps.defendGoal.transform.position - ps.agentRB.position); //dir from player to defend goal
//         // CollectVector3State(ps.state, ps.targetGoal.transform.position - ballRB.position); //dir from ball to target goal
//         // CollectVector3State(ps.state, ps.defendGoal.transform.position - ballRB.position); //dir from ball to defend goal
//         CollectVector3State(ps.state, ballRB.velocity);




//         // Vector3 playerDirToTargetGoal = ps.targetGoal.transform.position - ps.agentRB.position;
//         // // Vector3 playerDirToDefendGoal = ps.defendGoal.transform.position - ps.agentRB.position;
//         // Vector3 playerPos = ps.agentRB.position - ground.transform.position;
//         // Vector3 playerDirToBall = ballRB.position - ps.agentRB.position;
//         // // Vector3 playerDirToRedGoal = redGoal.transform.position - ps.agentRB.position;
//         // // Vector3 playerDirToBlueGoal = blueGoal.transform.position - ps.agentRB.position;
//         // // Vector3 playerPos = ps.agentRB.position - ground.transform.position;
//         // // Vector3 playerDirToBall = ballRB.position - ps.agentRB.position;

//         // // float playerDistToBall = playerDirToBall.sqrMagnitude;
//         // // ps.state.Add(playerDistToBall);

//         // // Vector3 redGoalPosition = redGoal.transform.position - ground.transform.position;
//         // // Vector3 blueGoalPosition = blueGoal.transform.position - ground.transform.position;
//         // // Vector3 ballDirToRedGoal = redGoal.transform.position - ballRB.position;
//         // // Vector3 ballDirToBlueGoal = blueGoal.transform.position - ballRB.position;
//         // Vector3 ballDirToTargetGoal = ps.targetGoal.transform.position - ballRB.position;
//         // // Vector3 ballDirToDefendGoal =  ps.defendGoal.transform.position - ballRB.position;

//         // //35 total per player
//         // ps.state.Clear(); //instead of creating a new list each tick we will reuse this one
//         // // ps.state.Add(ps.playerID); //whoami 
//         // // ps.state.Add(ps.teamFloat); //which team 
//         // CollectVector3State(ps.state, ps.agentRB.velocity); //agent's vel
//         // CollectRotationState(ps.state, ps.agentRB.transform); //agent's rotation
//         // CollectVector3State(ps.state, playerPos); //dir from player to red goal
//         // CollectVector3State(ps.state, playerDirToBall); //dir from agent to ball
//         // // CollectVector3State(ps.state, redGoalPosition);  //red goal abs position
//         // // CollectVector3State(ps.state, blueGoalPosition); //blue goal abs position
//         // CollectVector3State(ps.state, playerDirToTargetGoal); //dir from player to red goal
//         // // CollectVector3State(ps.state, playerDirToDefendGoal); //dir from player to blue goal
//         // CollectVector3State(ps.state, ballDirToTargetGoal); //dir from ball to target goal
//         // // CollectVector3State(ps.state, ballDirToDefendGoal); //dir from ball to defend goal


//         // // CollectVector3State(ps.state, ballDirToRedGoal); //dir from ball to target goal
//         // // CollectVector3State(ps.state, ballDirToBlueGoal); //dir from ball to defend goal
//         // // CollectVector3State(ps.state, playerDirToRedGoal); //dir from player to red goal
//         // // CollectVector3State(ps.state, playerDirToBlueGoal); //dir from player to blue goal
//         // // CollectVector3State(ps.state, ballDirToRedGoal); //dir from ball to target goal
//         // // CollectVector3State(ps.state, ballDirToBlueGoal); //dir from ball to defend goal


//         // // CollectVector3State(ps.state, ps.targetGoal.transform.position - ps.agentRB.position); //dir from player to target goal
//         // // CollectVector3State(ps.state, ps.defendGoal.transform.position - ps.agentRB.position); //dir from player to defend goal
//         // // CollectVector3State(ps.state, ps.targetGoal.transform.position - ballRB.position); //dir from ball to target goal
//         // // CollectVector3State(ps.state, ps.defendGoal.transform.position - ballRB.position); //dir from ball to defend goal
//         // CollectVector3State(ps.state, ballRB.velocity);
        
        
        
        
        
        
        
        
        
        
        
//         // if(ps.teamFloat == 1) //blue team
//         // {
//         //     bluePlayers.Add(ps);
//         // }
//         // else if (ps.teamFloat == 0) //redteam
//         // {
//         //     redPlayers.Add(ps);
//         // }







//         // //26 total per player
//         // ps.state.Clear(); //instead of creating a new list each tick we will reuse this one
//         // ps.state.Add(ps.playerID); //whoami 
//         // ps.state.Add(ps.teamFloat); //which team 
//         // CollectVector3State(ps.state, ps.agentRB.velocity); //agent's vel
//         // CollectRotationState(ps.state, ps.agentRB.rotation); //agent's rotation
//         // CollectVector3State(ps.state, redGoal.transform.position - ps.agentRB.position); //dir from player to red goal
//         // CollectVector3State(ps.state, blueGoal.transform.position - ps.agentRB.position); //dir from player to target goal
//         // CollectVector3State(ps.state, redGoal.transform.position - ballRB.position); //dir from ball to target goal
//         // CollectVector3State(ps.state, blueGoal.transform.position - ballRB.position); //dir from ball to defend goal


//         // // CollectVector3State(ps.state, ps.targetGoal.transform.position - ps.agentRB.position); //dir from player to target goal
//         // // CollectVector3State(ps.state, ps.defendGoal.transform.position - ps.agentRB.position); //dir from player to defend goal
//         // // CollectVector3State(ps.state, ps.targetGoal.transform.position - ballRB.position); //dir from ball to target goal
//         // // CollectVector3State(ps.state, ps.defendGoal.transform.position - ballRB.position); //dir from ball to defend goal
//         // CollectVector3State(ps.state, ballRB.position - ps.agentRB.position); //dir from agent to ball
//         // CollectVector3State(ps.state, ballRB.velocity);
//         // if(ps.teamFloat == 1) //blue team
//         // {
//         //     bluePlayers.Add(ps);
//         // }
//         // else if (ps.teamFloat == 0) //redteam
//         // {
//         //     redPlayers.Add(ps);
//         // }
//     }


//     void FixedUpdate()
//     {
//         // print("clear player lists");
//         // bluePlayers.Clear();
//         // redPlayers.Clear();
//         if(playerStates.Count > 0)
//         {
//             //UPDATE ALL PLAYERS FIRST
//             foreach(PlayerState ps in playerStates)
//             {
//                 CollectPlayerState(ps);
//             }
            

//             // //SEPARATE BY TEAM
//             // foreach(PlayerState ps in playerStates)
//             // {
//             //     if(ps.teamFloat == 1) //blue team
//             //     {
//             //         bluePlayers.Add(ps);
//             //     }
//             //     else if (ps.teamFloat == 0)
//             //     {
//             //         redPlayers.Add(ps);
//             //     }
//             // }
//         }


//     }



// }

























// // using System.Collections;
// // using System.Collections.Generic;
// // using UnityEngine;


// // [System.Serializable]
// // public class PlayerState
// // {
// //     //all of this gets setup in AgentSoccer.CS Awake()
// //     public int playerIndex; //index pos on the team
// //     public float playerID; //index pos on the team
// //     public float teamFloat; //1 = blue, 0 = red
// //     public List<float> state = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// //     public GameObject defendGoal; //goal we need to defend
// //     public GameObject targetGoal; //goal we need to attack
// //     public Rigidbody agentRB; //the agent's rb
// //     public Vector3 startingPos; //the agent's starting position
// //     public AgentSoccer agentScript; //this is the agent's script
// //     public AgentSoccerRed agentScriptRed; //this is the agent's script
// //     public AgentSoccerBlue agentScriptBlue; //this is the agent's script
// //     // public List<float> myState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// // 	// public List<float> myTeamsState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// // 	// public List<float> otherTeamsState = new List<float>(); //list for state data. to be updated every FixedUpdate in this script
// // 	// public List<float> stateToSubmitToBrain = new List<float>(); //list for state data. to be updated every FixedUpdate in this script

// // }

// // public class SoccerFieldArea : MonoBehaviour {

// //     // void SortPlayerStates()
// //     // {
// //     //     if(playerStates.Count > 0)
// //     //     {
// //     //         foreach(Player)
// //     //     }
// //     // }

// //     public Brain redBrain;
// //     public Brain blueBrain;
// //     public GameObject redGoal;
// //     public GameObject blueGoal;
// //     public GameObject ball;
// //     [HideInInspector]
// //     public Rigidbody ballRB;
// //     public GameObject ground; //to be used to determine spawn areas
// //     public LayerMask groundLayer;
// //     SoccerBallController ballController;
// //     public List<PlayerState> redPlayers = new List<PlayerState>();
// //     public List<PlayerState> bluePlayers = new List<PlayerState>();
// //     public List<PlayerState> playerStates = new List<PlayerState>();
// //     float blueTeamID; //1 if blue, 0 if red
// // 	float redTeamID; //1 if blue, 0 if red
// //     // [HideInInspector]
// //     [HideInInspector]
// //     public Vector3 ballStartingPos;

// //     Bounds areaBounds;
// //     public bool drawSpawnAreaGizmo;
// //     Vector3 spawnAreaSize;
// //     public float spawnAreaMarginMultiplier; //ex: .9 means 90% of spawn area will be used.... .1 margin will be left (so players don't spawn off of the edge)
// //     // public List<Collider> cannotSpawnOverTheseColliders = new List<Collider>(); //to prevent ball from spawning in the goal
// //     // bool canUseThisPos = false;
// //     // public GameObject goal1Holder;
// //     // public GameObject goal2Holder;
// //     // public GameObject academy;


// //     public float agentRunSpeed;
// //     public float agentRotationSpeed;

// //     public float goalScoreByTeamReward; //if red scores they get this reward & vice versa
// //     public float goalScoreAgainstTeamReward; //if red scores we deduct some from blue & vice versa
// // 	Camera camera;
// //     [HideInInspector]
// // 	public Vector3 cameraForward;
// //     [HideInInspector]
// // 	public Vector3 cameraRight;
// //     Vector3 lastCameraPos;
// //     Quaternion lastCameraRot;

// //     public GameObject goalTextUI;
// //     public float totalPlayers;
// //     public float minWaitTimeToRespawnBall = 5; //the ball can a spawn every 5 sec
// //     [HideInInspector]
// //     public bool canResetBall;
// //     public bool useSpawnPoint;
// //     public Transform spawnPoint;

    
// //     void CameraMoveCheck()
// //     {
// //         if (camera.transform.position != lastCameraPos || camera.transform.rotation != lastCameraRot)
// //         {
// //             UpdateCameraDir();
// //         }
// //     }
// //     public void UpdateCameraDir()
// // 	{
// // 		cameraForward = camera.transform.forward;
// // 		cameraForward.y = 0;
// // 		cameraForward.Normalize();
// // 		cameraRight = camera.transform.right;
// // 		cameraRight.Normalize();
// // 	}

// //     // [HideInInspector]
// //     // public float redTeamFloat; //0 (for state capture)
// //     // public float blueTeamFloat; //1 (for state capture)
// // 	// Use this for initialization

    
// // 	void Start () {
// //         canResetBall = true;
// //         camera = Camera.main;
// //         lastCameraPos = camera.transform.position;
// //         lastCameraRot = camera.transform.rotation;

// //         UpdateCameraDir();

// //         if(goalTextUI){goalTextUI.SetActive(false);}
        
// //         ballRB = ball.GetComponent<Rigidbody>();
// //         ballController = ball.GetComponent<SoccerBallController>();
// //         ballStartingPos = ball.transform.position;
// //         Mesh mesh = ground.GetComponent<MeshFilter>().mesh;  //get the ground's mesh
// //         // Vector3[] vertices = mesh.vertices;
// //         // Vector2[] uvs = new Vector2[vertices.Length];
// //         areaBounds = ground.GetComponent<Collider>().bounds; //get the ground's bounds
// // 		// academy = GameObject.Find("Academy");
// // 	}

// //     void Update()
// //     {
// //         CameraMoveCheck();
// //         if (!Physics.Raycast(ballRB.position, Vector3.down, 3, groundLayer)) //if the agent or the block has gone over the edge, we done.
// // 		{
// //             ResetBall();
// // 		}
// //     }
// //     IEnumerator ShowGoalUI()
// //     {
// //         if(goalTextUI)goalTextUI.SetActive(true);
// //         yield return new WaitForSeconds(.25f);
// //         if(goalTextUI)goalTextUI.SetActive(false);

// //     }


// //     public void RedScores()  //called if the read team scores
// //     {
// //         foreach(PlayerState ps in redPlayers)
// //         {
// //             ps.agentScript.reward += goalScoreByTeamReward;
// //             ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
// //         }
// //         foreach(PlayerState ps in bluePlayers)
// //         {
// //             ps.agentScript.reward -= goalScoreAgainstTeamReward; //small penalty for oppoosing team
// //             ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
// //         }

// //         ResetBall();
// //         if(goalTextUI)
// //         {
// //             StartCoroutine(ShowGoalUI());
// //         }
// //     }

// //     public void AllPlayersDone()
// //     {
// //         foreach(PlayerState ps in redPlayers)
// //         {
// //             ps.agentScript.done = true; 
// //         }
// //         foreach(PlayerState ps in bluePlayers)
// //         {
// //             ps.agentScript.done = true;
// //         }

// //     }

// //     public void PlayerScoredWrongGoal(AgentSoccer player)
// //     {
// //         player.reward -= 1;
// //         AllPlayersDone();
// //         ResetBall();
// //     }

// //     public void PlayerScoredCorrectGoal(AgentSoccer player)
// //     {
// //         // AgentSoccer agent = player.GetComponent<AgentSoccer>();
// //         player.reward += 1;
// //         AllPlayersDone();
// //         ResetBall();
// //     }
// //     public void BlueScores() //called if the blue team scores
// //     {
// //         foreach(PlayerState ps in redPlayers)
// //         {
// //             ps.agentScript.reward -= goalScoreAgainstTeamReward; //small penalty for oppoosing team
// //             ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
// //         }
// //         foreach(PlayerState ps in bluePlayers)
// //         {
// //             ps.agentScript.reward += goalScoreByTeamReward;
// //             ps.agentScript.done = true; //is this necessary? can we just reset the area without marking the agent done?
// //         }
// //         ResetBall();

// //         if(goalTextUI)
// //         {
// //             StartCoroutine(ShowGoalUI());
// //         }
// //     }


// //     void OnDrawGizmos()
// //     {
// //         if(drawSpawnAreaGizmo)
// //         {
// //             // spawnAreaSize = areaBounds.size * spawnAreaMarginMultiplier;
// //             spawnAreaSize = areaBounds.extents;
// //             Gizmos.DrawWireCube(ground.transform.position, spawnAreaSize);
// //         }
// //     }

// //     public Vector3 GetRandomSpawnPos()
// //     {
// //         Vector3 randomSpawnPos = Vector3.zero;
// //         float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
// //         // float posY = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier, areaBounds.extents.x * spawnAreaMarginMultiplier);
// //         float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier, areaBounds.extents.z * spawnAreaMarginMultiplier);
// //         if(useSpawnPoint)
// //         {
// //             randomSpawnPos = spawnPoint.position + new Vector3(randomPosX, 2, randomPosZ );
// //         }
// //         else
// //         {
// //             randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 2, randomPosZ );
// //         }
// //         return randomSpawnPos;
// //     }
	
// //     void SpawnObjAtPos(GameObject obj, Vector3 pos)
// //     {
// //         obj.transform.position = pos;
// //         // canUseThisPos = true;
// //     }

// // 	// // Update is called once per frame
// // 	// void Update () {

// //     //     // }
// // 	// }
// //     IEnumerator ResetBallTimer()
// //     {
// //         canResetBall = false;
// //         yield return new WaitForSeconds(minWaitTimeToRespawnBall);
// //         canResetBall = true;
// //     }
// //     public void ResetBall()
// //     {
        
// //         ballRB.velocity = Vector3.zero;
// //         ballRB.angularVelocity = Vector3.zero;
// //         ball.transform.position = GetRandomSpawnPos();
// //         ballController.lastTouchedBy = null;
// //         // if(canResetBall)
// //         // {

// //         // }
// //         StartCoroutine(ResetBallTimer());
// //         // randomPos = GetRandomSpawnPos(); //get a random pos
// //         // if (!Physics.CheckSphere(GetRandomSpawnPos(), .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
// //         // {
// //         //     SpawnObjAtPos(ball, GetRandomSpawnPos());
// //         // }
// //         // else //otherwise let's use the default spawnLocation
// //         // {
// //         //     SpawnObjAtPos(ball, ballStartingPos);
// //         // } 
// //     }
// // 	public void ResetAllPlayers()
// // 	{

// //         Vector3 randomPos;

// //         foreach(PlayerState ps in playerStates)
// //         {
// //              randomPos = GetRandomSpawnPos(); //get a random pos

// //             if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
// //             {
// //                 SpawnObjAtPos(ps.agentScript.gameObject, randomPos);
// //             }
// //             else //otherwise let's use the default spawnLocation
// //             {
// //                 SpawnObjAtPos(ps.agentScript.gameObject, ps.startingPos);
// //             }        
// //         }




// //         // foreach(AgentSoccer player in redPlayers)
// //         // {
// //         //     // player.agentRB.velocity = Vector3.zero;
// //         //     randomPos = GetRandomSpawnPos(); //get a random pos

// //         //     if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
// //         //     {
// //         //         SpawnObjAtPos(player.gameObject, randomPos);
// //         //     }
// //         //     else //otherwise let's use the default spawnLocation
// //         //     {
// //         //         SpawnObjAtPos(player.gameObject, player.startingPos);
// //         //     }
// //         // }

// //         // foreach(AgentSoccer player in bluePlayers)
// //         // {
// //         //     // player.agentRB.velocity = Vector3.zero;
// //         //     randomPos = GetRandomSpawnPos(); //get a random pos

// //         //     if (!Physics.CheckSphere(randomPos, .6f)) //if not overlapping we can use it. .6 is used because it's a bit bigger than the radius of the ball which is .5
// //         //     {
// //         //         SpawnObjAtPos(player.gameObject, randomPos);
// //         //     }
// //         //     else //otherwise let's use the default spawnLocation
// //         //     {
// //         //         SpawnObjAtPos(player.gameObject, player.startingPos);
// //         //     }
// //         // }
// // 	}



// //     //STATE COLLECTER STUFF

// //     //Add the x,y,z components of this Vector3 to the state list
// //     void CollectVector3State(List<float> state, Vector3 v)
// //     {
// //         state.Add(v.x);
// //         state.Add(v.y);
// //         state.Add(v.z);
// //     }

// //     void CollectRotationState(List<float> state, Quaternion q)
// //     {
// //         state.Add(q.x);
// //         state.Add(q.y);
// //         state.Add(q.z);
// //     }

// //     //we can only collect floats in CollecState so we need to convert bools to floats
// //     float ConvertBoolToFloat(bool b)
// //     {
// //         float f = b == true? 1 : 0;
// //         return f;
// //     }

// //     void CollectPlayerState(PlayerState ps)
// //     {
// //         //26 total per player
// //         ps.state.Clear(); //instead of creating a new list each tick we will reuse this one
// //         ps.state.Add(ps.playerID); //whoami 
// //         ps.state.Add(ps.teamFloat); //which team 
// //         CollectVector3State(ps.state, ps.agentRB.velocity); //agent's vel
// //         CollectRotationState(ps.state, ps.agentRB.rotation); //agent's rotation
// //         CollectVector3State(ps.state, ps.targetGoal.transform.position - ps.agentRB.position); //dir from player to target goal
// //         CollectVector3State(ps.state, ps.defendGoal.transform.position - ps.agentRB.position); //dir from player to defend goal
// //         CollectVector3State(ps.state, ps.targetGoal.transform.position - ballRB.position); //dir from ball to target goal
// //         CollectVector3State(ps.state, ps.defendGoal.transform.position - ballRB.position); //dir from ball to defend goal
// //         CollectVector3State(ps.state, ballRB.position - ps.agentRB.position); //dir from agent to ball
// //         CollectVector3State(ps.state, ballRB.velocity);
// //         if(ps.teamFloat == 1) //blue team
// //         {
// //             bluePlayers.Add(ps);
// //         }
// //         else if (ps.teamFloat == 0) //redteam
// //         {
// //             redPlayers.Add(ps);
// //         }
// //     }


// //     void FixedUpdate()
// //     {
// //         // print("clear player lists");
// //         bluePlayers.Clear();
// //         redPlayers.Clear();
// //         if(playerStates.Count > 0)
// //         {
// //             //UPDATE ALL PLAYERS FIRST
// //             foreach(PlayerState ps in playerStates)
// //             {
// //                 CollectPlayerState(ps);
// //             }
            

// //             // //SEPARATE BY TEAM
// //             // foreach(PlayerState ps in playerStates)
// //             // {
// //             //     if(ps.teamFloat == 1) //blue team
// //             //     {
// //             //         bluePlayers.Add(ps);
// //             //     }
// //             //     else if (ps.teamFloat == 0)
// //             //     {
// //             //         redPlayers.Add(ps);
// //             //     }
// //             // }
// //         }


// //     }



// // }
