//-----------------------------------------------------------------------
// <copyright file="MRUENtry.cs" company="StrikeByte">
//     StrikeByte Software, Inc. 2011-
// </copyright>
//-----------------------------------------------------------------------
namespace Pickler
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    /// <summary>
    /// Allows for the manipulation of the MRU (4 items)
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Personal Preference")]
    public class MruEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MruEntry"/> class.
        /// </summary>
        public MruEntry()
        {
            this.Name = "...";
            this.FilePath = "...";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MruEntry"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public MruEntry(string filePath)
        {
            this.FilePath = filePath;
            this.Name = Path.GetFileName(filePath);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the entry
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path of the entry.
        /// </value>
        public string FilePath { get; set; }
    }
}
