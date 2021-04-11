namespace MerkleTrees.Web.Services
{
    using MerkleTrees.Core;

    public class FileMerkleTree : MerkleTree
    {
        public FileMerkleTree(string fileName, byte[] bytes, int pieceSize = 1024):
            base(bytes, pieceSize)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
    }
}
