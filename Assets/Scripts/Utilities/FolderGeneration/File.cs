using System.Collections.Generic;

namespace GGJ.Utilities.FolderGeneration
{
    // file contains its name and containing folder
    public class File
    {
        private readonly FolderStub _parentFolder;
        private readonly string _fileName;
        private readonly string _fileExtension;

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