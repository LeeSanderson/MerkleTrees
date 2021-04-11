namespace MerkleTrees.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    public class MerkleTree
    {
        private MerkleTreeNode[] leafNodes;

        public MerkleTree(byte[] bytes, int pieceSize = 1024)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0)
                throw new ArgumentException("Must have more that 1 byte in bytes array", nameof(bytes));
            if (pieceSize < 1)
                throw new ArgumentException("piece size must be more that 1", nameof(pieceSize));

            using (SHA256 hasher = SHA256.Create())
            {
                this.leafNodes = CreateLeafNodes(hasher, bytes, pieceSize).ToArray();
                BuildTreeRecursively(hasher, LeafEnumerator());
            }
        }

        public MerkleTreeNode Root { get; private set; }

        public int PieceCount => this.leafNodes.Length;

        public MerkleTreeNode this[int index] => this.leafNodes[index];

        public IEnumerable<byte[]> GetProof(int piece)
        {
            var leaf = this[piece];
            while (leaf.Parent != null)
            {
                yield return leaf.Sibling.Hash;
                leaf = leaf.Parent;
            }
        }

        private IEnumerable<MerkleTreeNode> CreateLeafNodes(HashAlgorithm hasher, byte[] bytes, int pieceSize)
        {
            var len = bytes.Length;
            Memory<byte> memory = bytes;
            for (var i = 0; i < len; i += pieceSize)
            {
                yield return new MerkleTreeNode
                {
                    Hash = hasher.ComputeHash(bytes, i, Math.Min(pieceSize, len - i)),
                    Content = memory.Slice(i, Math.Min(pieceSize, len - i))
                };
            }
        }

        private void BuildTreeRecursively(HashAlgorithm hasher, IEnumerable<MerkleTreeNode> layer)
        {
            List<MerkleTreeNode> nextLayer = new List<MerkleTreeNode>();

            using (var enumerator = layer.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var left = enumerator.Current;

                    if (!enumerator.MoveNext())
                        throw new ArgumentException("Number of nodes is layer must be divisibly by 2");

                    var right = enumerator.Current;

                    var parent = new MerkleTreeNode { Left = left, Right = right };
                    left.Parent = parent;
                    right.Parent = parent;

                    parent.Hash = hasher.ComputeHash(left.Hash.Concat(right.Hash).ToArray());
                    nextLayer.Add(parent);
                }
            }

            if (nextLayer.Count > 1)
            {
                BuildTreeRecursively(hasher, nextLayer);
            }
            else
            {
                Root = nextLayer[0];
            }
        }

        /// <summary>
        /// Get the initial set of leaf nodes that are recursively paired to form the tree
        /// This method always returns a number of nodes equal to a power of 2 so we can
        /// build a perfectly balanced tree. Any extra filler nodes are create will a 0 hash.
        /// </summary>
        private IEnumerable<MerkleTreeNode> LeafEnumerator()
        {
            var nodes = RoundToPowerOf2(this.leafNodes.Length);
            if (nodes < 2)
            {
                // Handle edge case where only 1 node.
                nodes = 2;
            }

            foreach (var node in this.leafNodes)
                yield return node;

            for (int i = this.leafNodes.Length; i < nodes; i++)
                yield return new MerkleTreeNode { Hash = new byte[32] };
        }

        private static int RoundToPowerOf2(int v)
        {
            // https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }
    }
}