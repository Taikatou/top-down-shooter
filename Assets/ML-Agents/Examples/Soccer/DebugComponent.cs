using System.Collections.Generic;
using UnityEngine;

namespace Examples.Soccer
{
    public class DebugComponent : MonoBehaviour
    {
        public List<AgentSoccer> agents;

        // Update is called once per frame
        void LateUpdate()
        {
            var debugTxt = "";
            foreach (var agent in agents)
            {
                debugTxt += "(" + agent.debugDir + " " + agent.rotation + ")" + "\t";
            }
            Debug.Log(debugTxt);
        }
    }
}
