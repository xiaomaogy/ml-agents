using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {

	// public class Platform
	// {}
	public bool enabled = true;
	// GameObject platformObj;
	Rigidbody rb;
	Vector3 rbStartPos;
	// [InspectorMarginAttribute(20)]
	public bool rotateRB;
	// public Rigidbody rotateRBObject;
	public Vector3 rotationSpeed;
	public AnimationCurve rotationSpeedMultiplierX;
	public AnimationCurve rotationSpeedMultiplierY;
	public AnimationCurve rotationSpeedMultiplierZ;

	// public float rotationSmoothing;
	public bool adjustMaterialFrictionBasedOnRotationSpeed;
	public PhysicMaterial platformPhysicsMaterial;




	// [InspectorMarginAttribute(20)]
	public bool oscillate;
	// public float oscillateMaxDistDelta;
	public float oscillateForce;
	public bool oscillateX;
	public AnimationCurve oscillateXCurve;
	public bool oscillateY;
	public AnimationCurve oscillateYCurve;
	public bool oscillateZ;
	public AnimationCurve oscillateZCurve;


	// [InspectorMarginAttribute(20)]
	public bool matchRotation;
	public bool matchXRot;
	public bool matchYRot;
	public bool matchZRot;
	public float maxDegreesDelta;
	public float matchRotAngVel;
	public Transform matchRotationTargetTransform;


	// [InspectorMarginAttribute(20)]
	public bool matchPosition;
	public Transform matchPositionTargetTransform;
	public float matchPosForce;
	// public float matchPosMaxDistDelta;



	public float targetVel;
	public float maxVel;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();
		rbStartPos =  rb.position;
		if(adjustMaterialFrictionBasedOnRotationSpeed)
		{
			platformPhysicsMaterial = GetComponent<Collider>().material;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(enabled)
		{
			if(adjustMaterialFrictionBasedOnRotationSpeed && rotateRB)
			{
				var rotationTarget = rb.rotation * Quaternion.Euler(rotationSpeed.x * rotationSpeedMultiplierX.Evaluate(Time.time), rotationSpeed.y * rotationSpeedMultiplierY.Evaluate(Time.time),rotationSpeed.z * rotationSpeedMultiplierZ.Evaluate(Time.time));

				// platformPhysicsMaterial.dynamicFriction = rotationSpeed.y/100;
				// platformPhysicsMaterial.staticFriction = rotationSpeed.y/100;
				platformPhysicsMaterial.dynamicFriction = Mathf.Abs(rotationSpeed.y * rotationSpeedMultiplierY.Evaluate(Time.time))/100;
				platformPhysicsMaterial.staticFriction = Mathf.Abs(rotationSpeed.y * rotationSpeedMultiplierY.Evaluate(Time.time))/100;
			}


			if(rotateRB)
			{
				RotateRigidbody();
			}
			if(matchRotation)
			{
				MatchObjectRotation();
			}

			if(matchPosition)
			{
				if(oscillate)
				{
					MatchObjectPositionAndOscillate();
				}
				else
				{
					MatchObjectPosition();
				}
			}
			if(oscillate && !matchPosition)
			{
				OscillateRigidbody();
			}
		}
	}

	public void RotateRigidbody()
	{
		// var rotationTarget = rb.rotation * Quaternion.Euler(rotationSpeed);
		var rotationTarget = rb.rotation * Quaternion.Euler(rotationSpeed.x * rotationSpeedMultiplierX.Evaluate(Time.time), rotationSpeed.y * rotationSpeedMultiplierY.Evaluate(Time.time),rotationSpeed.z * rotationSpeedMultiplierZ.Evaluate(Time.time));
		rb.MoveRotation(Quaternion.Lerp(rb.transform.rotation, rotationTarget, Time.deltaTime)); //do it bro
		// rb.MoveRotation(Quaternion.Lerp(rb.transform.rotation, rotationTarget, Time.deltaTime * rotationSmoothing)); //do it bro
	}

	public void OscillateRigidbody()
	{
		// print("oscillating rb");
		Vector3 newPos = rbStartPos;

		// if(oscillateX){newPos += Vector3.Scale(transform.right, new Vector3(oscillateXCurve.Evaluate(Time.time), 0, 0));}
		// if(oscillateY){newPos += Vector3.Scale(transform.up, new Vector3(0, oscillateYCurve.Evaluate(Time.time), 0));}
		// if(oscillateZ){newPos += Vector3.Scale(transform.forward, new Vector3(0, 0, oscillateZCurve.Evaluate(Time.time)));}
		if(oscillateX){newPos += transform.right * oscillateXCurve.Evaluate(Time.time);}
		if(oscillateY){newPos += transform.up * oscillateYCurve.Evaluate(Time.time);}
		if(oscillateZ){newPos += transform.forward * oscillateZCurve.Evaluate(Time.time);}
		// if(oscillateX){newPos += new Vector3(oscillateXCurve.Evaluate(Time.time), 0, 0);}
		// if(oscillateY){newPos += new Vector3(0, oscillateYCurve.Evaluate(Time.time), 0);}
		// if(oscillateZ){newPos += new Vector3(0, 0, oscillateZCurve.Evaluate(Time.time));}

		Vector3 positionDelta = newPos - rb.position;  //dir to object

				// Vector3 moveToPos = targetPos - rb.worldCenterOfMass;  //cube needs to go to the standard Pos
		Vector3 velocityTarget = positionDelta * targetVel * Time.deltaTime; //not sure of the logic here, but it modifies velTarget
		// if (float.IsNaN(velocityTarget.x) == false) //sanity check. if the velocity is NaN that means it's going way too fast. this check isn't needed for slow moving objs
		// {
			rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
		// }
		// rb.MovePosition(rb.position + positionDelta * oscillateForce * Time.deltaTime);
	}

	// public void OscillateRigidbody()
	// {
	// 	Vector3 newPos = rbStartPos;

	// 	// if(oscillateX){newPos += Vector3.Scale(transform.right, new Vector3(oscillateXCurve.Evaluate(Time.time), 0, 0));}
	// 	// if(oscillateY){newPos += Vector3.Scale(transform.up, new Vector3(0, oscillateYCurve.Evaluate(Time.time), 0));}
	// 	// if(oscillateZ){newPos += Vector3.Scale(transform.forward, new Vector3(0, 0, oscillateZCurve.Evaluate(Time.time)));}
	// 	if(oscillateX){newPos += transform.right * oscillateXCurve.Evaluate(Time.time);}
	// 	if(oscillateY){newPos += transform.up * oscillateYCurve.Evaluate(Time.time);}
	// 	if(oscillateZ){newPos += transform.forward * oscillateZCurve.Evaluate(Time.time);}
	// 	// if(oscillateX){newPos += new Vector3(oscillateXCurve.Evaluate(Time.time), 0, 0);}
	// 	// if(oscillateY){newPos += new Vector3(0, oscillateYCurve.Evaluate(Time.time), 0);}
	// 	// if(oscillateZ){newPos += new Vector3(0, 0, oscillateZCurve.Evaluate(Time.time));}

	// 	Vector3 positionDelta = newPos - rb.position;  //dir to object
	// 	rb.MovePosition(rb.position + positionDelta * oscillateForce * Time.deltaTime);
	// }


	public void MatchObjectRotation()
	{




		// // rotationDelta = rb.rotation * Quaternion.Inverse(force.matchRotTarget.rotation);
		// 	Quaternion rotationDelta;
		// 	rotationDelta = rb.rotation * Quaternion.Inverse(matchRotationTargetTransform.rotation);
		// 	//Converts a rotation to angle-axis representation (angles in degrees).
		// 	float angle;
		// 	Vector3 axis;
		// 	rotationDelta.ToAngleAxis(out angle, out axis);

		// 	if (angle > 180)
		// 		angle -= 360;

		// 	if (angle != 0)
		// 	{
		// 		Vector3 angularTarget = angle * axis;
		// 		if (float.IsNaN(angularTarget.x) == false)
		// 		{
		// 	// print(angle);
		// 			angularTarget = (angularTarget * matchRotAngVel) * Time.deltaTime;
		// 			rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, maxDegreesDelta);
		// 			// this.Rigidbody.angularVelocity = Vector3.MoveTowards(this.Rigidbody.angularVelocity, angularTarget, MaxAngularVelocityChange);
		// 		}
		// 	}



			// float step = maxDegreesDelta * Time.deltaTime;
			Quaternion newRotation = Quaternion.Euler(0,0,0);
			if(matchXRot){newRotation *= Quaternion.Euler(matchRotationTargetTransform.rotation.eulerAngles.x, 0, 0);}
			if(matchYRot){newRotation *= Quaternion.Euler(0, matchRotationTargetTransform.rotation.eulerAngles.y, 0);}
			if(matchZRot){newRotation *= Quaternion.Euler(0, 0, matchRotationTargetTransform.rotation.eulerAngles.z);}
			// rb.rotation = Quaternion.RotateTowards(rb.rotation, matchRotationTargetTransform.rotation, step);
			rb.rotation = Quaternion.RotateTowards(rb.rotation, newRotation, maxDegreesDelta);
			// rb.rotation = Quaternion.RotateTowards(rb.rotation, newRotation, step);
	}

	public void MatchObjectPosition()
	{
		Vector3 matchObjPos = matchPositionTargetTransform.position - rb.position;  //dir to target object
		rb.MovePosition(rb.position + matchObjPos * matchPosForce * Time.deltaTime); //move it to the new pos
	}
	public void MatchObjectPositionAndOscillate()
	{

		Vector3 matchObjPos = matchPositionTargetTransform.position - rb.position;  //dir to target object
		rb.MovePosition(rb.position + matchObjPos * matchPosForce * Time.deltaTime); //move it to the new pos
		
		Vector3 addOscPos = matchObjPos;
		//if want to oscillate we need to adjust new pos
		if(oscillate)
		{
			if(oscillateX){addOscPos += new Vector3(oscillateXCurve.Evaluate(Time.time), 0, 0);}
			if(oscillateY){addOscPos += new Vector3(0, oscillateYCurve.Evaluate(Time.time), 0);}
			if(oscillateZ){addOscPos += new Vector3(0, 0, oscillateZCurve.Evaluate(Time.time));}
		}

		Vector3 oscObjPos = matchPositionTargetTransform.position + addOscPos - rb.position;  //dir to object
		rb.MovePosition(rb.position + oscObjPos * oscillateForce * Time.deltaTime);

	}



}






	// //moves a rigidbody towards a position with a smooth controlled movement.
	// void MoveTowards(Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
	// {
	// 	Vector3 moveToPos = targetPos - rb.worldCenterOfMass;  //cube needs to go to the standard Pos
	// 	Vector3 velocityTarget = moveToPos * targetVel * Time.deltaTime; //not sure of the logic here, but it modifies velTarget
	// 	if (float.IsNaN(velocityTarget.x) == false) //sanity check. if the velocity is NaN that means it's going way too fast. this check isn't needed for slow moving objs
	// 	{
	// 		rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
	// 	}
	// }