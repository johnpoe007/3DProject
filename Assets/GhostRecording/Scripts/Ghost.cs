using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class WB_Vector3 {

	private float x;
	private float y;
	private float z;

	public WB_Vector3() { }
	public WB_Vector3(Vector3 vec3) {
		this.x = vec3.x;
		this.y = vec3.y;
		this.z = vec3.z;
	}

	public static implicit operator WB_Vector3(Vector3 vec3) {
		return new WB_Vector3(vec3);
	}
	public static explicit operator Vector3(WB_Vector3 wb_vec3) {
		return new Vector3(wb_vec3.x, wb_vec3.y, wb_vec3.z);
	}
}

[System.Serializable]
public class WB_Quaternion {

    private float w;
	private float x;
	private float y;
	private float z;

	public WB_Quaternion() { }
	public WB_Quaternion(Quaternion quat3) {
		this.x = quat3.x;
		this.y = quat3.y;
		this.z = quat3.z;
        this.w = quat3.w;
	}

	public static implicit operator WB_Quaternion(Quaternion quat3) {
		return new WB_Quaternion(quat3);
	}
	public static explicit operator Quaternion(WB_Quaternion wb_quat3) {
		return new Quaternion(wb_quat3.x, wb_quat3.y, wb_quat3.z, wb_quat3.w);
	}
}

[System.Serializable]
public class GhostShot
{
    public float timeMark = 0.0f;       // mark at which the position and rotation are of af a given shot

    private WB_Vector3 _posMark;
    public Vector3 posMark {
		get {
			if (_posMark == null) {
				return Vector3.zero;
			} else {
				return (Vector3)_posMark;
			}
		}
		set {
			_posMark = (WB_Vector3)value;
		}
	}

    private WB_Quaternion _rotMark;
    public Quaternion rotMark {
		get {
			if (_rotMark == null) {
				return Quaternion.identity;
			} else {
				return (Quaternion)_rotMark;
			}
		}
		set {
			_rotMark = (WB_Quaternion)value;
		}
	}

}

public class Ghost : MonoBehaviour {

    private List<GhostShot> framesList;
    private List<GhostShot> lastReplayList = null;

	GameObject theGhost;

    private float replayTimescale = 1;
    private int replayIndex = 0;
    private float recordTime = 0.0f;
    private float replayTime = 0.0f;

    //Check whether we should be recording or not
    bool startRecording = false, recordingFrame = false, playRecording = false;

	public void loadFromFile() {
		//Check if Ghost file exists. If it does load it
		if(File.Exists(Application.persistentDataPath + "/Ghost")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/Ghost", FileMode.Open);
			lastReplayList = (List<GhostShot>)bf.Deserialize(file);
			file.Close();
		} else {
			Debug.Log("No Ghost Found");
		}
	}

	void FixedUpdate () {
        if (startRecording) {
            startRecording = false;
			Debug.Log("Recording Started");
            StartRecording();
        } else if (recordingFrame) {
		    RecordFrame();
        }
        if (lastReplayList != null && playRecording) {
            MoveGhost();
        }
	}

	private void RecordFrame() {
		recordTime += Time.smoothDeltaTime * 1000;
        GhostShot newFrame = new GhostShot()
        {
            timeMark = recordTime,
			posMark = this.transform.position,
			rotMark = this.transform.rotation
        };

        framesList.Add(newFrame);
	}

	public void StartRecording() {
        framesList = new List<GhostShot>();
        replayIndex = 0;
        recordTime = Time.time * 1000;
        recordingFrame = true;
		playRecording = false;
    }

	public void StopRecordingGhost() {
		recordingFrame = false;
		lastReplayList = new List<GhostShot>(framesList);

		//This will overwrite any previous Save
		//Run function if new highscore achieved or change filename in function
        //SaveGhostToFile(); //Save Ghost to file on device/computer

	}

	public void playGhostRecording() {
		CreateGhost();
		replayIndex = 0;
		playRecording = true;
	}

    public void StartRecordingGhost() {
        startRecording = true;
    }

    public void MoveGhost()
    {
        replayIndex++;

        if (replayIndex < lastReplayList.Count)
        {
            GhostShot frame = lastReplayList[replayIndex];
            DoLerp(lastReplayList[replayIndex - 1], frame);
            replayTime += Time.smoothDeltaTime * 1000 * replayTimescale;
        }
    }

    private void DoLerp(GhostShot a, GhostShot b)
    {
		if(GameObject.FindWithTag("Ghost") != null) {
	        theGhost.transform.position = Vector3.Slerp(a.posMark, b.posMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
	        theGhost.transform.rotation = Quaternion.Slerp(a.rotMark, b.rotMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
		}
    }

    public void SaveGhostToFile()
    {
        // Prepare to write
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Ghost");
        Debug.Log("File Location: " + Application.persistentDataPath + "/Ghost");
        // Write data to disk
        bf.Serialize(file, lastReplayList);
        file.Close();
    }

    public void CreateGhost()
    {
		//Check if ghost exists or not, no reason to destroy and create it everytime.
		if(GameObject.FindWithTag("Ghost") == null) {
	        theGhost = Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
	        theGhost.gameObject.tag = "Ghost";

	        //Disable RigidBody
	        theGhost.GetComponent<Rigidbody>().isKinematic = false;

            MeshRenderer mr = theGhost.gameObject.GetComponent<MeshRenderer>();
            mr.material = Resources.Load("Ghost_Shader", typeof(Material)) as Material;
		}
    }
}
