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

namespace EmbeddedBrowser
{
    public partial class EmbeddedBrowserControl : UserControl
    {
        private WebEntryInfo webEntryInfo = null;

        //todo settings
        private const string DefaultUrlForAddedTabs = "https://www.google.com";

        // Default to a small increment:
        private const double ZoomIncrement = 0.10;

        private bool multiThreadedMessageLoopEnabled;

        private string initialUrl = null;

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
        }
        private void BrowserFormLoad(object sender, EventArgs e)
        {
            if (initialUrl != null) AddTab(initialUrl);
            else AddTab(DefaultUrlForAddedTabs);
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

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            //new AboutBox().ShowDialog();
        }

        private void FindMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.ShowFind();
            }
        }

        private void CopySourceToClipBoardAsyncClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.CopySourceToClipBoardAsync();
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
            AddTab(DefaultUrlForAddedTabs);
        }

        private void CloseTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            if(browserTabControl.Controls.Count == 0)
            {
                return;
            }

            var currentIndex = browserTabControl.SelectedIndex;

            var tabPage = browserTabControl.Controls[currentIndex];

            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Dispose();
            }

            browserTabControl.Controls.Remove(tabPage);

            tabPage.Dispose();

            browserTabControl.SelectedIndex = currentIndex - 1;

        }

        private void UndoMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Undo();
            }
        }

        private void RedoMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Redo();
            }
        }

        private void CutMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Cut();
            }
        }

        private void CopyMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Copy();
            }
        }

        private void PasteMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Paste();
            }
        }

        private void DeleteMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Delete();
            }
        }

        private void SelectAllMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.SelectAll();
            }
        }

        private void PrintToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Print();
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.ShowDevTools();
            }
        }

        private void CloseDevToolsMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.CloseDevTools();
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
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Load(url);
            }
        }


        private async void PrintToPdfToolStripMenuItemClick(object sender, EventArgs e)
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


        private void OpenHttpBinOrgToolStripMenuItemClick(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.Browser.Load("https://httpbin.org/");
            }
        }

        private void deleteCurrentCookiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.DeleteCookies(null, null, false);
            }
        }

        private void deleteGlobalCookiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var control = GetCurrentTabControl();
            if (control != null)
            {
                control.DeleteCookies(null, null, true);
            }

        }
    }
}
