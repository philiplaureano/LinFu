using System.Collections.Generic;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a class that can list the files
    /// in a given directory.
    /// </summary>
    public interface IDirectoryListing
    {
        /// <summary>
        /// Returns the names of the files in the specified directory
        /// that match the specified search pattern.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of the files in the <paramref name="path"/>.</param>
        /// <returns>The list of files that match the <paramref name="path"/> and <paramref name="searchPattern"/></returns>
        IEnumerable<string> GetFiles(string path, string searchPattern);
    }
}