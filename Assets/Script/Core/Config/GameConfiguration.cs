using System;

namespace Script.Core.Config
{
    [Serializable]
    public struct GameConfiguration
    {
        // Resources definition
        public string resPath;
        public RawImageResData[] rawImageResData;
        
        public string backgroundImageName;
        
        // Game configuration
        public float rewardPoints;
        public float penaltyPoints;
        public float fragmentRespawnInterval;
        public float duration;
        public int initialFragmentsCount;
        public int targetPlayerMark;
        
        // End game string
        public string endGameMessagesWin;
        public string endGameMessagesLose;
    }

    [Serializable]
    public struct RawImageResData
    {
        public string name;
        public int typeID;
    }
}