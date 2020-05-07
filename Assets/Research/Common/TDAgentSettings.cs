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

    public struct ObservationSettings
    {
        public bool ObserveHealth;

        public bool ObserveSpriteRenderer;

        public bool ObserveWeaponTrace;

        public bool ObserveInput;
    }

    public struct TrainingSettings
    {
        public bool SecondaryInputEnabled;
        public bool ShootEnabled;
        public bool SecondaryAbilityEnabled;
        public bool PunishTime;
    }
}
