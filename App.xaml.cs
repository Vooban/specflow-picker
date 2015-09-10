//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="StrikeByte">
//     StrikeByte Software, Inc. 2011-
// </copyright>
//-----------------------------------------------------------------------
namespace Pickler
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets or sets the parmeters.
        /// </summary>
        /// <value>
        /// The parms for the application.
        /// </value>
        public static string[] Parms { get; set; }

        /// <summary>
        /// Handles the Startup event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0) 
            {
                Parms = new string[e.Args.Length];
                Parms = e.Args; 
            } 
        }
    }
}
