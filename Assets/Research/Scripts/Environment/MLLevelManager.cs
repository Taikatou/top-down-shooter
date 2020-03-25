using System.Collections;
using System.Collections.Generic;
using MLAgents;
using MLAgents.Policies;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Scripts.Environment
{
    public class MLLevelManager : GrasslandsMultiplayerLevelManager
    {
        private int _colourSwitch = 0;

        public int teamSize = 2;

        public AgentQueue agentQueue;

        private int[] GetTeamDeaths()
        {
            var teamDeaths = new[] { 0, 0 };
            foreach (var character in Players)
            {
                if (MlUtils.Dead(character))
                {
                    var behaviour = character.GetComponent<BehaviorParameters>();
                    var index = behaviour.TeamId;
                    teamDeaths[index]++;
                }
            }
            return teamDeaths;
        }

        protected IEnumerator WaitForRestart()
        {
            yield return new WaitForSeconds(1);

            agentQueue.ReturnCharacters(Players);

            Initialization();
            
            InstantiatePlayableCharacters();

            SpawnMultipleCharacters();

            MMGameEvent.Trigger("Load");
        }

        public virtual void Restart()
        {
            foreach (var player in Players)
            {
                var requester = player.GetComponent<DecisionRequester>();
            
                requester.allowDecisions = false;
            }
            
            StartCoroutine(WaitForRestart());
        }

        protected override void InstantiatePlayableCharacters()
        {
            Players = new List<Character>();
            var blueTeam = _colourSwitch % 2;
            for (var i = 0; i < teamSize; i++)
            {
                var mlCharacter = agentQueue.PopRandomMlCharacter();
                SpawnTeamPlayer(mlCharacter, blueTeam, false);
                var priorMlCharacter = agentQueue.PopRandomPriorMlCharacter();
                SpawnTeamPlayer(priorMlCharacter, blueTeam, true);
            }
            _colourSwitch++;
        }

        protected virtual void SpawnTeamPlayer(Character newPlayer, int blueTeam, bool prior)
        {
            Players.Add(newPlayer);
            
            // Set TeamId
            var behaviour = newPlayer.GetComponent<BehaviorParameters>();
            // behaviour.TeamId = prior? 0 : 1;

            // Set outline
            var outline = newPlayer.GetComponentInChildren<SpriteOutline>();
            var teamId = behaviour.TeamId;
            outline.IsBlue = blueTeam == teamId;
        }

        protected override bool GameOverCondition()
        {
            var teamDeaths = GetTeamDeaths();

            var gameOver = teamDeaths[0] == 2 || teamDeaths[1] == 2;
            return gameOver;
        }

        private int WinningTeam()
        {
            var teamDeaths = GetTeamDeaths();
            var draw = teamDeaths[0] == teamDeaths[1];
            if (!draw)
            {
                return teamDeaths[0] > teamDeaths[1] ? 0 : 1;
            }

            return -1;
        }
        
        private int GetReward(int teamId, int winningTeam)
        {
            if(winningTeam != -1)
            {
                return winningTeam == teamId? 1: -1;
            }

            return 0;
        }

        protected override IEnumerator GameOver()
        {
            var winningTeamId = WinningTeam();
            
            var log = "";
            foreach (var player in Players)
            {
                var agent = player.GetComponent<TopDownAgent>();
                
                var teamId = agent.GetComponent<BehaviorParameters>().TeamId;
                var reward = GetReward(teamId, winningTeamId);
                agent.AddReward(reward);
                agent.EndEpisode();

                log += reward + ", ";
            }
            Debug.Log("Winning Team Id: " + winningTeamId + "\tReward: " + log);
            
            Restart();
            yield break;
        }
    }
}
