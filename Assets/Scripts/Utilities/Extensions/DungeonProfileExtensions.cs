using System;
using System.Collections.Generic;
using GGJ.Levels;
using Random = UnityEngine.Random;

namespace GGJ.Utilities.Extensions
{
    //TODO This is a WIP
    public static class DungeonProfileExtensions
    {
        public static void GenerateDungeon(this DungeonProfile dungeonProfile, in Room rootRoom, in Room[] rooms)
        {
            var outList = new List<RoomConnectionData>();
            var flatRoomArray = new RoomConnectionData[dungeonProfile.roomCount]; 
            
            var roomOptions = new List<int>(rooms.Length);
            for (int i = 0; i < rooms.Length; i++)
            {
                roomOptions.Add(i);   
            }
            
            //------------------------------------------------//
            
            var rootConnections = rootRoom.MaxFolderCount;

        }

        private static RoomConnectionData SetupRoom(in DungeonProfile dungeonProfile, int roomIndex, in Room room, int depth)
        {
            if (depth + 1 > dungeonProfile.maxDepth)
                return new RoomConnectionData()
                {
                    roomIndex = roomIndex,
                    depth = depth,
                    subRooms = null
                };;
            
            //Need to use +1 since Random.Range<int> is exclusive of max
            var connectionCount = Random.Range(0, room.MaxFolderCount + 1);
            var outRoomConnection = new RoomConnectionData()
            {
                roomIndex = roomIndex,
                depth = depth,
                subRooms = new RoomConnectionData[connectionCount]
            };

            for (int i = 0; i < connectionCount; i++)
            {
                //TODO Need to get the room & Room index
                outRoomConnection.subRooms[i] = SetupRoom(dungeonProfile, default, default, depth + 1);
            }

            throw new NotImplementedException();
        }
    }
}