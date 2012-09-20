using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GitBook
{
    public partial class Form1 : Form
    {
        String nomFichier;
        String SaveName = "";
        String delimStr = "--[";

        public Form1()
        {
            InitializeComponent();
            textBox1.Left = 4;
            textBox1.Top = 0;
                                  
            textBox1.Width = this.Width - 15;
            textBox1.Height = this.Height - 50;
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.AcceptsTab = true;
            textBox1.AcceptsReturn = true;
            textBox1.WordWrap = true;
            textBox1.Dock = DockStyle.Fill;
            textBox1.MaxLength = 0;

            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Count() > 1)
            {
                SaveName = arguments[1].ToString();
                Load_file(SaveName);
            }

            MainMenu leMenu = new MainMenu();
            this.Menu = leMenu;
            MenuItem miFile = leMenu.MenuItems.Add("&File");
            miFile.MenuItems.Add(new MenuItem("&New GitBook...", new EventHandler(this.Create_Clicked)));
            miFile.MenuItems.Add(new MenuItem("&Open your GitBook...", new EventHandler(this.Ouvrir_Clicked)));
            miFile.MenuItems.Add(new MenuItem("Save your GitBook...", new EventHandler(this.EnregistrerSous_Clicked)));
            miFile.MenuItems.Add(new MenuItem("Save your GitBook as...", new EventHandler(this.EnregistrerSous2_Clicked)));
            miFile.MenuItems.Add("-");
            miFile.MenuItems.Add(new MenuItem("Close", new EventHandler(this.Fermer_Clicked)));
            MenuItem miEdit = leMenu.MenuItems.Add("&Edit");
            miEdit.MenuItems.Add(new MenuItem("New Chapter ...", new EventHandler(this.EOC_Clicked)));
            MenuItem miInfo = leMenu.MenuItems.Add("&Info");
            miInfo.MenuItems.Add(new MenuItem("&About ...", new EventHandler(this.Apropos_Clicked)));

            textBox1.MouseWheel += new MouseEventHandler(panel1_MouseWheel);
        }

        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta > 0) {

            }else{

            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                this.SaveDialog();
                return true;
            }

            if (keyData == (Keys.Alt | Keys.E))
            {
                textBox1.Paste(delimStr);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void Fermer_Clicked(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void EOC_Clicked(object sender, System.EventArgs e)
        {
            textBox1.Paste(delimStr);
        }

        private void Apropos_Clicked(object sender, System.EventArgs e)
        {
            MessageBox.Show("GitBook is an open book editor that permits to write collaborative books", "GitBook");
        }

        private void Load_file(string NomFichier)
        {
            int counter = 1;
            String message = "";
            StreamReader sr;

            SaveName = NomFichier;
            this.Text = "GitBook" + " " + SaveName;


            // load description file
            sr = new StreamReader(SaveName);
            while (!sr.EndOfStream){
                sr.ReadLine();
            }
            sr.Close();

            try
            {



                // load data files
                while (true)
                {
                    nomFichier = SaveName + (counter.ToString());
                    sr = new StreamReader(nomFichier);
                    message = message + sr.ReadToEnd();
                    sr.Close();
                    counter++;
                }

            }
            catch (Exception myException)
            {
                textBox1.Text = "Can not read the file : ";
                textBox1.Text = myException.Message;
            }
            textBox1.Text = message;
        }

        protected void Create_Clicked(object sender, System.EventArgs e)
        {
            this.Text = "GitBook";
            textBox1.Text = "";
            SaveName = "";
        }

        protected void Ouvrir_Clicked(object sender, System.EventArgs e)
        {
            OpenFileDialog oFD = new OpenFileDialog();

            //oFD.InitialDirectory = "c:\\";
            oFD.Filter = "File GitBook (*.gbk)|*.gbk";
            oFD.RestoreDirectory = true;

            if (oFD.ShowDialog() == DialogResult.OK)
            {
                nomFichier = oFD.FileName;
                Load_file(nomFichier);
            }
        }
        
        private void Save_File(string FileName) {
            try
            {
                String text = textBox1.Text;
                StreamWriter sw;
                String page = "";

                int counter = 1;
                int pagesize = 100 * 1024;
                int indexofstart = 0;
                int indexofend = 0;
                
                FileInfo fi = new FileInfo(FileName);
                string[] filePaths = Directory.GetFiles(fi.DirectoryName);
                foreach (string filePath in filePaths)
                {
                    if (filePath.Contains(fi.Name))
                    {
                        File.Delete(filePath);
                    }
                }

                sw = new StreamWriter(FileName);
                sw.Write("GitBook Definition: 0.1");
                sw.Close();
                
                SaveName = FileName;
                this.Text = "GitBook" + " " + SaveName;

                while (text.Length > 0)
                {
                    // save can not exceed pagesize
                    if (text.Length > pagesize)
                    {
                        page = text.Substring(0, pagesize);
                    }else{
                        page = text.Substring(0, text.Length);
                    }

                    // begining of file
                    indexofstart = page.IndexOf(delimStr);
                    // end of file (begin of next)
                    indexofend = page.IndexOf(delimStr, indexofstart+delimStr.Length);
                    
                    if (indexofstart > -1)
                    {
                        if (indexofend > -1)
                        {
                            page = page.Substring(indexofstart, indexofend);
                            text = text.Substring(indexofend);
                        }
                        else
                        {
                            page = page.Substring(indexofstart, page.Length);
                            text = text.Substring(page.Length);
                            
                        }
                    } else {
                        text = text.Substring(page.Length, text.Length);
                    }

                    FileName = SaveName + (counter.ToString());
                    sw = new StreamWriter(FileName);
                    sw.Write(page);
                    sw.Close();
                    counter++;
                }
            }
            catch (Exception argh)
            {
                MessageBox.Show(argh.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        protected void SaveDialog()
        {
            if (SaveName != "")
            {
                this.Save_File(SaveName);
            }
            else
            {
                SaveFileDialog sFD = new SaveFileDialog();
                sFD.Filter = "File GitBook (*.gbk)|*.gbk";
                sFD.RestoreDirectory = true;

                if (sFD.ShowDialog() == DialogResult.OK)
                {
                    this.Save_File(sFD.FileName);
                }
            }
        }

        protected void EnregistrerSous_Clicked(object sender, System.EventArgs e)
        {
            this.SaveDialog();
        }

        protected void EnregistrerSous2_Clicked(object sender, System.EventArgs e)
        {
            SaveName = "";
            this.SaveDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
