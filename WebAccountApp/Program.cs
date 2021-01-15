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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using FrwSoftware;

namespace Dionext
{
    static class Program
    {
        private static Log log;
        private static BaseApplicationContext applicationContext = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppManager.AjustVideoSetting();
            try
            {
                Form form = null;
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    if (!AppManager.CheckForSingleInstance()) return;
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    log = Log.GetLogger();
                    //http://stackoverflow.com/questions/8137070/force-application-close-on-system-shutdown
                    //SystemEvents can help you. The SessionEnding occurs when the user is trying to log off or shut down the system.
                    Microsoft.Win32.SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                    //If you don't want to cancel the event, but just react to it appropriately, you should handle the SystemEvents.SessionEnded event instead.
                    Microsoft.Win32.SystemEvents.SessionEnded += SystemEvents_SessionEnded;

                    //create instances of manager classes 
                    FrwConfig.Instance = new FrwSimpleWinCRUDConfig(); //WebAccountConfig();
                    Dm.Instance = new WebAccountDm();
                    AppManager.Instance = new WebAccountLibAppManager();
                    AppManager.Instance.MainAppFormType = typeof(WebAccountMainForm);
                    AppManager.Instance.DefaultViewWindowType = typeof(BrowserViewWindow);

                    //force load dlls with entities
                    VpnSelectorLibLoader.Load();
                    WebAccountLibLoader.Load();

                    AppManager.Instance.InitApplication();

                    JSetting setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
                    {
                        Name = "RunAppInWindowsTray",
                        Description = VpnSelectorLibRes.Run_application_in_windows_system_tray__next_run_,
                        Value = true,
                        IsUser = true,
                        IsAttachedToComputer = false
                    });
                    if (FrwConfig.Instance.GetPropertyValueAsBool(setting.Name, true))
                    {
                        applicationContext = new BaseApplicationContext();
                        applicationContext.DefaultTooltip = VpnSelectorLibRes.VPN_selector;
                        applicationContext.IconFileName = "bookmark_icon.ico";
                        applicationContext.Load();
                        BaseApplicationContext.NotifyIcon.Icon = VpnSelectorLibRes.vpn_off;
                        applicationContext.ThreadExit += ApplicationContext_ThreadExit;
                        Application.Run(applicationContext);
                    }
                    else
                    {
                        form = AppManager.Instance.LoadDocPanelContainersState(true);
                        form.FormClosing += Form_FormClosing;
                        form.FormClosed += Form_FormClosed;
                    }

                }
                catch (Exception ex)
                {
                    Log.ShowError("Error start app", ex);
                    Application.Exit();
                }
                if (form != null && !form.IsDisposed)
                {
                    Application.ThreadException += Application_ThreadException;
                    Application.Run(form);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError("Error running main app form", ex);
                MessageBox.Show("Unexpected error: " + ex);
                Application.Exit();
            }
        }
        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppManager.Instance.SaveAndClose((Form)sender);
        }

        private static void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                AppManager.Instance.DestroyApp();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            log.Error("OnApplicationThreadException", e.Exception);
        }
        private static void SystemEvents_SessionEnded(object sender, Microsoft.Win32.SessionEndedEventArgs e)
        {
        }

        private static void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            if (applicationContext != null) applicationContext.ExitThread();
        }
        private static void ApplicationContext_ThreadExit(object sender, EventArgs e)
        {
            try
            {
                JobManager.Instance.AbortAllJobsAndJobBatches();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
            AppManager.Instance.DestroyApp();
        }
    }
}
