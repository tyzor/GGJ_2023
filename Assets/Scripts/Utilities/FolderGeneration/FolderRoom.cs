using System.Collections.Generic;
using GGJ.Levels;

namespace GGJ.Utilities.FolderGeneration
{
    public class FolderRoom
    {
        private readonly int _roomLayoutIndex;
        private Room _roomTemplate;
        private readonly string _titleName;
        private readonly FolderStub[] _subfolders;
        private readonly File[] _files;
        
        //Meta Data
        private readonly int _folderRoomListIndex; // randall
        private readonly FolderStub _parentStub; // randall

        public int FolderRoomListIndex => _folderRoomListIndex;
        public int RoomLayoutIndex => _roomLayoutIndex;
        public Room RoomTemplate => _roomTemplate;
        public string FolderName => _titleName;
        public FolderStub ParentStub => _parentStub;
        public FolderRoom ParentFolder { get; set; }

        public File[] Files => _files;
        public FolderStub[] Subfolders => _subfolders;

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
}