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
    [JEntityPlugin(typeof(JVPNServer))]
    public class JVPNServerBasePlugin : IFormsEntityPlugin
    {
        private JJobType currQueueJobType = null;

        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            //base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);
            menuItemList.Add(new ToolStripSeparator());

            JVPNServer item = (JVPNServer)selectedObject;
            List<ToolStripItem> subitems = new List<ToolStripItem>();
            if (item != null) VpnConnUtils.MakeContextMenuForBaseVPNServer(subitems, item);
            VpnConnUtils.MakeContextMenuForAllBaseVPNServers(subitems);
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
                menuItem.Text = ((list.SelectedObjects.Count > 1) ? VpnSelectorLibRes.Run_test_for_selected_VPN_servers : VpnSelectorLibRes.Run_test_for_this_VPN_server);
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        ScheduleJobChekVpnAll(list, list.SelectedObjects, false, false);
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
                    JobManager.MakeTitleContextMenuForRunningJob(item.JRunningJob, subitems2, list.ContentContainer);
                    groupItem.DropDownItems.AddRange(subitems2.ToArray<ToolStripItem>());
                }
                if (currQueueJobType != null)
                {
                    List<ToolStripItem> subitems22 = new List<ToolStripItem>();
                    JobManager.MakeTitleContextMenuForRunningJobTypeQueue(currQueueJobType, subitems22, list.ContentContainer);
                    groupItem.DropDownItems.AddRange(subitems22.ToArray<ToolStripItem>());
                }
            }
            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Run_test_for_favorite_VPN_servers;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    ScheduleJobChekVpnAll(list, null, false, true);
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
                    ScheduleJobChekVpnAll(list, null, true, false);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            groupItem.DropDownItems.Add(menuItem);
        }
        private void ScheduleJobChekVpnAll(IListProcessor blist, IList selectedItems, bool all, bool favoritOnly)
        {
            blist.StartRefreshing();
            // create background tasks, which nevertheless are executed sequentially
            // it would be possible to create one background thread and perform all the tasks sequentially,
            // but at the same time the opportunity to destroy the hanging task is lost and to let the others continue
            // todo worth thinking about limiting the number of waiting threads

            //prepare task list
            IList list = null;
            if (all) list = Dm.Instance.FindAll(typeof(JVPNServer));
            else if (favoritOnly)
            {
                IList list0 = Dm.Instance.FindAll(typeof(JVPNServer));
                list = new List<JVPNServer>();
                foreach (var n in list0)
                {
                    JVPNServer m = (JVPNServer)n;
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
                JVPNServer m = (JVPNServer)n;
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
                JVPNServer m = (JVPNServer)n;
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

            //Schedule tasks
            foreach (var n in list)
            {
                JVPNServer m = (JVPNServer)n;
                ScheduleJobChekVpnServer(m);
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

        private static void ScheduleJobChekVpnServer(JVPNServer item)
        {
            JRunningJob job = null;
            job = JobManager.Instance.CreateJob("ChekVpn", VpnSelectorLibRes.Test_for_VPN_server, item.Url, JobConcurrentTypeEnum.Wait);
            job.DoJob += (sd, ew) =>
            {
                job.JobLog.Info("Plan VPN server test for " + item.Url);
                bool res = VpnConnUtils.TestConnectionWithConfirmation(item, job.JobLog);
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
