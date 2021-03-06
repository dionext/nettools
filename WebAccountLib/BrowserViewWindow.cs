﻿/**********************************************************************************
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
        static public  void CreateCefBrowser(BrowserPrivateType BrowserPrivateType, object LinkedObject, string WebEntityInfoPropertyName, string FileFullPath,  ref UserControl viewControl)
        {
            WebEntryInfo webEntryInfo = null;
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
            else
            {
                webEntryInfo = new WebEntryInfo();
            }
            //if (viewControl == null || !(viewControl is EmbeddedBrowserControl))
            //{
                webEntryInfo.BrowserPrivateType = BrowserPrivateType;
            if (BrowserPrivateType == BrowserPrivateType.PERSONAL_OLD_DISK_CACHE 
                //|| BrowserPrivateType == BrowserPrivateType.PERSONAL_NEW_DISK_CACHE
                )
            {
                string path = null;
                
                JActor actor = null;

                if (LinkedObject != null)
                {

                    var pl = LinkedObject.GetType().GetProperties();
                    Type at = typeof(JActor);
                    foreach (var p in pl)
                    {
                        if (p.PropertyType == at)
                        {
                            actor = AttrHelper.GetPropertyValue(LinkedObject, p) as JActor;
                            break;
                        }
                    }
                }
                //PropertyInfo actorProperty = AttrHelper.GetProperty(typeof(JActor), LinkedObject.GetType());
                //if (actorProperty != null) actor = AttrHelper.GetPropertyValue(LinkedObject, actorProperty) as JActor;
                if (actor != null)
                {
                    path = Dm.Instance.GetCacheFullPathForObjectUniqueForCompAndUser(actor);
                }
                else
                {
                    path = Dm.Instance.GetCacheFullPathForObjectUniqueForCompAndUser(LinkedObject);
                }
                webEntryInfo.CachePath = path;
            }
            else if (BrowserPrivateType == BrowserPrivateType.COMMON_CACHE)
            {
                string path = Dm.Instance.GetBrowserCommonCachePathUniqueForCompAndUser();
                webEntryInfo.CachePath = path;
            }
            viewControl = new EmbeddedBrowserControl(FileFullPath, webEntryInfo, EmbeddedBrowserHelper.MultiThreadedMessageLoop);
        }

        override public void ProcessView()
        {
            if (viewType != ViewType.NONE)
            {
                bool newControlCreated = false;
                if (viewType == ViewType.CefBrowser)
                {
                    if (viewControl == null || !(viewControl is EmbeddedBrowserControl))
                    {
                        CreateCefBrowser(BrowserPrivateType, LinkedObject, WebEntityInfoPropertyName, FileFullPath, ref viewControl);
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
