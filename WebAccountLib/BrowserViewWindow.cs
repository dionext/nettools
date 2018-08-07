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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using EmbeddedBrowser;
using FrwSoftware;

namespace Dionext
{
    public partial class BrowserViewWindow : BaseViewWindow
    {
        public BrowserViewWindow()
        {
            InitializeComponent();
        }
        override public void ProcessView()
        {
            //if (viewControl != null) throw new InvalidOperationException();
            //if (viewProcessed) throw new InvalidOperationException();
            //else viewProcessed = true;

            WebEntryInfo webEntryInfo = null;
            //WebEntryInfo webEntryInfo = WebEntryInfo.GetWebEntryInfoFromObject(LinkedObject);
            if (LinkedObject != null)
            {
                IList<WebEntryInfoWrap> webEntryInfos = WebEntryInfo.GetWebEntryInfosFromObject(LinkedObject);
                if (webEntryInfos.Count > 1)
                {
                    if (WebEntityInfoPropertyName == null) throw new Exception("WebEntityInfoPropertyName not set, but found more than one WebEntityInfo");
                    else
                    {
                        WebEntryInfoWrap webEntryInfoWrap = webEntryInfos.Where<WebEntryInfoWrap>(s => s.Property.Name == WebEntityInfoPropertyName).FirstOrDefault();
                        if (webEntryInfoWrap == null) throw new Exception("Not found WebEntityInfo with specified name: " + WebEntityInfoPropertyName);
                        else webEntryInfo = webEntryInfoWrap.WebEntryInfo;
                    }
                }
                else if (webEntryInfos.Count == 1)
                {
                    webEntryInfo = webEntryInfos[0].WebEntryInfo;
                }
            }
            if (viewType != ViewType.NONE)
            {
                bool newControlCreated = false;
                if (viewType == ViewType.CefBrowser)
                {
                    if (viewControl == null || !(viewControl is EmbeddedBrowserControl))
                    {
                        webEntryInfo.BrowserPrivateType = BrowserPrivateType;
                        if (BrowserPrivateType == BrowserPrivateType.PERSONAL_OLD_DISK_CACHE ||
                            BrowserPrivateType == BrowserPrivateType.PERSONAL_NEW_DISK_CACHE)
                        {
                            string path = Dm.Instance.GetCacheFullPathForObject(LinkedObject);
                            webEntryInfo.CachePath = path;
                        }
                        else if (BrowserPrivateType == BrowserPrivateType.COMMON_CACHE)
                        {
                            string path = Path.Combine(Dm.Instance.GetCommonCachePath(), EmbeddedBrowserHelper.BROWSER_CACHE_PATH);
                            webEntryInfo.CachePath = path;
                        }
                        viewControl = new EmbeddedBrowserControl(FileFullPath, webEntryInfo, true);
                        newControlCreated = true;
                    }
                }
                else if (viewType == ViewType.Simple)
                {
                    if (viewControl == null || !(viewControl is SimpleViewControl))
                    {
                        viewControl = new SimpleViewControl();
                        newControlCreated = true;
                    }
                }
                else
                    throw new InvalidOperationException();

                if (newControlCreated)
                {
                    AddSpecialTask();//todo Заваисмость от урл? 
                    viewControl.Dock = DockStyle.Fill;
                    this.Controls.Add(viewControl);
                }

                if (FileFullPath != null)
                {
                    if (viewType == ViewType.Simple)
                    {
                        //todo (SimpleViewControl)viewControl;
                    }
                    else if (viewType == ViewType.CefBrowser)
                    {
                        //((CefBrowserControl)viewControl).Navigate(FileFullPath);
                    }
                    else
                        throw new InvalidOperationException();
                }

            }
            string cap = null;
            if (LinkedObject != null) cap = ModelHelper.GetNameForObject(LinkedObject);
            else if (FileFullPath != null) cap = FileFullPath;
            if (cap != null && cap.Length > 200) cap = cap.Substring(0, 200) + "...";
            if (cap != null) SetNewCaption(cap);
        }



        public override void ClosingContent()
        {
            try
            {
                if (viewControl is EmbeddedBrowserControl)
                {
                    ((EmbeddedBrowserControl)viewControl).Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

    }
}
