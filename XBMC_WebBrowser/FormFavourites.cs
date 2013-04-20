using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XBMC_WebBrowser
{
    public partial class FormFavourites : Form
    {
        public FormFavourites(String userDataFolder)
        {
            InitializeComponent();
            if (Directory.Exists(userDataFolder))
            {
                DirectoryInfo dir = new DirectoryInfo(userDataFolder + "\\sites");
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (file.FullName.EndsWith(".link"))
                    {
                        ListBoxEntry listBoxEntry = new ListBoxEntry();
                        StreamReader str = new StreamReader(file.FullName);
                        String line;
                        while ((line = str.ReadLine()) != null)
                        {
                            if (line.Contains("="))
                            {
                                String entry = line.Substring(0, line.IndexOf("="));
                                String content = line.Substring(line.IndexOf("=")+1);
                                if (entry == "title")
                                    listBoxEntry.title = content.Trim();
                                else if (entry == "url")
                                    listBoxEntry.url = content.Trim();
                            }
                        }
                        str.Close();
                        listBoxFavs.Items.Add(listBoxEntry);
                    }
                }
                if (listBoxFavs.Items.Count > 0)
                    listBoxFavs.SelectedIndex = 0;
            }
            int height = 110;
            if (listBoxFavs.Items.Count > 1)
                height = listBoxFavs.Items.Count * 55 + 50;
            if (height >= 720)
                height = 720;
            this.Size = new Size(this.Size.Width, height);
        }

        private void listBoxFavs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                listBoxFavs.SelectedIndex = (listBoxFavs.SelectedIndex + listBoxFavs.Items.Count - 1) % listBoxFavs.Items.Count;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                listBoxFavs.SelectedIndex = (listBoxFavs.SelectedIndex + 1) % listBoxFavs.Items.Count;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
                this.Close();
        }
    }
}
