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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
using System.Windows.Forms;
using FrwSoftware;
using System.Diagnostics;

namespace Dionext
{
    public class WebAccountLibAppManager : VpnSelectorAppManager
    {
        public bool CheckIfWeNeedVPN(WebEntryInfo webEntryInfo)
        {
            string mes = null;
            JVPNServer allowedVPNServer = null;
            if (webEntryInfo.AllowedVPNServerId != null)
            {
                allowedVPNServer = Dm.Instance.Find<JVPNServer>(webEntryInfo.AllowedVPNServerId);
            }
            //if (webEntryInfo.AllowedVPNServerId != null || webEntryInfo.AllowedVPNCountrу != null || webEntryInfo.AllowedVPNTown != null)
            if (webEntryInfo.AllowedVPNServerId != null)
            {
                string allowedMes = WebAccountLibRes.For_this_entity_you_need_access_to_the_Internet_through_a_specific_VPN;
                if (VpnConnUtils.CurrentVPNServer == null)
                {
                    mes = allowedMes + WebAccountLibRes.Current_VPN_connection_is_not_established;
                }
                else 
                {
                    if (webEntryInfo.AllowedVPNServerId != null)
                    {
                        if (allowedVPNServer == null)
                        {
                            mes = allowedMes + WebAccountLibRes.This_VPN_connection_has_not_been_established;
                        }
                        else if (VpnConnUtils.CurrentVPNServer != allowedVPNServer)
                        {
                            mes = allowedMes + WebAccountLibRes.The_current_VPN_connection_does_not_match;
                        }
                    }
                }
            }
                
            if (mes != null)
            {
                if (allowedVPNServer != null)
                {
                    DialogResult res = MessageBox.Show(mes + " " + WebAccountLibRes.Click_Yes_to_connect_to_the_desired_VPN_server,
                        FrwConstants.WARNING, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Yes)
                    {
                        try {     
                            Cursor.Current = Cursors.WaitCursor;
                            bool conRes = VpnConnUtils.ConnectWithConfirmation(allowedVPNServer);
                            return conRes;
                        }
                        finally
                        {
                            Cursor.Current = Cursors.Default;
                        }
                    }
                    else if (res == DialogResult.No)
                    {
                        //do nothing
                    }
                    else //Cancel or close
                    {
                        return false;
                    }
                }
                else
                {
                    DialogResult res = MessageBox.Show(mes + " " + WebAccountLibRes.Click_OK_to_ignore_the_warning_and_continue_Click_Cancel_to_cancel,
                        FrwConstants.WARNING, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (res != DialogResult.OK) //Cancel or close
                    {
                        return false;
                    }
                }
            }
                
            return true;
        }


        private void CreateOpenInInternalBrowserContextMenu(List<ToolStripItem> subs2,
            ViewType viewType,
            ViewType recоmmendedViewType,
            BrowserPrivateType browserPrivateType,
            string url,
            WebEntryInfo webEntryInfo,
            IContentContainer contentContainer,
            object selectedObject,
            string webEntityInfoPropertyName = null)
        {

            ToolStripMenuItem menuItem = new ToolStripMenuItem();

            StringBuilder text = new StringBuilder();
            text.Append(WebAccountLibRes.Open_in_ + " ");
            //if (viewType == ViewType.Awesomium) text.Append(WebAccountLibRes.Awesomium);
            if (viewType == ViewType.CefBrowser) text.Append(WebAccountLibRes.Chromium_Embedded);
            //else if (viewType == ViewType.IE) text.Append(WebAccountLibRes.IE);
            else if (viewType == ViewType.Simple) text.Append(WebAccountLibRes.Simple_editor);
            else if (viewType == ViewType.WORD) text.Append(WebAccountLibRes.MS_Word);
            if (viewType == ViewType.CefBrowser)
            {
                if (browserPrivateType == BrowserPrivateType.COMMON_CACHE) text.Append(WebAccountLibRes.__common_cache);
                else if (browserPrivateType == BrowserPrivateType.PERSONAL_IN_MEMORY_CACHE) text.Append(WebAccountLibRes.__private_in_memory_cache_);
                else if (browserPrivateType == BrowserPrivateType.PERSONAL_OLD_DISK_CACHE) text.Append(WebAccountLibRes.__private_persistent_cache_);
                //else if (browserPrivateType == BrowserPrivateType.PERSONAL_NEW_DISK_CACHE) text.Append(WebAccountLibRes.__private_persistent_cache__cleared__);
            }
            menuItem.Text = text.ToString();
            menuItem.Click += (s, em) =>
            {
                try
                {

                    if (CheckIfWeNeedVPN(webEntryInfo) == false) return;

                    BaseViewWindow itemViewWindow = AppManager.Instance.CreateContent(contentContainer, AppManager.Instance.DefaultViewWindowType,
                            new Dictionary<string, object>() {
                                    { "Item", selectedObject },
                                    { "WebEntityInfoPropertyName", webEntityInfoPropertyName },
                                    { "BrowserPrivateType",  browserPrivateType }
                            }) as BaseViewWindow;
                    itemViewWindow.ViewType = viewType;
                    itemViewWindow.FileFullPath = url;
                    itemViewWindow.ProcessView();
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            subs2.Add(menuItem);
        }


 
        virtual protected void OpenOtherTools(WebEntryInfo webEntryInfo, List<ToolStripItem> subs)
        {
        }
        virtual protected void OpenInOtherBrowsers(WebEntryInfo webEntryInfo, string url, List<ToolStripItem> subs1)
        {
        }

        override public List<ToolStripItem> CreateOpenInBrowserContextMenu(WebEntryInfo webEntryInfo, IContentContainer contentContainer, object selectedObject, string webEntityInfoPropertyName = null)
        {
            string url = webEntryInfo != null ? webEntryInfo.Url : null;

            ToolStripMenuItem menuItem = null;
            List<ToolStripItem> subs = new List<ToolStripItem>();
            List<ToolStripItem> subs1 = new List<ToolStripItem>();
            List<ToolStripItem> subs2 = null;


            if (string.IsNullOrEmpty(url) == false)
            {
                ViewType recоmmendedViewType = webEntryInfo.RecоmmendedViewType;

                bool isFile = false;
                bool isUrl = false;
                FileUtils.IsFilePath(url, out isFile, out isUrl);
                string oTitle = isFile ? WebAccountLibRes.Path : WebAccountLibRes.URL;

                string uri = isFile? FileUtils.FilePathWithBookmarkToFileUrl(url) : url;

                if (isFile)
                {
                    if (recоmmendedViewType == ViewType.NONE) recоmmendedViewType = GetViewTypeByFilePath(url);
                }
                else recоmmendedViewType = ViewType.CefBrowser;

                //////////////////
                //open favorit 
                /////////////////

                //default
                if (FileUtils.IsHtmlFileWithBookmark(url) && AppLocator.ChromePath != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_Chrome;
                    menuItem.Font = new Font(menuItem.Font, FontStyle.Bold);
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.ChromePath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }
                else
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in + " " + WebAccountLibRes.In_default_external_application;
                    menuItem.Font = new Font(menuItem.Font, FontStyle.Bold);
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.OpenFile(url);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }



                //internal
                CreateOpenInInternalBrowserContextMenu(subs, ViewType.CefBrowser, recоmmendedViewType, BrowserPrivateType.PERSONAL_OLD_DISK_CACHE,
                   url, webEntryInfo, contentContainer, selectedObject, webEntityInfoPropertyName);
                //external
                if (isFile)
                {
                    //open folder
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_folder;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.OpenFile(FileUtils.GetDirectorNameForPath(url));
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }
                //////////////////////////////////
                OpenOtherTools(webEntryInfo, subs);


                //////////////////////////////
                ///internal browser

                subs2 = new List<ToolStripItem>();

                CreateOpenInInternalBrowserContextMenu(subs2, ViewType.CefBrowser, recоmmendedViewType, BrowserPrivateType.COMMON_CACHE,
                    url, webEntryInfo, contentContainer, selectedObject, webEntityInfoPropertyName);
                CreateOpenInInternalBrowserContextMenu(subs2, ViewType.CefBrowser, recоmmendedViewType, BrowserPrivateType.PERSONAL_OLD_DISK_CACHE,
                    url, webEntryInfo, contentContainer, selectedObject, webEntityInfoPropertyName);
                CreateOpenInInternalBrowserContextMenu(subs2, ViewType.CefBrowser, recоmmendedViewType, BrowserPrivateType.PERSONAL_IN_MEMORY_CACHE,
                    url, webEntryInfo, contentContainer, selectedObject, webEntityInfoPropertyName);

                string commonCachePath = Dm.Instance.GetBrowserCommonCachePathUniqueForCompAndUser();
                string objectCachepath = Dm.Instance.GetCacheFullPathForObjectUniqueForCompAndUser(selectedObject);
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Clear_browser_cache_for_entity;
                if (Directory.Exists(objectCachepath) == false)
                {
                    menuItem.Enabled = false;
                }
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        if (Directory.Exists(objectCachepath))
                            FileUtils.DeleteDirectory(objectCachepath);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                subs2.Add(menuItem);
                
                JActor actor = null;
                var pl = selectedObject.GetType().GetProperties();
                Type at = typeof(JActor);
                foreach (var p in pl)
                {
                    if (p.PropertyType == at)
                    {
                        actor = AttrHelper.GetPropertyValue(selectedObject, p) as JActor;
                        break;
                    }
                }
                if (actor != null)
                {
                    string actorCachepath = Dm.Instance.GetCacheFullPathForObjectUniqueForCompAndUser(actor);
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Clear_browser_cache_for_entity_actor;
                    if (Directory.Exists(actorCachepath) == false)
                    {
                        menuItem.Enabled = false;
                    }
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (Directory.Exists(actorCachepath))
                                FileUtils.DeleteDirectory(actorCachepath);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Clear_common_browser_cache;
                if (Directory.Exists(commonCachePath) == false)
                {
                    menuItem.Enabled = false;
                }
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        if (Directory.Exists(commonCachePath))
                            FileUtils.DeleteDirectory(commonCachePath);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                subs2.Add(menuItem);
                //open folder
                if (isFile)
                {
                    CreateOpenInInternalBrowserContextMenu(subs2, ViewType.Simple, recоmmendedViewType, BrowserPrivateType.COMMON_CACHE,
                        url, webEntryInfo, contentContainer, selectedObject, webEntityInfoPropertyName);
                    CreateOpenInInternalBrowserContextMenu(subs2, ViewType.WORD, recоmmendedViewType, BrowserPrivateType.COMMON_CACHE,
                        url, webEntryInfo, contentContainer, selectedObject, webEntityInfoPropertyName);
                }

                //level 1 menu - open 
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Open_in_embedded_browser;
                menuItem.DropDownItems.AddRange(subs2.ToArray<ToolStripItem>());
                subs1.Add(menuItem);

                /////////////////////////////////
                /////Open in external browser
                //////////////////////////////

                //open external app level 2 
                subs2 = new List<ToolStripItem>();
                if (AppLocator.EdgePath != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_Microsoft_Edge;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            //open local files (or local uri to file) not supported
                            //https://stackoverflow.com/questions/34798285/how-can-i-open-a-local-html-file-in-microsoft-edge-browser
                            ProcessUtils.ExecuteProgram("cmd.exe", "/c start microsoft-edge:" + url);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (AppLocator.InternetExplorerPath != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_IE__deprecated_;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.InternetExplorerPath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);

                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_IE__deprecated_ + " (" + WebAccountLibRes.private_mode + ")";
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            //InPrivate Browsing helps prevent your browsing history, temporary Internet files, form data, cookies, and user names and passwords from being retained by the browser. You can start InPrivate Browsing from the Safety menu, by pressing Ctrl+Shift+P, or from the New Tab page. Internet Explorer will launch a new browser session that won’t keep any information about webpages you visit or searches you perform. Closing the browser window will end your InPrivate Browsing session.
                            ProcessUtils.ExecuteProgram(AppLocator.InternetExplorerPath, "-private " + uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (AppLocator.ChromePath != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_Chrome;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.ChromePath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_Chrome + " (" + WebAccountLibRes.private_mode + ")"; 
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.ChromePath, "-incognito " + uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (AppLocator.FirefoxPath != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_FireFox;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.FirefoxPath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (AppLocator.FirefoxProtablePath != null && File.Exists(AppLocator.FirefoxProtablePath))
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_FireFox + " " + WebAccountLibRes.Portable;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.FirefoxProtablePath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (AppLocator.ChromeProtablePath != null && File.Exists(AppLocator.ChromeProtablePath))
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_Chrome + " " + WebAccountLibRes.Portable;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.ChromeProtablePath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (AppLocator.OperaPath != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_Opera;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.OperaPath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (AppLocator.SafariPath != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_Safary;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.SafariPath, uri);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }
                if (webEntryInfo.Url != null && webEntryInfo.Login != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Open_in_IE__KeeForm__and_try_to_autocomplate_web_form__ + webEntryInfo.Login;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;

                            StringBuilder args = new StringBuilder();
                            args.Append(webEntryInfo.Url);
                            args.Append(" ");
                            args.Append(webEntryInfo.Login);
                            args.Append(" ");
                            args.Append(webEntryInfo.Password != null ? webEntryInfo.Password : "PASSWORD");
                            if (webEntryInfo.Password != null)
                            {
                                args.Append(" ");
                                //args.Append("{TAB}{TAB}{ENTERFORM}");
                                args.Append("{ENTERFORM}");
                            }
                            Console.WriteLine("Args: " + args.ToString());
                            ProcessUtils.ExecuteProgram("KeeForm.exe", args.ToString());

                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs2.Add(menuItem);
                }

                //level 1 menu - open 
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Open_in_external + "...";
                menuItem.DropDownItems.AddRange(subs2.ToArray<ToolStripItem>());
                subs1.Add(menuItem);

                OpenInOtherBrowsers(webEntryInfo, uri, subs1);
        

                //level 0 menu - open 
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Open_in + "...";
                menuItem.DropDownItems.AddRange(subs1.ToArray<ToolStripItem>());
                subs.Add(menuItem);

                //copy url 
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Copy + " " + oTitle + " " + WebAccountLibRes.to_clipboard + ": " + (url.Length <= 50 ? url : (url.Substring(0, 50) + "..."));
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(url, TextDataFormat.Text);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                subs.Add(menuItem);
                if (url.Equals(uri) == false)
                {
                    //copy uri 
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = WebAccountLibRes.Copy + " " + oTitle + " " + WebAccountLibRes.to_clipboard + ": " + (uri.Length <= 50 ? uri : (uri.Substring(0, 50) + "..."));
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            Clipboard.Clear();
                            Clipboard.SetText(uri, TextDataFormat.Text);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }
            }

            //////////////login password 
            if (webEntryInfo != null && webEntryInfo.Login != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Copy_login_to_clipboard + ": "+ webEntryInfo.Login;
                menuItem.Enabled = !string.IsNullOrEmpty(webEntryInfo.Login);
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(webEntryInfo.Login, TextDataFormat.Text);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                subs.Add(menuItem);
            }

            if (webEntryInfo != null && webEntryInfo.Password != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = WebAccountLibRes.Copy_password_to_clipboard;
                menuItem.Enabled = !string.IsNullOrEmpty(webEntryInfo.Password);
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(webEntryInfo.Password, TextDataFormat.Text);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                subs.Add(menuItem);
            }

            return subs;
        }
        static public ViewType GetViewTypeByFilePath(string path)
        {

            ViewType viewType = ViewType.NONE;
            if (new Uri(path).IsFile == false) viewType = ViewType.CefBrowser;
            else
            {
                if (FileUtils.CheckPathIsDirectory(path)) viewType = ViewType.CefBrowser;
                else
                {
                    FileInfo fi = new FileInfo(path);
                    string ext = fi.Extension;
                    if (".doc".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        viewType = ViewType.WORD;
                    }
                    else if (".docx".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        viewType = ViewType.WORD;

                    }
                    else if (".rtf".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        viewType = ViewType.WORD;
                    }
                    else if (".htm".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        viewType = ViewType.CefBrowser;
                    }
                    else if (".html".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        viewType = ViewType.CefBrowser;
                    }
                    else if (".txt".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        viewType = ViewType.WORD;
                        //viewType = ViewType.Simple;
                    }
                    else
                    {
                        viewType = ViewType.CefBrowser;
                    }
                }
            }
            return viewType;
        }


    }
}
