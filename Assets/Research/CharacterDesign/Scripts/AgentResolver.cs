using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public abstract class AgentResolver : MonoBehaviour
{
    public DirectionsKeyMapper directionsKeyMapper;
    public TrainingSettings trainingSettings;
    public TopDownInputManager inputManager;

    protected AgentResolver(TrainingSettings trainingSettings, DirectionsKeyMapper directionsKeyMapper, TopDownInputManager inputManager)
    {
        this.trainingSettings = trainingSettings;
        this.directionsKeyMapper = directionsKeyMapper;
        this.inputManager = inputManager;
    }

    public abstract void OnActionReceivedImp(ActionBuffers vectorAction);

    public abstract void CollectObservations(VectorSensor sensor);

    public abstract void HeuristicImp(in ActionBuffers actions);

    public abstract void OnEpisodeBegin();
}
