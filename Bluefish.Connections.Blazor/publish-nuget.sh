#!/usr/bin/env bash
set -euo pipefail

RELEASE_BRANCH="main"

# This script will publish to nuget using the api key in nuget-api-key.txt in the same folder.
# The api key issued by nuget.org should ideally only have permissions to update a single package
# with new versions.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

API_KEY_FILE="nuget-api-key.txt"
if [[ ! -f "$API_KEY_FILE" ]]; then
    echo "Error: $API_KEY_FILE does not exist" >&2
    exit 1
fi
API_KEY="$(<"$API_KEY_FILE")"

echo "Fetching latest commits..."
git fetch

ORIGINAL_BRANCH="$(git rev-parse --abbrev-ref HEAD)"

cleanup() {
    if [[ "$ORIGINAL_BRANCH" != "$RELEASE_BRANCH" && "$(git rev-parse --abbrev-ref HEAD)" != "$ORIGINAL_BRANCH" ]]; then
        echo "Switching back to $ORIGINAL_BRANCH branch"
        git checkout "$ORIGINAL_BRANCH"
    fi
}
trap cleanup EXIT

if [[ "$ORIGINAL_BRANCH" != "$RELEASE_BRANCH" ]]; then
    read -r -p "Not on $RELEASE_BRANCH branch - merge '$ORIGINAL_BRANCH' into $RELEASE_BRANCH and release? [y/N] " REPLY
    if [[ ! "$REPLY" =~ ^[Yy]$ ]]; then
        echo "ABORTED."
        exit 1
    fi

    echo "Checking out $RELEASE_BRANCH..."
    git checkout "$RELEASE_BRANCH"

    echo "Pulling..."
    git pull

    echo "Merging $ORIGINAL_BRANCH into $RELEASE_BRANCH..."
    git merge "$ORIGINAL_BRANCH" --no-edit

    echo "Pushing $RELEASE_BRANCH..."
    git push
fi

# Build and pack
dotnet build -c Release
dotnet pack -c Release

MOST_RECENT_PACKAGE="$(ls -t bin/Release/*.nupkg | head -n1)"
echo "Publishing $MOST_RECENT_PACKAGE..."
dotnet nuget push "$MOST_RECENT_PACKAGE" --source https://api.nuget.org/v3/index.json --api-key "$API_KEY"

VERSION_STRING="$(basename "$MOST_RECENT_PACKAGE" | grep -oE '[0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z.-]+)?\.nupkg$' | sed 's/\.nupkg$//')"
echo "Finished building version ${VERSION_STRING}."

read -r -p "Create and push tag '${VERSION_STRING}'? [y/N] " REPLY
if [[ ! "$REPLY" =~ ^[Yy]$ ]]; then
    echo "ABORTED."
    exit 1
fi

echo "Adding tag..."
git tag -a "$VERSION_STRING" -m "Tagging version ${VERSION_STRING}"

echo "Pushing tag to origin..."
git push origin "$VERSION_STRING"
