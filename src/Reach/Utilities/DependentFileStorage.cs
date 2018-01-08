using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reach
{
    /// <summary>
    /// DependentFileStorage manages an area on the local disk to store file dependencies for the current workspace.
    /// </summary>
    /// <remarks>
    /// Expectation is that the app will clear storage at session restart, but the storage will be maintained throughout a single session.
    /// </remarks>
    public static class DependentFileStorage
    {
        /// <summary>
        /// Local path for the root of the storage area.
        /// </summary>
        public static string StorageRootPath {
            get
            {
                return Path.Combine(Path.GetTempPath(), "ReachExecutionDependencies");
            }
        }

        /// <summary>
        /// Create the storage area if it does not already exist, returning the path.
        /// </summary>
        public static string Create()
        {
            if (!Directory.Exists(StorageRootPath))
            {
                Directory.CreateDirectory(StorageRootPath);
            }
            return StorageRootPath;
        }

        /// <summary>
        /// Erase the storage area and all its contents.
        /// </summary>
        public static void Clear()
        {
            if (File.Exists(StorageRootPath))
            {
                File.Delete(StorageRootPath);
            }
            else if (Directory.Exists(StorageRootPath))
            {
                Directory.Delete(StorageRootPath, true);
            }
        }

        /// <summary>
        /// Attempt to erase the storage area and all its contents; return false if the operation failed for any reason.
        /// </summary>
        public static bool TryClear()
        {
            try
            {
                Clear();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Create a folder in the local storage for this home workspace and returns it.
        /// </summary>
        public static string CreateLocalStorageForWorkspace(Guid homeWorkspaceId)
        {
            string path = Create();
            path = Path.Combine(path, homeWorkspaceId.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary>
        /// Returns a local path for a dependent file attached to one or more nodes.
        /// </summary>
        /// <remarks>
        /// nodeIds must contain at least one node Guid.
        /// originalFilePath must be a non-null, non-empty string.
        /// </remarks>
        public static string CreateLocalPathForDependentFile(Guid homeWorkspaceId, IEnumerable<Guid> nodeIds, string originalFilePath)
        {
            if (nodeIds.Count() == 0)
            {
                throw new ArgumentException("NodeId array cannot be empty.");
            }
            if (originalFilePath == null || originalFilePath.Length == 0)
            {
                throw new ArgumentException("originalFilePath cannot be empty.");
            }
            string nodePath = Path.Combine(CreateLocalStorageForWorkspace(homeWorkspaceId), nodeIds.First().ToString());
            if (!Directory.Exists(nodePath))
            {
                Directory.CreateDirectory(nodePath);
            }

            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string sanitizedPath = Regex.Replace(originalFilePath, "[" + invalidChars + "\\\\/]", "_", RegexOptions.None);
            return Path.Combine(nodePath, sanitizedPath);
        }
    }
}
