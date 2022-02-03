using Research.CharacterDesign.Scripts.Characters;

namespace Research.LevelDesign.Scripts.MLAgents
{
    public static class TrainingConfig
    {
        public static readonly bool GroupReward = true;

        public static readonly bool RewardShotSuccess = false;

        public static readonly bool AsymmetricMatches = true;

        public static bool WinWithHealth = false;
        private static int _fixedIndexCounter=1;

        private static readonly bool _incrementFixedCounter = true;
        public static int GetPlayerId(MlCharacter[] array)
        {
            if (_incrementFixedCounter)
            {
                _fixedIndexCounter = (_fixedIndexCounter + 1)  % array.Length;;
            }

            return _fixedIndexCounter;
        }
    }
}
