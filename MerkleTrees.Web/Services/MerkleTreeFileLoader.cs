
namespace MerkleTrees.Web.Services
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Scan a given folder and initialize the merkle tree store when app starts.
    /// Could be extended to watch directory/file for changes.
    /// </summary>
    public class MerkleTreeFileLoader : IHostedService
    {
        private readonly IMerkelTreeStore store;
        private readonly IHostEnvironment hostEnvironment;
        private readonly ILogger<MerkleTreeFileLoader> logger;

        public MerkleTreeFileLoader(IMerkelTreeStore store, IHostEnvironment hostEnvironment, ILogger<MerkleTreeFileLoader> logger)
        {
            this.store = store;
            this.hostEnvironment = hostEnvironment;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                string rootPath = ProgramArgs.CommandLineFileName;
                if (string.IsNullOrEmpty(rootPath) || (!File.Exists(rootPath) && !Directory.Exists(rootPath)))
                {
                    logger.LogInformation("Loading files from default path wwwroot/Files");
                    rootPath = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot", "Files");
                }

                FileAttributes attr = File.GetAttributes(rootPath);

                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (var file in Directory.GetFiles(rootPath))
                    {
                        await store.AddAsync(new FileMerkleTree(Path.GetFileName(file), File.ReadAllBytes(file)));
                    }
                }
                else
                {
                    // File
                    if (File.Exists(rootPath))
                    {
                        await store.AddAsync(new FileMerkleTree(Path.GetFileName(rootPath), File.ReadAllBytes(rootPath)));
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to load hashes from file system");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
