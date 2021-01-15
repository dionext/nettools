using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Компьюретное хранилище")]
    [JEntity(ImageName = "fr_storage", Resource = typeof(NetworkAccountHelperLibRes))]
    public class JCompDeviceStorage
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JCompDeviceStorageId { get; set; }

        [JDisplayName("Наименование")]
        [JNameProperty, JRequired]
        public string Name { get; set; }

        [JDisplayName("Компьютер")]
        [JManyToOne]
        public JCompDevice JCompDevice { get; set; }

        //
        [JDisplayName("Сайт")]
        [JManyToOne]
        public JSite JSite { get; set; }

        //
        [JDisplayName("Размер")]
        public string Size { get; set; }

        [JDisplayName("Краткое описание")]
        public string Description { get; set; }

        [JDisplayName("Путь")]
        public string Path { get; set; }

        //
        [JDisplayName("Подключаемый")]
        public bool Connectable { get; set; }

        [JDisplayName("Физическое устройство для монтирования")]
        public string DevicePath { get; set; }

        [JDisplayName("Samba имя")]
        public string SambaName { get; set; }

        [JDisplayName("Samba опции")]
        public string SambaOptions { get; set; }

        [JDisplayName("Путь к контейнеру")]
        public string ContainerPath { get; set; }

        [JDisplayName("Опции FS контейнера")]
        public string ContainerFS { get; set; }

        [JDisplayName("Мастер хранилище")]
        [JManyToOne]
        public JCompDeviceStorage MasterStorage { get; set; }

        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo();
                if (JCompDevice != null) w = JCompDevice.WebEntryInfo;
                w.Path = Path;
                return w;
            }
        }


        public static string MakeSambaConfigBlock(JCompDeviceStorage storage)
        {
            StringBuilder str = new StringBuilder();
            str.Append("[" + storage.SambaName + "]");
            HtmlUtils.AppendDosBr(str);
            str.Append("comment = " + storage.Name);
            HtmlUtils.AppendDosBr(str);
            str.Append("path = " + storage.Path);
            if (string.IsNullOrEmpty(storage.SambaOptions))
            {
                HtmlUtils.AppendDosBr(str);
                str.Append("read only = no");
                HtmlUtils.AppendDosBr(str);
                str.Append("locking = no");
                HtmlUtils.AppendDosBr(str);
                str.Append("guest ok = yes");
            }
            else
            {
                HtmlUtils.AppendDosBr(str);
                str.Append(storage.SambaOptions);
            }
            HtmlUtils.AppendDosBr(str);
            return str.ToString();
        }


        public static string MakeMountScript(JCompDeviceStorage storage)
        {
            StringBuilder str = new StringBuilder();
            str.Append("mount " + storage.DevicePath + " " + storage.Path + " -t auto -o rw,nls=utf8,noatime");
            return str.ToString();
        }
        public static string MakeUnMountScript(JCompDeviceStorage storage)
        {
            StringBuilder str = new StringBuilder();
            str.Append("umount " + storage.Path);
            return str.ToString();
        }

        public static string MakeSambaClientPrepareScript(JCompDeviceStorage storage)
        {
            StringBuilder str = new StringBuilder();
            str.Append("mkdir /mnt/" + storage.SambaName);
            HtmlUtils.AppendDosBr(str);
            str.Append("chmod 0777 /mnt/" + storage.SambaName);
            HtmlUtils.AppendDosBr(str);
            return str.ToString();
        }
        public static string MakeSambaClientConnectScript(JCompDeviceStorage storage)
        {
            StringBuilder str = new StringBuilder();
            str.Append("mount -t cifs -o username=root,password= //" + storage.JCompDevice.GetInternalAddress() + "/" + storage.SambaName + "/mnt/" + storage.SambaName);
            HtmlUtils.AppendDosBr(str);
            return str.ToString();
        }
        public static string MakeSambaClientConnectFstabScript(JCompDeviceStorage storage)
        {
            StringBuilder str = new StringBuilder();
            str.Append("//" + storage.JCompDevice.GetInternalAddress() + "/" + storage.SambaName + " /mnt/" + storage.SambaName + " cifs username=root,password= 0 0");
            HtmlUtils.AppendDosBr(str);
            return str.ToString();
        }

        public static string MakeRsyncScript(JCompDeviceStorage storage, JCompDeviceStorage master, bool full)
        {
            //JCompDeviceStorage master = storage.MasterStorage;
            bool sameComp = (storage.JCompDevice == master.JCompDevice);
            StringBuilder str = new StringBuilder();
            str.Append("rsync -rvz --progress --delete");
            if (!sameComp && master.JCompDevice.WebEntryInfo.ExtPortSSH != null)
            {
                str.Append(" -e \'ssh -p " + master.JCompDevice.WebEntryInfo.ExtPortSSH + "\'");
            }
            if (!sameComp && full)
            {
                str.Append(" -i /root/.ssh/id_rsa -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null'");
            }
            str.Append(" ");
            str.Append(storage.Path);
            if (storage.Path.EndsWith("/") == false) str.Append("/");
            str.Append(" ");
            if (!sameComp)
            {
                str.Append(master.JCompDevice.Login);
                str.Append("@");
                str.Append(master.JCompDevice.JCompDeviceNetworkParentLevel.GetExternalAddress());
                str.Append(":");
            }
            str.Append(master.Path);
            if (master.Path.EndsWith("/") == false) str.Append("/");
            return str.ToString();
        }
        static private void AppendBr(StringBuilder str)
        {
            str.Append("<br/>");
        }
        static private void AppendDosBr(StringBuilder str)
        {
            str.Append("\n\r");
        }
        static public void ShowSummaryInfo(JCompDeviceStorage storage)
        {
            StringBuilder str = new StringBuilder();

            str.Append(ModelHelper.ModelPropertyList(storage, "<br/>", null, null));

            //str.Append(phone.Number);
            AppendBr(str);
            AppendBr(str);
            str.Append(storage.Name);
            AppendBr(str);
            AppendBr(str);
            if (storage.JCompDevice != null && storage.JCompDevice.Description != null)
            {
                AppendBr(str);
                str.Append(storage.JCompDevice.Name + storage.JCompDevice.GetInternalAddress());
                AppendBr(str);
                str.Append(storage.JCompDevice.Description);
                AppendBr(str);
            }

            if (!string.IsNullOrEmpty(storage.SambaName))
            {
                AppendBr(str);
                str.Append("Samba config for server");
                AppendBr(str);
                str.Append("<pre>");
                str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeSambaConfigBlock(storage)));
                str.Append("</pre>");
                str.Append("Samba prepare for client");
                AppendBr(str);
                str.Append("<pre>");

                if (storage.JCompDevice != null && !string.IsNullOrEmpty(storage.JCompDevice.GetInternalAddress()))
                {
                    str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeSambaClientPrepareScript(storage)));
                    str.Append("</pre>");
                    str.Append("Samba connect for client (/etc/init.d/rc.local)");
                    AppendBr(str);
                    str.Append("<pre>");
                    str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeSambaClientConnectScript(storage)));
                    str.Append("</pre>");
                    str.Append("Samba connect for client fstab (/etc/fstab )");
                    AppendBr(str);
                    str.Append("<pre>");
                    str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeSambaClientConnectFstabScript(storage)));
                    str.Append("</pre>");
                }
            }

            if (!string.IsNullOrEmpty(storage.DevicePath))
            {
                AppendBr(str);
                str.Append("Cкрипт монтирования физического устройства");
                AppendBr(str);
                str.Append("<pre>");
                str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeMountScript(storage)));
                str.Append("</pre>");
                AppendBr(str);
                str.Append("Cкрипт размонтирования физического устройства");
                AppendBr(str);
                str.Append("<pre>");
                str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeUnMountScript(storage)));
                str.Append("</pre>");
            }


            if (storage.MasterStorage != null)
            {
                JCompDeviceStorage master = storage.MasterStorage;
                AppendBr(str);
                str.Append("Синхронизация односторонняя. Файлы, которых нет на базе, будут удалены на целевом. Файлы базы, которых нет на целевом, будут туда добавлены");
                AppendBr(str);


                str.Append("Основной скрипт синхронизации базы мастер хранилища (запускать на нем) \"" + master.JCompDevice.Name + "\"  c слейв-хранилищем   \"" + storage.JCompDevice.Name + "\" (добавить -n для теста)");
                AppendBr(str);
                str.Append("<pre>");
                str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeRsyncScript(master, storage, false)));
                str.Append("</pre>");

                str.Append("Дополнительный скрипт синхронизации базы слейв хранилища (запускать на нем) \"" + storage.JCompDevice.Name + "\"  c мастер-хранилищем   \"" + master.JCompDevice.Name + "\" (добавить -n для теста)");
                AppendBr(str);
                str.Append("<pre>");
                str.Append(HttpUtility.HtmlEncode(JCompDeviceStorage.MakeRsyncScript(storage, master, false)));
                str.Append("</pre>");

                AppendBr(str);
            }

            SimpleHtmlTextEditDialog dlg = new SimpleHtmlTextEditDialog();
            dlg.EditedText = str.ToString();
            dlg.ShowDialog();
        }
        [JEntityPlugin(typeof(JCompDeviceStorage))]
        public class JCompDeviceStirageBasePlugin : IFormsEntityPlugin
        {
            public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
            {
                //обработчик вызова контекстного меню 
                JCompDeviceStorage storage = (JCompDeviceStorage)selectedObject;
                ToolStripMenuItem menuItem = null;

                menuItem = new ToolStripMenuItem();
                menuItem.Text = "Скрипты подключения и синхронизации";
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        ShowSummaryInfo(storage);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                if (storage.MasterStorage != null)
                {
                    //если слейв хранилище является локальным и либо находится на данном компе, либо доступно через шару
                    //запускаем локальный процесс WinSCP 
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = "Синхронизировать с мастер хранилищем через WinSCP (запуск на локальном мастере)";
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            WinSCPUtils.SynchronizeDirectories(storage.MasterStorage.Path, storage, true);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);
                    //если слейв хранилище является удаленным
                    //то запускаем команду синхронизации 
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = "Синхронизировать с мастер хранилищем через SSH команду (запуск на удаленном мастере)";
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            string cmd = JCompDeviceStorage.MakeRsyncScript(storage.MasterStorage, storage, false);
                            //необходимо чтобы были настроены доверенные сертификаты или надо пробовать с паролем 
                            WinSCPUtils.ExecuteCommand(cmd, storage.MasterStorage.JCompDevice, true);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);
                }


            }
        }
    }
}
