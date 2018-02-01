using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpAcademy : Academy {


	public float agentRunSpeed; 
	public float agentRotationSpeed;
	public float spawnAreaMarginMultiplier; //ex: .9 means 90% of spawn area will be used.... .1 margin will be left (so players don't spawn off of the edge). the higher this value, the longer training time required
    public Material goalScoredMaterial; //when a goal is scored the ground will use this material for a few seconds.
    public Material failMaterial; //when fail, the ground will use this material for a few seconds. 

	public float minWallHeight;
    public float maxWallHeight;

	public override void AcademyReset()
	{
        minWallHeight = (float)resetParameters["min_wall_height"];
        maxWallHeight = (float)resetParameters["max_wall_height"];
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
