using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace XBMC_WebBrowser
{
    public partial class FormMain : Form
    {
        private String mainUrl;
        private String mainTitle;
        private String userDataFolder;
        
        private FormZoom formZoom;
        private FormKeyboard formKeyboardNavi;
        private FormKeyboard formKeyboardSearch;
        private FormPopup formPopup;
        private FormCursor formCursor;
        private FormFavourites formFavourites;
        private FormShortcuts formShortcuts;
        
        private bool showPopups;
        private bool useCustomCursor;
        
        private int acceleration;
        private int minMouseSpeed;
        private int maxMouseSpeed;
        private int zoom;
        private int magnifierWidth;
        private int magnifierHeigth;
        private int customCursorSize;

        private Point lastMousePosition;
        private long lastMousePositionChange;
        
        private ArrayList allKeys;
        private String keyMapUp, keyMapDown, keyMapLeft, keyMapRight, keyMapUpLeft, keyMapUpRight, keyMapDownLeft, keyMapDownRight, keyMapClick, keyMapDoubleClick, keyMapZoomIn, keyMapZoomOut, keyMapMagnifier, keyMapNavigate, keyMapClose, keyMapKeyboard, keyMapFavourites, keyMapShortCuts, keyMapTAB, keyMapESC;
        
        private const UInt32 MOUSEEVENTF_MOVE = 0x0001;
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        private const int SW_SHOWMAXIMIZED = 3;

        private SHDocVw.WebBrowser nativeBrowser;

        [DllImport("User32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, uint dwExtraInf);
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        [DllImport("User32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public FormMain(String[] args)
        {
            InitializeComponent();

            mainTitle = "";
            mainUrl = "http://www.heise.de/";
            minMouseSpeed = 10;
            maxMouseSpeed = 10;
            userDataFolder = "";
            zoom = 100;
            magnifierWidth = 1280;
            magnifierHeigth = 720;
            showPopups = false;
            useCustomCursor = true;
            customCursorSize = 64;
            
            if (args.Length > 0)
            {
                userDataFolder = args[0].Replace("\"", "");
                mainTitle = args[1].Replace("\"", "");
                mainUrl = Uri.UnescapeDataString(args[2]);
                zoom = Convert.ToInt32(args[3]);
                showPopups = (args[4] == "yes");
                minMouseSpeed = Convert.ToInt32(args[5]);
                maxMouseSpeed = Convert.ToInt32(args[6]);
                String[] spl = args[7].Split('x');
                magnifierWidth = Convert.ToInt32(spl[0]);
                magnifierHeigth = Convert.ToInt32(spl[1]);
                useCustomCursor = (args[8] == "true");
                customCursorSize = Convert.ToInt32(args[9]);
            }
                            
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
                keyMapMagnifier = "F1";
                keyMapFavourites = "F2";
                keyMapShortCuts = "F3";
                keyMapNavigate = "Divide";
                keyMapZoomIn = "Add";
                keyMapZoomOut = "Subtract";
                keyMapClick = "NumPad5";
                keyMapDoubleClick = "Decimal";
                keyMapKeyboard = "Multiply";
                keyMapTAB = "";
                keyMapESC = "";
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
            allKeys.Add(keyMapFavourites);
            allKeys.Add(keyMapShortCuts);
            allKeys.Add(keyMapTAB);
            allKeys.Add(keyMapESC);
            
            formZoom = null;
            formPopup = null;
            formKeyboardNavi = null;
            formKeyboardSearch = null;
            formFavourites = null;
            formShortcuts = null;

            lastMousePositionChange = 0;
            acceleration = minMouseSpeed;
            this.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            if (mainUrl.StartsWith("http://www.lovefilm.de/apps/catalog/module/player/player_popout.mhtml") || mainUrl.StartsWith("http://www.watchever.de/player/"))
            {
                webBrowser1.ScrollBarsEnabled = false;
            }
            if (useCustomCursor)
            {
                Cursor.Hide();
                formCursor = new FormCursor();
                String cursorPath = userDataFolder + "\\cursor.png";
                Bitmap oImage = null;
                if (File.Exists(cursorPath))
                {
                    oImage = new Bitmap(cursorPath);
                }
                else
                {
                    oImage = new Bitmap(XBMC_WebBrowser.Properties.Resources.cursorBlue);
                }
                formCursor.BackgroundImage = oImage;
                formCursor.MinimumSize = new System.Drawing.Size(32, 32);
                formCursor.Size = new System.Drawing.Size(customCursorSize, customCursorSize);
                formCursor.Location = new Point(Cursor.Position.X + 1, Cursor.Position.Y + 1);
                formCursor.Show();
            }
            webBrowser1.Navigate(mainUrl);
            nativeBrowser = (SHDocVw.WebBrowser)webBrowser1.ActiveXInstance;
            nativeBrowser.NewWindow2 += nativeBrowser_NewWindow2;
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            mouse_event(MOUSEEVENTF_MOVE, 1, 1, 0, 0);
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
                    else if (spl[0] == "ShowFavourites")
                        keyMapFavourites = spl[1].Trim();
                    else if (spl[0] == "ShowShortcuts")
                        keyMapShortCuts = spl[1].Trim();
                    else if (spl[0] == "PressTAB")
                        keyMapTAB = spl[1].Trim();
                    else if (spl[0] == "PressESC")
                        keyMapESC = spl[1].Trim();

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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (useCustomCursor)
                formCursor.Location = new Point(Cursor.Position.X + 1, Cursor.Position.Y + 1);
            if (Cursor.Position != lastMousePosition && formZoom != null)
            {
                lastMousePosition = Cursor.Position;
                formZoom.Hide();
                Bitmap bmp = new Bitmap(formZoom.pictureBox1.Size.Width / 6, formZoom.pictureBox1.Size.Height / 6);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(Cursor.Position.X - (formZoom.pictureBox1.Size.Width / 12), Cursor.Position.Y - (formZoom.pictureBox1.Size.Height / 12), 0, 0, new Size(formZoom.pictureBox1.Size.Width / 6, formZoom.pictureBox1.Size.Height / 6));
                g.Dispose();
                formZoom.Show();
                formZoom.Location = new Point(Cursor.Position.X - formZoom.Width / 2, Cursor.Position.Y - formZoom.Height / 2);
                formZoom.pictureBox1.BackgroundImage = bmp;
            }
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
                        if (Cursor.Position.X == 0)
                            webBrowser1.Navigate("javascript:window.scrollBy(-40, 0);");
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
                        if (Cursor.Position.X == this.Size.Width - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(40, 0);");
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
                        if (Cursor.Position.X == 0)
                            webBrowser1.Navigate("javascript:window.scrollBy(-40, 0);");
                    }
                    else if (keys == keyMapUpRight)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y - acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == 0)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, -40);");
                        if (Cursor.Position.X == this.Size.Width - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(40, 0);");
                    }
                    else if (keys == keyMapDownLeft)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X - acceleration, Cursor.Position.Y + acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == this.Size.Height - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, 40);");
                        if (Cursor.Position.X == 0)
                            webBrowser1.Navigate("javascript:window.scrollBy(-40, 0);");
                    }
                    else if (keys == keyMapDownRight)
                    {
                        setAcceleration();
                        Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y + acceleration);
                        lastMousePositionChange = DateTime.Now.Ticks;
                        if (Cursor.Position.Y == this.Size.Height - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(0, 40);");
                        if (Cursor.Position.X == this.Size.Width - 1)
                            webBrowser1.Navigate("javascript:window.scrollBy(40, 0);");
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
                        {
                            Process[] p = Process.GetProcessesByName("xbmc");
                            if (p.Count() > 0)
                            {
                                ShowWindow(p[0].MainWindowHandle, SW_SHOWMAXIMIZED);
                                SetForegroundWindow(p[0].MainWindowHandle);
                            }
                            Application.Exit();
                        }
                    }
                    else if (keys == keyMapZoomIn)
                    {
                        SendKeys.Send("^{ADD}");
                    }
                    else if (keys == keyMapZoomOut)
                    {
                        SendKeys.Send("^{SUBTRACT}");
                    }
                    else if (keys == keyMapTAB)
                    {
                        SendKeys.Send("{TAB}");
                    }
                    else if (keys == keyMapESC)
                    {
                        SendKeys.Send("{ESC}");
                    }
                    else if (keys == keyMapMagnifier)
                    {
                        if (formZoom == null)
                        {
                            formZoom = new FormZoom();
                            formZoom.Size = new System.Drawing.Size(magnifierWidth, magnifierHeigth);
                            Bitmap bmp = new Bitmap(formZoom.pictureBox1.Size.Width / 6, formZoom.pictureBox1.Size.Height / 6);
                            Graphics g = Graphics.FromImage(bmp);
                            g.CopyFromScreen(Cursor.Position.X - (formZoom.pictureBox1.Size.Width / 12), Cursor.Position.Y - (formZoom.pictureBox1.Size.Height / 12), 0, 0, new Size(formZoom.pictureBox1.Size.Width / 6, formZoom.pictureBox1.Size.Height / 6));
                            g.Dispose();
                            formZoom.Show();
                            formZoom.Location = new Point(Cursor.Position.X - formZoom.Width/2, Cursor.Position.Y - formZoom.Height/2);
                            formZoom.pictureBox1.BackgroundImage = bmp;
                        }
                        else
                        {
                            formZoom.Close();
                            formZoom = null;
                        }
                    }
                    else if (keys == keyMapNavigate)
                    {
                        if (formKeyboardNavi == null)
                        {
                            formKeyboardNavi = new FormKeyboard("Enter URL:", "http://", true, allKeys);
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
                            formKeyboardSearch = new FormKeyboard("Enter text:", "", false, allKeys);
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
                    else if (keys == keyMapFavourites)
                    {
                        if (formFavourites == null)
                        {
                            formFavourites = new FormFavourites(userDataFolder);
                            formFavourites.ShowDialog();
                            mainTitle = ((ListBoxEntry)formFavourites.listBoxFavs.SelectedItem).title;
                            importPageSettings(mainTitle);
                            formFavourites = null;
                        }
                        else
                        {
                            formFavourites.Close();
                            formFavourites = null;
                        }
                    }
                    else if (keys == keyMapShortCuts)
                    {
                        if (formShortcuts == null)
                        {
                            formShortcuts = new FormShortcuts(userDataFolder, mainTitle, mainUrl, webBrowser1.Url.ToString(), allKeys);
                            formShortcuts.ShowDialog();
                            webBrowser1.Navigate(((ListBoxEntry)formShortcuts.listBoxFavs.SelectedItem).url);
                            formShortcuts = null;
                        }
                        else
                        {
                            formShortcuts.Close();
                            formShortcuts = null;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void importPageSettings(String title)
        {
            if (Directory.Exists(userDataFolder))
            {
                StreamReader str = new StreamReader(userDataFolder + "\\sites\\" + title + ".link");
                String line;
                while ((line = str.ReadLine()) != null)
                {
                    if (line.Contains("="))
                    {
                        String entry = line.Substring(0, line.IndexOf("="));
                        String content = line.Substring(line.IndexOf("=") + 1);
                        if (entry == "url")
                            mainUrl = content.Trim();
                        else if (entry == "zoom")
                            zoom = Convert.ToInt32(content.Trim());
                        else if (entry == "showPopups")
                            showPopups = (content.Trim() == "yes");
                    }
                }
                webBrowser1.Navigate(mainUrl);
                webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (mainUrl.StartsWith("http://www.watchever.de/player/") && webBrowser1.Document.Body.InnerHtml.Contains("<DIV class=einloggen>"))
            {
                FormLogin formLogin = new FormLogin();
                formLogin.ShowDialog();
                ASCIIEncoding Encode = new ASCIIEncoding();
                byte[] post = Encode.GetBytes("login=" + formLogin.textBoxEmail.Text + "&password=" + formLogin.textBoxPW.Text + "&remember=rememberMe");
                string url = "http://www.watchever.de/CN/";
                string PostHeaders = "Content-Type: application/x-www-form-urlencoded";
                webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted;
                nativeBrowser.Navigate(url, null, null, post, PostHeaders);
            }
            if (zoom != 100 && e.Url.AbsolutePath == webBrowser1.Url.AbsolutePath)
            {
                System.Threading.Thread.Sleep(100);
                ((SHDocVw.WebBrowser)webBrowser1.ActiveXInstance).ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, zoom, IntPtr.Zero);
                webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted;
                webBrowser1.Navigate("javascript:window.scrollBy(0,-500);");
            }
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
