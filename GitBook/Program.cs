using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Threading;

namespace GitBook
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
            Application.Run(new Form1());
        }
    }
}
