using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone;
using Research.LevelDesign.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public enum CharacterTypes {Base, AOE, Healer, Tank}
    public struct TeamMember
    {
        public int TeamID;
        public string Type;
        public float SkillRating;

        public static readonly Dictionary<string, CharacterTypes> CharacterNames = new Dictionary<string, CharacterTypes>
        {
            { "Koala", CharacterTypes.Base },
            { "KoalaAOE", CharacterTypes.AOE },
            { "KoalaHealer", CharacterTypes.Healer },
            { "KoalaTank", CharacterTypes.Tank }
        };
    }

    public static class WinLossMap
    {
        public static readonly Dictionary<WinLossCondition, int> RewardMap = new Dictionary<WinLossCondition, int>
        {
            { WinLossCondition.Win, 1 },
            { WinLossCondition.Draw, 0},
            { WinLossCondition.Loss, -1}
        };
    }
    
    public enum WinLossCondition { Win, Loss, Draw};
    public class EnvironmentInstance : GetEnvironmentMapPositions
    {
        public static int TeamCount = 2;
        
        public MlCharacter[] possibleCharactersPolicy;
        public MlCharacter[] possibleCharactersPrior;
        
        public int gameTime = 120;
        public int changeLevelMap = 10;
        
        public AnalysisTool outPutter;
        public NuclearThroneLevelGenerator levelGenerator;
        public GetSpawnProcedural getSpawnProcedural;
        public List<Character> mlCharacters;
        
        public float CurrentTimer { get; private set; }

        
        public int CurrentLevelCounter { get; private set; }

        private int _randomSeed;
        private bool _gameOver;
        private readonly Dictionary<int, SimpleMultiAgentGroup> _agentGroups = new Dictionary<int, SimpleMultiAgentGroup>();
        
        public GameObject characterLayer;
        
        public override BaseMapPosition[] EntityMapPositions => GetComponentsInChildren<BaseMapPosition>();

        private int _fixedIndex;

        private void Start()
        {
            _fixedIndex = TrainingConfig.GetPlayerId(possibleCharactersPolicy);
            
            CurrentTimer = gameTime;
            CurrentLevelCounter = changeLevelMap;
            mlCharacters = new List<Character>();
            
            SpawnTeams();
        }

        private int[] GetTeamDeaths()
        {
            var teamDeaths = new[] { 0, 0 };
            foreach (var character in mlCharacters)
            {
                if (MlUtils.Dead(character))
                {
                    var behaviour = character.GetComponentInChildren<BehaviorParameters>();
                    var index = behaviour.TeamId;
                    teamDeaths[index]++;
                }
            }
            return teamDeaths;
        }

        private void SpawnMultipleCharacters()
        {
            var characterDict = new Dictionary<int, List<Character>>();
            foreach (var character in mlCharacters)
            {
                var teamId = character.GetComponentInChildren<BehaviorParameters>().TeamId;
                if (!characterDict.ContainsKey(teamId))
                {
                    characterDict.Add(teamId, new List<Character>());
                }
                characterDict[teamId].Add(character);
            }
            
            for (var i = 0; i < mlCharacters.Count; i++)
            {
                var spawnPoint = getSpawnProcedural.PointDict[i];
                spawnPoint.SpawnPlayer(characterDict[i%2][i/2]);
                // Debug.Assert([i], "Agent component was not found on this gameObject and is required.");
            }
        }

        private void SpawnCharacters(MlCharacter[] possibleCharacters)
        {
            var index = _fixedIndex;
            var random = new System.Random();
            for (var j = 0; j < TeamCount; j++)
            {
                if (TrainingConfig.AsymmetricMatches)
                {
                    index = random.Next(0, possibleCharacters.Length);
                }
                var characterPrefab = possibleCharacters[index];
                    
                var character = Instantiate(characterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                character.transform.parent = characterLayer.transform;
                mlCharacters.Add(character);
                AddToGroup(character.GetComponentInChildren<Agent>());
            }  
        }

        private void AddToGroup(Agent agent)
        {
            if (TrainingConfig.GroupReward)
            {
                var teamId = agent.GetComponent<BehaviorParameters>().TeamId;
                if (!_agentGroups.ContainsKey(teamId))
                {
                    _agentGroups.Add(teamId, new SimpleMultiAgentGroup());
                }
                _agentGroups[teamId].RegisterAgent(agent);
            }
        }

        private void SpawnTeams()
        {
            SpawnCharacters(possibleCharactersPolicy);
            SpawnCharacters(possibleCharactersPrior);
        }

        private void ChangeLevelDesign()
        {
            if (levelGenerator)
            {
                CurrentLevelCounter++;
                if (CurrentLevelCounter >= changeLevelMap)
                {
                    DestroyCharacters();
                    SpawnTeams();
                    
                    _randomSeed = GetRandomSeed();
                    levelGenerator.GenerateMap(_randomSeed);
                    CurrentLevelCounter = 0;
                }
            }
        }

        private void DestroyCharacters()
        {
            foreach (var t in mlCharacters)
            {
                if (TrainingConfig.GroupReward)
                {
                    var behaviour = t.GetComponentInChildren<BehaviorParameters>();
                    var agent = t.GetComponentInChildren<Agent>();
                    _agentGroups[behaviour.TeamId].UnregisterAgent(agent);
                } 
                Destroy(t.gameObject);
            }
            
            mlCharacters.Clear();
        }

        private static int GetRandomSeed()
        {
            var randomSeed = (int) System.DateTime.Now.Ticks;
            return Mathf.Abs(randomSeed);
        }

        public void StartSimulation(int randomSeed)
        {
            if (randomSeed == -1)
            {
                randomSeed = GetRandomSeed();
            }
            _randomSeed = randomSeed * GetHashCode();
            StartCoroutine(Restart());
        }

        public override IEnumerator Restart()
        {
            // Pause AI and start stuff
            SetAllowDecisions(false);
            yield return new WaitForEndOfFrame();
            
            ChangeLevelDesign();
            
            foreach (var pick in Resources.FindObjectsOfTypeAll<HealthPickup>())
            {
                pick.gameObject.SetActive(true);
            }
            
            // Restart the game
            InstantiatePlayableCharacters();
            SpawnMultipleCharacters();

            // reenable characters
            SetAllowDecisions(true);
            
            CurrentTimer = gameTime;
            _gameOver = false;
        }

        private void SetAllowDecisions(bool allow)
        {
            foreach (var agent in mlCharacters)
            {
                var decisionRequester = agent.GetComponentInChildren<DecisionRequester>();
                if (decisionRequester != null)
                {
                    decisionRequester.allowDecisions = allow;   
                }
            }
        }

        private void InstantiatePlayableCharacters()
        {
            foreach(var agent in mlCharacters)
            {
                var health = agent.GetComponent<MlHealth>();
                health.Revive();
            }
        }

        public void OnPlayerDeath(Character playerCharacter)
        {
            var gameOver = GameOverCondition();
            if (gameOver)
            {
                StartCoroutine(GameOver());
            }
        }
        
        private bool GameOverCondition()
        {
            var teamDeaths = GetTeamDeaths();
            var gameOver = teamDeaths[0] == TeamCount || teamDeaths[1] == TeamCount;
            return gameOver;
        }

        private static int GetVictoryCondition(IReadOnlyList<int> teamData)
        {
            var draw = teamData[0] == teamData[1];
            if (!draw)
            {
                return teamData[0] < teamData[1] ? 0 : 1;
            }

            return -1;
        }

        private int WinningTeam()
        {
            var endCondition = GetVictoryCondition(GetTeamDeaths());
            if (endCondition == -1 && TrainingConfig.WinWithHealth)
            {
                var teamHealth = new[] { 0, 0 };
                foreach (var character in mlCharacters)
                {
                    var health = character.GetComponent<MlHealth>();
                    var behaviour = character.GetComponentInChildren<BehaviorParameters>();
                    teamHealth[behaviour.TeamId] += (int) health.CurrentHealth;
                }
                endCondition = GetVictoryCondition(teamHealth);
            }

            return endCondition;
        }

        private static WinLossCondition GetRewardCondition(int teamId, int winningTeam)
        {
            if(winningTeam != -1)
            {
                return winningTeam == teamId? WinLossCondition.Win: WinLossCondition.Loss;
            }
            return WinLossCondition.Draw;
        }

        private void FixedUpdate()
        {
            if (!_gameOver)
            {
                CurrentTimer -= Time.deltaTime;
                if (CurrentTimer <= 0)
                {
                    _gameOver = true;
                    StartCoroutine(GameOver());
                }   
            }
        }

        private IEnumerator GameOver()
        {
            var winningTeamId = WinningTeam();
 
            var teamMembers = new TeamMember[mlCharacters.Count];
            for (var j = 0; j < mlCharacters.Count; j++)
            {
                var player = mlCharacters[j];

                var agent = player.GetComponentInChildren<Agent>();
                var behaviour = agent.GetComponent<BehaviorParameters>();
                SetAgentReward(agent, behaviour, winningTeamId);
                
                Debug.Log(behaviour.BehaviorName);
                var key = behaviour.BehaviorName + behaviour.TeamId;
                var skillRating = Academy.Instance.EnvironmentParameters.GetElo(key, 1200);
                teamMembers[j] = new TeamMember
                {
                    TeamID = behaviour.TeamId,
                    Type = behaviour.BehaviorName,
                    SkillRating = skillRating
                };
            }

            EndEpisode();
            
            var playerWin = GetRewardCondition(0, winningTeamId);
            
            outPutter.AddResult(playerWin, teamMembers);

            if (MlLevelManager.UnitySimulation)
            {
                Application.Quit();
            }
            else
            {
                StartCoroutine(Restart());   
            }
            yield break;
        }
        
        private void EndEpisode()
        {
            if (TrainingConfig.GroupReward)
            {
                foreach (var group in _agentGroups.Values)
                {
                    group.EndGroupEpisode();
                }
            }
            else
            {
                foreach (var character in mlCharacters)
                {
                    var agent = character.GetComponentInChildren<Agent>();
                    agent.EndEpisode();
                }
            }
        }

        private void SetAgentReward(Agent agent, BehaviorParameters behaviour, int winningTeamId)
        {
            var winLossCondition = GetRewardCondition(behaviour.TeamId, winningTeamId);
            var reward = WinLossMap.RewardMap[winLossCondition];

            if (TrainingConfig.GroupReward)
            {
                if (_agentGroups.ContainsKey(behaviour.TeamId))
                {
                    _agentGroups[behaviour.TeamId].AddGroupReward(reward);
                }
            }
            else
            {
                agent.AddReward(reward);
            }
        }
    }
}
