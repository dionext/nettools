/**********************************************************************************
 *   Dionext network tools   https://github.com/dionext/nettools
 *   The open source tools for access to network
 *   MIT License Copyright (c) 2017 Dionext Software
 *   
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using CefSharp.WinForms.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;

using System.IO;
using FrwSoftware;

namespace EmbeddedBrowser
{
    public partial class EmbeddedBrowserTabUserControl : UserControl
    {
        public IWinFormsWebBrowser Browser { get; private set; }
        private IntPtr browserHandle;
        private ChromeWidgetMessageInterceptor messageInterceptor;
        private bool multiThreadedMessageLoopEnabled;
        // Default to a small increment:
        private const double ZoomIncrement = 0.10;
        //private Action<string, int?> openNewTab;
        private WebEntryInfo webEntryInfo = null;
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                return webEntryInfo;
            }
            set
            {
                webEntryInfo = value;
                loginButton.Enabled = (webEntryInfo != null && webEntryInfo.Login != null);
                passwordButton.Enabled = (webEntryInfo != null && webEntryInfo.Password != null);
            }
        }
        Action<string, int?> openNewTab;
        public EmbeddedBrowserTabUserControl(Action<string, int?> openNewTab, string url, WebEntryInfo webEntryInfo, bool multiThreadedMessageLoopEnabled)
        {
            InitializeComponent();
            WebEntryInfo = webEntryInfo;
            this.openNewTab = openNewTab;
            this.multiThreadedMessageLoopEnabled = multiThreadedMessageLoopEnabled;

            EmbeddedBrowserHelper.InitIfNeed();//we need this to correctly init first browser instance

            //https://stackoverflow.com/questions/34549565/separate-cache-per-browser
            ChromiumWebBrowser browser = null;
            if (WebEntryInfo.BrowserPrivateType == BrowserPrivateType.COMMON_CACHE
               || WebEntryInfo.BrowserPrivateType == BrowserPrivateType.PERSONAL_OLD_DISK_CACHE 
               || WebEntryInfo.BrowserPrivateType == BrowserPrivateType.PERSONAL_NEW_DISK_CACHE)
            {
                if (WebEntryInfo.CachePath != null)
                {
                    if (WebEntryInfo.BrowserPrivateType == BrowserPrivateType.PERSONAL_NEW_DISK_CACHE)
                    {
                        FileUtils.CreateOrClearDirectory(WebEntryInfo.CachePath);
                    }
                    else if (WebEntryInfo.BrowserPrivateType == BrowserPrivateType.PERSONAL_OLD_DISK_CACHE
                        || WebEntryInfo.BrowserPrivateType == BrowserPrivateType.COMMON_CACHE)
                    {
                        FileUtils.CreateDirectory(WebEntryInfo.CachePath);
                    }
                }
                else throw new NotSupportedException();
                BrowserSettings browserSettings = new BrowserSettings();
                RequestContextSettings requestContextSettings = new RequestContextSettings { CachePath = WebEntryInfo.CachePath };
                var requestContext = new RequestContext(requestContextSettings);
                browser = new ChromiumWebBrowser(url)
                {
                    BrowserSettings = browserSettings,
                    RequestContext = requestContext,
                    Dock = DockStyle.Fill
                };
            }
            else if (WebEntryInfo.BrowserPrivateType == BrowserPrivateType.PERSONAL_IN_MEMORY_CACHE)
            {
                var browserSettings = new BrowserSettings();
                var requestContextSettings = new RequestContextSettings { CachePath = null };
                var requestContext = new RequestContext(requestContextSettings);
                browser = new ChromiumWebBrowser(url)
                {
                    BrowserSettings = browserSettings,
                    RequestContext = requestContext,
                    Dock = DockStyle.Fill
                };
            }

            browserPanel.Controls.Add(browser);
            Browser = browser;

            MenuHandler handler = new MenuHandler();
            //handler.DownloadImage += Handler_DownloadImage;
            //handler.ViewPageSource += Handler_ViewPageSource;
           // handler.DownloadYouTubeVideo += Handler_DownloadYouTubeVideo;
            //handler.ViewImageExif += Handler_ViewImageExif;
            //handler.ViewFacebookIdNum += Handler_ViewFacebookIdNum;
           // handler.ViewTwitterIdNum += Handler_ViewTwitterIdNum;
            handler.CopyImageLocation += Handler_CopyImageLocation; ;
            handler.OpenInNewTabContextMenu += Handler_OpenInNewTabContextMenu;
            handler.ReverseImgSearch += Handler_ReverseImgSearch;
            //handler.ExtractLinks += Handler_ExtractLinks;
            //handler.AddPageToBookmarks += Handler_AddPageToBookmarks;
            handler.SearchText += Handler_SearchText;
            handler.SaveSelectedText += Handler_SaveSelectedText;

            Browser.MenuHandler = handler;


            //todo
            browser.RequestHandler = new WinFormsRequestHandler(openNewTab, webEntryInfo);
            browser.LoadingStateChanged += OnBrowserLoadingStateChanged;//todo some errors when press 'Go'
            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;
            browser.StatusMessage += OnBrowserStatusMessage;//to status label 
            browser.LoadError += OnLoadError;
            browser.RenderProcessMessageHandler = new RenderProcessMessageHandler();
            browser.DisplayHandler = new DisplayHandler();

            //var version = String.Format(EmbeddedBrowserControlRes.Chromium___0___CEF___1___CefSharp___2_, Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);
            var version = String.Format(EmbeddedBrowserControlRes.Chromium___0, Cef.ChromiumVersion);
            DisplayOutput(version);
        }
        private void Handler_CopyImageLocation(object sender, EventArgs e)
        {
            string link = ((TextEventArgs)e).Result;
            Clipboard.SetText(link);
        }
        private void Handler_OpenInNewTabContextMenu(object sender, EventArgs e)
        {
            openNewTab(((TextEventArgs)e).Result, null);
        }
        private void Handler_SaveSelectedText(object sender, EventArgs e)
        {
            try
            {
                //todo
                //File.WriteAllText(Constants.TempTextFile, ((TextEventArgs)e).Result);
                //this.InvokeIfRequired(() => new TextPreviewer(Enums.Actions.Text, URL).Show());
            }
            catch { MessageBox.Show("Error saving selected text.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void Handler_SearchText(object sender, EventArgs e)
        {
            string text = ((TextEventArgs)e).Result;
            //this.InvokeIfRequired(() => CreateTab("https://www.google.co.uk/search?q=" + text));
            openNewTab("https://www.google.co.uk/search?q=" + text, null);
        }
        private void Handler_ReverseImgSearch(object sender, EventArgs e)
        {
            string text = ((TextEventArgs)e).Result;
            openNewTab(text, null);
        }

        public void DeleteCookies(string url, string name, bool global)
        {
            ICookieManager cookieManager = null;
            if (global)
            {
                cookieManager = Cef.GetGlobalCookieManager();
            }
            else
            {
                cookieManager = ((ChromiumWebBrowser)Browser).RequestContext.GetDefaultCookieManager(null);
            }
            if (cookieManager != null)
            {
                cookieManager.DeleteCookies(url, name);
            }
        }
        /*
        public void GetCookiesInfo()
        {
            //var cc = await ((ChromiumWebBrowser)Browser).RequestContext.GetDefaultCookieManager(null).VisitAllCookiesAsync().ta
            var cookieVisitor = new TaskCookieVisitor();
            ((ChromiumWebBrowser)Browser).RequestContext.GetDefaultCookieManager(null).VisitAllCookies(cookieVisitor);
        }
        */
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }

                if (messageInterceptor != null)
                {
                    messageInterceptor.ReleaseHandle();
                    messageInterceptor = null;
                }
            }
            base.Dispose(disposing);
        }


        private void OnLoadError(object sender, LoadErrorEventArgs args)
        {
            //todo redirect this output to special console
            //DisplayOutput("Load Error:" + args.ErrorCode + ";" + args.ErrorText + " Url: " + args.FailedUrl);
            //Console.WriteLine("Load Error:" + args.ErrorCode + ";" + args.ErrorText + " Url: " + args.FailedUrl);
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            //todo redirect this output to special console
            //DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
            //Console.WriteLine(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(args.IsLoading));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            if (Parent != null)
                this.InvokeOnUiThreadIfRequired(() => Parent.Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private static void OnJavascriptEventArrived(string eventName, object eventData)
        {
            switch (eventName)
            {
                case "click":
                {
                    var message = eventData.ToString();
                    var dataDictionary = eventData as Dictionary<string, object>;
                    if (dataDictionary != null)
                    {
                        var result = string.Join(", ", dataDictionary.Select(pair => pair.Key + "=" + pair.Value));
                        message = "event data: " + result;
                    }
                    MessageBox.Show(message, "Javascript event arrived", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
            }
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";
            goButton.Image = isLoading ?
                Properties.Resources.nav_plain_red :
                Properties.Resources.nav_plain_green;

            HandleToolStripLayout();
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            if (args.IsBrowserInitialized)
            {
                //Get the underlying browser host wrapper
                var browserHost = Browser.GetBrowser().GetHost();
                var requestContext = browserHost.RequestContext;
                string errorMessage;
                // Browser must be initialized before getting/setting preferences
                var success = requestContext.SetPreference("enable_do_not_track", true, out errorMessage);
                if(!success)
                {
                    this.InvokeOnUiThreadIfRequired(() => MessageBox.Show("Unable to set preference enable_do_not_track errorMessage: " + errorMessage));
                }

                //Example of disable spellchecking
                //success = requestContext.SetPreference("browser.enable_spellchecking", false, out errorMessage);

                var preferences = requestContext.GetAllPreferences(true);
                var doNotTrack = (bool)preferences["enable_do_not_track"];

                //Use this to check that settings preferences are working in your code
                //success = requestContext.SetPreference("webkit.webprefs.minimum_font_size", 24, out errorMessage);

                //If we're using CefSetting.MultiThreadedMessageLoop (the default) then to hook the message pump,
                // which running in a different thread we have to use a NativeWindow
                if (multiThreadedMessageLoopEnabled)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                IntPtr chromeWidgetHostHandle;
                                if (ChromeWidgetHandleFinder.TryFindHandle(browserHandle, out chromeWidgetHostHandle))
                                {
                                    messageInterceptor = new ChromeWidgetMessageInterceptor((Control)Browser, chromeWidgetHostHandle, message =>
                                    {
                                        const int WM_MOUSEACTIVATE = 0x0021;
                                        const int WM_NCLBUTTONDOWN = 0x00A1;
                                        //const int WM_LBUTTONDOWN = 0x0201;

                                        if (message.Msg == WM_MOUSEACTIVATE)
                                        {
                                            // The default processing of WM_MOUSEACTIVATE results in MA_NOACTIVATE,
                                            // and the subsequent mouse click is eaten by Chrome.
                                            // This means any .NET ToolStrip or ContextMenuStrip does not get closed.
                                            // By posting a WM_NCLBUTTONDOWN message to a harmless co-ordinate of the
                                            // top-level window, we rely on the ToolStripManager's message handling
                                            // to close any open dropdowns:
                                            // http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ToolStripManager.cs,1249
                                            var topLevelWindowHandle = message.WParam;
                                            PostMessage(topLevelWindowHandle, WM_NCLBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                                        }
                                        //Forward mouse button down message to browser control
                                        //else if(message.Msg == WM_LBUTTONDOWN)
                                        //{
                                        //    PostMessage(browserHandle, WM_LBUTTONDOWN, message.WParam, message.LParam);
                                        //}

                                        // The ChromiumWebBrowserControl does not fire MouseEnter/Move/Leave events, because Chromium handles these.
                                        // However we can hook into Chromium's messaging window to receive the events.
                                        //
                                        //const int WM_MOUSEMOVE = 0x0200;
                                        //const int WM_MOUSELEAVE = 0x02A3;
                                        //
                                        //switch (message.Msg) {
                                        //    case WM_MOUSEMOVE:
                                        //        Console.WriteLine("WM_MOUSEMOVE");
                                        //        break;
                                        //    case WM_MOUSELEAVE:
                                        //        Console.WriteLine("WM_MOUSELEAVE");
                                        //        break;
                                        //}
                                    });

                                    break;
                                }
                                else
                                {
                                    // Chrome hasn't yet set up its message-loop window.
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        catch
                        {
                            // Errors are likely to occur if browser is disposed, and no good way to check from another thread
                        }
                    });
                }
            }
        }

        private void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            Browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            Browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                Browser.Load(url);
            }
        }

        public async void CopySourceToClipBoardAsync()
        {
            var htmlSource = await Browser.GetSourceAsync();

            Clipboard.SetText(htmlSource);
            DisplayOutput(EmbeddedBrowserControlRes.HTML_Source_copied_to_clipboard);
        }

        private void ToggleBottomToolStrip()
        {
            if (toolStrip2.Visible)
            {
                Browser.StopFinding(true);
                toolStrip2.Visible = false;
            }
            else
            {
                toolStrip2.Visible = true;
                findTextBox.Focus();
            }
        }

        private void FindNextButtonClick(object sender, EventArgs e)
        {
            Find(true);
        }

        private void FindPreviousButtonClick(object sender, EventArgs e)
        {
            Find(false);
        }

        private void Find(bool next)
        {
            if (!string.IsNullOrEmpty(findTextBox.Text))
            {
                Browser.Find(0, findTextBox.Text, next, false, false);
            }
        }

        private void FindTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            Find(true);
        }

        public void ShowFind()
        {
            ToggleBottomToolStrip();
        }

        private void FindCloseButtonClick(object sender, EventArgs e)
        {
            ToggleBottomToolStrip();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
               
                Clipboard.Clear();
                Clipboard.SetText(webEntryInfo.Login, TextDataFormat.Text);
                this.Focus();
                Browser.Focus();
                Browser.Paste();
                //alt
                //SendKeys.Send("^v"); // [CTRL]+[v]
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        private void passwordButton_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(webEntryInfo.Password, TextDataFormat.Text);
                Browser.Focus();
                Browser.Paste();
                //alt
                //SendKeys.Send("^v"); // [CTRL]+[v]
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            try
            {
                ZoomIn();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        public void ZoomIn()
        {
            var control = this;
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();

                task.ContinueWith(previous =>
                {
                    if (previous.Status == TaskStatus.RanToCompletion)
                    {
                        var currentLevel = previous.Result;
                        control.Browser.SetZoomLevel(currentLevel + ZoomIncrement);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected failure of calling CEF->GetZoomLevelAsync", previous.Exception);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }

        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            try
            {
                ZoomOut();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        public void ZoomOut()
        {
            var control = this;// GetCurrentTabControl();
            if (control != null)
            {
                var task = control.Browser.GetZoomLevelAsync();
                task.ContinueWith(previous =>
                {
                    if (previous.Status == TaskStatus.RanToCompletion)
                    {
                        var currentLevel = previous.Result;
                        control.Browser.SetZoomLevel(currentLevel - ZoomIncrement);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected failure of calling CEF->GetZoomLevelAsync", previous.Exception);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }

        }

        //not in use
        private void GetCurrentZoomLevel()
        {
            var task = Browser.GetZoomLevelAsync();
            task.ContinueWith(previous =>
            {
                if (previous.Status == TaskStatus.RanToCompletion)
                {
                    var currentLevel = previous.Result;
                    // currentLevel.ToString();
                }
                else
                {
                    throw new Exception("Unexpected failure of calling CEF->GetZoomLevelAsync: " + previous.Exception.ToString());
                }
            }, TaskContinuationOptions.HideScheduler);

        }
    }
}
