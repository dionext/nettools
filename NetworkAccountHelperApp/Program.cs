using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Dionext;


namespace FrwSoftware
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
                    //Microsoft.Win32.SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                    //If you don't want to cancel the event, but just react to it appropriately, you should handle the SystemEvents.SessionEnded event instead.
                    //Microsoft.Win32.SystemEvents.SessionEnded += SystemEvents_SessionEnded;

                    //create instances of manager classes 
                    FrwConfig.Instance = new FrwSimpleWinCRUDConfig();// WebAccountConfig();
                    Dm.Instance = new NADm();
                    AppManager.Instance = new NAAppManager();
                    AppManager.Instance.MainAppFormType = typeof(NetworkAccountHelperMainForm);
                    AppManager.Instance.DefaultViewWindowType = typeof(BrowserViewWindow);


                    //force load dlls with entities
                    VpnSelectorLibLoader.Load();
                    WebAccountLibLoader.Load();
                    NetworkAccountHelperLibLoader.Load();

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
                        applicationContext.IconFileName = "tools_icon.ico";// "bookmark_icon.ico";
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
