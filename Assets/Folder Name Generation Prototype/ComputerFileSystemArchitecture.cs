using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComputerFileSystemArchitecture
{
    /*
    private void CreateComputerMap()
    {

        dungeonRooms.Clear();
        dungeonFolders.Clear();
        dungeonFiles.Clear();
        // create the root folder
        Folder rootFolder = new Folder(null, "ROOT");
        CreateDungeonRoom(rootFolder);

        // create a debug for what was created
        string debugString = "Dungeon created with [" + dungeonRooms.Count + "] ROOMS, [" + dungeonFolders.Count + "] SUBFOLDERS, [" + dungeonFiles.Count + "] FILES\n";

        foreach (DungeonRoom r in dungeonRooms)
        {
            foreach (File f in r.Files())
            {
                debugString += f.GetAbsolutePath() + "\n";
            }
            foreach (Folder f in r.Folders())
            {
                //Debug.Log(f.FolderName);
                debugString += f.GetAbsolutePath() + "\n";
            }
        }

        Debug.Log(debugString);

    }
    */

    //private static void CreateDungeonRoom(Folder thisRoomsFolder)
    //{

    //    // check depth so we don't go infinitely deep
    //    int depth = 0;
    //    Folder parentFolder = thisRoomsFolder.ParentFolder;
    //    while (parentFolder != null)
    //    {
    //        parentFolder = parentFolder.ParentFolder;
    //        depth += 1;
    //    }

    //    //Debug.Log("Depth = " + depth);

    //    // create room navigation objects
    //    int roomIndex = dungeonRooms.Count;
    //    string roomName = thisRoomsFolder.FolderName;

    //    // determine number of subfolders to create
    //    //int numSubfoldersToCreate = (depth >= stageMaxDepth) ? 0 : Random.Range(minFolders, maxFolders + 1);
    //    int numSubfoldersToCreate = 0;
    //    int curRooms = dungeonRooms.Count + numSubfoldersToCreate;
    //    if (depth < stageMaxDepth && curRooms < maxRoomCount)
    //    {
    //        numSubfoldersToCreate = Random.Range(minFolders, maxFolders + 1);
    //    }
    //    // make sure the root folder has at least one subfolder
    //    numSubfoldersToCreate = (depth == 0) ? Mathf.Clamp(numSubfoldersToCreate, 1, numSubfoldersToCreate) : numSubfoldersToCreate;

    //    // determine number of files to create
    //    int numFilesToCreate = Random.Range(minFiles, maxFiles + 1);


    //    // create subfolders
    //    Folder[] roomSubfolders = new Folder[numSubfoldersToCreate];
    //    // also check that you have not reached max room count
    //    for (int i = 0; i < numSubfoldersToCreate; i += 1)
    //    {
    //        // create folder
    //        string newFolderName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
    //        Folder newSubfolder = new Folder(thisRoomsFolder, newFolderName);
    //        dungeonFolders.Add(newSubfolder);
    //        roomSubfolders[i] = newSubfolder;
    //    }


    //    // create files
    //    File[] roomFiles = new File[numFilesToCreate];
    //    for (int i = 0; i < numFilesToCreate; i += 1)
    //    {
    //        // create file
    //        string newFileName = availableFolderFileNames[Random.Range(0, availableFolderFileNames.Length)];
    //        string newFileExtension = availableFileExtensions[Random.Range(0, availableFileExtensions.Length)];
    //        File newFile = new File(thisRoomsFolder, newFileName, newFileExtension);
    //        dungeonFiles.Add(newFile);
    //        roomFiles[i] = newFile;
    //    }

    //    //DungeonRoom newRoom = new DungeonRoom(roomIndex, roomName, roomSubfolders, roomFiles, depth);
    //    DungeonRoom newRoom = new DungeonRoom(roomIndex, thisRoomsFolder, roomSubfolders, roomFiles, depth);
    //    dungeonRooms.Add(newRoom);

    //    // now create a dungeon room for each subfolder
    //    foreach (Folder subfolder in roomSubfolders)
    //    {
    //        CreateDungeonRoom(subfolder);
    //    }


    //}
}


//public class DungeonRoom
//{
//    // merge folder and dungeon room
//    // room name can be derived from folder name?


//    private int _fileSystemRoomIndex;
//    private int _roomLayoutIndex;
//    private Folder _selfFolder;
//    private Folder[] _subfolders;
//    private File[] _files;
//    private int _depth;

//    public File[] Files() { return _files; }
//    public Folder[] Folders() { return _subfolders; }

//    public Folder SelfFolder() { return _selfFolder; }

//    public DungeonRoom(int index, Folder selfFolder, Folder[] subfolders, File[] files, int depth)
//    {
//        _fileSystemRoomIndex = index;
//        _selfFolder = selfFolder;
//        _subfolders = subfolders;
//        _files = files;
//        _depth = depth;
//    }

//    public string ToString()
//    {
//        string s = "";
//        foreach (Folder f in _subfolders)
//        {
//            s += f.GetAbsolutePath() + "\n";
//        }

//        foreach (File f in _files)
//        {
//            s += f.GetAbsolutePath() + "\n";
//        }

//        return s;
//    }
//}

//public class Folder
//{
//    private Folder _parentFolder;
//    private string _folderName;

//    public Folder ParentFolder
//    {
//        get { return _parentFolder; }
//        set { _parentFolder = value; }
//    }
//    public string FolderName
//    {
//        get { return _folderName; }
//        set { _folderName = value; }
//    }

//    public Folder(Folder parentFolder, string folderName)
//    {
//        _parentFolder = parentFolder;
//        _folderName = folderName;
//    }

//    public string GetAbsolutePath()
//    {
//        string s = "Folder - ";

//        Folder p = _parentFolder;

//        List<Folder> path = new List<Folder>();

//        do
//        {
//            path.Add(p);
//            p = p.ParentFolder;
//        } while (p != null);

//        path.Reverse();

//        foreach (Folder f in path)
//        {
//            s += "\\" + f.FolderName;
//        }

//        s += "\\" + FolderName;

//        return s;
//    }
//}

//public  class File
//{
//    private Folder _parentFolder;
//    private string _fileName;
//    private string _fileExtension;

//    public string GetFileNameExtension()
//    {
//        return _fileName + "." + _fileExtension;
//    }

//    public File(Folder parentFolder, string fileName, string fileExtension)
//    {
//        _parentFolder = parentFolder;
//        _fileName = fileName;
//        _fileExtension = fileExtension;
//    }

//    public string GetAbsolutePath()
//    {
//        string s = "<File> - ";

//        Folder p = _parentFolder;

//        List<Folder> path = new List<Folder>();

//        do
//        {
//            path.Add(p);
//            p = p.ParentFolder;
//        } while (p != null);

//        path.Reverse();

//        foreach (Folder f in path)
//        {
//            s += "\\" + f.FolderName;
//        }

//        s += "\\" + GetFileNameExtension();

//        return s;
//    }
//}