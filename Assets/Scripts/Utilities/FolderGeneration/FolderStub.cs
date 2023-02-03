using System.Collections.Generic;

namespace GGJ.Utilities.FolderGeneration
{
    // FolderStub is required for recursive generation as a FolderRoom cannot be referenced as a parent until it is created
    public class FolderStub
    {
        public FolderStub ParentFolder { get; set; }

        public string FolderName { get; set; }

        public FolderStub(FolderStub parentFolder, string folderName)
        {
            ParentFolder = parentFolder;
            FolderName = folderName;
        }

        // return string with folder's path from root
        public string GetAbsolutePath()
        {
            string s = "Folder - ";

            FolderStub p = ParentFolder;

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