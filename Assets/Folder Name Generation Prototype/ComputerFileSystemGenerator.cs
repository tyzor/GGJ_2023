using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComputerFileSystemGenerator : MonoBehaviour
{
    [SerializeField] private int stageDifficulty = 10;
    [SerializeField] private int stageMaxDepth = 0;

    [SerializeField] private int minFolders = 1;
    [SerializeField] private int maxFolders = 3;

    [SerializeField] private int minFiles = 1;
    [SerializeField] private int maxFiles = 5;

    [SerializeField] private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();
    [SerializeField] private List<Folder> dungeonFolders = new List<Folder>();
    [SerializeField] private List<File> dungeonFiles = new List<File>();

    public List<DungeonRoom> DungeonRooms() { return dungeonRooms; }
    public List<Folder> DungeonFolders() { return dungeonFolders; }
    public List<File> DungeonFiles() { return dungeonFiles; }

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
"exe",
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
"zip"};


    void Awake()
    {
        // test making x maps
        for (int i = 0; i < 1; i++)
        {
            // create computer map
            CreateComputerMap();
        }
    }

    private void CreateComputerMap()
    {

        dungeonRooms.Clear();
        dungeonFolders.Clear();
        dungeonFiles.Clear();
        // create the root folder
        Folder rootFolder = new Folder(null, "ROOT");
        CreateDungeonRoom(rootFolder);

        // create a debug for what was created
        string debugString = "Dungeon created with [" + dungeonRooms.Count + "] ROOMS, [" + dungeonFolders.Count + "] FOLDERS, [" + dungeonFiles.Count + "] FILES\n";

        foreach (DungeonRoom r in dungeonRooms)
        {
            foreach (File f in r.Files())
            {
                debugString += f.GetAbsolutePath() + "\n";
            }
            foreach (Folder f in r.Folders())
            {
                debugString += f.GetAbsolutePath() + "\n";
            }
        }

        Debug.Log(debugString);

    }

    private void CreateDungeonRoom(Folder thisRoomsFolder)
    {

        // check depth so we don't go infinitely deep
        int depth = 0;
        Folder parentFolder = thisRoomsFolder.ParentFolder;
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
        if (depth < stageMaxDepth)
        {
            numSubfoldersToCreate = Random.Range(minFolders, maxFolders + 1);
        }
        // make sure the root folder has at least one subfolder
        numSubfoldersToCreate = (depth == 0) ? Mathf.Clamp(numSubfoldersToCreate, 1, numSubfoldersToCreate) : numSubfoldersToCreate;

        // determine number of files to create
        int numFilesToCreate = Random.Range(minFiles, maxFiles + 1);


        // create subfolders
        Folder[] roomSubfolders = new Folder[numSubfoldersToCreate];
        for (int i = 0; i < numSubfoldersToCreate; i += 1)
        {
            // create folder
            string newFolderName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
            Folder newSubfolder = new Folder(thisRoomsFolder, newFolderName);
            dungeonFolders.Add(newSubfolder);
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

        DungeonRoom newRoom = new DungeonRoom(roomIndex, roomName, roomSubfolders, roomFiles, depth);
        dungeonRooms.Add(newRoom);

        // now create a dungeon room for each subfolder
        foreach (Folder subfolder in roomSubfolders)
        {
            CreateDungeonRoom(subfolder);
        }


    }

}

public class DungeonRoom
{

    private int _index;
    private string _roomName;
    private Folder[] _folders;
    private File[] _files;
    private int _depth;

    public File[] Files() { return _files; }
    public Folder[] Folders() { return _folders; }

    public string RoomName
    {
        get { return _roomName; }
    }

    public DungeonRoom(int index, string roomName, Folder[] folders, File[] files, int depth)
    {
        _index = index;
        _roomName = roomName;
        _folders = folders;
        _files = files;
        _depth = depth;
    }

    public string ToString()
    {
        string s = "";
        foreach (Folder f in _folders)
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

public class Folder
{
    private Folder _parentFolder;
    private string _folderName;

    public Folder ParentFolder
    {
        get { return _parentFolder; }
        set { _parentFolder = value; }
    }
    public string FolderName
    {
        get { return _folderName; }
        set { _folderName = value; }
    }

    public Folder(Folder parentFolder, string folderName)
    {
        _parentFolder = parentFolder;
        _folderName = folderName;
    }

    public string GetAbsolutePath()
    {
        string s = "";

        Folder p = _parentFolder;

        List<Folder> path = new List<Folder>();

        do
        {
            path.Add(p);
            p = p.ParentFolder;
        } while (p != null);

        path.Reverse();

        foreach (Folder f in path)
        {
            s += "\\" + f.FolderName;
        }

        s += "\\" + FolderName;

        return s;
    }
}

public class File
{
    private Folder _parentFolder;
    private string _fileName;
    private string _fileExtension;

    public string GetFileNameExtension()
    {
        return _fileName + "." + _fileExtension;
    }

    public File(Folder parentFolder, string fileName, string fileExtension)
    {
        _parentFolder = parentFolder;
        _fileName = fileName;
        _fileExtension = fileExtension;
    }

    public string GetAbsolutePath()
    {
        string s = "";

        Folder p = _parentFolder;

        List<Folder> path = new List<Folder>();

        do
        {
            path.Add(p);
            p = p.ParentFolder;
        } while (p != null);

        path.Reverse();

        foreach (Folder f in path)
        {
            s += "\\" + f.FolderName;
        }

        s += "\\" + GetFileNameExtension();

        return s;
    }
}