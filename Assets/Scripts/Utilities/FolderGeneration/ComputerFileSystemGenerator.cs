using GGJ.Levels;
using System.Collections.Generic;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;

namespace GGJ.Utilities.Extensions
{
    public static class ComputerFileSystemGeneratorExtension
    {
        private static List<FolderRoom> folderRoomsList;
        private static List<FolderStub> folderStubsList;
        private static List<File> filesList;
        private static List<int> usedRoomIndexes;

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

        public static (FolderRoom root, List<FolderRoom> allFolders) GenerateFolderStructure(this DungeonProfile dungeonProfile, in Room[] rooms)
        {
            folderRoomsList = new List<FolderRoom>();
            folderStubsList = new List<FolderStub>();
            filesList = new List<File>();
            usedRoomIndexes = new List<int>(); // not used properly
            CreateFolderRoomMap(dungeonProfile, rooms);

            List<FolderRoom> folderList = new List<FolderRoom>();

            foreach (FolderRoom f in folderRoomsList)
            {
                folderList.Add(f);
            }

            return (folderRoomsList[0], folderList);
        }

        private static void CreateFolderRoomMap(DungeonProfile dungeonProfile, Room[] rooms)
        {
            // clear local list of rooms, folders, files
            folderRoomsList.Clear();
            folderStubsList.Clear();
            filesList.Clear();
            usedRoomIndexes.Clear();


            // create list to hold all rooms
            List<FolderRoom> folderList = new List<FolderRoom>();

            // create the root folder - and nested rooms recursively
            FolderStub rootFolder = new FolderStub(null, "ROOT");
            folderStubsList.Add(rootFolder);
            CreateFolderRoom(rootFolder, dungeonProfile, rooms);

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

                i += 1;
            }

            // print debug log to console
            PrintFileSystem();

            // verify each folder room has the correct data for its parent
            //string debugString = "";
            //foreach (FolderRoom f in folderRoomsList)
            //{
            //    if (f.ParentFolder != null)
            //    {
            //        debugString += f.FolderName + " has parent [" + f.ParentFolder.FolderName + "]\n";
            //    }
            //}
            //Debug.Log(debugString);
        }

        // print debug to console
        private static void PrintFileSystem()
        {
            // create a debug for what was created
            string debugString = "Dungeon created with [" + folderRoomsList.Count + "] ROOMS, [" +
                                 folderStubsList.Count + "] SUBFOLDERS, [" + filesList.Count + "] FILES\n";

            foreach (FolderRoom r in folderRoomsList)
            {
                foreach (File f in r.Files)
                {
                    debugString += f.GetAbsolutePath() + "\n";
                }

                foreach (FolderStub f in r.Subfolders)
                {
                    //Debug.Log(f.FolderName);
                    debugString += f.GetAbsolutePath() + "\n";
                }
            }

            Debug.Log(debugString);
        }

        // create a room
        private static void CreateFolderRoom(FolderStub folderStub, DungeonProfile dungeonProfile, Room[] rooms)
        {
            // select a room template for this room
            int roomTemplateIndex = Random.Range(0, rooms.Length);
            Room roomTemplate = rooms[roomTemplateIndex];

            // check depth so we don't go infinitely deep
            int depth = 0;
            FolderStub parentFolder = folderStub.ParentFolder;
            while (parentFolder != null)
            {
                parentFolder = parentFolder.ParentFolder;
                depth += 1;
            }

            // create room navigation objects
            int roomIndex = folderRoomsList.Count;
            string folderName = folderStub.FolderName;

            // determine number of subfolders to create
            //int numSubfoldersToCreate = (depth >= stageMaxDepth) ? 0 : Random.Range(minFolders, maxFolders + 1);
            int numSubfoldersToCreate = 0;
            int curRooms = folderRoomsList.Count + numSubfoldersToCreate;
            if (depth < dungeonProfile.maxDepth && curRooms < dungeonProfile.roomCount)
            {
                //numSubfoldersToCreate = Random.Range(minFolders, maxFolders + 1);
                numSubfoldersToCreate = Random.Range(0, roomTemplate.MaxFolderCount + 1);
            }

            // make sure the root folder has at least one subfolder
            numSubfoldersToCreate = (depth == 0)
                ? Mathf.Clamp(numSubfoldersToCreate, 1, numSubfoldersToCreate)
                : numSubfoldersToCreate;

            // determine number of files to create
            int numFilesToCreate = Random.Range(0, dungeonProfile.maxFiles + 1);


            // create subfolders
            FolderStub[] roomSubfolders = new FolderStub[numSubfoldersToCreate];
            // also check that you have not reached max room count
            for (int i = 0; i < numSubfoldersToCreate; i += 1)
            {
                // create folder
                string newFolderName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
                FolderStub newSubfolder = new FolderStub(folderStub, newFolderName);
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
                CreateFolderRoom(subfolder, dungeonProfile, rooms);
            }
        }

    }
}