//-----------------------------------------------------------------------
// <copyright file="Globals.cs" company="StrikeByte">
//     StrikeByte Software, Inc. 2011-
// </copyright>
//-----------------------------------------------------------------------
namespace Pickler
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Class to hold global values.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Personal Preference")]
    public class Globals
    {
        /// <summary>
        /// Global definition of the config file name.
        /// </summary>
        private static string configFileValue = "pickler.config.user";

        /// <summary>
        /// Global Keyword values list
        /// </summary>
        private static string[] keywordsList =
            new string[] { @"And", @"But", @"Given", @"Then", @"Transform", @"When" };

        /// <summary>
        /// Global Reserved wrodk values list
        /// </summary>
        private static string[] reservedWordsList =
            new string[] { @"Background:", @"Examples:", @"Feature:", @"Scenario:", @"Scenario Outline:" };

        /// <summary>
        /// Gets the config file name.
        /// </summary>
        public static string ConfigFile
        {
            get
            {
                return configFileValue;
            }
        }

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        public static string[] Keywords
        {
            get
            {
                return keywordsList;
            }
        }

        /// <summary>
        /// Gets the reserved words.
        /// </summary>
        public static string[] Reserved
        {
            get
            {
                return reservedWordsList;
            }
        }    
    }
}
