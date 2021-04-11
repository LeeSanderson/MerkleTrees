namespace MerkleTrees.Web.Services
{
    using MerkleTrees.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstract interface for Merkel tree store.
    /// Methods defined as asyn for potential future implementation via database etc.
    /// </summary>
    public interface IMerkelTreeStore
    {
        Task<FileMerkleTree> GetByRootHashAsync(string hash);

        Task AddAsync(FileMerkleTree tree);

        IAsyncEnumerable<FileMerkleTree> AllAsync();
    }
}
