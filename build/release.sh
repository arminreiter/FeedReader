#!/usr/bin/env bash
# Usage: build/release.sh 2.1.0  -> bumps csproj, commits, tags v2.1.0, pushes.
# The tag push triggers .github/workflows/release.yml (NuGet + GitHub release).
set -euo pipefail
cd "$(dirname "$0")/.."

V="${1:?usage: build/release.sh X.Y.Z}"
[[ "$V" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z.-]+)?$ ]] || { echo "bad version: $V"; exit 1; }
git diff --quiet && git diff --cached --quiet || { echo "working tree not clean, commit first"; exit 1; }

sed -i "s|<Version>.*</Version>|<Version>$V</Version>|" FeedReader/FeedReader.csproj
git commit -m "Release $V" FeedReader/FeedReader.csproj
git tag "v$V"
git push origin HEAD "v$V"
