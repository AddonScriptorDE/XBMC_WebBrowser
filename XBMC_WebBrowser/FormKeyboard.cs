using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace XBMC_WebBrowser
{
    public partial class FormKeyboard : Form
    {
        private ArrayList allKeys;
        private bool specialKeyPressed;
        private String startText;
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        
        public FormKeyboard(String startText, ArrayList allKeys)
        {
            InitializeComponent();
            this.allKeys = allKeys;
            this.startText = startText;
            specialKeyPressed = false;
            textBox1.Text = startText;
            if (startText == "")
                textBox1.ReadOnly = true;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                specialKeyPressed = true;
                buttonEnter.Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                specialKeyPressed = true;
                button0.Focus();
            }
            else if (allKeys.Contains(e.KeyCode.ToString()))
                specialKeyPressed = true;
            else
                specialKeyPressed = false;
            if (e.KeyCode == Keys.Enter)
                this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (specialKeyPressed)
                e.Handled = true;
        }

        private void button_Click(object sender, EventArgs e)
        {
            String text = ((Button)sender).Text.ToLower();
            if (text == "<")
            {
                if (textBox1.SelectionStart > 0)
                {
                    textBox1.SelectionStart--;
                }
            }
            else if (text == ">")
            {
                textBox1.SelectionStart++;
            }
            else if (text == "space")
            {
                textBox1.Text += " ";
                textBox1.SelectionStart = textBox1.Text.Length;
            }
            else if (text == "space")
            {
                textBox1.Text += " ";
                textBox1.SelectionStart = textBox1.Text.Length;
            }
            else if (text == "remove")
            {
                if (textBox1.Text.Length > 0)
                {
                    textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                    textBox1.SelectionStart = textBox1.Text.Length;
                }
            }
            else if (text == "remove all")
            {
                textBox1.Text = startText;
                textBox1.SelectionStart = textBox1.Text.Length;
            }
            else if (text == "enter")
                this.Close();
            else
            {
                textBox1.Text += text;
                textBox1.SelectionStart = textBox1.Text.Length;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    String keys = "";
            //    foreach (int i in Enum.GetValues(typeof(Keys)))
            //    {
            //        if (GetAsyncKeyState(i) == -32767)
            //        {
            //            keys += Enum.GetName(typeof(Keys), i) + " ";
            //        }
            //    }
            //    keys = keys.Trim();
            //    if (keys.StartsWith("ShiftKey "))
            //        keys = keys.Substring(9);
            //    if (keys.StartsWith("Menu "))
            //        keys = keys.Substring(5);

            //    if (keys != "")
            //    {
            //        if (keys == "Down")
            //        {
            //            if (ActiveControl is Button)
            //            {
            //                String[] spl = ((Button)ActiveControl).Tag.ToString().Split(',');
            //                int x = Convert.ToInt32(spl[1]);
            //                int y = Convert.ToInt32(spl[0]);

            //                foreach (Control control in this.Controls)
            //                {
            //                    if (control is Button)
            //                    {
            //                        try
            //                        {
            //                            String[] spl2 = ((Button)control).Tag.ToString().Split(',');
            //                            int xNew = Convert.ToInt32(spl2[1]);
            //                            int yNew = Convert.ToInt32(spl2[0]);
            //                            if ((x == xNew) && ((y + 1) == yNew))
            //                                ((Button)control).Focus();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        if (keys == "Up")
            //        {
            //            if (ActiveControl is Button)
            //            {
            //                String[] spl = ((Button)ActiveControl).Tag.ToString().Split(',');
            //                int x = Convert.ToInt32(spl[1]);
            //                int y = Convert.ToInt32(spl[0]);

            //                foreach (Control control in this.Controls)
            //                {
            //                    if (control is Button)
            //                    {
            //                        try
            //                        {
            //                            String[] spl2 = ((Button)control).Tag.ToString().Split(',');
            //                            int xNew = Convert.ToInt32(spl2[1]);
            //                            int yNew = Convert.ToInt32(spl2[0]);
            //                            if (x == xNew && ((y - 1) == yNew))
            //                                ((Button)control).Focus();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        if (keys == "Left")
            //        {
            //            if (ActiveControl is Button)
            //            {
            //                String[] spl = ((Button)ActiveControl).Tag.ToString().Split(',');
            //                int x = Convert.ToInt32(spl[1]);
            //                int y = Convert.ToInt32(spl[0]);

            //                foreach (Control control in this.Controls)
            //                {
            //                    if (control is Button)
            //                    {
            //                        try
            //                        {
            //                            String[] spl2 = ((Button)control).Tag.ToString().Split(',');
            //                            int xNew = Convert.ToInt32(spl2[1]);
            //                            int yNew = Convert.ToInt32(spl2[0]);
            //                            if (((x + 1) == xNew) && y == yNew)
            //                                ((Button)control).Focus();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        if (keys == "Right")
            //        {
            //            if (ActiveControl is Button)
            //            {
            //                String[] spl = ((Button)ActiveControl).Tag.ToString().Split(',');
            //                int x = Convert.ToInt32(spl[1]);
            //                int y = Convert.ToInt32(spl[0]);

            //                foreach (Control control in this.Controls)
            //                {
            //                    if (control is Button)
            //                    {
            //                        try
            //                        {
            //                            String[] spl2 = ((Button)control).Tag.ToString().Split(',');
            //                            int xNew = Convert.ToInt32(spl2[1]);
            //                            int yNew = Convert.ToInt32(spl2[0]);
            //                            if (((x - 1) == xNew) && y == yNew)
            //                                ((Button)control).Focus();
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch
            //{
            //}
        }

        private void button_Enter(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = Color.Black;
            ((Button)sender).ForeColor = Color.WhiteSmoke;
        }

        private void button_Leave(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = Color.WhiteSmoke;
            ((Button)sender).ForeColor = Color.Black;
        }
    }
}
