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

using CefSharp.WinForms.Internals;
using System.Security.Cryptography.X509Certificates;

using CefSharp;
using CefSharp.WinForms;
using FrwSoftware;

namespace EmbeddedBrowser
{
    public class WinFormsRequestHandler : JustRequestHandler
    {
        private Action<string, int?> openNewTab;

        public WinFormsRequestHandler(Action<string, int?> openNewTab, WebEntryInfo webEntryInfo)
        {
            this.openNewTab = openNewTab;
            this.WebEntryInfo = webEntryInfo;
        }

        protected override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            if(openNewTab == null)
            {
                return false;
            }

            var control = (Control)browserControl;

            control.InvokeOnUiThreadIfRequired(delegate ()
            {
                openNewTab(targetUrl, null);
            });		

            return true;
        }

        protected override bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            var control = (Control)browserControl;

            control.InvokeOnUiThreadIfRequired(delegate()
            {
                /*/////////////////////////////////////////////////////////////////////////

                var selectedCertificateCollection = X509Certificate2UI.SelectFromCollection(certificates, "Certificates Dialog", "Select Certificate for authentication", X509SelectionFlag.SingleSelection);

                //X509Certificate2UI.SelectFromCollection returns a collection, we've used SingleSelection, so just take the first
                //The underlying CEF implementation only accepts a single certificate
                callback.Select(selectedCertificateCollection[0]);
                */
            });            
            
            return true;
        }
    }
}
