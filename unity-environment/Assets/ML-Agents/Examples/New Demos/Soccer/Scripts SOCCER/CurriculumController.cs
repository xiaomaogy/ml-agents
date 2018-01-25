using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurriculumController : MonoBehaviour {


	SoccerAcademy academy;
	[HideInInspector]
	public ReadRewardData readRewardData;
	public Dictionary<string, Brain> brainsDict = new Dictionary<string, Brain>();
	public SoccerFieldArea[] fields;
	public bool lesson1;
	public bool lesson2;
	public bool lesson3;
	public bool lesson4;
	public int currentLesson;

	// Use this for initialization
	void Start () {
		// lesson1 = true;
		academy = FindObjectOfType<SoccerAcademy>(); //get the academy
		currentLesson = 1;
		academy.currentLesson = 1;
		fields = FindObjectsOfType<SoccerFieldArea>();
		readRewardData = FindObjectOfType<ReadRewardData>(); //get reward data script
		// Brain[] brains = academy.GetComponentsInChildren<Brain>(true);

		SetActiveBlueGoalies(false);
		SetActiveRedGoalies(false);
		SetActiveRedStrikers(false);
		SetActiveBlueStrikers(false);

		foreach(Brain brain in academy.GetComponentsInChildren<Brain>(true))//get inactive as well
		{
			brainsDict.Add(brain.gameObject.name, brain);
		}
	}
	

	// IEnumerator TrainUntilThresholdReached(string brainName, float threshold, bool currentLesson, bool nextLesson)
	// IEnumerator TrainUntilThresholdReached(string brainName, float threshold, int lesson, bool lessonBool)
	IEnumerator TrainUntilThresholdReached(string brainName, float threshold, int lesson)
	// IEnumerator TrainUntilThresholdReached(string brainName, float threshold)
	{
		if(brainsDict.ContainsKey(brainName) && readRewardData.rewardDataDict.ContainsKey(brainName))
		{
			print(brainName); 
			print(threshold); 
			print(lesson); 
			while(readRewardData.rewardDataDict[brainName].currentMeanReward < threshold && currentLesson == lesson)
			{
				yield return new WaitForSeconds(5);
			}
			// lessonBool = false;
			GoToNextLesson();
			// nextLesson = true;
		}
		else
		{
			Debug.LogWarning(lesson + " failed because there's no brain found");
		}
	}
	// IEnumerator TrainForSeconds(string brainName, float seconds, bool currentLesson, bool nextLesson)
	IEnumerator TrainForSeconds(string brainName, float seconds, int lesson)
	// IEnumerator TrainForSeconds(string brainName, float seconds)
	{
		if(brainsDict.ContainsKey(brainName) && readRewardData.rewardDataDict.ContainsKey(brainName) && currentLesson == lesson)
		{
			yield return new WaitForSeconds(seconds);
			// lessonBool = false;
			GoToNextLesson();
			// nextLesson = true;
		}
		else
		{
			Debug.LogWarning(lesson + " failed because there's no brain found");
		}
	}

	void AllLessonsFalse()
	{
		lesson1 = false;
		lesson2 = false;
		lesson3 = false;
		// lesson4 = false;
	}

	void GoToLesson1()
	{
		print("starting lesson1");
		// AllLessonsFalse();
		lesson1 = true;
		currentLesson = 1;
	}

	void GoToLesson2()
	{
		print("starting lesson2");
		// AllLessonsFalse();
		lesson2 = true;
		currentLesson = 2;
	}

	void GoToLesson3()
	{
		print("starting lesson3");
		// AllLessonsFalse();
		lesson3 = true;
		currentLesson = 3;
	}

	void GoToNextLesson()
	{
		AllLessonsFalse();
		if(currentLesson < 3)
		{
			currentLesson += 1;
		}
		else
		{
			currentLesson = 0;
		}
		// currentLesson += Mathf.Clamp(1, 1, 3);
	}

	// void GoToPreviousLesson(bool disableLesson)
	// {
	// 	currentLesson -= Mathf.Clamp(1, 1, 3);
	// 	// currentLesson -= 1;
	// }

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			AllLessonsFalse();
			currentLesson = 1;

			// GoToLesson1();
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			AllLessonsFalse();
			currentLesson = 2;
			// GoToLesson2();
		}
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			AllLessonsFalse();
			currentLesson = 3;
			// GoToLesson3();
		}

		//lesson 1 train the blue strikers
		if(currentLesson == 1 && !lesson1)
		// if(academy.currentLesson == 1 && !lesson1)
		{
			AllLessonsFalse();
			lesson1 = true;
			SetActiveBlueStrikers(true);
			SetActiveRedStrikers(false);
			// academy.AcademyReset();
			// GoToLesson1();
			// StartCoroutine(TrainUntilThresholdReached(academy.brainStriker.name, .8f, currentLesson, lesson1));
			// StartCoroutine(TrainUntilThresholdReached(academy.brainStriker.name, .8f, 1));
		}

		//lesson 2 train the red strikers
		// if(academy.currentLesson == 2 && !lesson2)
		if(currentLesson == 2 && !lesson2)
		{
			AllLessonsFalse();
			lesson2 = true;

			SetActiveRedStrikers(true);
			SetActiveBlueStrikers(false);
			// academy.AcademyReset();
			// GoToLesson2();
			// StartCoroutine(TrainUntilThresholdReached(academy.brainStriker.name, .8f, 2));
		}

		//lesson 3 train the red & blue strikers together
		// if(academy.currentLesson == 3 && !lesson3)
		if(currentLesson == 3 && !lesson3)
		{
			AllLessonsFalse();
			lesson3 = true;
			SetActiveBlueStrikers(true);
			SetActiveRedStrikers(true);
			// academy.AcademyReset();
			// GoToLesson3();
			// StartCoroutine(TrainForSeconds(academy.brainStriker.name, 500, 3));
		}



		//lesson 3 train the red goalie
		//lesson 4 train the blue goalie



		// //lesson 2 activate the goalies so they can train
		// if(!lesson2 && brainsDict.ContainsKey(academy.brainStriker.name) && readRewardData.rewardDataDict.ContainsKey(academy.brainStriker.name) && readRewardData.rewardDataDict[academy.brainStriker.name].currentMeanReward > -.6)
		// {
		// 	lesson1 = false;
		// 	lesson2 = true;
		// 	SetActiveBlueGoalies(true);

		// 	// print("reward > .8");
		// }
		
	}

	void SetActiveBlueGoalies(bool active)
	{
		foreach(SoccerFieldArea field in fields)
		{
			field.blueGoalie.gameObject.SetActive(active);
		}
	}
	void SetActiveRedGoalies(bool active)
	{
		foreach(SoccerFieldArea field in fields)
		{
			field.redGoalie.gameObject.SetActive(active);
		}
	}
	void SetActiveRedStrikers(bool active)
	{
		print("SetActiveRedStriker");
		foreach(SoccerFieldArea field in fields)
		{
			field.redStriker.gameObject.SetActive(active);
		}
	}
	void SetActiveBlueStrikers(bool active)
	{
		print("SetActiveBlueStriker");
		foreach(SoccerFieldArea field in fields)
		{
			field.blueStriker.gameObject.SetActive(active);
		}
	}
}
