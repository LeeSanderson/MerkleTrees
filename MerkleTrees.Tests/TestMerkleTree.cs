namespace MerkleTrees.Tests
{
    using MerkleTrees.Core;
    using Xunit;

    public class TestMerkleTree
    {        
        [Fact]
        public void WhenOneNodeThenTreeConsistsOfRootNodeAndFiller()
        {
            // Arrange / Act
            var tree = new MerkleTree(new byte[] { 0x1 });

            // Assert
            Assert.Equal(1, tree.PieceCount);
            Assert.NotNull(tree.Root);
            Assert.NotNull(tree.Root.Left);
            Assert.NotNull(tree.Root.Right);
            Assert.Equal(tree[0], tree.Root.Left);
            Assert.NotEqual(TestConstants.FillerNodeHash, tree.Root.Left.Hash.ToHexString());
            Assert.Equal(TestConstants.FillerNodeHash, tree.Root.Right.Hash.ToHexString());
            Assert.Equal(new byte[] { 0x1 }, tree[0].Content.ToArray());
        }

        [Fact]
        public void WhenTwoNodesThenTreeConsistsOfRootAndNodes()
        {
            // Arrange / Act
            var tree = new MerkleTree(new byte[] { 0x1, 0x2 }, 1);

            // Assert
            Assert.Equal(2, tree.PieceCount);
            Assert.NotNull(tree.Root);
            Assert.NotNull(tree.Root.Left);
            Assert.NotNull(tree.Root.Right);
            Assert.Equal(tree[0], tree.Root.Left);
            Assert.Equal(tree[1], tree.Root.Right);
            Assert.NotEqual(TestConstants.FillerNodeHash, tree.Root.Left.Hash.ToHexString());
            Assert.NotEqual(TestConstants.FillerNodeHash, tree.Root.Right.Hash.ToHexString());
            Assert.Equal(new byte[] { 0x1 }, tree[0].Content.ToArray());
            Assert.Equal(new byte[] { 0x2 }, tree[1].Content.ToArray());
        }

        [Fact]
        public void WhenThreeNodesThenTreeConsistsOf2LayersAndFiller()
        {
            // Arrange / Act
            var tree = new MerkleTree(new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }, 2);

            // Assert
            Assert.Equal(3, tree.PieceCount);
            Assert.NotNull(tree.Root);
            Assert.NotNull(tree.Root.Left);
            Assert.NotNull(tree.Root.Left.Left);
            Assert.NotNull(tree.Root.Left.Right);
            Assert.NotNull(tree.Root.Right);
            Assert.NotNull(tree.Root.Right.Left);
            Assert.NotNull(tree.Root.Right.Right);
            Assert.Equal(tree[0], tree.Root.Left.Left);
            Assert.Equal(tree[1], tree.Root.Left.Right);
            Assert.Equal(tree[2], tree.Root.Right.Left);
            Assert.Equal(TestConstants.FillerNodeHash, tree.Root.Right.Right.Hash.ToHexString());
            Assert.Equal(new byte[] { 0x1, 0x2 }, tree[0].Content.ToArray());
            Assert.Equal(new byte[] { 0x3, 0x4 }, tree[1].Content.ToArray());
            Assert.Equal(new byte[] { 0x5 }, tree[2].Content.ToArray());
        }
    }
}
