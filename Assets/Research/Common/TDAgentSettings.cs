namespace Research.Common
{
    public enum Directions { None, Left, Right, Up, Down }
    
    public enum AimControl
    {
        Addition,
        EightWay,
        SixTeenWay,
        ThirtyTwoWay
    };

    public enum LevelCurriculum
    {
        NoAdversaryNoMap,
        NoAdversary,
        AllActive
    }

    [System.Serializable]
    public struct ObservationSettings
    {
        public bool observeHealth;
        public bool observeWeaponTrace;
        public bool observeInput;
        public bool observeCloseToWall;
    }

    [System.Serializable]
    public struct TrainingSettings
    {
        public bool secondaryInputEnabled;
        public bool shootEnabled;
        public bool secondaryAbilityEnabled;
        public bool punishTime;
        public bool enableCurriculum;
    }
}
