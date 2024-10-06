using System;
[System.Serializable]
public class PlayerCheckpointData
{
    public ulong ClientId;
    public string CheckpointTag;

    public PlayerCheckpointData(ulong clientId, string checkpointTag)
    {
        ClientId = clientId;
        CheckpointTag = checkpointTag;
    }
}


