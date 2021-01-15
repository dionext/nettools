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

using System.Threading.Tasks;
using System.Text;
using CefSharp.WinForms.Internals;
using EmbeddedBrowser.Properties;
using CefSharp;
using CefSharp.WinForms;
using FrwSoftware;
using System.Drawing;

using System.Runtime.InteropServices; //  Do not forget this namespace or else DllImport won't work       


namespace EmbeddedBrowser
{
    
    public partial class EmbeddedBrowserControl : UserControl
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;

        private WebEntryInfo webEntryInfo = null;

        //todo settings
        private const string DefaultUrlForAddedTabs = "https://www.google.com";

        // Default to a small increment:
        private const double ZoomIncrement = 0.10;

        private bool multiThreadedMessageLoopEnabled;

        private string initialUrl = null;
        private Image closeImage;
        private Image addImage;

        public EmbeddedBrowserControl(string initialUrl, WebEntryInfo webEntryInfo, bool multiThreadedMessageLoopEnabled)
        {
            InitializeComponent();
            this.initialUrl = initialUrl;
            this.webEntryInfo = webEntryInfo;
            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            Text = "CefSharp.WinForms - " + bitness;
            //WindowState = FormWindowState.Maximized;
       
            Load += BrowserFormLoad;

            //Only perform layout when control has completly finished resizing
            //todo
            //ResizeBegin += (s, e) => SuspendLayout();
            //ResizeEnd += (s, e) => ResumeLayout(true);

            this.multiThreadedMessageLoopEnabled = multiThreadedMessageLoopEnabled;

            //close button support 
            closeImage = new Bitmap(Resources.closeTab);
            addImage = new Bitmap(Resources.addTab);
            browserTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            browserTabControl.DrawItem += BrowserTabControl_DrawItem;
            browserTabControl.HandleCreated += BrowserTabControl_HandleCreated;
            //browserTabControl.SelectedIndexChanged += BrowserTabControl_SelectedIndexChanged;
            browserTabControl.MouseClick += BrowserTabControl_MouseClick;
            browserTabControl.Padding = new Point(10, 3);
        }

        //private Point _imageLocation = new Point(13, 5);
        //private Point _imgHitArea = new Point(13, 2);
        private static Rectangle GetRTLCoordinates(Rectangle container, Rectangle drawRectangle)
        {
            return new Rectangle(
                container.Width - drawRectangle.Width - drawRectangle.X,
                drawRectangle.Y,
                drawRectangle.Width,
                drawRectangle.Height);
        }
        private void BrowserTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                //e.Index -  The System.Windows.Forms.Control.ControlCollection index value of the item that is being drawn.
                if (e.Index > (browserTabControl.TabPages.Count - 1)) return;//xak to prevent error when adding new tab by click "+"

                var tabRect = browserTabControl.GetTabRect(e.Index);
                tabRect.Inflate(-2, -2);
                var imageRect = new Rectangle(tabRect.Right - closeImage.Width,
                                         tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                                         closeImage.Width,
                                         closeImage.Height);

                var sf = new StringFormat(StringFormat.GenericDefault);
                if (browserTabControl.RightToLeft == System.Windows.Forms.RightToLeft.Yes &&
                    browserTabControl.RightToLeftLayout == true)
                {
                    tabRect = GetRTLCoordinates(browserTabControl.ClientRectangle, tabRect);
                    imageRect = GetRTLCoordinates(browserTabControl.ClientRectangle, imageRect);
                    sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                }

                e.Graphics.DrawString(browserTabControl.TabPages[e.Index].Text + ((e.Index == browserTabControl.TabCount - 1)?"":"  "),
                                      this.Font, Brushes.Black, tabRect, sf);
                e.Graphics.DrawImage((e.Index == browserTabControl.TabCount - 1)?addImage : closeImage, imageRect.Location);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        private void BrowserTabControl_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                for (var i = 0; i < browserTabControl.TabPages.Count; i++)
                {
                    var tabRect = browserTabControl.GetTabRect(i);
                    tabRect.Inflate(-2, -2);
                    var imageRect = new Rectangle(tabRect.Right - closeImage.Width,
                                             tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                                             closeImage.Width,
                                             closeImage.Height);
                    if (imageRect.Contains(e.Location))
                    {
                        if (i != (browserTabControl.TabCount - 1))
                        {
                            CloseTab(i);
                        }
                        else
                        {
                            AddTabLocal(null);
                        }
                        //browserTabControl.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        private void CloseTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                if (browserTabControl.Controls.Count == 0)
                {
                    return;
                }

                var currentIndex = browserTabControl.SelectedIndex;
                CloseTab(currentIndex);
                browserTabControl.SelectedIndex = currentIndex - 1;
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void CloseTab(int currentIndex)
        {
            var tabPage = browserTabControl.TabPages[currentIndex];//.Controls[currentIndex];
            if (tabPage != null)
            {
                if (tabPage.Controls != null && tabPage.Controls.Count > 0 
                    && tabPage.Controls[0] is EmbeddedBrowserTabUserControl)
                {
                    var control = (EmbeddedBrowserTabUserControl)tabPage.Controls[0];
                    if (control != null)
                    {
                        control.Dispose();
                    }
                }
                //browserTabControl.Controls.Remove(tabPage);
                browserTabControl.TabPages.RemoveAt(currentIndex);
                tabPage.Dispose();
            }
        }

        private void BrowserTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // If the last TabPage is selected then Create a new TabPage
                if (browserTabControl.SelectedIndex == browserTabControl.TabPages.Count - 1)
                {
                    AddTabLocal(null);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void BrowserTabControl_HandleCreated(object sender, EventArgs e)
        {
            try
            {
                SendMessage(browserTabControl.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
   

        private void BrowserFormLoad(object sender, EventArgs e)
        {
            try
            {
                if (initialUrl != null) AddTab(initialUrl);
                else AddTab(DefaultUrlForAddedTabs);
                AddDummyTab();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void AddTab(string url, int? insertIndex = null)
        {
            //need Invoke for call from context menu
            if (browserTabControl.InvokeRequired)
            {
                browserTabControl.Invoke((MethodInvoker)delegate ()
                {
                    AddTabLocal(url, insertIndex);
                });
            }
            else
            {
                AddTabLocal(url, insertIndex);
            }
        }
        private void AddTabLocal(string url, int? insertIndex = null)
        {
            browserTabControl.SuspendLayout();

            var browser = new EmbeddedBrowserTabUserControl(AddTab, url, this.webEntryInfo, multiThreadedMessageLoopEnabled)
            {
                Dock = DockStyle.Fill,
            };

            var tabPage = new TabPage(url)
            {
                Dock = DockStyle.Fill
            };

            //This call isn't required for the sample to work. 
            //It's sole purpose is to demonstrate that #553 has been resolved.
            browser.CreateControl();

            tabPage.Controls.Add(browser);

            if (insertIndex == null)
            {
                if (browserTabControl.TabPages.Count > 0)
                    browserTabControl.TabPages.Insert((browserTabControl.TabPages.Count - 1), tabPage);
                else 
                    browserTabControl.TabPages.Add(tabPage);
            }
            else
            {
                browserTabControl.TabPages.Insert(insertIndex.Value, tabPage);
            }

            //Make newly created tab active
            browserTabControl.SelectedTab = tabPage;

            browserTabControl.ResumeLayout(true);
        }
        private void AddDummyTab(int? insertIndex = null)
        {
            browserTabControl.SuspendLayout();


            var tabPage = new TabPage("")
            {
                Dock = DockStyle.Fill
            };

            if (insertIndex == null)
            {
                browserTabControl.TabPages.Add(tabPage);
            }
            else
            {
                browserTabControl.TabPages.Insert(insertIndex.Value, tabPage);
            }

            //Make newly created tab active
            //browserTabControl.SelectedTab = tabPage;

            browserTabControl.ResumeLayout(true);
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                //new AboutBox().ShowDialog();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void FindMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.ShowFind();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void CopySourceToClipBoardAsyncClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.CopySourceToClipBoardAsync();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private EmbeddedBrowserTabUserControl GetCurrentTabControl()
        {
            if (browserTabControl.SelectedIndex == -1)
            {
                return null;
            }

            var tabPage = browserTabControl.Controls[browserTabControl.SelectedIndex];
            var control = (EmbeddedBrowserTabUserControl)tabPage.Controls[0];

            return control;
        }

        private void NewTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                AddTab(DefaultUrlForAddedTabs);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

   
        private void UndoMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Undo();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void RedoMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Redo();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void CutMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Cut();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Copy();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Paste();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void DeleteMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Delete();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void SelectAllMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void PrintToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Print();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.ShowDevTools();
                }

                //ver 75 
                /*
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    var isDevToolsOpen = await control.Browser.CheckIfDevToolsIsOpenAsync();
                    if (!isDevToolsOpen)
                    {
                        if (control.Browser.LifeSpanHandler != null)
                        {
                            control.Browser.ShowDevToolsDocked();
                        }
                    }
                }
                */
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void CloseDevToolsMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.CloseDevTools();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.ZoomIn();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.ZoomOut();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        public void Navigate(string url)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Load(url);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        private async void PrintToPdfToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    var dialog = new SaveFileDialog
                    {
                        DefaultExt = ".pdf",
                        Filter = EmbeddedBrowserControlRes.Pdf_documents___pdf____pdf
                    };

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        var success = await control.Browser.PrintToPdfAsync(dialog.FileName, new PdfPrintSettings
                        {
                            MarginType = CefPdfPrintMarginType.Custom,
                            MarginBottom = 10,
                            MarginTop = 0,
                            MarginLeft = 20,
                            MarginRight = 10
                        });

                        if (success)
                        {
                            MessageBox.Show(EmbeddedBrowserControlRes.Pdf_was_saved_to_ + dialog.FileName);
                        }
                        else
                        {
                            MessageBox.Show(EmbeddedBrowserControlRes.Unable_to_save_Pdf__check_you_have_write_permissions_to_ + dialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        private void OpenHttpBinOrgToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.Browser.Load("https://httpbin.org/");
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void deleteCurrentCookiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.DeleteCookies(null, null, false);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void deleteGlobalCookiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.DeleteCookies(null, null, true);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }

        private void ПоказатьCookieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var control = GetCurrentTabControl();
                if (control != null)
                {
                    control.ShowCookies(null, null, true);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
    }
}
