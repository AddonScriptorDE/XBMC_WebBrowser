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
    public partial class FormContextMenu : Form
    {
        public FormContextMenu()
        {
            InitializeComponent();
            ListBoxEntry listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Enter URL";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Show Keyboard";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Show Magnifier";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Show Favourites";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Show Shortcuts";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Double Click";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Press TAB";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Press ESC";
            listBoxMenu.Items.Add(listBoxEntry);
            listBoxEntry = new ListBoxEntry();
            listBoxEntry.title = "Press F5";
            listBoxMenu.Items.Add(listBoxEntry);
            if (listBoxMenu.Items.Count > 0)
                listBoxMenu.SelectedIndex = 0;
            int height = 110;
            if (listBoxMenu.Items.Count > 1)
                height = listBoxMenu.Items.Count * 55 + 50;
            if (height >= 720)
                height = 720;
            this.Size = new Size(this.Size.Width, height);
        }

        private void listBoxMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                listBoxMenu.SelectedIndex = (listBoxMenu.SelectedIndex + listBoxMenu.Items.Count - 1) % listBoxMenu.Items.Count;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                listBoxMenu.SelectedIndex = (listBoxMenu.SelectedIndex + 1) % listBoxMenu.Items.Count;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
                this.Close();
        }
    }
}
