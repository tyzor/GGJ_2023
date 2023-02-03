using GGJ.Levels;
using System.Collections.Generic;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using Cinemachine;

namespace GGJ.Utilities.Extensions
{
    public static class ComputerFileSystemGeneratorExtension
    {
        private static int runningRoomCount;
        private static List<FolderRoom> folderRoomsList;
        private static List<FolderStub> folderStubsList;
        private static List<File> filesList;
        private static List<int> usedRoomIndexes;
        private static List<string> usedFolderNames;
        private static List<string> usedFileNames;

        private static readonly string[] availableFolderFileNames = new string[]
        {
            "Boot",
            "Cache",
            "Common",
            "Config",
            "Css",
            "Cursors",
            "Data",
            "Debug",
            "Drivers",
            "DVD",
            "Engine",
            "Errors",
            "Fonts",
            "Games",
            "Globalization",
            "Graphics",
//"Help",
            "Helper",
            "Images",
            "Interface",
            "Interface",
            "Language",
            "libs",
            "Locale",
            "Logs",
            "Media",
            "Options",
            "Packages",
            "Plugins",
            "Printer",
            "ProgramData",
            "Saves",
            "Schemas",
            "Security",
            "Settings",
            "Setup",
            "Shaders",
            "Shell",
            "Sounds",
            "Symbols",
            "Tasks",
            "Temp",
            "Textures",
            "UI",
            "Users",
            "Utils",
            "Web",
            "Workshop"
        };

        private static readonly string[] availableFileExtensions = new string[]
        {
            "avi",
            "bat",
            "cur",
            "db",
            "dll",
//"exe",
            "html",
            "index",
            "info",
            "jar",
            "json",
            "mp3",
            "pdf",
            "png",
            "tmp",
            "txt",
            "xml",
//"zip"
        };

        public static FolderRoom GenerateFolderStructure(this DungeonProfile dungeonProfile, in Room[] rooms)
        {
            folderRoomsList = new List<FolderRoom>();
            folderStubsList = new List<FolderStub>();
            filesList = new List<File>();
            usedRoomIndexes = new List<int>(); // not used properly
            usedFolderNames = new List<string>();
            usedFileNames = new List<string>();

        int maxIterations = 50;
            int iterationCount = 0;
            do
            {
                CreateFolderRoomMap(dungeonProfile, rooms, iterationCount);
                iterationCount += 1;
            } while (iterationCount < maxIterations
                        && folderRoomsList.Count - 1 < dungeonProfile.roomCount);
            //Debug.Log("Generation iterations = " + iterationCount);

            return folderRoomsList[0];
        }

        private static void CreateFolderRoomMap(DungeonProfile dungeonProfile, Room[] rooms, int iterationCount)
        {
            // clear local list of rooms, folders, files
            runningRoomCount = 0;
            folderRoomsList.Clear();
            folderStubsList.Clear();
            filesList.Clear();
            usedRoomIndexes.Clear();
            usedFolderNames.Clear();
            usedFileNames.Clear();


            // create list to hold all rooms
            List<FolderRoom> folderList = new List<FolderRoom>();

            // create the root folder - and nested rooms recursively
            FolderStub rootFolder = new FolderStub(null, "ROOT");
            folderStubsList.Add(rootFolder);
            //runningRoomCount += 1;
            CreateFolderRoom(rootFolder, dungeonProfile, rooms, iterationCount);

            // need to iterate back through the FolderRooms and use the FolderStub indexes to properly set the parentFolders
            int i = 0;
            foreach (FolderRoom f in folderRoomsList)
            {
                // find appropriate index
                int index = f.FolderRoomListIndex;
                FolderStub folderStub = folderStubsList[index];
                int parentStubIndex = folderStubsList.IndexOf(folderStub.ParentFolder);

                // using the index, apply the parent FolderRoom to this folderRooom
                if (parentStubIndex > -1)
                {
                    f.ParentFolder = folderRoomsList[parentStubIndex];
                }

                // populate list of subfolder stubs
                List<int> childFolderStubIndexList = new List<int>();
                foreach(FolderStub childStub in f.ChildStubs)
                {
                    // get index of child stub
                    int indexOfChildStub = folderStubsList.IndexOf(childStub);
                    childFolderStubIndexList.Add(indexOfChildStub);
                }
                List<FolderRoom> childFolderRooms = new List<FolderRoom>();
                foreach (int listIndex in childFolderStubIndexList)
                {
                    childFolderRooms.Add(folderRoomsList[listIndex]);
                }
                f.Subfolders = childFolderRooms.ToArray();

                i += 1;
            }

            // print debug log to console
            PrintFileSystem();
        }

        // print debug to console
        private static void PrintFileSystem()
        {
            // create a debug for what was created
            string debugString = "Dungeon created with [" + (folderRoomsList.Count - 1) + "] ROOMS, [" +
                                 (folderStubsList.Count - 1) + "] SUBFOLDERS, [" + filesList.Count + "] FILES\n";

            foreach (FolderRoom r in folderRoomsList)
            {
                foreach (File f in r.Files)
                {
                    debugString += f.GetAbsolutePath() + "\n";
                }

                foreach (FolderStub f in r.ChildStubs)
                {
                    //Debug.Log(f.FolderName);
                    debugString += f.GetAbsolutePath() + "\n";
                }
            }

            //Debug.Log(debugString);
        }

        // create a room
        private static void CreateFolderRoom(FolderStub folderStub, DungeonProfile dungeonProfile, Room[] rooms, int iterationCount)
        {
            // select a room template for this room
            int roomTemplateIndex = Random.Range(0, rooms.Length);
            Room roomTemplate = rooms[roomTemplateIndex];
            for (int i = 0; i < iterationCount; i++)
            {
                int roomFolderSlots = roomTemplate.MaxFolderCount;
                if (roomFolderSlots == 0)
                {
                    int newIndex = Random.Range(0, rooms.Length);
                    roomTemplate = rooms[newIndex];
                }
            }

            // check depth so we don't go infinitely deep
            int depth = 0;
            FolderStub parentFolder = folderStub.ParentFolder;
            while (parentFolder != null)
            {
                parentFolder = parentFolder.ParentFolder;
                depth += 1;
            }

            // create room navigation objects
            //int roomIndex = folderRoomsList.Count;
            string folderName = folderStub.FolderName;

            // determine number of subfolders to create
            //int numSubfoldersToCreate = (depth >= stageMaxDepth) ? 0 : Random.Range(minFolders, maxFolders + 1);
            int numSubfoldersToCreate1 = 0;
            int numSubfoldersToCreate2 = 0;
            int numSubfoldersToCreate3 = 0;
            //int curRooms = folderRoomsList.Count;
            //int curRooms = folderStubsList.Count;
            int curRooms = runningRoomCount;
            if (depth < dungeonProfile.maxDepth && curRooms < dungeonProfile.roomCount)
            {
                //numSubfoldersToCreate = Random.Range(minFolders, maxFolders + 1);
                numSubfoldersToCreate1 = Random.Range(Random.Range(0, iterationCount), roomTemplate.MaxFolderCount + 1);


                int remainingRoomsNeeded = dungeonProfile.roomCount - (curRooms);// + numSubfoldersToCreate1);
                numSubfoldersToCreate2 = Mathf.Clamp(numSubfoldersToCreate1, numSubfoldersToCreate1, remainingRoomsNeeded);
            }

            // make sure the root folder has at least one subfolder
            numSubfoldersToCreate3 = (depth == 0)
                ? Mathf.Clamp(numSubfoldersToCreate2, 1, numSubfoldersToCreate2)
                : numSubfoldersToCreate2;

            // determine number of files to create
            int numFilesToCreate = Random.Range(0, dungeonProfile.maxFiles + 1);


            // create subfolders
            FolderStub[] roomSubfolders = new FolderStub[numSubfoldersToCreate3];
            // also check that you have not reached max room count
            for (int i = 0; i < numSubfoldersToCreate3; i += 1)
            {
                // create folder
                string newFolderName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
                // check this name hasn't been used
                int maxNameIterations = 10;
                int nameInteration = 0;
                while(usedFolderNames.Contains(newFolderName) && nameInteration < maxNameIterations)
                {
                    //Debug.Log(newFolderName + " already used");
                    newFolderName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
                    nameInteration += 1;
                };
                usedFolderNames.Add(newFolderName);
                FolderStub newSubfolder = new FolderStub(folderStub, newFolderName);
                runningRoomCount += 1;
                //folderStubsList.Add(newSubfolder);
                roomSubfolders[i] = newSubfolder;
            }


            // create files
            File[] roomFiles = new File[numFilesToCreate];
            for (int i = 0; i < numFilesToCreate; i += 1)
            {
                // create file
                string newFileName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
                string newFileExtension = availableFileExtensions[Random.Range(0, availableFileExtensions.Length)];

                string combinedName = newFileName + "." + newFileExtension;
                // check this name hasn't been used
                int maxNameIterations = 10;
                int nameInteration = 0;
                while (usedFileNames.Contains(combinedName) && nameInteration < maxNameIterations)
                {
                    //Debug.Log(combinedName + " already used");
                    newFileName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
                    newFileExtension = availableFileExtensions[Random.Range(0, availableFileExtensions.Length)];
                    combinedName = newFileName + "." + newFileExtension;
                    nameInteration += 1;
                };
                usedFileNames.Add(combinedName);

                File newFile = new File(folderStub, newFileName, newFileExtension);
                filesList.Add(newFile);
                roomFiles[i] = newFile;
            }

            int folderRoomCount = folderRoomsList.Count;
            FolderRoom newRoom = new FolderRoom(folderRoomCount, roomTemplateIndex, folderName, folderStub,
                roomSubfolders, roomFiles);
            folderRoomsList.Add(newRoom);

            // now create a dungeon room for each subfolder
            foreach (FolderStub subfolder in roomSubfolders)
            {
                folderStubsList.Add(subfolder);
                CreateFolderRoom(subfolder, dungeonProfile, rooms, iterationCount);
            }

        }

    }
}