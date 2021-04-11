namespace MerkleTrees.Web.Controllers
{
    using MerkleTrees.Core;
    using MerkleTrees.Web.Models;
    using MerkleTrees.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly IMerkelTreeStore store;
        private readonly ILogger<HomeController> logger;

        public HomeController(IMerkelTreeStore store, ILogger<HomeController> logger)
        {
            this.store = store;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View(store);
        }

        [HttpGet("hashes")]
        public async Task<IActionResult> Hashes()
        {
            var hashes = new Dictionary<string, int>();
            await foreach (var tree in this.store.AllAsync())
            {
                hashes[tree.Root.Hash.ToHexString()] = tree.PieceCount;
            }

            return Json(hashes.Select(h => new { hash = h.Key, pieces = h.Value }));
        }

        [HttpGet("piece/{hash}/{piece}")]
        public async Task<IActionResult> Piece(string hash, int piece)
        {
            var tree = await this.store.GetByRootHashAsync(hash);
            if (tree == null || piece < 0 || piece >= tree.PieceCount)
                return NotFound();

            var node = tree[piece];
            var proof = tree.GetProof(piece).Select(p => p.ToHexString()).ToArray();

            return Json(new { content = Convert.ToBase64String(node.Content.ToArray()), proof });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
