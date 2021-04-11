namespace MerkleTrees.Tests
{
    using MerkleTrees.Core;
    using System.Linq;
    using Xunit;

    public class TestMerkleTreeProofs
    {
        [Fact]
        public void WhenOneNodeThenProofConsistsOfFillerHash()
        {
            // Arrange / Act
            var tree = new MerkleTree(new byte[] { 0x1 });
            var proofs = tree.GetProof(0).ToList();

            // Assert
            Assert.Single(proofs);
            Assert.Equal(TestConstants.FillerNodeHash, proofs[0].ToHexString());
        }

        [Fact]
        public void WhenThreeNodesProofConsistsOfSiblingAndUncle()
        {
            // Arrange / Act
            var tree = new MerkleTree(new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }, 2);
            var proofs = tree.GetProof(1).ToList();

            // Assert
            Assert.Equal(2, proofs.Count);
            Assert.Equal(tree[0].Hash.ToHexString(), proofs[0].ToHexString()); // Sibling
            Assert.Equal(tree.Root.Right.Hash.ToHexString(), proofs[1].ToHexString()); // Uncle
        }

        [Fact]
        public void WhenTreeIsDeepProofConsistsOfExpectedSiblingAndUncles()
        {
            // Arrange
            // - ~17K = 17 leaves + 15 filler = 32 nodes = 2^5 = 5 layers
            byte[] bytes = Enumerable.Range(0, 17000).Select(i => (byte)i).ToArray();

            // Act
            var tree = new MerkleTree(bytes);
            var proofs = tree.GetProof(14).ToList();

            // Assert
            Assert.Equal(5, proofs.Count);
            Assert.Equal(tree[15].Hash.ToHexString(), proofs[0].ToHexString()); // Sibling
            Assert.Equal(tree.Root.Left.Right.Right.Left.Hash.ToHexString(), proofs[1].ToHexString()); // Uncle
            Assert.Equal(tree.Root.Left.Right.Left.Hash.ToHexString(), proofs[2].ToHexString()); // Uncle Syed
            Assert.Equal(tree.Root.Left.Left.Hash.ToHexString(), proofs[3].ToHexString()); // Uncle Fester
            Assert.Equal(tree.Root.Right.Hash.ToHexString(), proofs[4].ToHexString()); // Monkey's Uncle
        }

    }
}
