using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

public class Curriculum
{

    public string measure = "progress";
    public List<float> thresholds = new List<float>();
    //public List<string> threshold_measures;
    //public List<string> threshold_brains; 
    public int min_lesson_length = 0;
    public bool signal_smoothing = false;
    public List<ResetParameters> lessons = new List<ResetParameters>();
}

public class TrainerConfig
{
    public int trainer_type;
    //PPO only
    public int batch_size = 256;
    public float beta = 2.5e-3f;
    public int buffer_size = 5000;
    public float epsilon = 0.2f;
    public float gamma = 0.99f;
    public int hidden_units = 128;
    public float lambd = 0.95f;
    public float learning_rate = 3.0e-4f;
    public int max_steps = 1000000;
    public bool normalize = false;
    public int num_epoch = 5;
    public int num_layers = 2;
    public int time_horizon = 64;
    public int sequence_length = 32;
    public int summary_freq = 1000;
    public bool use_recurrent = false;
    //Imitation only
    public string brain_to_imitate;
    public int batches_per_epoch = 25;
    //Ghost only
    public string brain_to_copy;
    public int new_model_freq = 10000;
    public int max_num_models = 20;


    public void ChooseTrainer(string[] brainNames)
    {
        trainer_type = GUILayout.Toolbar(trainer_type, new string[] { "PPO", "Imitation", "Ghost" });
        switch (trainer_type)
        {
            case 0:
                UsePPO();
                break;
            case 1:
                UseImitation(brainNames);
                break;
            case 2:
                UseGhost(brainNames);
                break;
            default:
                UsePPO();
                break;
        }
    }

    void UsePPO()
    {
        batch_size = EditorGUILayout.IntField("Batch Size", batch_size);
        beta = EditorGUILayout.FloatField("Beta", beta);
        buffer_size = EditorGUILayout.IntField("Buffer Size", buffer_size);
        epsilon = EditorGUILayout.FloatField("Epsilon", epsilon);
        gamma = EditorGUILayout.FloatField("Gamma", gamma);
        hidden_units = EditorGUILayout.IntField("Hidden Units", hidden_units);
        lambd = EditorGUILayout.FloatField("Lambda", lambd);
        learning_rate = EditorGUILayout.FloatField("Learning Rate", learning_rate);
        max_steps = EditorGUILayout.IntField("Max Steps", max_steps);
        normalize = EditorGUILayout.Toggle("Normalize", normalize);
        num_epoch = EditorGUILayout.IntField("Number Of Epochs", num_epoch);
        num_layers = EditorGUILayout.IntField("Number Of Layers", num_layers);
        time_horizon = EditorGUILayout.IntField("Time Horizon", time_horizon);
        summary_freq = EditorGUILayout.IntField("Summary Frequency", summary_freq);
        use_recurrent = EditorGUILayout.Toggle("Use Recurrent", use_recurrent);
        if (use_recurrent)
        {
            sequence_length = EditorGUILayout.IntField("Sequence Length", sequence_length);
        }

    }
    void UseGhost(string[] brainNames)
    {
        //TODO : Make a dropdown using brainNames
        brain_to_copy = EditorGUILayout.TextField("Brain to Copy", brain_to_copy);
        new_model_freq = EditorGUILayout.IntField("New Model Frequency", new_model_freq);
        max_num_models = EditorGUILayout.IntField("Max Number Of Models", max_num_models);
    }
    void UseImitation(string[] brainNames)
    {
        batch_size = EditorGUILayout.IntField("Batch Size", batch_size);
        brain_to_imitate = EditorGUILayout.TextField("Brain to Copy", brain_to_imitate);
        time_horizon = EditorGUILayout.IntField("Time Horizon", time_horizon);
        summary_freq = EditorGUILayout.IntField("Summary Frequency", summary_freq);
        max_steps = EditorGUILayout.IntField("Max Steps", max_steps);
        batches_per_epoch = EditorGUILayout.IntField("Batches Per Epoch", batches_per_epoch);
    }

}


public class MLAgents : EditorWindow
{
    int tab;
    string currentSceneName;
    string currentDirectoryPath;
    string rootAssetDirectory = "Assets/MyEnvironments";
    string rootBinaryDirectory = "../built-environments";

    Vector2 scrollPosition = Vector2.zero;
    // Generate Scene Fields
    string _sceneName;
    int numberOfBrains;
    //string path;
    bool attachComponents;

    // Curriculum editing Fields
    string curriculumName;
    Curriculum curr;
    int selectedMeasureIndex;
    string[] measureOptions = new string[] { "progress", "reward" }; //TODO : Make one for each lesson
    int numberOfLessons;
    Academy _aca;

    //TrainerEditing
    List<string> brainNames = new List<string>();
    int selectedBrainIndex;
    Dictionary<string, TrainerConfig> trainerConfigurations;


    [MenuItem("Window/ML-Agents")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MLAgents));
    }

    void OnInspectorUpdate()
    {
        // Call Repaint on OnInspectorUpdate as it repaints the windows
        // less times as if it was OnGUI/Update
        Repaint();
    }

    void OnGUI()
    {

        tab = GUILayout.Toolbar(tab, new string[] { "New Scene", "Curriculum", "Trainers", "Build/Run" });
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		currentSceneName = EditorSceneManager.GetActiveScene().name;
        currentDirectoryPath= rootAssetDirectory+"/"+currentSceneName;
        switch (tab)
        {
            case 0:
                EditorGUILayout.LabelField("  Generate New Scene", EditorStyles.largeLabel);
                GenerateSceneInterface();
                AttachAcademyAndAgent();
                break;
            case 1:
                EditCurriculum();
                break;
            case 2:
                EditTrainers();
                break;
            case 3:
                BuildEnvironment();
                break;
            default:
                break;

        }
        GUILayout.EndScrollView();

    }

    void GenerateSceneInterface()
    {
        _sceneName = EditorGUILayout.TextField("Name of the scene", _sceneName);
        numberOfBrains = EditorGUILayout.IntSlider("Number of Brains", numberOfBrains, 1, 10);

        //string path = "Assets/ML-Agents/" + sceneName;

        if (!attachComponents)
        {
            EditorGUI.BeginDisabledGroup(!isValidSceneName());
            if (GUILayout.Button("Build Scene"))
            {
                BuildScene();
            }
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Button("Please wait...");
            EditorGUI.EndDisabledGroup();
        }


    }

    bool isValidSceneName()
    {
        if (Directory.Exists(rootAssetDirectory +"/"+_sceneName) == true)
        {
            return false;
        }
        foreach (char c in _sceneName)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    void BuildScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);


        Directory.CreateDirectory(rootAssetDirectory + "/" + _sceneName);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), rootAssetDirectory + "/" + _sceneName + "/" + _sceneName + ".unity");

        string scriptPath = rootAssetDirectory + "/" + _sceneName + "/Scripts";
        Directory.CreateDirectory(scriptPath);
        CreateAcademyScript(scriptPath, _sceneName + "Academy");
        CreateAgentScript(scriptPath, _sceneName + "Agent");
        attachComponents = true;
        AssetDatabase.Refresh();


    }
    void AttachAcademyAndAgent()
    {
        string acaType = typeof(Academy).AssemblyQualifiedName;
        string agentType = typeof(Agent).AssemblyQualifiedName;
        if (attachComponents && (Type.GetType(_sceneName + acaType) != null)
               && (Type.GetType(_sceneName + agentType) != null))
        {
            GameObject acaGameObject = new GameObject(_sceneName + "Academy");
            Brain _b = null;
            if (numberOfBrains > 1)
            {
                for (int i = 0; i < numberOfBrains; i++)
                {
                    GameObject brain = new GameObject(_sceneName + "Brain" + i.ToString());
                    brain.AddComponent<Brain>();
                    brain.transform.parent = acaGameObject.transform;
                    if (i == 0)
                    {
                        _b = brain.GetComponent<Brain>();
                    }
                }
            }
            else
            {
                GameObject brain = new GameObject(_sceneName + "Brain");
                brain.AddComponent<Brain>();
                brain.transform.parent = acaGameObject.transform;
                _b = brain.GetComponent<Brain>();
            }




            acaGameObject.AddComponent(Type.GetType(_sceneName + acaType));
            attachComponents = false;
            GameObject agent = new GameObject(_sceneName + "Agent");
            agent.AddComponent(Type.GetType(_sceneName + agentType));
            Agent _a = agent.GetComponent<Agent>();
            _a.GiveBrain(_b);


        }


    }




    void CreateAcademyScript(string copyPath, string scriptName)
    {

        using (StreamWriter outfile =
               new StreamWriter(copyPath + "/" + scriptName + ".cs"))
        {
            outfile.WriteLine("using System.Collections;");
            outfile.WriteLine("using System.Collections.Generic;");
            outfile.WriteLine("using UnityEngine;");
            outfile.WriteLine("");
            outfile.WriteLine("public class " + scriptName + " : Academy \n{");
            outfile.WriteLine("\tpublic override void InitializeAcademy()\n\t{\n\n\t}\n");
            outfile.WriteLine("\tpublic override void AcademyReset()\n\t{\n\n\t}\n");
            outfile.WriteLine("\tpublic override void AcademyStep()\n\t{\n\n\t}\n");
            outfile.WriteLine("}");

        }
    }

    void CreateAgentScript(string copyPath, string scriptName)
    {
        using (StreamWriter outfile =
               new StreamWriter(copyPath + "/" + scriptName + ".cs"))
        {
            outfile.WriteLine("using System.Collections;");
            outfile.WriteLine("using System.Collections.Generic;");
            outfile.WriteLine("using UnityEngine;");
            outfile.WriteLine("");
            outfile.WriteLine("public class " + scriptName + " : Agent {");
            outfile.WriteLine("\tpublic override void InitializeAgent()\n\t{\n\n\t}");
            outfile.WriteLine("\tpublic override List<float> CollectState()\n\t{\n\n\t\treturn state;\n\t}");
            outfile.WriteLine("\tpublic override void AgentStep(float[] act)\n\t{\n\n\t}");
            outfile.WriteLine("\tpublic override void AgentReset()\n\t{\n\n\t}");
            outfile.WriteLine("\tpublic override void AgentOnDone()\n\t{\n\n\t}");
            outfile.WriteLine("}");

        }
    }

    public void EditCurriculum()
    {

        Academy[] _acas = FindObjectsOfType<Academy>() as Academy[];
        if (_acas.Count() < 1)
        {
            EditorGUILayout.HelpBox("There is no Academy Component in the current scene.", MessageType.Error);
            return;
        }
        _aca = _acas[0];

        if (curriculumName != currentSceneName+"Curriculum")
        {
            curr = null;
        }
        curriculumName = currentSceneName + "Curriculum";
        string curriculumPath = rootBinaryDirectory+"/" + currentSceneName+ "/" + curriculumName + ".json";

        List<string> parameterKeys = new List<string>();
        if (_aca.resetParameters != null)
        {
            parameterKeys = _aca.resetParameters.Keys.ToList();
        }

        if (curr == null)
        {
            // TODO : Decide if json or scriptable object, Could make it to a struct --> JSON
            //curr = AssetDatabase.LoadAssetAtPath(curriculumPath, typeof(Curriculum)) as Curriculum;
            try
            {
                using (StreamReader infile =
                       new StreamReader(curriculumPath))
                {
                    curr = JsonConvert.DeserializeObject<Curriculum>(infile.ReadToEnd());
                }
            }
            catch
            {
                curr = null;
            }

            if (curr == null)
            {
                //curr = ScriptableObject.CreateInstance<Curriculum>();
                curr = new Curriculum();

                //curr.measure = "progress";
                //curr.min_lesson_length = 0;
                //curr.signal_smoothing = false;
                //curr.lessons = new List<ResetParameters>();

                //curr.thresholds = new List<float>();
                ////curr.brain_threshold = new List<string>();

                //curr.lessons = new List<ResetParameters>();
            }
            selectedMeasureIndex = ArrayUtility.IndexOf(measureOptions, curr.measure);
            selectedMeasureIndex = Math.Max(0, selectedMeasureIndex);
            numberOfLessons = curr.thresholds.Count() + 1;
            //for (int i = 0; i < numberOfLessons - 1; i++){
            //    curr.brain_threshold.Add("");
            //}

        }
        try
        {
            numberOfLessons = EditorGUILayout.IntField("Number of Lessons", numberOfLessons);
            selectedMeasureIndex = EditorGUILayout.Popup("Measure Type", selectedMeasureIndex, measureOptions);
            curr.measure = measureOptions[selectedMeasureIndex];
            curr.min_lesson_length = EditorGUILayout.IntField("Minimum Lesson Length", curr.min_lesson_length);
            curr.signal_smoothing = EditorGUILayout.Toggle("Signal Smoothing", curr.signal_smoothing);


            if (curr.lessons.Count() > numberOfLessons)
            {
                curr.lessons = curr.lessons.GetRange(0, numberOfLessons);
            }
            if (curr.lessons.Count() < numberOfLessons)
            {
                for (int j = 0; j < numberOfLessons - curr.lessons.Count(); j++)
                {
                    curr.lessons.Add(new ResetParameters());
                }
            }

            if (curr.thresholds.Count() > numberOfLessons - 1)
            {
                curr.thresholds = curr.thresholds.GetRange(0, Math.Max(numberOfLessons - 1, 0));
            }
            if (curr.thresholds.Count() < numberOfLessons - 1)
            {
                curr.thresholds.AddRange(new float[numberOfLessons - curr.thresholds.Count() - 1]);
            }

            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            for (int i = 0; i < numberOfLessons; i++)
            {
                EditorGUILayout.LabelField("Lesson " + i.ToString(), EditorStyles.boldLabel);
                if (i != 0)
                {
                    //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    curr.thresholds[i - 1] = EditorGUILayout.FloatField("Threshold", curr.thresholds[i - 1]);
                    //selectedMeasureIndex = EditorGUILayout.Popup("Measure Type", selectedMeasureIndex, measureOptions);
                    //curr.measure = measureOptions[selectedMeasureIndex];
                }

                EditorGUILayout.LabelField("Reset Parameters");
                foreach (string _key in parameterKeys)
                {
                    if (!curr.lessons[i].ContainsKey(_key))
                    {
                        curr.lessons[i][_key] = 0f;
                    }

                    curr.lessons[i][_key] = EditorGUILayout.FloatField("\t" + _key, curr.lessons[i][_key]);
                }

                foreach (string _key in curr.lessons[i].Keys.ToList())
                {
                    if (!parameterKeys.Contains(_key))
                    {
                        curr.lessons[i].Remove(_key);
                    }
                }

            }

            // TODO : gray button if jsons are equal
            if (GUILayout.Button("Save"))
            {
				if (!Directory.Exists(rootBinaryDirectory + "/" + currentSceneName))
				{
					Directory.CreateDirectory(rootBinaryDirectory + "/" + currentSceneName);
				}
                string curr_json = JsonConvert.SerializeObject(curr, Formatting.Indented);
                using (StreamWriter outfile =
                       new StreamWriter(curriculumPath))
                {
                    outfile.WriteLine(curr_json);
                }
            }
            //if (AssetDatabase.LoadAssetAtPath(curriculumPath, typeof(Curriculum)) == null)
            //{
            //    if (GUILayout.Button("Build Curriculum"))
            //    {
            //        AssetDatabase.CreateAsset(curr, "Assets/" + curriculumName + ".asset");
            //    }
            //}
            //else
            //{
            //    if (GUILayout.Button("Save Curriculum"))
            //    {
            //        AssetDatabase.Refresh();
            //        EditorUtility.SetDirty(curr);
            //        AssetDatabase.SaveAssets();
            //    }
            //}
        }
        catch
        {
            return;
        }

    }

    void EditTrainers()
    {
        Brain[] brains = FindObjectsOfType<Brain>() as Brain[];
        if (brains.Count() < 1)
        {
            EditorGUILayout.HelpBox("There is no Brain Components in the current scene.", MessageType.Error);
            return;
        }

        //TODO : Make sure that the brains have an Academy parent
        brainNames.Clear();
        foreach (Brain _b in brains)
        {
            brainNames.Add(_b.gameObject.name);
            if (_b.gameObject.transform.parent == null)
            {
                EditorGUILayout.HelpBox("All Brains must have an Academy GameObject as Parent.", MessageType.Error);
                return;
            }
            else if (_b.gameObject.transform.parent.GetComponent<Academy>() == null)
            {
                EditorGUILayout.HelpBox("All Brains must have an Academy GameObject as Parent.", MessageType.Error);
                return;
            }
        }

        selectedBrainIndex = EditorGUILayout.Popup("Brain", selectedBrainIndex, brainNames.ToArray());
        string currentBrain = brainNames[selectedBrainIndex];
        if (brains[selectedBrainIndex].brainType != BrainType.External)
        {
            EditorGUILayout.HelpBox("This brain is not set to External.", MessageType.Warning);
        }

        string trainerConfigurationPath = rootBinaryDirectory + "/" + currentSceneName+ "/" + currentSceneName + "Trainers.json";

        if (trainerConfigurations == null)
        {
            try
            {
                using (StreamReader infile =
                       new StreamReader(trainerConfigurationPath))
                {
                    trainerConfigurations = JsonConvert.DeserializeObject<Dictionary<string, TrainerConfig>>(infile.ReadToEnd());
                }
            }
            catch
            {
                trainerConfigurations = new Dictionary<string, TrainerConfig>();
            }

        }

        if (!trainerConfigurations.ContainsKey(currentBrain))
        {
            trainerConfigurations[currentBrain] = new TrainerConfig();
        }
        trainerConfigurations[currentBrain].ChooseTrainer(brainNames.ToArray());
        if (GUILayout.Button("Save"))
        {
            if (!Directory.Exists(rootBinaryDirectory + "/" + currentSceneName)){
                Directory.CreateDirectory(rootBinaryDirectory + "/" + currentSceneName);
            }
            string trainer_json = JsonConvert.SerializeObject(trainerConfigurations, Formatting.Indented);
            using (StreamWriter outfile =
                   new StreamWriter(trainerConfigurationPath))
            {
                outfile.WriteLine(trainer_json);
            }
        }


    }

    void BuildEnvironment()
    {
        EditorGUILayout.HelpBox("To implement.", MessageType.Error);
        if (GUILayout.Button("Build"))
        {
            BuildGame();
        }
    }

    public void BuildGame()
    {
		// Get filename.
		//string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
		if (!Directory.Exists(rootBinaryDirectory + "/" + currentSceneName))
		{
			Directory.CreateDirectory(rootBinaryDirectory + "/" + currentSceneName);
		}
        string path = rootBinaryDirectory + "/" + currentSceneName; 
        string[] levels = new string[] { currentDirectoryPath+ "/"+currentSceneName +".unity" };

        UnityEngine.Debug.Log(levels[0]);
        UnityEngine.Debug.Log(path + "/" + currentSceneName + ".app");

		// Build player.
#if UNITY_EDITOR_WIN
        BuildPipeline.BuildPlayer(levels, path + "/"+currentSceneName+".exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
#elif UNITY_EDITOR_OSX
		BuildPipeline.BuildPlayer(levels, path + "/"+currentSceneName +".app", BuildTarget.StandaloneOSXIntel64, BuildOptions.Development);
#else
        BuildPipeline.BuildPlayer(levels, path + "/"+currentSceneName+".x86_64", BuildTarget.StandaloneLinux64, BuildOptions.Development);
#endif
		// Copy a file from the project folder to the build folder, alongside the built game.
		//FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

		//      // Run the game (Process class from System.Diagnostics).
		//      Process proc = new Process();
		//proc.StartInfo.FileName = path + "/BuiltGame.exe";
		//proc.Start();
		UnityEngine.Debug.Log("Build!");
	}


}
