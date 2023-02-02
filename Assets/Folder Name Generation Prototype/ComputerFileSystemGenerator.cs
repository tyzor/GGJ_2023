using GGJ.Levels;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;
using UnityEngine;
using static ComputerFileSystemGenerator;

public class ComputerFileSystemGenerator : MonoBehaviour
{
    //[SerializeField] private int stageDifficulty = 10;
    //[SerializeField] private int stageMaxDepth = 2;
    //[SerializeField] private int minFolders = 0;
    //[SerializeField] private int maxFolders = 3;
    [SerializeField] private int minFiles = 0;
    [SerializeField] private int maxFiles = 2;
    //[SerializeField] private int maxRoomCount = 10;

    [SerializeField] private List<FolderRoom> folderRoomsList = new List<FolderRoom>();
    [SerializeField] private List<FolderStub> folderStubsList = new List<FolderStub>();
    [SerializeField] private List<File> filesList = new List<File>();

    [SerializeField] private List<int> usedRoomIndexes = new List<int>(); // not used properly


    private string[] availableFolderFileNames = new string[]{"Boot",
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
"Workshop"};
    private string[] availableFileExtensions = new string[]{"avi",
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

    [SerializeField] DungeonProfile dungeonProfile;


    public List<FolderRoom> GenerateFolderStructure(DungeonProfile dungeonProfile, in Room[] rooms)
    {
        CreateFolderRoomMap(dungeonProfile, rooms);

        List<FolderRoom> folderList = new List<FolderRoom>();

        foreach (FolderRoom f in folderRoomsList)
        {
            folderList.Add(f);
        }

        return folderList;
    }

    private void CreateFolderRoomMap(DungeonProfile dungeonProfile, Room[] rooms)
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
            if(parentStubIndex > -1)
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
    private void PrintFileSystem()
    {
        // create a debug for what was created
        string debugString = "Dungeon created with [" + folderRoomsList.Count + "] ROOMS, [" + folderStubsList.Count + "] SUBFOLDERS, [" + filesList.Count + "] FILES\n";

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

    /*
    private void CreateComputerMap()
    {
        dungeonRooms.Clear();
        folderRooms.Clear();
        simpleFolders.Clear();
        dungeonFiles.Clear();
        // create the root folder
        SimpleFolder rootFolder = new SimpleFolder(null, "ROOT");
        CreateDungeonRoom(rootFolder);

        // create a debug for what was created
        string debugString = "Dungeon created with [" + dungeonRooms.Count + "] ROOMS, [" + simpleFolders.Count + "] SUBFOLDERS, [" + dungeonFiles.Count + "] FILES\n";

        foreach (DungeonRoom r in dungeonRooms)
        {
            foreach (File f in r.Files())
            {
                debugString += f.GetAbsolutePath() + "\n";
            }
            foreach (SimpleFolder f in r.Folders())
            {
                //Debug.Log(f.FolderName);
                debugString += f.GetAbsolutePath() + "\n";
            }
        }

        Debug.Log(debugString);

    }
    */

    // create a room
    private void CreateFolderRoom(FolderStub folderStub, DungeonProfile dungeonProfile, Room[] rooms)
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
        numSubfoldersToCreate = (depth == 0) ? Mathf.Clamp(numSubfoldersToCreate, 1, numSubfoldersToCreate) : numSubfoldersToCreate;

        // determine number of files to create
        int numFilesToCreate = Random.Range(minFiles, maxFiles + 1);


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
        FolderRoom newRoom = new FolderRoom(folderRoomCount, roomTemplateIndex, folderName, folderStub, roomSubfolders, roomFiles);
        folderRoomsList.Add(newRoom);

        // now create a dungeon room for each subfolder
        foreach (FolderStub subfolder in roomSubfolders)
        {
            folderStubsList.Add(subfolder);
            CreateFolderRoom(subfolder, dungeonProfile, rooms);
        }
    }

    // FolderRoom is the main object with all room connection information in the file system
    public class FolderRoom
    {
        private int _folderRoomListIndex;
        private int _roomLayoutIndex;
        private Room _roomTemplate;
        private string _titleName;
        private FolderRoom _parentFolder;
        private FolderStub _parentStub;
        private FolderStub[] _subfolders;
        private File[] _files;

        public int FolderRoomListIndex { get { return _folderRoomListIndex; } }
        public int RoomLayoutIndex { get { return _roomLayoutIndex; } }
        public Room RoomTemplate { get { return _roomTemplate; } }
        public string FolderName { get { return _titleName; } }
        public FolderStub ParentStub { get { return _parentStub; } }
        public FolderRoom ParentFolder { get { return _parentFolder; } set { _parentFolder = value; } }
        public File[] Files { get { return _files; } }
        public FolderStub[] Subfolders { get { return _subfolders; } }

        public FolderRoom(int folderRoomCount, int roomLayoutIndex, string folderName, FolderStub parentFolder, FolderStub[] subfolders, File[] files)
        {
            _folderRoomListIndex = folderRoomCount;
            _roomLayoutIndex = roomLayoutIndex;
            _titleName = folderName;
            _parentStub = parentFolder;
            _subfolders = subfolders;
            _files = files;
        }

        // return string with folder's path from root
        public string GetAbsolutePath()
        {
            string s = "Folder - ";

            FolderStub p = _parentStub;

            List<FolderStub> path = new List<FolderStub>();

            do
            {
                path.Add(p);
                p = p.ParentFolder;
            } while (p != null);

            path.Reverse();

            foreach (FolderStub f in path)
            {
                s += "\\" + f.FolderName;
            }

            s += "\\" + FolderName;

            return s;
        }
    }

    // FolderStub is required for recursive generation as a FolderRoom cannot be referenced as a parent until it is created
    public class FolderStub
    {
        private FolderStub _parentFolder;
        private string _folderName;

        public FolderStub ParentFolder
        {
            get { return _parentFolder; }
            set { _parentFolder = value; }
        }
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; }
        }

        public FolderStub(FolderStub parentFolder, string folderName)
        {
            _parentFolder = parentFolder;
            _folderName = folderName;
        }

        // return string with folder's path from root
        public string GetAbsolutePath()
        {
            string s = "Folder - ";

            FolderStub p = _parentFolder;

            List<FolderStub> path = new List<FolderStub>();

            do
            {
                path.Add(p);
                p = p.ParentFolder;
            } while (p != null);

            path.Reverse();

            foreach (FolderStub f in path)
            {
                s += "\\" + f.FolderName;
            }

            s += "\\" + FolderName;

            return s;
        }
    }

    // file contains its name and containing folder
    public class File
    {
        private FolderStub _parentFolder;
        private string _fileName;
        private string _fileExtension;

        public string GetFileNameExtension()
        {
            return _fileName + "." + _fileExtension;
        }

        public File(FolderStub parentFolder, string fileName, string fileExtension)
        {
            _parentFolder = parentFolder;
            _fileName = fileName;
            _fileExtension = fileExtension;
        }

        // return string with file's path from root
        public string GetAbsolutePath()
        {
            string s = "<File> - ";

            FolderStub p = _parentFolder;

            List<FolderStub> path = new List<FolderStub>();

            do
            {
                path.Add(p);
                p = p.ParentFolder;
            } while (p != null);

            path.Reverse();

            foreach (FolderStub f in path)
            {
                s += "\\" + f.FolderName;
            }

            s += "\\" + GetFileNameExtension();

            return s;
        }
    }
}