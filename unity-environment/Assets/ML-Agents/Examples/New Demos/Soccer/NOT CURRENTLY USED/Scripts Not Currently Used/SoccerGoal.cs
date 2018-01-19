// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// public class SoccerGoal : MonoBehaviour
// {
    
// 	// [HideInInspector]
//     // public SoccerAgent opponentAgent;
//     // public Agent myAgentPVP1;
//     // public GameObject myGoal;
//     Rigidbody rb;
//     public Rigidbody ballRB;
//     public bool rotateTowardsBall;
//     public float rotateTowardsBallSpeed = 100f;
//     // public GameObject goalTextUI;
//     public Team teamGoal; //which team does this goal belong to? if it's the red teams goal mark this red
//     public SoccerFieldArea area;
//     // Use this for initialization
//     void Start()
//     {
//         if(teamGoal == Team.red)
//         {
//             area.redGoal = this;
//         }
//         else if(teamGoal == Team.blue)
//         {
//             area.blueGoal = this;
//         }
        
//         rb = GetComponent<Rigidbody>();
// 		// ballRB = myAgent.area.ball.GetComponent<Rigidbody>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(rotateTowardsBall)
//         {
//             RotateTowardsBall(ballRB.transform.position);
//         }
        
//     }

//     void RotateTowardsBall(Vector3 pos)
// 	{
// 		Vector3 dirToBox = pos - transform.position; //get dir
// 		Quaternion targetRotation = Quaternion.LookRotation(dirToBox); //get our needed rotation
// 		rb.MoveRotation(Quaternion.Lerp(rb.transform.rotation, targetRotation, Time.deltaTime * rotateTowardsBallSpeed)); 
// 	}



//     private void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject == ballRB.gameObject)
//         {

//             if(teamGoal == Team.red)
//             {
//                 area.BlueScores();
//                 print("blue scored");

//             }
//             else if(teamGoal == Team.blue)
//             {
//                 area.RedScores();
//                 print("red scored");
//             }

//             // // Agent agent = myAgent.GetComponent<Agent>();
//             // opponentAgent.done = true;
//             // opponentAgent.reward += 1f;
//             // // myAgentPVP1.reward -= 1f;


//         }
// 	}

// }