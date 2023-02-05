using GGJ.Levels;
using System.Collections.Generic;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using Cinemachine;

namespace GGJ.Utilities.Extensions
{
    public static class ComputerFileSystemGeneratorExtension
    {
        private static int _runningRoomCount;
        private static List<FolderRoom> _folderRoomsList;
        private static List<FolderStub> _folderStubsList;
        private static List<File> _filesList;
        private static List<int> _usedRoomIndexes;
        private static List<string> _usedFolderNames;
        private static List<string> _usedFileNames;

        private static readonly string[] AvailableFolderFileNames = new string[]
        {
"Action",
"Assets",
"Audio",
"Banned",
"Boot",
"Build",
"Cache",
"Client",
"Codec",
"Common",
"Config",
"Content",
"CSS",
"Cursors",
"Data",
"Debug",
"Desc",
"Drivers",
"DVD",
"Engine",
"Errors",
"EULA",
"Extensions",
"Flash",
"Fonts",
"Games",
"Global",
"Graphics",
"Help",
"Images",
"Initialize",
"Input",
"Interface",
"Internal",
"Java",
"Language",
"Launcher",
"Level",
"Libs",
"License",
"Local",
"Logs",
"Manager",
"Media",
"Metadata",
"Model",
"Mods",
"Module",
"Options",
"Packages",
"Part",
"Player",
"Plugins",
"Printer",
"Readme",
"Registry",
"Runtime",
"Saves",
"Schemas",
"Script",
"Security",
"Server",
"Settings",
"Setup",
"Shader",
"Shell",
"Sounds",
"Splash",
"Start",
"Stats",
"Streaming",
"Symbols",
"Tasks",
"Temp",
"Textures",
"Toolbar",
"UI",
"Users",
"Utilities",
"Web",
"Workshop",
        };

        private static readonly string[] AvailableFileExtensions = new string[]
        {
"avi",
"bat",
"cur",
"db",
"dll",
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
        };

        public static FolderRoom GenerateFolderStructure(this DungeonProfile dungeonProfile, in Room[] rooms)
        {
            _folderRoomsList = new List<FolderRoom>();
            _folderStubsList = new List<FolderStub>();
            _filesList = new List<File>();
            _usedRoomIndexes = new List<int>(); // not used properly
            _usedFolderNames = new List<string>();
            _usedFileNames = new List<string>();


            int maxIterations = 50;
            int iterationCount = 0;
            do
            {
                CreateFolderRoomMap(dungeonProfile, rooms, iterationCount);
                iterationCount += 1;
            } while (iterationCount < maxIterations
                     && _folderRoomsList.Count - 1 < dungeonProfile.roomCount);
            //Debug.Log("Generation iterations = " + iterationCount);

            _folderRoomsList[0].ClearFiles();
            return _folderRoomsList[0];
        }

        private static void CreateFolderRoomMap(DungeonProfile dungeonProfile, Room[] rooms, int iterationCount)
        {
            // clear local list of rooms, folders, files
            _runningRoomCount = 0;
            _folderRoomsList.Clear();
            _folderStubsList.Clear();
            _filesList.Clear();
            _usedRoomIndexes.Clear();
            _usedFolderNames.Clear();
            _usedFileNames.Clear();


            // create list to hold all rooms
            List<FolderRoom> folderList = new List<FolderRoom>();

            // create the root folder - and nested rooms recursively
            FolderStub rootFolder = new FolderStub(null, "ROOT");
            _folderStubsList.Add(rootFolder);
            //runningRoomCount += 1;
            CreateFolderRoom(rootFolder, dungeonProfile, rooms, iterationCount);

            // need to iterate back through the FolderRooms and use the FolderStub indexes to properly set the parentFolders
            int i = 0;
            foreach (FolderRoom f in _folderRoomsList)
            {
                // find appropriate index
                int index = f.FolderRoomListIndex;
                FolderStub folderStub = _folderStubsList[index];
                int parentStubIndex = _folderStubsList.IndexOf(folderStub.ParentFolder);

                // using the index, apply the parent FolderRoom to this folderRooom
                if (parentStubIndex > -1)
                {
                    f.ParentFolder = _folderRoomsList[parentStubIndex];
                }

                // populate list of subfolder stubs
                List<int> childFolderStubIndexList = new List<int>();
                foreach (FolderStub childStub in f.ChildStubs)
                {
                    // get index of child stub
                    int indexOfChildStub = _folderStubsList.IndexOf(childStub);
                    childFolderStubIndexList.Add(indexOfChildStub);
                }

                List<FolderRoom> childFolderRooms = new List<FolderRoom>();
                foreach (int listIndex in childFolderStubIndexList)
                {
                    childFolderRooms.Add(_folderRoomsList[listIndex]);
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
            string debugString = "Dungeon created with [" + (_folderRoomsList.Count - 1) + "] ROOMS, [" +
                                 (_folderStubsList.Count - 1) + "] SUBFOLDERS, [" + _filesList.Count + "] FILES\n";

            foreach (FolderRoom r in _folderRoomsList)
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
        private static void CreateFolderRoom(FolderStub folderStub, DungeonProfile dungeonProfile, Room[] rooms,
            int iterationCount)
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
            int curRooms = _runningRoomCount;
            if (depth < dungeonProfile.maxDepth && curRooms < dungeonProfile.roomCount)
            {
                //numSubfoldersToCreate = Random.Range(minFolders, maxFolders + 1);
                numSubfoldersToCreate1 = Random.Range(Random.Range(0, iterationCount), roomTemplate.MaxFolderCount + 1);


                int remainingRoomsNeeded = dungeonProfile.roomCount - (curRooms); // + numSubfoldersToCreate1);
                numSubfoldersToCreate2 =
                    Mathf.Clamp(numSubfoldersToCreate1, numSubfoldersToCreate1, remainingRoomsNeeded);
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
                string newFolderName = AvailableFolderFileNames[Random.Range(0, AvailableFolderFileNames.Length)];
                // check this name hasn't been used
                int maxNameIterations = 10;
                int nameInteration = 0;
                while (_usedFolderNames.Contains(newFolderName) && nameInteration < maxNameIterations)
                {
                    //Debug.Log(newFolderName + " already used");
                    newFolderName = AvailableFolderFileNames[Random.Range(0, AvailableFolderFileNames.Length)];
                    nameInteration += 1;
                }

                ;
                _usedFolderNames.Add(newFolderName);
                FolderStub newSubfolder = new FolderStub(folderStub, newFolderName);
                _runningRoomCount += 1;
                //folderStubsList.Add(newSubfolder);
                roomSubfolders[i] = newSubfolder;
            }


            // create files
            File[] roomFiles = new File[numFilesToCreate];
            for (int i = 0; i < numFilesToCreate; i += 1)
            {
                // create file
                string newFileName = AvailableFolderFileNames[Random.Range(0, AvailableFolderFileNames.Length)];
                string newFileExtension = AvailableFileExtensions[Random.Range(0, AvailableFileExtensions.Length)];

                string combinedName = newFileName + "." + newFileExtension;
                // check this name hasn't been used
                int maxNameIterations = 10;
                int nameInteration = 0;
                while (_usedFileNames.Contains(combinedName) && nameInteration < maxNameIterations)
                {
                    //Debug.Log(combinedName + " already used");
                    newFileName = AvailableFolderFileNames[Random.Range(0, AvailableFolderFileNames.Length)];
                    newFileExtension = AvailableFileExtensions[Random.Range(0, AvailableFileExtensions.Length)];
                    combinedName = newFileName + "." + newFileExtension;
                    nameInteration += 1;
                }

                ;
                _usedFileNames.Add(combinedName);

                File newFile = new File(folderStub, newFileName, newFileExtension);
                _filesList.Add(newFile);
                roomFiles[i] = newFile;
            }

            int folderRoomCount = _folderRoomsList.Count;
            FolderRoom newRoom = new FolderRoom(folderRoomCount, roomTemplateIndex, folderName, folderStub,
                roomSubfolders, roomFiles, depth);
            _folderRoomsList.Add(newRoom);

            // now create a dungeon room for each subfolder
            foreach (FolderStub subfolder in roomSubfolders)
            {
                _folderStubsList.Add(subfolder);
                CreateFolderRoom(subfolder, dungeonProfile, rooms, iterationCount);
            }

        }

    }
}