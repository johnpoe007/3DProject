using UnityEngine;
using System.Collections;
namespace HotTotem.GhostToolPro.DemoScripts
{
    public class GoalController : MonoBehaviour
    {
        public Ghostable[] targets;
        bool isRecording = false;
        string uniqueID = "";
        public string saveName;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Car")
            {
                if (!isRecording)
                {
                    GhostTool.instance.startRecording(targets, saveName);
                    isRecording = true;
                }
                else
                {
                    GhostTool.instance.stopRecording(saveName);
                    uniqueID = GhostTool.instance.loadReplay(saveName);
                    if (uniqueID != "")
                    {
                        GhostTool.instance.startReplaying(uniqueID);
                        uniqueID = "";
                    }
                    GhostTool.instance.startRecording(targets, saveName);
                }
            }
        }

    }
}