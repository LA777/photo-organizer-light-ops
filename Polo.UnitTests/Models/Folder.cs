using System.Collections.Generic;

namespace Polo.UnitTests.Models
{
    public class Folder
    {
        public string Name { get; set; }
        public List<Folder> SubFolders { get; set; }
        public List<FotoFile> Files { get; set; }

        public Folder()
        {
            SubFolders = new List<Folder>();
            Files = new List<FotoFile>();
        }
    }
}
