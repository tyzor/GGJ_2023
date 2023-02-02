using System;
using System.Collections.Generic;
using GGJ.Levels;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ.Utilities.Extensions
{
    //TODO This is a WIP
    public static class DungeonProfileExtensions
    {
        public static RoomConnectionData GenerateDungeon(this DungeonProfile dungeonProfile, in Room rootRoom, in Room[] rooms)
        {
            //var flatRoomList = new List<RoomConnectionData>(dungeonProfile.roomCount); 
            return SetupFirstRoom(dungeonProfile, rootRoom, in rooms);
        }

        //============================================================================================================//
        private static RoomConnectionData SetupFirstRoom(in DungeonProfile dungeonProfile, Room rootRoom, in Room[] rooms)
        {
            //If any rooms cannot repeat, we need to remove them from the list of options
            //------------------------------------------------//
            var roomIndexOptions = new List<int>(rooms.Length);
            for (int i = 0; i < rooms.Length; i++)
            {
                roomIndexOptions.Add(i);   
            }
            //------------------------------------------------//
            
            int roomCount = 0;
            //Need to use +1 since Random.Range<int> is exclusive of max
            var connectionCount = Random.Range(1, rootRoom.MaxFolderCount + 1);

            //------------------------------------------------//
            var rootRoomConnectionData = new RoomConnectionData
            {
                roomIndex = -1,
                depth = 0,
                availableConnections = rootRoom.MaxFolderCount - connectionCount,
                subRooms = new RoomConnectionData[connectionCount]
            };

            for (int i = 0; i < connectionCount; i++)
            {
                var randomRoomIndex = roomIndexOptions[Random.Range(0, roomIndexOptions.Count)];

                rootRoomConnectionData.subRooms[i] = SetupRoom(
                    in dungeonProfile, 
                    randomRoomIndex, 
                    1, 
                    in rooms, 
                    ref roomIndexOptions,
                    ref roomCount);
            }

            
            //------------------------------------------------//
            int timeoutCounter = 0;
            while (roomCount < dungeonProfile.roomCount)
            {
                //Debug.Log($"Only generated {roomCount} of {dungeonProfile.roomCount}. Filling remaining...");
                FillRemainingRooms(in dungeonProfile,
                    rootRoomConnectionData,
                    rootRoom,
                    0,
                    rooms,
                    ref roomIndexOptions,
                    ref roomCount);

                if (++timeoutCounter >= 1000)
                    throw new TimeoutException();
            }
            //------------------------------------------------//
            //Debug.Log($"{roomCount} rooms generated");

            return rootRoomConnectionData;
        }
        private static RoomConnectionData SetupRoom(in DungeonProfile dungeonProfile, int roomIndex, int depth, in Room[] rooms, 
            ref List<int> roomIndexOptions, 
            ref int roomCount,
            int minConnection = 0)
        {
            var room = rooms[roomIndex];

            if (room.CannotRepeat)
                roomIndexOptions.Remove(roomIndex);

            if (depth >= dungeonProfile.maxDepth)
            {
                roomCount++;
                return new RoomConnectionData
                {
                    roomIndex = roomIndex,
                    availableConnections = 0,
                    depth = depth,
                    subRooms = null
                };
            }
            
            roomCount++;
            //------------------------------------------------//
            var outRoomConnection = new RoomConnectionData
            {
                roomIndex = roomIndex,
                depth = depth,
            };

            //Need to use +1 since Random.Range<int> is exclusive of max
            var connectionCount = Random.Range(minConnection, room.MaxFolderCount + 1);

            //If the connections go beyond the max room allowance, set the connections to the remaining allowed
            if (roomCount + connectionCount > dungeonProfile.roomCount)
                connectionCount = dungeonProfile.roomCount - roomCount;

            outRoomConnection.availableConnections = room.MaxFolderCount - connectionCount;
            outRoomConnection.subRooms = new RoomConnectionData[connectionCount];
            
            //------------------------------------------------//
            
            for (int i = 0; i < connectionCount; i++)
            {
                if (roomCount >= dungeonProfile.roomCount)
                    return outRoomConnection;
                
                var randomRoomIndex = roomIndexOptions[Random.Range(0, roomIndexOptions.Count)];
                
                //TODO Need to get the room & Room index
                outRoomConnection.subRooms[i] = SetupRoom(
                    in dungeonProfile, 
                    randomRoomIndex, 
                    depth + 1, 
                    in rooms, 
                    ref roomIndexOptions,
                    ref roomCount);
            }

            return outRoomConnection;
        }
        
        //============================================================================================================//

        private static void FillRemainingRooms(in DungeonProfile dungeonProfile, in RoomConnectionData roomConnectionData, in Room room, in int depth, in Room[] rooms, ref List<int> roomIndexOptions, ref int roomCount)
        {
            if (depth >= dungeonProfile.maxDepth)
                return;
            if (roomCount == dungeonProfile.roomCount)
                return;
            
            if (roomConnectionData.availableConnections > 0)
            {
                var temp = new RoomConnectionData[room.MaxFolderCount];
                Array.Copy(roomConnectionData.subRooms, temp, roomConnectionData.subRooms.Length);
                roomConnectionData.subRooms = temp;

                for (int i = 0; i < room.MaxFolderCount; i++)
                {
                    if (roomConnectionData.subRooms[i] != null)
                        continue;
                    
                    if (roomCount == dungeonProfile.roomCount)
                        return;
                    
                    var randomRoomIndex = roomIndexOptions[Random.Range(0, roomIndexOptions.Count)];
                    roomConnectionData.subRooms[i] = SetupRoom(
                        in dungeonProfile, 
                        randomRoomIndex, 
                        depth + 1, 
                        in rooms, 
                        ref roomIndexOptions,
                        ref roomCount,
                        1);
                }
            }
            else
            {
                for (int i = 0; i < roomConnectionData.subRooms.Length; i++)
                {
                    try
                    {
                        FillRemainingRooms(dungeonProfile, 
                            roomConnectionData.subRooms[i], 
                            rooms[roomConnectionData.subRooms[i].roomIndex],
                            depth + 1,
                            in rooms,
                            ref roomIndexOptions, 
                            ref roomCount);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }
    }
}