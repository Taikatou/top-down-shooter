using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    public class MultiplayerGUIManager : GUIManager
    {
        [Header("Multiplayer GUI")]
        public GameObject SplitHUD;
        public GameObject GroupHUD;
        /// a UI object used to display the splitters UI images
        public GameObject SplittersGUI;
    }
}
