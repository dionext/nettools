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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Reflection;
using System.Threading;
using FrwSoftware;

namespace Dionext
{
    public partial class JProxyServerListWindow : SimpleListWindow
    {

        private JJobType currQueueJobType = null;

        public JProxyServerListWindow()
        {
            InitializeComponent();
        }
        override protected void MakeListColumns()
        {
            base.MakeListColumns();
            listView.UseCellFormatEvents = true;
            listView.FormatCell += ListView_FormatCell;
        }
        private void ListView_FormatCell(object sender, FormatCellEventArgs e)
        {
            object item = e.Model;
            if (item != null)
            {
                PropertyInfo p = item.GetType().GetProperty("IsArchive");
                if (p != null && p.PropertyType == typeof(bool) && (bool)(p.GetValue(item)) == true)
                    e.SubItem.ForeColor = Color.Gray;
            }
        }
        override protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);
            menuItemList.Add(new ToolStripSeparator());

            BaseProxyServer item = (BaseProxyServer)selectedObject;
            List<ToolStripItem> subitems = new List<ToolStripItem>();
            NetConnUtils.MakeContextMenuForBaseProxyServer(subitems, item);
            NetConnUtils.MakeContextMenuForAllBaseProxyServers(subitems);
            menuItemList.AddRange(subitems.ToArray());

            ToolStripMenuItem groupItem = null;
            ToolStripMenuItem menuItem = null;

            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Import_VPN_servers_from_vpngate_net;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    VpngateImporter.DowloadAndParse();
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemList.Add(menuItem);


            groupItem = new ToolStripMenuItem();
            groupItem.Text = VpnSelectorLibRes.Run_test;
            menuItemList.Add(groupItem);
            
            if (item != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = ((SelectedObjects.Count > 1) ? VpnSelectorLibRes.Run_test_for_selected_VPN_servers : VpnSelectorLibRes.Run_test_for_this_VPN_server);
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        ScheduleJobChekVpnAll(SelectedObjects, false, false);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                groupItem.DropDownItems.Add(menuItem);

                if (item.JRunningJob != null && item.JRunningJob.JJobType != null)//JRunningJob.JJobType = null for manualy deleted job
                {
                    List<ToolStripItem> subitems2 = new List<ToolStripItem>();
                    JobManager.MakeTitleContextMenuForRunningJob(item.JRunningJob, subitems2, this.ContentContainer);
                    groupItem.DropDownItems.AddRange(subitems2.ToArray<ToolStripItem>());
                }
                if (currQueueJobType != null)
                {
                    List<ToolStripItem> subitems22 = new List<ToolStripItem>();
                    JobManager.MakeTitleContextMenuForRunningJobTypeQueue(currQueueJobType, subitems22, this.ContentContainer);
                    groupItem.DropDownItems.AddRange(subitems22.ToArray<ToolStripItem>());
                }
            }
            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Run_test_for_favorite_VPN_servers;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    ScheduleJobChekVpnAll(null,  false, true);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            groupItem.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Run_test_for_all_VPN_servers;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    ScheduleJobChekVpnAll(null, true, false);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            groupItem.DropDownItems.Add(menuItem);
        }
        private void ScheduleJobChekVpnAll(IList selectedItems, bool all, bool favoritOnly)
        {
            StartRefreshing();
            // create background tasks, which nevertheless are executed sequentially
            // it would be possible to create one background thread and perform all the tasks sequentially,
            // but at the same time the opportunity to destroy the hanging task is lost and to let the others continue
            // todo worth thinking about limiting the number of waiting threads

            //prepare task list
            IList list = null;
            if (all) list = Dm.Instance.FindAll(BaseProxyServer.CurrentType);
            else if (favoritOnly)
            {
                IList list0 = Dm.Instance.FindAll(BaseProxyServer.CurrentType);
                list = new List<BaseProxyServer>();
                foreach (var n in list0)
                {
                    BaseProxyServer m = (BaseProxyServer)n;
                    if (m.Favorite)
                    {
                        list.Add(m);
                    }
                }
            }
            else list = selectedItems;
            if (list == null || list.Count == 0) throw new Exception("No items to test found");

            //check for allready running tasks and stop it
            bool foundWorking = false;
            foreach (var n in list)
            {
                BaseProxyServer m = (BaseProxyServer)n;
                if (m.JRunningJob != null && m.JRunningJob.JJobType != null && m.JRunningJob.IsWorking())
                {
                    foundWorking = true;
                    break;
                }
            }
            if (foundWorking)
            {
                DialogResult res = MessageBox.Show(VpnSelectorLibRes.Running_job_found__They_will_be_aborted_or_press__Cancel__to_exit, FrwConstants.WARNING, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel)
                {
                    return;
                }
            }
            foreach (var n in list)
            {
                BaseProxyServer m = (BaseProxyServer)n;
                if (favoritOnly == false || m.Favorite)
                {
                    if (m.JRunningJob != null)
                    {
                        if (m.JRunningJob.JJobType != null)
                        {
                            if (m.JRunningJob.IsWorking())
                            {
                                JobManager.Instance.AbortJob(m.JRunningJob);
                            }
                            else
                            {
                                //m.JRunningJob.Stage = RunningJobStageEnum.initial.ToString();
                                //Dm.Instance.SaveObject(m.JRunningJob);
                            }
                        }
                        m.JRunningJob = null;
                        Dm.Instance.SaveObject(m);
                    }
                }
            }
            string homeIP = null;
            
            //close active vpns
            if (NetConnUtils.IsActiveConnectionPresent())
            {
                NetConnUtils.CloseAllActiveConnections(false);
                homeIP = NetConnUtils.GetMyExternalIP();
            }
            else homeIP = NetConnUtils.MyExternalIP;
            
            //Schedule tasks
            foreach (var n in list)
            {
                BaseProxyServer m = (BaseProxyServer)n;
                ScheduleJobChekVpnServer(m, homeIP);
                JobManager.ScheduleJobToQueue(m.JRunningJob);
                currQueueJobType = m.JRunningJob.JJobType;
            }
            if (currQueueJobType != null)
            {
                currQueueJobType.RemoveAllPostJobBatchEventHandlers();
                currQueueJobType.PostJobBatch += currQueueJobType.StandartPostLJobBatchEventHandler;
                JobManager.StartProcessingJobBatch(currQueueJobType);
            }
        }
        static Random rnd = new Random();
        private static void ScheduleJobChekVpnServer(BaseProxyServer item, string homeIP)
        {
            JRunningJob job = null;
            job = JobManager.Instance.CreateJob("ChekVpn", VpnSelectorLibRes.Test_for_VPN_server, item.Url, JobConcurrentTypeEnum.Wait);
            job.DoJob += (sd, ew) =>
            {
                job.JobLog.Info("Plan VPN server test for " + item.Url);
                bool res = NetConnUtils.ConnectWithConfirmation(item, homeIP, job.JobLog);
                Thread.Sleep(5 * 1000);
                job.JobLog.Info("End VPN server test for " + item.Url);
                JobManager.Instance.ReportProgresJob(job, 100);
                if (res) ew.StageResult = RunningJobResultEnum.ok;
                else ew.StageResult = RunningJobResultEnum.error;
            };
            item.JRunningJob = job;
            Dm.Instance.SaveObject(item);

        }
    }
}
