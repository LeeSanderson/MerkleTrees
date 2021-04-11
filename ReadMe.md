# Simple Merkle Tree implementation

The algorithm to calculate the merkle hash will follow closely the
bittorrent method of calculating a file's hash as seen in *Simple
Merkle Hashes* in: <http://www.bittorrent.org/beps/bep_0030.html>.

That is:
* The merkle tree will be a perfectly balanced binary tree with a
  filler hash value of 0.
* Hash values in the higher levels of the tree are calculated by
  concatenating the children left to right and computing the hash of
  that aggregate.

The differences being:
* We will use the SHA-256 algorithm rather than SHA1
* We will have a hardcoded piece size of 1KB

## Building, testing and running 

Download source code or clone repo to c:\merkletree

Compile with command: 
> c:\merkletrees dotnet build

Execute tests:
> c:\merkletrees dotnet test

Run:
> c:\merkletrees dotnet run --project .\MerkleTrees.Web\

By default the web application will parse all files in the .\MerkleTrees.Web\wwwroot\Files directory. You can override this behaviour by passing a file or directory on the command line:

> c:\merkletrees dotnet run --project .\MerkleTrees.Web\ c:\path\to\a\file\or\folder

Open a browser:

> https://localhost:5001

The index page will show a list of files and provide links to the API endpoints (see below)

## API Endpoints

### GET /hashes

Should return a json list of the merkle hashes and number of 1KB
pieces of the files this server is serving. In our case this will be a
singleton array.

Example:
```
curl -i -H "Accept: application/json" -H "Content-Type: application/json" -X GET https://localhost:5001/hashes
```
```
[{
  "hash": "9b39e1edb4858f7a3424d5a3d0c4579332640e58e101c29f99314a12329fc60b",
  "pieces": 17
}]
```

### GET /piece/:hashId/:pieceIndex

Should return a verifiable piece of the content.

Parameter   | Description
----------- | -------------
:hashId     | the merkle hash of the file we want to download (in our case there will only be one)
:pieceIndex | the index of the piece we want to download (from zero to 'filesize divided by 1KB')

The returned object will contain two fields:

Field   | Description
------- | -------------
content | The binary content of the piece encoded in base64.
proof   | A list of hashes hex encoded to prove that the piece is legitimate. The first hash will be the hash of the sibling and the next will be the uncle's hash, the next the uncle of the uncle's hash and so on. With this information the client will be able to recalculate the root hash of the tree and compare it to the known root hash. (Please see *Simple Merkle Hashes* in <http://www.bittorrent.org/beps/bep_0030.html> for more thorough discussion)

