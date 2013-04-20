using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XBMC_WebBrowser
{
    public partial class FormShortcuts : Form
    {
        private FormKeyboard formKeyboard;
        private ArrayList allKeys;
        private String userDataFolder;
        private String mainTitle;
        private String currentUrl;
        private String mainUrl;

        public FormShortcuts(String userDataFolder, String mainTitle, String mainUrl, String currentUrl, ArrayList allKeys)
        {
            InitializeComponent();
            this.mainUrl = mainUrl;
            this.currentUrl = currentUrl;
            this.allKeys = allKeys;
            this.mainTitle = mainTitle;
            this.userDataFolder = userDataFolder;
            importShortcuts();
        }

        public void importShortcuts()
        {
            listBoxFavs.Items.Clear();
            if (Directory.Exists(userDataFolder))
            {
                ListBoxEntry entryMain = new ListBoxEntry();
                entryMain.title = mainTitle;
                entryMain.url = mainUrl;
                listBoxFavs.Items.Add(entryMain);
                String shortcutFolder = userDataFolder + "\\shortcuts";
                if (!Directory.Exists(shortcutFolder))
                {
                    Directory.CreateDirectory(shortcutFolder);
                }
                else
                {
                    String filename = userDataFolder + "\\shortcuts\\" + mainTitle + ".links";
                    if (File.Exists(filename))
                    {
                        StreamReader str = new StreamReader(filename);
                        String line;
                        while ((line = str.ReadLine()) != null)
                        {
                            if (line.Contains("="))
                            {
                                String entry = line.Substring(0, line.IndexOf("="));
                                String content = line.Substring(line.IndexOf("=") + 1);
                                ListBoxEntry listBoxEntry = new ListBoxEntry();
                                listBoxEntry.title = entry.Trim();
                                listBoxEntry.url = content.Trim();
                                listBoxFavs.Items.Add(listBoxEntry);
                            }
                        }
                        str.Close();
                    }
                }
                ListBoxEntry entryNew = new ListBoxEntry();
                entryNew.title = "- Add Current URL";
                listBoxFavs.Items.Add(entryNew);
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
            {
                if (((ListBoxEntry)listBoxFavs.SelectedItem).title == "- Add Current URL")
                {
                    formKeyboard = new FormKeyboard("Enter shortcut title:", "", true, allKeys);
                    formKeyboard.ShowDialog();
                    if (formKeyboard.textBox1.Text != "")
                    {
                        String title = formKeyboard.textBox1.Text;
                        File.AppendAllText(userDataFolder + "\\shortcuts\\" + mainTitle + ".links", title + "=" + currentUrl + "\n");
                        importShortcuts();
                    }
                }
                else
                    this.Close();
            }
        }
    }
}
