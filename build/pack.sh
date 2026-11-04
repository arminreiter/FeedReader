#!/usr/bin/env bash
# Builds and packs the signed NuGet package on Linux. See openssl-sha1.cnf for why.
set -euo pipefail
cd "$(dirname "$0")/.."

export OPENSSL_CONF="$PWD/build/openssl-sha1.cnf"
# The shared compiler server (VBCSCompiler) may already run without OPENSSL_CONF -> signing
# fails. A failed signing run leaves 0-byte DLLs in obj/ that an incremental build would pack,
# so always rebuild from scratch.
dotnet build FeedReader/FeedReader.csproj -c Release --no-incremental -p:UseSharedCompilation=false
dotnet pack FeedReader/FeedReader.csproj -c Release --no-build -o artifacts "$@"
