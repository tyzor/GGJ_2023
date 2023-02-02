using GGJ.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ComputerFileSystemGenerator;

// review DungeonProfileExtensions for static

public class ComputerFileSystemGenerator : MonoBehaviour
{
    [SerializeField] private int stageDifficulty = 10;
    [SerializeField] private int stageMaxDepth = 2;

    [SerializeField] private int minFolders = 0;
    [SerializeField] private int maxFolders = 3;

    [SerializeField] private int minFiles = 1;
    [SerializeField] private int maxFiles = 2;

    [SerializeField] private int maxRoomCount = 10;

    //[SerializeField] private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();
    [SerializeField] private List<FolderRoom> folderRooms = new List<FolderRoom>();
    [SerializeField] private List<SimpleFolder> simpleFolders = new List<SimpleFolder>();
    [SerializeField] private List<File> dungeonFiles = new List<File>();

    [SerializeField] private List<int> usedRoomIndexes = new List<int>();

    //public List<DungeonRoom> DungeonRooms() { return dungeonRooms; }
    //public List<SimpleFolder> DungeonFolders() { return simpleFolders; }
    //public List<File> DungeonFiles() { return dungeonFiles; }

    private string[] availableFolderFileNames = new string[]{"Boot",
"Cache",
"common",
"config",
"Config",
"css",
"Cursors",
"data",
"debug",
"Drivers",
"DVD",
"Engine",
"Errors",
"Fonts",
"Games",
"Globalization",
"Graphics",
"Help",
"Helper",
"images",
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
"saves",
"schemas",
"security",
"Settings",
"Setup",
"Shaders",
"shell",
"sounds",
"symbols",
"Tasks",
"Temp",
"Textures",
"UI",
"Users",
"Utils",
"Web",
"workshop"};

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

    void Awake()
    {
        //// test making x maps
        //for (int i = 0; i < 1; i++)
        //{
        //    // create computer map
        //    CreateComputerMap();
        //}


        //RandallFunction
    }

    
    public List<FolderRoom> RandallFunction(DungeonProfile dungeonProfile, in Room[] rooms)
    {
        CreateAlexMap(dungeonProfile, rooms);

        List<FolderRoom> returnListOfRooms = new List<FolderRoom>();

        foreach(FolderRoom f in folderRooms)
        {
            returnListOfRooms.Add(f);
        }

        return returnListOfRooms;
    }
    


    private void CreateAlexMap(DungeonProfile dungeonProfile, Room[] rooms)
    {
        // clear local list of rooms, folders, files
        //dungeonRooms.Clear();
        folderRooms.Clear();
        simpleFolders.Clear();
        dungeonFiles.Clear();
        usedRoomIndexes.Clear();

        // create the root folder
        SimpleFolder rootFolder = new SimpleFolder(null, "ROOT");
        CreateAlexDungeonRoom(rootFolder, dungeonProfile, rooms);

        // create a debug for what was created
        string debugString = "Dungeon created with [" + folderRooms.Count + "] ROOMS, [" + simpleFolders.Count + "] SUBFOLDERS, [" + dungeonFiles.Count + "] FILES\n";

        foreach (FolderRoom r in folderRooms)
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

    private void CreateAlexDungeonRoom(SimpleFolder thisRoomsFolder, DungeonProfile dungeonProfile, Room[] rooms)
    {
        // select a room template for this room
        Room roomTemplate = rooms[Random.Range(0, rooms.Length)];

        // check depth so we don't go infinitely deep
        int depth = 0;
        SimpleFolder parentFolder = thisRoomsFolder.ParentFolder;
        while (parentFolder != null)
        {
            parentFolder = parentFolder.ParentFolder;
            depth += 1;
        }

        // create room navigation objects
        int roomIndex = folderRooms.Count;
        string roomName = thisRoomsFolder.FolderName;

        // determine number of subfolders to create
        //int numSubfoldersToCreate = (depth >= stageMaxDepth) ? 0 : Random.Range(minFolders, maxFolders + 1);
        int numSubfoldersToCreate = 0;
        int curRooms = folderRooms.Count + numSubfoldersToCreate;
        if (depth < dungeonProfile.maxDepth && curRooms < dungeonProfile.roomCount)
        {
            numSubfoldersToCreate = Random.Range(minFolders, maxFolders + 1);
        }
        // make sure the root folder has at least one subfolder
        numSubfoldersToCreate = (depth == 0) ? Mathf.Clamp(numSubfoldersToCreate, 1, numSubfoldersToCreate) : numSubfoldersToCreate;

        // determine number of files to create
        int numFilesToCreate = Random.Range(minFiles, maxFiles + 1);


        // create subfolders
        SimpleFolder[] roomSubfolders = new SimpleFolder[numSubfoldersToCreate];
        // also check that you have not reached max room count
        for (int i = 0; i < numSubfoldersToCreate; i += 1)
        {
            // create folder
            string newFolderName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
            SimpleFolder newSubfolder = new SimpleFolder(thisRoomsFolder, newFolderName);
            simpleFolders.Add(newSubfolder);
            roomSubfolders[i] = newSubfolder;
        }


        // create files
        File[] roomFiles = new File[numFilesToCreate];
        for (int i = 0; i < numFilesToCreate; i += 1)
        {
            // create file
            string newFileName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
            string newFileExtension = availableFileExtensions[Random.Range(0, availableFileExtensions.Length)];
            File newFile = new File(thisRoomsFolder, newFileName, newFileExtension);
            dungeonFiles.Add(newFile);
            roomFiles[i] = newFile;
        }

        //DungeonRoom newRoom = new DungeonRoom(roomIndex, roomName, roomSubfolders, roomFiles, depth);
        FolderRoom newRoom = new FolderRoom(roomIndex, thisRoomsFolder, roomSubfolders, roomFiles);
        folderRooms.Add(newRoom);

        // now create a dungeon room for each subfolder
        foreach (SimpleFolder subfolder in roomSubfolders)
        {
            CreateAlexDungeonRoom(subfolder, dungeonProfile, rooms);
        }
    }

    /*
    private void CreateDungeonRoom(SimpleFolder thisRoomsFolder)
    {

        // check depth so we don't go infinitely deep
        int depth = 0;
        SimpleFolder parentFolder = thisRoomsFolder.ParentFolder;
        while (parentFolder != null)
        {
            parentFolder = parentFolder.ParentFolder;
            depth += 1;
        }

        //Debug.Log("Depth = " + depth);

        // create room navigation objects
        int roomIndex = dungeonRooms.Count;
        string roomName = thisRoomsFolder.FolderName;

        // determine number of subfolders to create
        //int numSubfoldersToCreate = (depth >= stageMaxDepth) ? 0 : Random.Range(minFolders, maxFolders + 1);
        int numSubfoldersToCreate = 0;
        int curRooms = dungeonRooms.Count + numSubfoldersToCreate;
        if (depth < stageMaxDepth && curRooms < maxRoomCount)
        {
            numSubfoldersToCreate = Random.Range(minFolders, maxFolders + 1);
        }
        // make sure the root folder has at least one subfolder
        numSubfoldersToCreate = (depth == 0) ? Mathf.Clamp(numSubfoldersToCreate, 1, numSubfoldersToCreate) : numSubfoldersToCreate;

        // determine number of files to create
        int numFilesToCreate = Random.Range(minFiles, maxFiles + 1);


        // create subfolders
        SimpleFolder[] roomSubfolders = new SimpleFolder[numSubfoldersToCreate];
        // also check that you have not reached max room count
        for (int i = 0; i < numSubfoldersToCreate; i += 1)
        {
            // create folder
            string newFolderName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
            SimpleFolder newSubfolder = new SimpleFolder(thisRoomsFolder, newFolderName);
            simpleFolders.Add(newSubfolder);
            roomSubfolders[i] = newSubfolder;
        }


        // create files
        File[] roomFiles = new File[numFilesToCreate];
        for (int i = 0; i < numFilesToCreate; i += 1)
        {
            // create file
            string newFileName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
            string newFileExtension = availableFileExtensions[Random.Range(0, availableFileExtensions.Length)];
            File newFile = new File(thisRoomsFolder, newFileName, newFileExtension);
            dungeonFiles.Add(newFile);
            roomFiles[i] = newFile;
        }

        //DungeonRoom newRoom = new DungeonRoom(roomIndex, roomName, roomSubfolders, roomFiles, depth);
        DungeonRoom newRoom = new DungeonRoom(roomIndex, thisRoomsFolder, roomSubfolders, roomFiles, depth);
        dungeonRooms.Add(newRoom);

        // now create a dungeon room for each subfolder
        foreach (SimpleFolder subfolder in roomSubfolders)
        {
            CreateDungeonRoom(subfolder);
        }


    }
    */

    public class FolderRoom
    {
        private int _roomLayoutIndex;
        private Room _roomTemplate;
        private SimpleFolder _selfFolder;
        private SimpleFolder[] _subfolders;
        private File[] _files;

        public int RoomLayoutIndex() { return _roomLayoutIndex; }
        public Room RoomTemplate() { return _roomTemplate; }
        public File[] Files() { return _files; }
        public SimpleFolder[] Folders() { return _subfolders; }
        public SimpleFolder SelfFolder() { return _selfFolder; }

        public FolderRoom(int roomLayoutIndex, SimpleFolder selfFolder, SimpleFolder[] subfolders, File[] files)
        {
            _roomLayoutIndex = roomLayoutIndex;
            _selfFolder = selfFolder;
            _subfolders = subfolders;
            _files = files;
        }
    }

    public class DungeonRoom
    {
        // merge folder and dungeon room
        // room name can be derived from folder name?


        private int _fileSystemRoomIndex;
        private int _roomLayoutIndex;
        private SimpleFolder _selfFolder;
        private SimpleFolder[] _subfolders;
        private File[] _files;
        private int _depth;

        public File[] Files() { return _files; }
        public SimpleFolder[] Folders() { return _subfolders; }

        public SimpleFolder SelfFolder() { return _selfFolder; }

        public DungeonRoom(int index, SimpleFolder selfFolder, SimpleFolder[] subfolders, File[] files, int depth)
        {
            _fileSystemRoomIndex = index;
            _selfFolder = selfFolder;
            _subfolders = subfolders;
            _files = files;
            _depth = depth;
        }

        public string ToString()
        {
            string s = "";
            foreach (SimpleFolder f in _subfolders)
            {
                s += f.GetAbsolutePath() + "\n";
            }

            foreach (File f in _files)
            {
                s += f.GetAbsolutePath() + "\n";
            }

            return s;
        }
    }

    public class SimpleFolder
    {
        private SimpleFolder _parentFolder;
        private string _folderName;

        public SimpleFolder ParentFolder
        {
            get { return _parentFolder; }
            set { _parentFolder = value; }
        }
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; }
        }

        public SimpleFolder(SimpleFolder parentFolder, string folderName)
        {
            _parentFolder = parentFolder;
            _folderName = folderName;
        }

        public string GetAbsolutePath()
        {
            string s = "Folder - ";

            SimpleFolder p = _parentFolder;

            List<SimpleFolder> path = new List<SimpleFolder>();

            do
            {
                path.Add(p);
                p = p.ParentFolder;
            } while (p != null);

            path.Reverse();

            foreach (SimpleFolder f in path)
            {
                s += "\\" + f.FolderName;
            }

            s += "\\" + FolderName;

            return s;
        }
    }

    public class File
    {
        private SimpleFolder _parentFolder;
        private string _fileName;
        private string _fileExtension;

        public string GetFileNameExtension()
        {
            return _fileName + "." + _fileExtension;
        }

        public File(SimpleFolder parentFolder, string fileName, string fileExtension)
        {
            _parentFolder = parentFolder;
            _fileName = fileName;
            _fileExtension = fileExtension;
        }

        public string GetAbsolutePath()
        {
            string s = "<File> - ";

            SimpleFolder p = _parentFolder;

            List<SimpleFolder> path = new List<SimpleFolder>();

            do
            {
                path.Add(p);
                p = p.ParentFolder;
            } while (p != null);

            path.Reverse();

            foreach (SimpleFolder f in path)
            {
                s += "\\" + f.FolderName;
            }

            s += "\\" + GetFileNameExtension();

            return s;
        }
    }
}