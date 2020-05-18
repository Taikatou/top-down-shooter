using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This component allows the definition of a level that can then be accessed and loaded. Used mostly in the level map scene.
    /// </summary>
    [AddComponentMenu("TopDown Engine/GUI/LevelSelector")]
    public class LevelSelector : MonoBehaviour
	{
		/// the exact name of the target level
	    public string LevelName;

		/// <summary>
		/// Loads the level specified in the inspector
		/// </summary>
	    public virtual void GoToLevel()
	    {
	        LevelManager.Instance.GotoLevel(LevelName);
	    }

        /// <summary>
        /// Restarts the current level, without reloading the whole scene
        /// </summary>
        public virtual void RestartLevel()
        {
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.UnPause, null);
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RespawnStarted, null);
        }

		/// <summary>
		/// Reloads the current level
		/// </summary>
	    public virtual void ReloadLevel()
		{
			// we trigger an unPause event for the GameManager (and potentially other classes)
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.UnPause, null);
			LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
	    }
		
	}
}