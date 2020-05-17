using UnityEngine;
using System.Collections;

namespace HotTotem.GhostToolPro.DemoScripts
{
    public class ButtonTest2D : MonoBehaviour
    {
        public Ghostable2D[] targets;
        public static string uniq;
        public void Start()
        {
            GhostTool2D.instance.OnSaved += GhostTool_instance_OnSaved;
        }

        void GhostTool_instance_OnSaved(string name)
        {
            Debug.Log(name + " is saved");
        }
        public void startRecording()
        {
            Debug.Log("Rec Started : " + Time.realtimeSinceStartup);
            GhostTool2D.instance.startRecording(targets, "test");
            Debug.Log("Rec Started Finished : " + Time.realtimeSinceStartup);
        }
        public void stopRecording()
        {
            Debug.Log("Rec Stopped : " + Time.realtimeSinceStartup);
            GhostTool2D.instance.stopRecording("test");
            Debug.Log("Rec Stopped Finished : " + Time.realtimeSinceStartup);
        }
        public void loadReplaying()
        {
            Debug.Log("Replay Loading Started : " + Time.realtimeSinceStartup);
            uniq = GhostTool2D.instance.loadReplay("test");
            Debug.Log("Replay Loading Finished : " + Time.realtimeSinceStartup);
        }
        public void startReplaying()
        {
            Debug.Log("Replay Replaying Started : " + Time.realtimeSinceStartup);
            GhostTool2D.instance.startReplaying(uniq);
            Debug.Log("Replay Replaying Finished : " + Time.realtimeSinceStartup);
        }
        public void pauseReplaying()
        {
            Debug.Log("Replay Pause Started : " + Time.realtimeSinceStartup);
            GhostTool2D.instance.pauseReplaying(uniq);
            Debug.Log("Replay Pause Finished : " + Time.realtimeSinceStartup);
        }
        public void stopReplaying()
        {
            Debug.Log("Replay Stop Started : " + Time.realtimeSinceStartup);
            GhostTool2D.instance.stopReplaying(uniq);
            Debug.Log("Replay Stop Finished : " + Time.realtimeSinceStartup);
        }
        public void resumeReplaying()
        {
            Debug.Log("Replay Resume Started : " + Time.realtimeSinceStartup);
            GhostTool2D.instance.resumeReplaying(uniq);
            Debug.Log("Replay Resume Finished : " + Time.realtimeSinceStartup);
        }
        public void saveRecording()
        {
            Debug.Log("Saving Started : " + Time.realtimeSinceStartup);
            GhostTool2D.instance.saveRecordingFromCache("test", true, "test");
            var bytesArray = GhostTool2D.instance.shareReplay("test");
            Debug.Log(bytesArray.Length);
            GhostTool2D.instance.receiveReplay("newSaveName", bytesArray);
            GhostTool2D.instance.startReplaying(GhostTool2D.instance.loadReplay("newSaveName"));
            Debug.Log("Saving Stopped : " + Time.realtimeSinceStartup);
        }
    }
}