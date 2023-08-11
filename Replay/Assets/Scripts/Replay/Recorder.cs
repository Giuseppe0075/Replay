using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Recorder : MonoBehaviour
{
    [Header("Prefab to instantiate")]
    [SerializeField] private GameObject replayObjectPrefab;
    public Queue<ReplayData> recordingQueue { get; private set; }

    public List<Recording> recordings;

    private bool isDoingReplay = false;
    private bool replayed = false;
    private bool recording = false;

    private void Awake()
    {
        recordingQueue = new Queue<ReplayData>();
        recordings = new List<Recording>();
    }

    private void Update()
    {
        //Press R to start  and stop recording player
        if (Input.GetKeyDown(KeyCode.R) && !isDoingReplay)
        {
            recording = !recording;
            if (recording) { Debug.Log("Start recording player"); }
            else { Debug.Log("Stop recording player"); }
        }

        if (!recording)
        {
            //Add record to the list
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Added record to the list");
                AddRecording();
            }

            //start the replay
            if (Input.GetKeyDown(KeyCode.Alpha2) && !isDoingReplay)
            {
                if (!replayed)
                {
                    Debug.Log("Starting replay");
                    StartReplay();
                }
                else
                {
                    Debug.Log("Restarting replay");
                    RestartReplay();
                }
            }

            //Destroy replay objects
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                DestroyInstances();
            }

            //Destroy all replay data
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("Reset list of replays");
                ResetReplay();
            }
        }
        if (!isDoingReplay)
        {
            return;
        }
        bool hasMoreFrames = false, someHaveMoreFrames = false;
        foreach(Recording recording in recordings)
        {
            hasMoreFrames = recording.PlayNextFrame();
            if (hasMoreFrames)
            {
                someHaveMoreFrames = true;
            }
        }
        if(!someHaveMoreFrames)
        {
            isDoingReplay = false;
        }
    }

    private void LateUpdate()
    {
        if (!recording) { return; }
        //record replay data for this frame
        ReplayData data = new PlayerReplayData(this.transform.position);
        RecordRaplayFrame(data);
    }

    private void DestroyInstances()
    {
        isDoingReplay = false;
        foreach (Recording recording in recordings)
        {
            recording.DestroyReplayObjectIfExists();
        }
    }

    public void RecordRaplayFrame(ReplayData data)
    {
        recordingQueue.Enqueue(data);
    }

    void StartReplay()
    {
        if (recordings.Count == 0) return;
        isDoingReplay = true;
        //instantiate the replay objects
        replayed = true;
        foreach (Recording recording in recordings)
        {
            recording.IstantiateReplayObject(replayObjectPrefab);
        }
    }

    private void RestartReplay()
    {
        if (recordings.Count == 0) return;
        isDoingReplay = true;
        foreach (Recording recording in recordings)
        {
            recording.RestartFromBeginning();
        }
    }

    private void ResetReplay()
    {
        DestroyInstances();
        recordingQueue.Clear();
        recordings.Clear();
        replayed = false;
    }

    public void AddRecording()
    {
        if (recordingQueue.Count == 0) return;

        //initialize the recording
        recordings.Add(new Recording(recordingQueue));
        //reset the current recording queue for next time
        recordingQueue.Clear();
    }
}

