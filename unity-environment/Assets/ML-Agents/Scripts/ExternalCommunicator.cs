using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using UnityEngine;

using Newtonsoft.Json;

/// <summary>
/// Implements the Communicator interface by using sockets to communicate
/// with the Python API.
/// </summary>
public class ExternalCommunicator : Communicator
{
    /// Size of the message buffer, <see cref="messageBuffer"/>.
    const int MESSAGE_BUFFER_SIZE = 12000;

    /// Size of the length buffer. <see cref="lengthBuffer"/>.
    const int LENGTH_BUFFER_SIZE = 4;

    /// Contains the log filename.
    const string LOG_FILENAME = "unity-environment.log";

    /// API version dictates the format of the messages sent, so any changes
    /// in the message format dictates a new API version.
    const string API_VERSION = "API-2";

    /// Default number of agents - used to initialize data structure capacity.
    const int DEFAULT_NUM_AGENTS = 32;

    /// Default number of observations per agent - used to initialize data
    /// structure capacity.
    const int DEFAULT_NUM_OBSERVATIONS = 32;

    /// The Academy for the learning environment that is communicating with
    /// the Python API.
    Academy academy;

    /// List of brain GameObjects in the environment that are children of the
    /// Academy GameObject.
    List<Brain> brains;

    /// List of Agent (ids) attached to each Brain. Dictionary maps brain name
    /// (as specified in Unity environment GameObject) to list of agent IDs
    /// (an agent ID is a unique identifier each agent GameObject receives
    /// at initialization).
    Dictionary<string, List<int>> brainAgents;

    /// Maps Brain name to whether it has sent its state (information about all
    /// its Agent objects attached to the brain). The value is flipped to True
    /// in <see cref="GiveBrainInfo"/> as that is where all the Brain info is
    /// sent. Size of dictionary corresponds to the number of brains defined
    /// in the Unity environment.
    Dictionary<string, bool> hasSentState;

    /// Maps Brain name to a dictionary which in turn maps each Agent ID to
    /// an array of actions. Size of outer dictionary corresponds to the number
    /// of brains defined in the Unity environment. Size of inner dictionary
    /// corresponds to the number of agents attached to the corresponding brain.
    /// Updated following <see cref="UpdateActions"/>. 
    Dictionary<string, Dictionary<int, float[]>> storedActions;

    /// Maps Brain name to a dictionary which in turn maps each Agent ID to
    /// an array of memories. Size of outer dictionary corresponds to the
    /// number of brains defined in the Unity environment. Size of inner
    /// dictionary corresponds to the number of agents attached to the
    /// corresponding brain. Updated following <see cref="UpdateActions"/>.
    Dictionary<string, Dictionary<int, float[]>> storedMemories;

    /// Maps Brain name to a dictionary which in turn maps each Agent ID to an
    /// array of value estimates. Size of outer dictionary corresponds to the
    /// number of brains defined in the Unity environment. Size of inner 
    /// dictionary corresponds to the number of agents attached to the 
    /// corresponding brain. Updated following <see cref="UpdateActions"/>.
    Dictionary<string, Dictionary<int, float>> storedValues;

    // For Messages
    List<float> concatenatedStates =
        new List<float>(DEFAULT_NUM_AGENTS * DEFAULT_NUM_OBSERVATIONS);
    List<float> concatenatedRewards =
        new List<float>(DEFAULT_NUM_AGENTS);
    List<float> concatenatedMemories =
        new List<float>(DEFAULT_NUM_AGENTS * DEFAULT_NUM_OBSERVATIONS);
    List<bool> concatenatedDones =
        new List<bool>(DEFAULT_NUM_AGENTS);
    List<bool> concatenatedMaxes =
        new List<bool>(DEFAULT_NUM_AGENTS);
    List<float> concatenatedActions =
        new List<float>(DEFAULT_NUM_AGENTS * DEFAULT_NUM_OBSERVATIONS);

    /// Socket used to send and receive messages with Python API.
    Socket socket;

    /// Re-usable buffer to store messages received from Python API.
    byte[] messageBuffer;

    /// Re-usable buffer to store length of messages forthcoming from Python
    /// API. Before we receive a large message from the Python API, it first
    /// sends length information. This buffer stores that length information.
    byte[] lengthBuffer;

    /// Path to the log file that is used by the communicator. It is passed to
    /// the Python API as part of the first message so it can load and display
    /// logging information.
    string logPath;

    int comPort;
    int randomSeed;
    StreamWriter logWriter;
    string sMessageString;
    string rMessage;

    /// Format of the Step message which is sent to the Python API.
    [System.Serializable]
    public struct StepMessage
    {
        public string brain_name;
        public List<int> agents;
        public List<float> states;
        public List<float> rewards;
        public List<float> actions;
        public List<float> memories;
        public List<bool> dones;
        public List<bool> maxes;
    }

    /// Buffer object that is re-used to store StepMessages (container for
    /// messages sent from Unity to Python).
    StepMessage stepMessageBuffer;

    /// Format of the Agent message which is sent from the Python API to Unity.
    struct AgentMessage
    {
        public Dictionary<string, List<float>> action { get; set; }
        public Dictionary<string, List<float>> memory { get; set; }
        public Dictionary<string, List<float>> value { get; set; }
    }

    /// Format of the ResetParameters message. Contains both the reset
    /// parameters and a boolean train model flag which indicates whether
    /// the learning environment mode is training (true) or inference (false).
    struct ResetParametersMessage
    {
        public Dictionary<string, float> parameters { get; set; }
        public bool train_model { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalCommunicator"/>
    /// class. Specifically, initializes the internal data structures.
    /// </summary>
    /// <param name="academy">
    /// Academy that this communicator handles communication for.
    /// </param>
    public ExternalCommunicator(Academy academy)
    {
        this.academy = academy;
        brains = new List<Brain>();
        brainAgents = new Dictionary<string, List<int>>();

        hasSentState = new Dictionary<string, bool>();

        storedActions = new Dictionary<string, Dictionary<int, float[]>>();
        storedMemories = new Dictionary<string, Dictionary<int, float[]>>();
        storedValues = new Dictionary<string, Dictionary<int, float>>();
    }

    /// <summary> <inheritdoc/> </summary>
    /// <param name="brain">Brain to register.</param>
    public void SubscribeBrain(Brain brain)
    {
        brains.Add(brain);
        hasSentState[brain.gameObject.name] = false;
    }

    /// <summary> <inheritdoc/> </summary>
    /// <remarks>
    /// Attempts a handshake with external API. Note that in the current
    /// implementation, only the validity of the command-line arguments is
    /// verified and no actualy handshake is performed. Furthermore, this call
    /// is stateful since the command-line arguments are parsed and stored
    /// within an instance variable.
    /// </remarks>
    /// <returns>
    /// <c>true</c>, if handshake was successful; <c>false</c> otherwise.
    /// </returns>
    public bool CommunicatorHandShake()
    {
        try
        {
            ReadArgs();
        }
        catch
        {
            return false;
        }
        return true;
    }

    /// <summary> <inheritdoc/> </summary>
    /// <remarks>
    /// Initializes the socket and sends the first message to the Python API.
    /// </remarks>
    public void InitializeCommunicator()
    {
        // Attach logger callback to the logMessageReceived event
        Application.logMessageReceived += HandleLog;

        // Initialize log file.
        logPath =
            Path.GetFullPath(".") + Path.DirectorySeparatorChar + LOG_FILENAME;
        logWriter = new StreamWriter(logPath, false);
        logWriter.WriteLine(System.DateTime.Now.ToString());
        logWriter.WriteLine(" ");
        logWriter.Close();
        messageBuffer = new byte[MESSAGE_BUFFER_SIZE];
        lengthBuffer = new byte[LENGTH_BUFFER_SIZE];

        // Create a TCP/IP socket connection.
        socket = new Socket(
            AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("localhost", comPort);

        // Initialize and populate the academy parameters.
        var academyParamerters = new AcademyParameters
        {
            AcademyName = academy.gameObject.name,
            resetParameters = academy.resetParameters,
            brainParameters = new List<BrainParameters>(),
            brainNames = new List<string>(),
            externalBrainNames = new List<string>(),
            apiNumber = API_VERSION,
            logPath = logPath
        };
        foreach (Brain b in brains)
        {
            academyParamerters.brainParameters.Add(b.brainParameters);
            academyParamerters.brainNames.Add(b.gameObject.name);
            if (b.brainType == BrainType.External)
            {
                academyParamerters.externalBrainNames.Add(b.gameObject.name);
            }
        }

        // Send academy parameters.
        SendParameters(academyParamerters);

        stepMessageBuffer = new StepMessage();
    }

    /// Callback method attached to the logMessageReceived event. Simply prints
    /// the log message details to file.
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logWriter = new StreamWriter(logPath, true);
        logWriter.WriteLine(type.ToString());
        logWriter.WriteLine(logString);
        logWriter.WriteLine(stackTrace);
        logWriter.Close();
    }

    /// <summary> <inheritdoc/> </summary>
    /// <remarks>
    /// If the command received is not recognized, it defaults to QUIT.
    /// </remarks>
    /// <returns>The command received from the socket.</returns>
    public ExternalCommand GetCommand()
    {
        int location = socket.Receive(messageBuffer);
        string message = Encoding.ASCII.GetString(messageBuffer, 0, location);
        switch (message)
        {
            case "STEP":
                return ExternalCommand.STEP;
            case "RESET":
                return ExternalCommand.RESET;
            case "QUIT":
                return ExternalCommand.QUIT;
            default:
                return ExternalCommand.QUIT;
        }
    }

    /// <summary> <inheritdoc/> </summary>
    public Dictionary<string, float> GetResetParameters()
    {
        socket.Send(Encoding.ASCII.GetBytes("CONFIG_REQUEST"));
        Receive();
        var resetParams = JsonConvert.DeserializeObject<ResetParametersMessage>(rMessage);
        academy.SetInference(!resetParams.train_model);
        return resetParams.parameters;
    }

    /// Used to read Python-provided environment parameters.
    void ReadArgs()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        var inputPort = "";
        var inputSeed = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--port")
            {
                inputPort = args[i + 1];
            }
            if (args[i] == "--seed")
            {
                inputSeed = args[i + 1];
            }
        }

        comPort = int.Parse(inputPort);
        randomSeed = int.Parse(inputSeed);
        Random.InitState(randomSeed);
    }

    /// Sends Academy parameters to Python API.
    void SendParameters(AcademyParameters envParams)
    {
        string envMessage = JsonConvert.SerializeObject(envParams, Formatting.Indented);
        socket.Send(Encoding.ASCII.GetBytes(envMessage));
    }

    /// Receives a short message from the Python API.
    void Receive()
    {
        int location = socket.Receive(messageBuffer);
        rMessage = Encoding.ASCII.GetString(messageBuffer, 0, location);
    }

    /// Receives a long message from the Python API by piecing multiple chunks.
    string ReceiveAll()
    {
        socket.Receive(lengthBuffer);
        int totalLength = System.BitConverter.ToInt32(lengthBuffer, 0);
        int location = 0;
        rMessage = "";
        while (location != totalLength)
        {
            int fragment = socket.Receive(messageBuffer);
            location += fragment;
            rMessage += Encoding.ASCII.GetString(messageBuffer, 0, fragment);
        }
        return rMessage;
    }

    /// Ends connection and closes environment.
    void OnApplicationQuit()
    {
        socket.Close();
        socket.Shutdown(SocketShutdown.Both);
    }

    /// Converts texture object to a byte array to enable sending to the
    /// Python API.
    static byte[] TextureToByteArray(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);
        Resources.UnloadUnusedAssets();
        return bytes;
    }

    /// Creates a new array object that contains the contents of the input
    /// array, except that the first 4 elements of the new array represent
    /// the length of the original array.
    static byte[] PrependLength(byte[] input)
    {
        byte[] newArray = new byte[input.Length + 4];
        input.CopyTo(newArray, 4);
        System.BitConverter.GetBytes(input.Length).CopyTo(newArray, 0);
        return newArray;
    }

    /// <summary>
    /// Collects the information from the brains agents and sends it to the
    /// Python API, in addition to a boolean flag if the Academy is done.
    /// Sends a single <see cref="StepMessage"/> message (which encompasses 
    /// information from all the agents) and then one image for every
    /// camera-agent combination. Following each sent message is a recieve
    /// call that is not parsed. Lastly, a boolean flag is sent to indicate
    /// if the academy is done.
    /// </summary>
    /// <param name="brain">Brain.</param>
    public void GiveBrainInfo(Brain brain)
    {
        var brainName = brain.gameObject.name;
        brainAgents[brainName] = new List<int>(brain.agents.Keys);
        brain.CollectEverything();

        concatenatedStates.Clear();
        concatenatedRewards.Clear();
        concatenatedMemories.Clear();
        concatenatedDones.Clear();
        concatenatedMaxes.Clear();
        concatenatedActions.Clear();

        foreach (int id in brainAgents[brainName])
        {
            concatenatedStates.AddRange(brain.currentStates[id]);
            concatenatedRewards.Add(brain.currentRewards[id]);
            concatenatedMemories.AddRange(brain.currentMemories[id].ToList());
            concatenatedDones.Add(brain.currentDones[id]);
            concatenatedMaxes.Add(brain.currentMaxes[id]);
            concatenatedActions.AddRange(brain.currentActions[id].ToList());
        }

        stepMessageBuffer.brain_name = brainName;
        stepMessageBuffer.agents = brainAgents[brainName];
        stepMessageBuffer.states = concatenatedStates;
        stepMessageBuffer.rewards = concatenatedRewards;
        stepMessageBuffer.actions = concatenatedActions;
        stepMessageBuffer.memories = concatenatedMemories;
        stepMessageBuffer.dones = concatenatedDones;
        stepMessageBuffer.maxes = concatenatedMaxes;

        sMessageString = JsonUtility.ToJson(stepMessageBuffer);
        socket.Send(PrependLength(Encoding.ASCII.GetBytes(sMessageString)));
        Receive();
        int i = 0;
        foreach (resolution res in brain.brainParameters.cameraResolutions)
        {
            foreach (int id in brainAgents[brainName])
            {
                socket.Send(
                    PrependLength(
                        TextureToByteArray(
                            brain.ObservationToTex(
                                brain.currentCameras[id][i],
                                res.width,
                                res.height))));
                Receive();
            }
            i++;
        }

        hasSentState[brainName] = true;

        if (hasSentState.Values.All(x => x))
        {
            // if all the brains listed have sent their state
            socket.Send(Encoding.ASCII.GetBytes((academy.IsDone() ? "True" : "False")));
            List<string> brainNames = hasSentState.Keys.ToList();
            foreach (string k in brainNames)
            {
                hasSentState[k] = false;
            }
        }

    }

    /// <summary>
    /// Listens for actions, memories, and values from the Python API.
    /// Sends a STEPPING message to the Python API and receives an
    /// <see cref="AgentMessage"/> message which is folded into
    /// <see cref="storedActions"/>, <see cref="storedMemories"/>, and
    /// <see cref="storedValues"/>.
    /// </summary>
    public void UpdateActions()
    {
        socket.Send(Encoding.ASCII.GetBytes("STEPPING"));
        ReceiveAll();
        var agentMessage =
            JsonConvert.DeserializeObject<AgentMessage>(rMessage);

        foreach (Brain brain in brains)
        {
            if (brain.brainType == BrainType.External)
            {
                var brainName = brain.gameObject.name;

                var actionDict = new Dictionary<int, float[]>();
                var memoryDict = new Dictionary<int, float[]>();
                var valueDict = new Dictionary<int, float>();

                for (int i = 0; i < brainAgents[brainName].Count; i++)
                {
                    if (brain.brainParameters.actionSpaceType ==
                        StateType.continuous)
                    {
                        actionDict.Add(
                            brainAgents[brainName][i],
                            agentMessage.action[brainName]
                                .GetRange(
                                    i * brain.brainParameters.actionSize,
                                    brain.brainParameters.actionSize)
                                .ToArray());
                    }
                    else
                    {
                        actionDict.Add(
                            brainAgents[brainName][i],
                            agentMessage.action[brainName]
                                .GetRange(i, 1)
                            .ToArray());
                    }

                    memoryDict.Add(
                        brainAgents[brainName][i],
                        agentMessage.memory[brainName]
                            .GetRange(i * brain.brainParameters.memorySize,
                                      brain.brainParameters.memorySize)
                            .ToArray());

                    valueDict.Add(
                        brainAgents[brainName][i],
                        agentMessage.value[brainName][i]);
                }
                storedActions[brainName] = actionDict;
                storedMemories[brainName] = memoryDict;
                storedValues[brainName] = valueDict;
            }
        }
    }

    /// <summary>
    /// Returns the actions corresponding to the provided Brain that were
    /// received from the Python API during the last call to
    /// <see cref="UpdateActions"></see>
    /// </summary>
    /// <returns>The actions for all agents attached to the brain,
    /// indexed by the agent ids.</returns>
    /// <param name="brainName">Brain name.</param>
    public Dictionary<int, float[]> GetDecidedAction(string brainName)
    {
        return storedActions[brainName];
    }

    /// <summary>
    /// Returns the memories corresponding to the provided Brain that were
    /// received from the Python API during the last call to
    /// <see cref="UpdateActions"></see>
    /// </summary>
    /// <returns>The memories for all agents attached to the brain,
    /// indexed by the agent ids.</returns>
    /// <param name="brainName">Brain name.</param>
    public Dictionary<int, float[]> GetMemories(string brainName)
    {
        return storedMemories[brainName];
    }

    /// <summary>
    /// Returns the values corresponding to the provided Brain that were
    /// received from the Python API during the last call to
    /// <see cref="UpdateActions"></see>
    /// </summary>
    /// <returns>The values for all agents attached to the brain,
    /// indexed by the agent ids.</returns>
    /// <param name="brainName">Brain name.</param>
    public Dictionary<int, float> GetValues(string brainName)
    {
        return storedValues[brainName];
    }
}