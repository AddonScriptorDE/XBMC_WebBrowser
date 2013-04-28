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
        private Button lastButtonTop, lastButtonBottom;

        public FormKeyboard(String title, String startText, Boolean inputEnabled, ArrayList allKeys)
        {
            InitializeComponent();
            this.labelTitle.Text = title;
            this.startText = startText;
            this.allKeys = allKeys;
            specialKeyPressed = false;
            textBox1.Text = startText;
            lastButtonTop = button0;
            if (!inputEnabled)
            {
                textBox1.ReadOnly = true;
                textBox1.TabStop = false;
            }
            buttonsToLower();
        } 

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Close();
            else if (e.KeyCode == Keys.Up)
                specialKeyPressed = true;
            else if (e.KeyCode == Keys.Down)
            {
                specialKeyPressed = true;
                lastButtonTop.Focus();
            }
            else if (allKeys.Contains(e.KeyCode.ToString()))
                specialKeyPressed = true;
            else
                specialKeyPressed = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (specialKeyPressed)
                e.Handled = true;
        }

        private void button_Click(object sender, EventArgs e)
        {
            String text = ((Button)sender).Text.ToLower();
            if (text == "space")
            {
                textBox1.Text += " ";
                textBox1.SelectionStart = textBox1.Text.Length;
            }
            else if (text == "shift")
            {
                buttonsToUpper();
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
                textBox1.Text += ((Button)sender).Text;
                textBox1.SelectionStart = textBox1.Text.Length;
                buttonsToLower();
            }
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

        private void button_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (textBox1.Text.Length > 0)
                {
                    textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                    textBox1.SelectionStart = textBox1.Text.Length;
                }
            }

            else if (e.KeyCode == Keys.Down)
            {
                if (ActiveControl is Button)
                {
                    String[] spl = ((Button)sender).Tag.ToString().Split(',');
                    int x = Convert.ToInt32(spl[1]);
                    int y = Convert.ToInt32(spl[0]);

                    if (y == 4)
                    {
                        lastButtonBottom = (Button)sender;
                        buttonSpace.Focus();
                    }
                    else if (y == 5)
                        buttonEnter.Focus();

                    else
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control is Button)
                            {
                                try
                                {
                                    String[] spl2 = ((Button)control).Tag.ToString().Split(',');
                                    int xNew = Convert.ToInt32(spl2[1]);
                                    int yNew = Convert.ToInt32(spl2[0]);
                                    if ((x == xNew) && ((y + 1) == yNew))
                                        ((Button)control).Focus();
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (ActiveControl is Button)
                {
                    String[] spl = ((Button)sender).Tag.ToString().Split(',');
                    int x = Convert.ToInt32(spl[1]);
                    int y = Convert.ToInt32(spl[0]);

                    if (y == 0 && textBox1.TabStop)
                    {
                        textBox1.Focus();
                        lastButtonTop = (Button)sender;
                    }
                    if (y == 5)
                        lastButtonBottom.Focus();
                    else if (y == 6)
                        buttonSpace.Focus();
                    else
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control is Button)
                            {
                                try
                                {
                                    String[] spl2 = ((Button)control).Tag.ToString().Split(',');
                                    int xNew = Convert.ToInt32(spl2[1]);
                                    int yNew = Convert.ToInt32(spl2[0]);
                                    if (x == xNew && ((y - 1) == yNew))
                                        ((Button)control).Focus();
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (ActiveControl is Button)
                {
                    String[] spl = ((Button)sender).Tag.ToString().Split(',');
                    int x = Convert.ToInt32(spl[1]);
                    int y = Convert.ToInt32(spl[0]);

                    foreach (Control control in this.Controls)
                    {
                        if (control is Button)
                        {
                            try
                            {
                                String[] spl2 = ((Button)control).Tag.ToString().Split(',');
                                int xNew = Convert.ToInt32(spl2[1]);
                                int yNew = Convert.ToInt32(spl2[0]);
                                int mod = 10;
                                if (y == 5)
                                    mod = 3;
                                if (((x + 1) % mod == xNew) && y == yNew)
                                    ((Button)control).Focus();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (ActiveControl is Button)
                {
                    String[] spl = ((Button)sender).Tag.ToString().Split(',');
                    int x = Convert.ToInt32(spl[1]);
                    int y = Convert.ToInt32(spl[0]);

                    foreach (Control control in this.Controls)
                    {
                        if (control is Button)
                        {
                            try
                            {
                                String[] spl2 = ((Button)control).Tag.ToString().Split(',');
                                int xNew = Convert.ToInt32(spl2[1]);
                                int yNew = Convert.ToInt32(spl2[0]);
                                int mod = 10;
                                if (y == 5)
                                    mod = 3;
                                if (((x + mod - 1) % mod == xNew) && y == yNew)
                                    ((Button)control).Focus();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        private void button_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }

        private void buttonsToLower()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    String[] spl = ((Button)control).Tag.ToString().Split(',');
                    int y = Convert.ToInt32(spl[0]);
                    if (y < 4)
                    {
                        try
                        {
                            ((Button)control).Text = ((Button)control).Text.ToLower();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void buttonsToUpper()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    String[] spl = ((Button)control).Tag.ToString().Split(',');
                    int y = Convert.ToInt32(spl[0]);
                    if (y < 4)
                    {
                        try
                        {
                            ((Button)control).Text = ((Button)control).Text.ToUpper();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}
