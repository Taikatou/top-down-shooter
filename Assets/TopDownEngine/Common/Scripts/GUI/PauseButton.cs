using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A simple component meant to be added to the pause button
	/// </summary>
	public class PauseButton : MonoBehaviour
	{
		/// <summary>
        /// Triggers a pause event
        /// </summary>
	    public virtual void PauseButtonAction()
	    {
			// we trigger a Pause event for the GameManager and other classes that could be listening to it too
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Pause, null);
	    }	

        /// <summary>
        /// Unpauses the game via an UnPause event
        /// </summary>
        public virtual void UnPause()
        {
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Pause, null);
        }
	}
}