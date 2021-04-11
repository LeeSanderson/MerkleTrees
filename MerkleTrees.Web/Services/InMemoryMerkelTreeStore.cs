namespace MerkleTrees.Web.Services
{
    using MerkleTrees.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InMemoryMerkelTreeStore : IMerkelTreeStore
    {
        private readonly Dictionary<string, FileMerkleTree> trees = new Dictionary<string, FileMerkleTree>();

        public Task AddAsync(FileMerkleTree tree)
        {
            trees[tree.Root.Hash.ToHexString()] = tree;
            return Task.CompletedTask;
        }

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async IAsyncEnumerable<FileMerkleTree> AllAsync()
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            foreach (var value in trees.Values)
                yield return value;
        }

        public Task<FileMerkleTree> GetByRootHashAsync(string hash)
        {
            trees.TryGetValue(hash, out var tree);
            return Task.FromResult(tree);
        }
    }
}
