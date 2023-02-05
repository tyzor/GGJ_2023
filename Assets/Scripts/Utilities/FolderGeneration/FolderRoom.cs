using System.Collections.Generic;
using GGJ.Levels;

namespace GGJ.Utilities.FolderGeneration
{
    public class FolderRoom
    {
        private readonly int _roomLayoutIndex;
        private Room _roomTemplate;
        private readonly string _titleName;
        private readonly FolderStub[] _childStubs;
        private File[] _files;
        
        //Meta Data
        private readonly int _folderRoomListIndex; // randall
        private readonly FolderStub _parentStub; // randall

        public int FolderRoomListIndex => _folderRoomListIndex;
        public int RoomLayoutIndex => _roomLayoutIndex;
        public Room RoomTemplate => _roomTemplate;
        public string FolderName => _titleName;
        public FolderStub ParentStub => _parentStub;
        public FolderRoom ParentFolder { get; set; }
        public FolderRoom[] Subfolders { get; set; }

        public File[] Files => _files;
        public FolderStub[] ChildStubs => _childStubs;

        public FolderRoom(int folderRoomCount, int roomLayoutIndex, string folderName, FolderStub parentFolder, FolderStub[] childStubs, File[] files)
        {
            _folderRoomListIndex = folderRoomCount;
            _roomLayoutIndex = roomLayoutIndex;
            _titleName = folderName;
            _parentStub = parentFolder;
            _childStubs = childStubs;
            _files = files;
        }

        public int Depth()
        {
            int i = 0;
            while(ParentFolder != null)
            {
                i += 1;
            }
            return i;
        }

        // return string with folder's path from root
        public string GetAbsolutePath()
        {
            string s = string.Empty;

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

            //s += "\\" + FolderName;

            return s;
        }

        public void ClearFiles()
        {
            _files = default;
        }
    }
}