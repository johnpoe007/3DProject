using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace HotTotem.GhostToolPro.DemoScripts
{
    public class ButtonTest : MonoBehaviour
    {
        public Ghostable[] targets;
        private string uniq;
        public string saveName;
        public void Start()
        {
            GhostTool.instance.OnSaved += GhostTool_instance_OnSaved;
        }

        void GhostTool_instance_OnSaved(string name)
        {
            Debug.Log(name + " is saved");
        }
        public void startRecording()
        {
            Debug.Log("Rec Started : " + Time.realtimeSinceStartup);
            GhostTool.instance.startRecording(targets, saveName, GhostToolResolution.VeryHigh);
            Debug.Log("Rec Started Finished : " + Time.realtimeSinceStartup);
        }
        public void stopRecording()
        {
            Debug.Log("Rec Stopped : " + Time.realtimeSinceStartup);
            GhostTool.instance.stopRecording(saveName);
            Debug.Log("Rec Stopped Finished : " + Time.realtimeSinceStartup);
        }
        public void loadReplaying()
        {
            Debug.Log("Replay Loading Started : " + Time.realtimeSinceStartup);
            uniq = GhostTool.instance.loadReplay(saveName);
            Debug.Log("Replay Loading Finished : " + Time.realtimeSinceStartup);
        }
        public void startReplaying()
        {
            Debug.Log("Replay Replaying Started : " + Time.realtimeSinceStartup);
            GhostTool.instance.startReplaying(uniq);
            Debug.Log("Replay Replaying Finished : " + Time.realtimeSinceStartup);
        }
        public void pauseReplaying()
        {
            Debug.Log("Replay Pause Started : " + Time.realtimeSinceStartup);
            GhostTool.instance.pauseReplaying(uniq);
            Debug.Log("Replay Pause Finished : " + Time.realtimeSinceStartup);
        }
        public void stopReplaying()
        {
            Debug.Log("Replay Stop Started : " + Time.realtimeSinceStartup);
            GhostTool.instance.stopReplaying(uniq);
            Debug.Log("Replay Stop Finished : " + Time.realtimeSinceStartup);
        }
        public void resumeReplaying()
        {
            Debug.Log("Replay Resume Started : " + Time.realtimeSinceStartup);
            GhostTool.instance.resumeReplaying(uniq);
            Debug.Log("Replay Resume Finished : " + Time.realtimeSinceStartup);
        }
        public void saveRecording()
        {
            Debug.Log("Replay Save Started (Blocking) : " + Time.realtimeSinceStartup);
            GhostTool.instance.saveRecordingFromCache(saveName, true, saveName);
            Debug.Log("Replay Save Finished (Blocking) : " + Time.realtimeSinceStartup);
            Debug.Log("Replay Export & Import Started : " + Time.realtimeSinceStartup);
            var replayExport = GhostTool.instance.shareReplay(saveName);
            GhostTool.instance.receiveReplay(saveName, replayExport);
            Debug.Log("Replay Export & Import Finished : " + Time.realtimeSinceStartup);
        }
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}