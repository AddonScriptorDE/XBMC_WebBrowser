using System;
using System.IO;
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
    public partial class Form1 : Form
    {
        private int acceleration;
        private int minMouseSpeed;
        private int maxMouseSpeed;
        private long lastMousePositionChange;
        private FormZoom formZoom;
        private FormKeyboard formKeyboardNavi, formKeyboardSearch;
        private FormPopup formPopup;
        private FormCursor formCursor;
        private String userDataFolder;
        private bool showPopups;
        private int zoom;
        private int magnifierWidth;
        private ArrayList allKeys;
        private String keyMapUp, keyMapDown, keyMapLeft, keyMapRight, keyMapUpLeft, keyMapUpRight, keyMapDownLeft, keyMapDownRight, keyMapClick, keyMapDoubleClick, keyMapZoomIn, keyMapZoomOut, keyMapMagnifier, keyMapNavigate, keyMapClose, keyMapKeyboard;
        private const UInt32 MOUSEEVENTF_MOVE = 0x0001;
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        [DllImport("User32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, uint dwExtraInf);
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        
        public Form1(String[] args)
        {
            InitializeComponent();
            lastMousePositionChange = 0;
            
            String url = "http://www.google.de";
            minMouseSpeed = 1;
            maxMouseSpeed = 20;
            userDataFolder = "";
            zoom = 100;
            magnifierWidth = 800;
            showPopups = false;
            if (args.Length > 0)
            {
                userDataFolder = args[0].Replace("\"", "");
                url = Uri.UnescapeDataString(args[1]);
                zoom = Convert.ToInt32(args[2]);
                showPopups = (args[3] == "yes");
                minMouseSpeed = Convert.ToInt32(args[4]);
                maxMouseSpeed = Convert.ToInt32(args[5]);
                magnifierWidth = Convert.ToInt32(args[6]);
            }
            acceleration = minMouseSpeed;
            //When using Windows
            String file = userDataFolder + "\\keymap";
            String file2 = "C:\\xbmc_webbrowser\\keymap";
            if (File.Exists(file)) 
            {
                importKeymap(file);
            }
            //When using Wine
            else if (File.Exists(file2))
            {
                importKeymap(file2);
            }
            //Default key mapping
            else
            {
                keyMapUp = "NumPad8";
                keyMapDown = "NumPad2";
                keyMapLeft = "NumPad4";
                keyMapRight = "NumPad6";
                keyMapUpLeft = "NumPad7";
                keyMapUpRight = "NumPad9";
                keyMapDownLeft = "NumPad1";
                keyMapDownRight = "NumPad3";
                keyMapClose = "NumPad0";
                keyMapMagnifier = "Multiply";
                keyMapNavigate = "Divide";
                keyMapZoomIn = "Add";
                keyMapZoomOut = "Subtract";
                keyMapClick = "NumPad5";
                keyMapDoubleClick = "Decimal";
                keyMapKeyboard = "Pause";
            }
            allKeys = new ArrayList();
            allKeys.Add(keyMapUp);
            allKeys.Add(keyMapDown);
            allKeys.Add(keyMapLeft);
            allKeys.Add(keyMapRight);
            allKeys.Add(keyMapUpLeft);
            allKeys.Add(keyMapUpRight);
            allKeys.Add(keyMapDownLeft);
            allKeys.Add(keyMapDownRight);
            allKeys.Add(keyMapClose);
            allKeys.Add(keyMapMagnifier);
            allKeys.Add(keyMapNavigate);
            allKeys.Add(keyMapZoomIn);
            allKeys.Add(keyMapZoomOut);
            allKeys.Add(keyMapClick);
            allKeys.Add(keyMapDoubleClick);
            allKeys.Add(keyMapKeyboard);
            this.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            formZoom = null;
            formPopup = null;
            formKeyboardNavi = null;
            formKeyboardSearch = null;
            timer1.Enabled = true;
            webBrowser1.Navigate(url);
            SHDocVw.WebBrowser nativeBrowser = (SHDocVw.WebBrowser)webBrowser1.ActiveXInstance;
            nativeBrowser.NewWindow2 += nativeBrowser_NewWindow2;
            if (zoom != 100)
                webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            mouse_event(MOUSEEVENTF_MOVE, 1, 1, 0, 0);
            //Cursor.Hide();
            //formCursor = new FormCursor();
            //formCursor.pictureBox1.BackgroundImage = Image.FromFile("D:\\mouseCursor.png");
            //formCursor.Location = Cursor.Position;
            //formCursor.Size = new System.Drawing.Size(64, 64);
            //formCursor.Show();
        }

        private void importKeymap(String file)
        {
            StreamReader str = new StreamReader(file);
            String line;
            while ((line = str.ReadLine()) != null)
            {
                if (line.Contains("="))
                {
                    String[] spl = line.Split('=');
                    if (spl[0] == "Up")
                        keyMapUp = spl[1].Trim();
                    else if (spl[0] == "Down")
                        keyMapDown = spl[1].Trim();
                    else if (spl[0] == "Left")
                        keyMapLeft = spl[1].Trim();
                    else if (spl[0] == "Right")
                        keyMapRight = spl[1].Trim();
                    else if (spl[0] == "UpLeft")
                        keyMapUpLeft = spl[1].Trim();
                    else if (spl[0] == "UpRight")
                        keyMapUpRight = spl[1].Trim();
                    else if (spl[0] == "DownLeft")
                        keyMapDownLeft = spl[1].Trim();
                    else if (spl[0] == "DownRight")
                        keyMapDownRight = spl[1].Trim();
                    else if (spl[0] == "Click")
                        keyMapClick = spl[1].Trim();
                    else if (spl[0] == "DoubleClick")
                        keyMapDoubleClick = spl[1].Trim();
                    else if (spl[0] == "ZoomIn")
                        keyMapZoomIn = spl[1].Trim();
                    else if (spl[0] == "ZoomOut")
                        keyMapZoomOut = spl[1].Trim();
                    else if (spl[0] == "EnterURL")
                        keyMapNavigate = spl[1].Trim();
                    else if (spl[0] == "Magnifier")
                        keyMapMagnifier = spl[1].Trim();
                    else if (spl[0] == "CloseWindow")
                        keyMapClose = spl[1].Trim();
                    else if (spl[0] == "ShowKeyboard")
                        keyMapKeyboard = spl[1].Trim();

                }
            }
            str.Close();
        }

        private void setAcceleration()
        {
            if ((DateTime.Now.Ticks - lastMousePositionChange) <= 1000000)
            {
                if (acceleration <= maxMouseSpeed)
                    acceleration++;
            }
            else
                acceleration = minMouseSpeed;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //formCursor.Location = Cursor.Position;
            try
            {
                String keys = "";
                foreach (int i in Enum.GetValues(typeof(Keys)))
                {
                    if (GetAsyncKeyState(i) == -32767)
                    {
                        keys += Enum.GetName(typeof(Keys), i) + " ";
                    }
                }
                keys = keys.Trim();
                if (keys.StartsWith("ShiftKey "))
                    keys = keys.Substring(9);
                if (keys.StartsWith("Menu "))
                    keys = keys.Substring(5);
                
                if (keys != "")
                {
                    if (keys == keyMapLeft)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X - acceleration, Cursor.Position.Y);
                        lastMousePositionChange = DateTime.Now.Ticks;
                    }
                    else if (keys == keyMapUp)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == 0)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, -40);");
                    }
                    else if (keys == keyMapRight)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y);
                        lastMousePositionChange = DateTime.Now.Ticks;
                    }
                    else if (keys == keyMapDown)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == this.Size.Height - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, 40);");
                    }
                    else if (keys == keyMapUpLeft)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X - acceleration, Cursor.Position.Y - acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == 0)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, -40);");
                    }
                    else if (keys == keyMapUpRight)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y - acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == 0)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, -40);");
                    }
                    else if (keys == keyMapDownLeft)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X - acceleration, Cursor.Position.Y + acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == this.Size.Height - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, 40);");
                    }
                    else if (keys == keyMapDownRight)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y + acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == this.Size.Height - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, 40);");
                    }
                    else if (keys == keyMapClick)
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    }
                    else if (keys == keyMapDoubleClick)
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    }
                    else if (keys == keyMapClose)
                    {
                        if (formPopup != null)
                        {
                            formPopup.Close();
                            formPopup = null;
                        }
                        else
                            Application.Exit();
                    }
                    else if (keys == keyMapZoomIn)
                    {
                        SendKeys.Send("^{ADD}");
                    }
                    else if (keys == keyMapZoomOut)
                    {
                        SendKeys.Send("^{SUBTRACT}");
                    }
                    else if (keys == keyMapMagnifier)
                    {
                        if (formZoom == null)
                        {
                            this.Location = new Point(magnifierWidth, 0);
                            this.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width - magnifierWidth, Screen.PrimaryScreen.Bounds.Height);
                            formZoom = new FormZoom();
                            formZoom.Show();
                            formZoom.Size = new System.Drawing.Size(magnifierWidth, Screen.PrimaryScreen.Bounds.Height);
                            formZoom.Location = new Point(0, 0);
                            formZoom.pictureBox1.Size = new System.Drawing.Size(formZoom.Size.Width - 2, formZoom.Size.Height - 2);
                        }
                        else
                        {
                            this.Location = new Point(0, 0);
                            this.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                            formZoom.Close();
                            formZoom = null;
                        }
                    }
                    else if (keys == keyMapNavigate)
                    {
                        if (formKeyboardNavi == null)
                        {
                            formKeyboardNavi = new FormKeyboard("http://", allKeys);
                            formKeyboardNavi.textBox1.SelectionStart = 7;
                            formKeyboardNavi.ShowDialog();
                            if (formKeyboardNavi.textBox1.Text != "")
                            {
                                String url = formKeyboardNavi.textBox1.Text;
                                webBrowser1.Navigate(url);
                            }
                            formKeyboardNavi = null;
                        }
                        else
                        {
                            formKeyboardNavi.Close();
                            formKeyboardNavi = null;
                        }
                    }
                    else if (keys == keyMapKeyboard)
                    {
                        if (formKeyboardSearch == null)
                        {
                            formKeyboardSearch = new FormKeyboard("", allKeys);
                            formKeyboardSearch.ShowDialog();
                            if (formKeyboardSearch.textBox1.Text != "")
                            {
                                Clipboard.SetText(formKeyboardSearch.textBox1.Text);
                                SendKeys.Send("^v");
                                SendKeys.Send("{ENTER}");
                            }
                            formKeyboardSearch = null;
                        }
                        else
                        {
                            formKeyboardSearch.Close();
                            formKeyboardSearch = null;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void timerZoom_Tick(object sender, EventArgs e)
        {
            try
            {
                if (formZoom!=null)
                {
                    Bitmap bmp = new Bitmap(formZoom.pictureBox1.Size.Width / 6, formZoom.pictureBox1.Size.Height / 6);
                    Graphics g = Graphics.FromImage(bmp);
                    g.CopyFromScreen(Cursor.Position.X - (formZoom.pictureBox1.Size.Width / 12), Cursor.Position.Y - (formZoom.pictureBox1.Size.Height / 12), 0, 0, new Size(formZoom.pictureBox1.Size.Width / 6, formZoom.pictureBox1.Size.Height / 6));
                    g.Dispose();
                    formZoom.pictureBox1.BackgroundImage = bmp;
                }
            }
            catch
            {
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Visible = true;
            ((SHDocVw.WebBrowser)webBrowser1.ActiveXInstance).ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, zoom, IntPtr.Zero);
            webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted;
            webBrowser1.Navigate("javascript:window.scrollBy(0,-500);");
        }

        void nativeBrowser_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            if (showPopups)
            {
                formPopup = new FormPopup();
                formPopup.Location = new Point(0, 0);
                formPopup.Size = this.Size;
                formPopup.Show();
                ppDisp = formPopup.webBrowser1.ActiveXInstance;
            }
            else
                Cancel = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyDaya)
        {
            if (allKeys.Contains(keyDaya.ToString()))
                return true;
            else
                return false;
        }
    }
}
