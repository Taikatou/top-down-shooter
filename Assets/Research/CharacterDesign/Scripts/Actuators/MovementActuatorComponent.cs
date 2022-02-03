using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Characters;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Unity.MLAgents.Actuators;

namespace Research.CharacterDesign.Scripts.Actuators
{
    public class MovementActuatorComponent : ActuatorComponent
    {
        public DirectionsKeyMapper directionsKeyMapper;
        public TopDownInputManager inputManager;
        private MovementActuator _movementActuator;
        private GunActuator _gunActuator;
        public MlCharacter character;

        public override ActionSpec ActionSpec =>
            ActionSpec.Combine(MovementActuator.MovementActionSpec, GunActuator.CompActionSpec);
        public override IActuator[] CreateActuators()
        {
            _movementActuator = new MovementActuator(directionsKeyMapper, inputManager, 0, ActionSpec, character);
            _gunActuator = new GunActuator(inputManager, 1, ActionSpec, character);
            
            return new IActuator[] { _movementActuator, _gunActuator };
        }
    }
    public class MovementActuator : TopDownActuator
    {
        private readonly int _actionIndex;
        private readonly DirectionsKeyMapper _directionsKeyMapper;
        private readonly TopDownInputManager _inputManager;
        public override ActionSpec ActionSpec { get; }
        public override string Name => "MovementActuator";
        public static ActionSpec MovementActionSpec => ActionSpec.MakeDiscrete(EDirectionsUtils.Length);

        public MovementActuator(DirectionsKeyMapper directionsKeyMapper, TopDownInputManager inputManager, int actionIndex, ActionSpec actionSpec, MlCharacter character) : base(character)
        {
            ActionSpec = actionSpec;
            _directionsKeyMapper = directionsKeyMapper;
            _inputManager = inputManager;
            _actionIndex = actionIndex;
        }

        protected override void ImpOnActionReceived(ActionBuffers actionBuffers)
        {
            var action = actionBuffers.DiscreteActions[_actionIndex];
            var primaryDirection = _directionsKeyMapper.GetVectorDirection(action);
            _inputManager.SetAiPrimaryMovement(primaryDirection);
        }

        public override void Heuristic(in ActionBuffers actionBuffersOut)
        {
            var discreteActions = actionBuffersOut.DiscreteActions;
            discreteActions[_actionIndex] = (int) _directionsKeyMapper.PrimaryDirections;
        }
    }
}
