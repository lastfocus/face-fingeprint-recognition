using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlServerCe;
//using Face_Recognition;
namespace protect_system
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main_menu());
        }
    }
}
