using System;

namespace Research.Common
{
    public static class EDirectionsUtils
    {
        public static int Length => Enum.GetNames(typeof(EDirections)).Length;
    }
    
    public enum EDirections { None, Left, Right, Up, Down }
    
    public enum AimControl
    {
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
