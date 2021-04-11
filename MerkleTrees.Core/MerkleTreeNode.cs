namespace MerkleTrees.Core
{
    using System;

    public class MerkleTreeNode
    {
        public byte[] Hash { get; set; }

        public Memory<byte> Content { get; set; }

        public MerkleTreeNode Parent { get; set; }

        public MerkleTreeNode Left { get; set; }

        public MerkleTreeNode Right { get; set; }

        public MerkleTreeNode Sibling
        {
            get
            {
                if (Parent == null)
                    return null;

                return Parent.Left == this ? Parent.Right : Parent.Left;
            }
        }
    }
}