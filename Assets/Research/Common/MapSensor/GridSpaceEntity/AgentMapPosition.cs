﻿using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Policies;

namespace Research.Common.MapSensor
{
    public class AgentMapPosition : EntityMapPosition
    {
        public BehaviorParameters behaviorParameters;
        public override GridSpace GetType(int teamId)
        {
            return behaviorParameters.TeamId == teamId ? GridSpace.Team1 : GridSpace.Team2;
        }
    }
}