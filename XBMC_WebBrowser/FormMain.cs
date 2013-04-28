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
        private String userAgent;
        private String userDataFolder;
        
        private FormZoom formZoom;
        private FormKeyboard formKeyboardNavi;
        private FormKeyboard formKeyboardSearch;
        private FormPopup formPopup;
        private FormCursor formCursor;
        private FormFavourites formFavourites;
        private FormShortcuts formShortcuts;
        private FormContextMenu formContextMenu;
        
        private bool showPopups;
        private bool showScrollBar;
        private bool useCustomCursor;
        private bool mouseEnabled;
        
        private int acceleration;
        private int minMouseSpeed;
        private int maxMouseSpeed;
        private int zoom;
        private int magnifierWidth;
        private int magnifierHeigth;
        private int magnifierZoom;
        private int customCursorSize;
        private int scrollSpeed;

        private Point lastMousePosition;
        private long lastMousePositionChange;
        
        private ArrayList allKeys;
        private String keyMapUp, keyMapDown, keyMapLeft, keyMapRight, keyMapUpLeft, keyMapUpRight, keyMapDownLeft, keyMapDownRight, keyMapClick, keyMapDoubleClick, keyMapZoomIn, keyMapZoomOut, keyMapMagnifier, keyMapNavigate, keyMapClose, keyMapKeyboard, keyMapFavourites, keyMapShortCuts, keyMapTAB, keyMapESC, keyMapToggleMouse, keyMapContextMenu, keyMapF5;
        
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
            mainUrl = "http://www.imdb.com/";
            userAgent = "";
            minMouseSpeed = 10;
            maxMouseSpeed = 10;
            userDataFolder = "";
            zoom = 100;
            magnifierWidth = 1280;
            magnifierHeigth = 720;
            magnifierZoom = 3;
            showPopups = false;
            showScrollBar = true;
            useCustomCursor = true;
            customCursorSize = 64;
            mouseEnabled = true;
            scrollSpeed = 20;

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
                showScrollBar = (args[10] == "yes");
                scrollSpeed = Convert.ToInt32(args[11]);
                userAgent = args[12].Replace("\"", "");
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
                keyMapToggleMouse = "Multiply";
                keyMapClick = "NumPad5";
                keyMapZoomIn = "Add";
                keyMapZoomOut = "Subtract";
                keyMapContextMenu = "Divide";
                keyMapClose = "NumPad0";
                keyMapMagnifier = "";
                keyMapFavourites = "";
                keyMapShortCuts = "";
                keyMapNavigate = "";
                keyMapDoubleClick = "";
                keyMapKeyboard = "";
                keyMapTAB = "";
                keyMapESC = "";
                keyMapF5 = "";
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
            allKeys.Add(keyMapToggleMouse);
            allKeys.Add(keyMapContextMenu);
            allKeys.Add(keyMapF5);

            formZoom = null;
            formPopup = null;
            formKeyboardNavi = null;
            formKeyboardSearch = null;
            formFavourites = null;
            formShortcuts = null;
            formContextMenu = null;

            lastMousePositionChange = 0;
            acceleration = minMouseSpeed;
            this.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            webBrowser1.ScrollBarsEnabled = showScrollBar;
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
            if (userAgent=="")
                webBrowser1.Navigate(mainUrl);
            else
                webBrowser1.Navigate(mainUrl, null, null, "User-Agent: " + userAgent);
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
                    else if (spl[0] == "ToggleMouse")
                        keyMapToggleMouse = spl[1].Trim();
                    else if (spl[0] == "PressF5")
                        keyMapF5 = spl[1].Trim();
                    else if (spl[0] == "ShowContextMenu")
                        keyMapContextMenu = spl[1].Trim();
                    
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
                updateMagnifier();
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
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X - acceleration, Cursor.Position.Y);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.X == 0)
                                webBrowser1.Navigate("javascript:window.scrollBy(-" + scrollSpeed + ", 0);");
                        }           
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(-" + scrollSpeed + ", 0);");
                    }
                    else if (keys == keyMapUp)
                    {
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - acceleration);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.Y == 0)
                                webBrowser1.Navigate("javascript:window.scrollBy(0, -" + scrollSpeed + ");");
                        }
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(0, -" + scrollSpeed + ");");
                    }
                    else if (keys == keyMapRight)
                    {
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.X == this.Size.Width - 1)
                                webBrowser1.Navigate("javascript:window.scrollBy(" + scrollSpeed + ", 0);");
                        }
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(" + scrollSpeed + ", 0);");
                    }
                    else if (keys == keyMapDown)
                    {
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + acceleration);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.Y == this.Size.Height - 1)
                                webBrowser1.Navigate("javascript:window.scrollBy(0, " + scrollSpeed + ");");
                        }
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(0, " + scrollSpeed + ");");
                    }
                    else if (keys == keyMapUpLeft)
                    {
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X - acceleration, Cursor.Position.Y - acceleration);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.Y == 0)
                                webBrowser1.Navigate("javascript:window.scrollBy(0, -" + scrollSpeed + ");");
                            if (Cursor.Position.X == 0)
                                webBrowser1.Navigate("javascript:window.scrollBy(-" + scrollSpeed + ", 0);");
                        }
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(-" + scrollSpeed + ", -" + scrollSpeed + ");");
                    }
                    else if (keys == keyMapUpRight)
                    {
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y - acceleration);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.Y == 0)
                                webBrowser1.Navigate("javascript:window.scrollBy(0, -" + scrollSpeed + ");");
                            if (Cursor.Position.X == this.Size.Width - 1)
                                webBrowser1.Navigate("javascript:window.scrollBy(" + scrollSpeed + ", 0);");
                        }
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(" + scrollSpeed + ", -" + scrollSpeed + ");");
                    }
                    else if (keys == keyMapDownLeft)
                    {
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X - acceleration, Cursor.Position.Y + acceleration);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.Y == this.Size.Height - 1)
                                webBrowser1.Navigate("javascript:window.scrollBy(0, " + scrollSpeed + ");");
                            if (Cursor.Position.X == 0)
                                webBrowser1.Navigate("javascript:window.scrollBy(-" + scrollSpeed + ", 0);");
                        }
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(-" + scrollSpeed + ", " + scrollSpeed + ");");
                    }
                    else if (keys == keyMapDownRight)
                    {
                        if (mouseEnabled)
                        {
                            setAcceleration();
                            Cursor.Position = new Point(Cursor.Position.X + acceleration, Cursor.Position.Y + acceleration);
                            lastMousePositionChange = DateTime.Now.Ticks;
                            if (Cursor.Position.Y == this.Size.Height - 1)
                                webBrowser1.Navigate("javascript:window.scrollBy(0, " + scrollSpeed + ");");
                            if (Cursor.Position.X == this.Size.Width - 1)
                                webBrowser1.Navigate("javascript:window.scrollBy(" + scrollSpeed + ", 0);");
                        }
                        else
                            webBrowser1.Navigate("javascript:window.scrollBy(" + scrollSpeed + ", " + scrollSpeed + ");");
                    }
                    else if (keys == keyMapClick)
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    }
                    else if (keys == keyMapDoubleClick)
                    {
                        doubleClick();
                    }
                    else if (keys == keyMapClose)
                    {
                        if (formPopup != null)
                        {
                            formPopup.Close();
                            formPopup = null;
                        }
                        else if (formZoom != null)
                        {
                            formZoom.Close();
                            formZoom = null;
                        }
                        else if (formKeyboardNavi != null)
                        {
                            formKeyboardNavi.Close();
                            formKeyboardNavi = null;
                        }
                        else if (formKeyboardSearch != null)
                        {
                            formKeyboardSearch.Close();
                            formKeyboardSearch = null;
                        }
                        else if (formFavourites != null)
                        {
                            formFavourites.Close();
                            formFavourites = null;
                        }
                        else if (formShortcuts != null)
                        {
                            formShortcuts.Close();
                            formShortcuts = null;
                        }
                        else if (formContextMenu != null)
                        {
                            formContextMenu.Close();
                            formContextMenu = null;
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
                        if (formZoom != null)
                        {
                            magnifierZoom++;
                            lastMousePosition = Cursor.Position;
                            formZoom.Hide();
                            updateMagnifier();
                        }
                        else
                            SendKeys.Send("^{ADD}");
                    }
                    else if (keys == keyMapZoomOut)
                    {
                        if (formZoom != null && magnifierZoom>2)
                        {
                            magnifierZoom--;
                            lastMousePosition = Cursor.Position;
                            formZoom.Hide();
                            updateMagnifier();
                        }
                        else
                            SendKeys.Send("^{SUBTRACT}");
                    }
                    else if (keys == keyMapTAB)
                    {
                        pressTab();
                    }
                    else if (keys == keyMapESC)
                    {
                        pressEsc();
                    }
                    else if (keys == keyMapF5)
                    {
                        pressF5();
                    }
                    else if (keys == keyMapMagnifier)
                    {
                        showMagnifier();
                    }
                    else if (keys == keyMapNavigate)
                    {
                        enterUrl();
                    }
                    else if (keys == keyMapKeyboard)
                    {
                        showKeyboard();
                    }
                    else if (keys == keyMapFavourites)
                    {
                        showFavourites();
                    }
                    else if (keys == keyMapShortCuts)
                    {
                        showShortcuts();
                    }
                    else if (keys == keyMapToggleMouse)
                        mouseEnabled = !mouseEnabled;
                    else if (keys == keyMapContextMenu)
                    {
                        showContextMenu();
                    }
                }
            }
            catch
            {
            }
        }

        private void updateMagnifier()
        {
            Bitmap bmp = new Bitmap(formZoom.pictureBox1.Size.Width / magnifierZoom, formZoom.pictureBox1.Size.Height / magnifierZoom);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(Cursor.Position.X - (formZoom.pictureBox1.Size.Width / (magnifierZoom * 2)), Cursor.Position.Y - (formZoom.pictureBox1.Size.Height / (magnifierZoom * 2)), 0, 0, new Size(formZoom.pictureBox1.Size.Width / magnifierZoom, formZoom.pictureBox1.Size.Height / magnifierZoom));
            g.Dispose();
            formZoom.Show();
            formZoom.Location = new Point(Cursor.Position.X - formZoom.Width / 2, Cursor.Position.Y - formZoom.Height / 2);
            formZoom.pictureBox1.BackgroundImage = bmp;
        }

        private void showContextMenu()
        {
            if (formContextMenu == null)
            {
                formContextMenu = new FormContextMenu();
                formContextMenu.ShowDialog();
                String entry = ((ListBoxEntry)formContextMenu.listBoxMenu.SelectedItem).title;
                if (entry == "Show Magnifier")
                    showMagnifier();
                else if (entry == "Enter URL")
                    enterUrl();
                else if (entry == "Show Keyboard")
                    showKeyboard();
                else if (entry == "Show Favourites")
                    showFavourites();
                else if (entry == "Show Shortcuts")
                    showShortcuts();
                else if (entry == "Toggle Mouse/Scroll")
                    mouseEnabled = !mouseEnabled;
                else if (entry == "Press TAB")
                    pressTab();
                else if (entry == "Press ESC")
                    pressEsc();
                else if (entry == "Press F5")
                    pressF5();
                else if (entry == "Double Click")
                    doubleClick();
                
                formContextMenu = null;
            }
            else
            {
                formContextMenu.Close();
                formContextMenu = null;
            }
        }

        private static void doubleClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private static void pressEsc()
        {
            SendKeys.Send("{ESC}");
        }

        private static void pressTab()
        {
            SendKeys.Send("{TAB}");
        }

        private static void pressF5()
        {
            SendKeys.Send("{F5}");
        }

        private void showMagnifier()
        {
            if (formZoom == null)
            {
                formZoom = new FormZoom();
                formZoom.Size = new System.Drawing.Size(magnifierWidth, magnifierHeigth);
                updateMagnifier();
            }
            else
            {
                formZoom.Close();
                formZoom = null;
            }
        }

        private void enterUrl()
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

        private void showKeyboard()
        {
            if (formKeyboardSearch == null)
            {
                formKeyboardSearch = new FormKeyboard("Enter text:", "", false, allKeys);
                formKeyboardSearch.ShowDialog();
                if (formKeyboardSearch.textBox1.Text != "")
                {
                    Clipboard.SetText(formKeyboardSearch.textBox1.Text);
                    SendKeys.Send("^v");
                }
                formKeyboardSearch = null;
            }
            else
            {
                formKeyboardSearch.Close();
                formKeyboardSearch = null;
            }
        }

        private void showFavourites()
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

        private void showShortcuts()
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
                        else if (entry == "showScrollbar")
                            showScrollBar = (content.Trim() == "yes");
                        else if (entry == "userAgent")
                            userAgent = content.Trim();
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
            if (e.Url.AbsolutePath == webBrowser1.Url.AbsolutePath)
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
