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
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
namespace EmbeddedBrowser
{
    internal class MenuHandler : IContextMenuHandler
    {


        private const int OpenLinkInNewTab = 26501;
        private const int CloseDevTools = 26502;
        private const int MenuSaveImage = 26503;
        private const int ViewSource = 26504;
        private const int SaveYouTubeVideo = 26505;
        private const int ViewImageExifData = 26506;
        private const int ViewFacebookId = 26507;
        private const int CopyImgLocation = 26508;

        private const int ExtractAllLinks = 26510;
        private const int Bookmark = 26511;
        private const int ViewTwitterId = 26512;
        private const int SearchSelectedText = 26513;
        private const int SaveText = 26517;

        private const int ReverseImgSearchSubMenu = 26514;
        private const int ReverseImageSearchTineye = 26509;
        private const int ReverseImageSearchGoogle = 26515;
        private const int ReverseImageSearchYandex = 26516;

        private const int ShowDevTools = 26517;

        public event EventHandler DownloadImage = delegate { };
        public event EventHandler ViewPageSource = delegate { };
        public event EventHandler DownloadYouTubeVideo = delegate { };
        public event EventHandler ViewImageExif = delegate { };
        public event EventHandler ViewFacebookIdNum = delegate { };
        public event EventHandler ViewTwitterIdNum = delegate { };
        public event EventHandler OpenInNewTabContextMenu = delegate { };
        public event EventHandler CopyImageLocation = delegate { };
        public event EventHandler ReverseImgSearch = delegate { };
        public event EventHandler ExtractLinks = delegate { };
        public event EventHandler AddPageToBookmarks = delegate { };
        public event EventHandler SearchText = delegate { };
        public event EventHandler SaveSelectedText = delegate { };



        //private Action<string, int?> openNewTab;

        public MenuHandler()
        {
            //this.openNewTab = openNewTab;
        }

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            // model.Clear();

            //Removing existing menu item
            //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

            //Add new custom menu items
            model.AddItem((CefMenuCommand)ShowDevTools, EmbeddedBrowserControlRes.Show_DevTools);
            model.AddItem((CefMenuCommand)CloseDevTools, EmbeddedBrowserControlRes.Close_DevTools);

            //BufferLink = null;
            if (string.IsNullOrEmpty(parameters.UnfilteredLinkUrl) == false)
            {
                //BufferLink = parameters.LinkUrl;
                model.AddItem((CefMenuCommand)OpenLinkInNewTab, EmbeddedBrowserControlRes.Open_Link_In_New_Tab);
            }


            if (parameters.TypeFlags.HasFlag(ContextMenuType.Selection))
            {
                model.AddItem((CefMenuCommand)SearchSelectedText, EmbeddedBrowserControlRes.Search_selected_text_using_Google);
                model.AddItem((CefMenuCommand)SaveText, EmbeddedBrowserControlRes.Save_selected_text);
                model.AddSeparator();
            }


            if (parameters.TypeFlags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents)
            {
                if (EmbeddedBrowserHelper.HasJpegExtension(parameters.SourceUrl))
                {
                    //model.AddItem((CefMenuCommand)ViewImageExifData, "View image EXIF data");
                }
                //model.AddItem((CefMenuCommand)MenuSaveImage, "Save image");
                model.AddItem((CefMenuCommand)CopyImgLocation, EmbeddedBrowserControlRes.Copy_image_location_to_clipboard);

                var sub = model.AddSubMenu((CefMenuCommand)ReverseImgSearchSubMenu, EmbeddedBrowserControlRes.Reverse_image_search_tools);
                sub.AddItem((CefMenuCommand)ReverseImageSearchTineye, EmbeddedBrowserControlRes.Reverse_image_search_using_TinEye);
                //sub.AddItem((CefMenuCommand)ReverseImageSearchYandex, EmbeddedBrowserControlRes.Reverse_image_search_using_Yandex);
                sub.AddItem((CefMenuCommand)ReverseImageSearchGoogle, EmbeddedBrowserControlRes.Reverse_image_search_using_Google);
                model.AddSeparator();
                //
            }
            if (EmbeddedBrowserHelper.IsOnYouTube(browserControl.Address))
            {
                //model.AddItem((CefMenuCommand)SaveYouTubeVideo, "Extract YouTube video");
            }
            if (EmbeddedBrowserHelper.IsOnFacebook(browserControl.Address))
            {
                //model.AddItem((CefMenuCommand)ViewFacebookId, "Show Facebook profile ID");
            }
            if (EmbeddedBrowserHelper.IsOnTwitter(browserControl.Address))
            {
                //model.AddItem((CefMenuCommand)ViewTwitterId, "Show Twitter profile ID");
            }

            //model.AddItem((CefMenuCommand)ViewSource, "*View page source");
            //model.AddItem((CefMenuCommand)ExtractAllLinks, "Extract all links on page");
            //model.AddItem((CefMenuCommand)Bookmark, "Add page to bookmarks");
            string s = EmbeddedBrowserControlRes.View_source;
            string s2 = EmbeddedBrowserControlRes.Forward;

            model.SetLabel(CefMenuCommand.ViewSource, EmbeddedBrowserControlRes.View_source);
            model.SetLabel(CefMenuCommand.Print, EmbeddedBrowserControlRes.Print);
            model.SetLabel(CefMenuCommand.Undo, EmbeddedBrowserControlRes.Undo);
            model.SetLabel(CefMenuCommand.Redo, EmbeddedBrowserControlRes.Redo);
            model.SetLabel(CefMenuCommand.Forward, EmbeddedBrowserControlRes.Forward);
            model.SetLabel(CefMenuCommand.Back, EmbeddedBrowserControlRes.Back);
            /*
            model.Remove(CefMenuCommand.ViewSource);
            model.Remove(CefMenuCommand.Print);
            model.Remove(CefMenuCommand.Undo);
            model.Remove(CefMenuCommand.Redo);
            model.Remove(CefMenuCommand.Forward);
            model.Remove(CefMenuCommand.Back);
            */
        }
        //private string BufferLink = null; 
        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int)commandId == ShowDevTools)
            {
                browser.ShowDevTools();
            }
            else if ((int)commandId == CloseDevTools)
            {
                browser.CloseDevTools();
            }
            /*
            else if ((int)commandId == OpenLinkInNewTab)
            {
                if (BufferLink != null)
                {
                    openNewTab(BufferLink, null);
                }
            }
            */
            if ((int)commandId == OpenLinkInNewTab)
            {
                OpenInNewTabContextMenu?.Invoke(this, new TextEventArgs(parameters.UnfilteredLinkUrl));
            }
            if ((int)commandId == CloseDevTools)
            {
                browser.CloseDevTools();
            }
            if ((int)commandId == MenuSaveImage)
            {
                DownloadImage?.Invoke(this, new TextEventArgs(parameters.SourceUrl));
            }
            if ((int)commandId == ViewSource)
            {
                ViewPageSource?.Invoke(this, null);
            }
            //if ((int)commandId == SaveYouTubeVideo)
            {
                DownloadYouTubeVideo?.Invoke(this, null);   //we have the address, anyway, so don't need to pass it via event args.
            }
            if ((int)commandId == ViewImageExifData)
            {
                ViewImageExif?.Invoke(this, new TextEventArgs(parameters.SourceUrl));
            }
            if ((int)commandId == ViewFacebookId)
            {
                ViewFacebookIdNum?.Invoke(this, EventArgs.Empty);
            }
            if ((int)commandId == ViewTwitterId)
            {
                ViewTwitterIdNum?.Invoke(this, EventArgs.Empty);
            }
            if ((int)commandId == CopyImgLocation)
            {
                CopyImageLocation?.Invoke(this, new TextEventArgs(parameters.SourceUrl));
            }

            if ((int)commandId == ReverseImageSearchTineye)
            {
                ReverseImgSearch?.Invoke(this, new TextEventArgs("http://www.tineye.com/search/?url=" + parameters.SourceUrl));
            }
            if ((int)commandId == ReverseImageSearchGoogle)
            {
                ReverseImgSearch?.Invoke(this, new TextEventArgs("https://www.google.com/searchbyimage?&image_url=" + Uri.EscapeUriString(parameters.SourceUrl)));
            }
            if ((int)commandId == ReverseImageSearchYandex)
            {
                ReverseImgSearch?.Invoke(this, new TextEventArgs("https://yandex.com/images/search?url=" + Uri.EscapeUriString(parameters.SourceUrl) + "&rpt=imageview"));
            }

            if ((int)commandId == ExtractAllLinks)
            {
                ExtractLinks?.Invoke(this, EventArgs.Empty);
            }
            if ((int)commandId == Bookmark)
            {
                AddPageToBookmarks?.Invoke(this, EventArgs.Empty);
            }
            if ((int)commandId == SearchSelectedText)
            {
                SearchText?.Invoke(this, new TextEventArgs(parameters.SelectionText));
            }
            if ((int)commandId == SaveText)
            {
                SaveSelectedText?.Invoke(this, new TextEventArgs(parameters.SelectionText));
            }

            return false;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
    public class TextEventArgs : EventArgs
    {
        public string Result { get; set; }

        public TextEventArgs(string result)
        {
            Result = result;
        }


    }
}