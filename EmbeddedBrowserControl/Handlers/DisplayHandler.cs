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
using System.Collections.Generic;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms.Internals;

using CefSharp.WinForms;
using CefSharp.Structs;

namespace EmbeddedBrowser
{
    public class DisplayHandler : IDisplayHandler
    {
        private Control parent;
        private Form fullScreenForm;

        public void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
        {

        }

        public bool OnAutoResize(IWebBrowser browserControl, IBrowser browser, Size newSize)
        {
            return false;
        }

        void IDisplayHandler.OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs)
        {
            
        }

        void IDisplayHandler.OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs)
        {
            
        }

        void IDisplayHandler.OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls)
        {
            
        }

        void IDisplayHandler.OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen)
        {
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            chromiumWebBrowser.InvokeOnUiThreadIfRequired(() =>
            {
                if (fullscreen)
                {
                    parent = chromiumWebBrowser.Parent;

                    parent.Controls.Remove(chromiumWebBrowser);

                    fullScreenForm = new Form();
                    fullScreenForm.FormBorderStyle = FormBorderStyle.None;
                    fullScreenForm.WindowState = FormWindowState.Maximized;

                    fullScreenForm.Controls.Add(chromiumWebBrowser);

                    fullScreenForm.ShowDialog(parent.FindForm());
                }
                else
                {
                    fullScreenForm.Controls.Remove(chromiumWebBrowser);

                    parent.Controls.Add(chromiumWebBrowser);

                    fullScreenForm.Close();
                    fullScreenForm.Dispose();
                    fullScreenForm = null;
                }
            });
        }

        //
        // Summary:
        //     Called when the browser is about to display a tooltip. text contains the text
        //     that will be displayed in the tooltip. You can optionally modify text and then
        //     return false to allow the browser to display the tooltip. When window rendering
        //     is disabled the application is responsible for drawing tooltips and the return
        //     value is ignored.
        //
        // Parameters:
        //   browserControl:
        //     The ChromiumWebBrowser control
        //
        //   text:
        //     the text that will be displayed in the tooltip
        //
        // Returns:
        //     To handle the display of the tooltip yourself return true otherwise return false
        //     to allow the browser to display the tooltip.
        //
        // Remarks:
        //     Option to modify tooltip is not currently implemented.
        //bool OnTooltipChanged(IWebBrowser browserControl, string text);
        //bool IDisplayHandler.OnTooltipChanged(IWebBrowser browserControl, string text)
        //{
        //text = "Sample text";
        //   return false;
        //}
        public bool OnTooltipChanged(IWebBrowser browserControl, ref string text)
        {
            return false;
        }

        void IDisplayHandler.OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs)
        {
            
        }

        bool IDisplayHandler.OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs)
        {
            return false;
        }
    }
}
