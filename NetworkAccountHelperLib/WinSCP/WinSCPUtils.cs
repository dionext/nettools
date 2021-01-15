using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinSCP;

namespace FrwSoftware
{
    //todo closing session
    public class WinSCPUtils
    {

        private static Dictionary<string, Session> sessions = new Dictionary<string, Session>();

        static public SessionOptions SessionOptionsFromJCompDeviceStorage(JCompDevice jd, bool remote = true)
        {
            SessionOptions o = new SessionOptions();
            o.Protocol = Protocol.Sftp;
            //todo webinfo
            if (remote)
            {
                o.HostName = jd.JCompDeviceNetworkParentLevel.GetExternalAddress();//.RemoteHostName;
                if (!string.IsNullOrEmpty(jd.WebEntryInfo.ExtPortSSH)) o.PortNumber = int.Parse(jd.WebEntryInfo.ExtPortSSH);
            }
            else
            {
                o.HostName = jd.GetInternalAddress();
                if (!string.IsNullOrEmpty(jd.WebEntryInfo.PortSSH)) o.PortNumber = int.Parse(jd.WebEntryInfo.PortSSH);
            }
            o.Password = jd.Password;
            o.UserName = jd.Login;
            if (string.IsNullOrEmpty(jd.SshHostKeyFingerprint))
            {
                o.GiveUpSecurityAndAcceptAnySshHostKey = true;
            }
            else
            {
                o.GiveUpSecurityAndAcceptAnySshHostKey = false;
                o.SshHostKeyFingerprint = jd.SshHostKeyFingerprint;
            }
            //https://winscp.net/eng/docs/faq_hostkey
            //In exceptional situations, when security is not required, such as when connecting within a trusted private network, you can use -hostkey=* or SessionOptions.GiveUpSecurityAndAcceptAnySshHostKey to blindly accept any host key. 
            //If you want to allow a user to manually verify the host key, use the Session.ScanFingerprint method to retrieve the key fingerprint.Then let the user to verify it and assign the verified value to the SessionOptions.SshHostKeyFingerprint property. 
            //https://winscp.net/eng/docs/library_example_known_hosts

            return o;
        }
        //todo cache session
        static public bool TestConnection(JCompDevice jo, bool remote = true)
        {
            try
            {
                Session session = Reconnect(jo, remote);
                bool testResult = IsRemoteSessionOpened(jo);
                CloseRemoteSession(jo);
                return testResult;
            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                throw sle;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                throw sre;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Download(string localPath, JCompDeviceStorage jo, bool remote = true)
        {
            try
            {
                Session session = Reconnect(jo.JCompDevice, remote);
                RemoteDirectoryInfo directory = session.ListDirectory(jo.Path);
                foreach (RemoteFileInfo fileInfo in directory.Files)
                {
                    Console.WriteLine(jo.Path + fileInfo.Name);
                    /*
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    transferOptions.FilePermissions = null;
                    transferOptions.PreserveTimestamp = false;
                    transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;
                    TransferOperationResult transferResult;
                    transferResult = session.GetFiles(jo.ExtraPath + fileInfo.Name, localPath, false, transferOptions);
                    transferResult.Check();
                    */
                }
                CloseRemoteSession(jo.JCompDevice);
            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                throw sle;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                throw sre;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static HashSet<string> GetRemoteFileList(JCompDeviceStorage jo, bool remote = true)
        {
            try
            {
                HashSet<string> list = new HashSet<string>();
                Session session = Reconnect(jo.JCompDevice, remote);
                //session.SessionLogPath = "";
                RemoteDirectoryInfo directory = session.ListDirectory(jo.Path);
                foreach (RemoteFileInfo fileInfo in directory.Files)
                {
                    //Console.WriteLine("{0} with size {1}, permissions {2} and last modification at {3}",
                    //  fileInfo.Name, fileInfo.Length, fileInfo.FilePermissions, fileInfo.LastWriteTime);
                    list.Add(fileInfo.Name);
                }

                // Select the most recent file
                /*
                RemoteFileInfo latest =
                    directoryInfo.Files
                        .Where(file => !file.IsDirectory)
                        .OrderByDescending(file => file.LastWriteTime)
                        .FirstOrDefault();
                 */
                /*
               DateTime remoteWriteTime = session.GetFileInfo(remotePath).LastWriteTime;
               DateTime localWriteTime = File.GetLastWriteTime(localPath);
               if (remoteWriteTime > localWriteTime)
               {
                   Console.WriteLine(
                       "File {0} as well as local backup {1} exist, " +
                       "but remote file is newer ({2}) than local backup ({3})",
                       remotePath, localPath, remoteWriteTime, localWriteTime);
                   download = true;
               }
               else
               {
                   Console.WriteLine(
                       "File {0} as well as local backup {1} exist, " +
                       "but remote file is not newer ({2}) than local backup ({3})",
                       remotePath, localPath, remoteWriteTime, localWriteTime);
                   download = false;
               }
               */
                CloseRemoteSession(jo.JCompDevice);
                return list;
            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                throw sle;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                throw sre;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool UploadFile(string localPath, JCompDeviceStorage jo, bool remote = true)
        {
            //see also https://winscp.net/eng/docs/library_example_moves_files_keeping_directory_structure

            bool success = false;
            try
            {
                Session session = Reconnect(jo.JCompDevice, remote);
                // Will continuously report progress of synchronization
                session.FileTransferred += FileTransferred;
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;


                // Upload files
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;


                TransferOperationResult transferResult = session.PutFiles(localPath + "*", jo.Path, false, transferOptions);

                // Throw on any error
                transferResult.Check();

                // Print results
                foreach (TransferEventArgs transfer in transferResult.Transfers)
                {
                    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    success = true;
                }
                CloseRemoteSession(jo.JCompDevice);

            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                throw sle;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                throw sre;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public static bool SynchronizeDirectories(string localPath, JCompDeviceStorage jo, bool remote = true)
        {
            bool success = false;
            try
            {
                Session session = Reconnect(jo.JCompDevice, remote);
                // Will continuously report progress of synchronization
                session.FileTransferred += FileTransferred;

                // Synchronize files
                SynchronizationResult synchronizationResult;
                synchronizationResult =
                    session.SynchronizeDirectories(
                        SynchronizationMode.Remote, localPath, jo.Path, false);

                // Throw on any error
                synchronizationResult.Check();
                success = true;
                CloseRemoteSession(jo.JCompDevice);

            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                throw sle;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                throw sre;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }


        static private Session GetRemoteSession(JCompDevice jd)
        {
            Session session = null;
            sessions.TryGetValue(jd.JCompDeviceId, out session);
            return session;
        }
        static private void AddRemoteSession(JCompDevice jd, Session session)
        {
            sessions.Add(jd.JCompDeviceId, session);
        }

        private static Session Reconnect(JCompDevice jd, bool remote = true)
        {
            Session session = GetRemoteSession(jd);
            if (session == null)
            {
                session = new Session();
                AddRemoteSession(jd, session);
            }
            if (session.Opened == false)
            {
                SessionOptions sessionOptions = SessionOptionsFromJCompDeviceStorage(jd, remote);
                session.Open(sessionOptions);
                session.OutputDataReceived += Session_OutputDataReceived;
            }
            return session;
        }
        public static void CloseRemoteSession(JCompDevice jd)
        {
            Session session = GetRemoteSession(jd);
            if (session != null)
            {
                if (session.Opened)
                {
                    //session.Abort();
                    session.Close();
                    session.Dispose();
                    sessions.Remove(jd.JCompDeviceId);
                }
            }
        }
        public static void CloseAllRemoteSession()
        {
            foreach (var session in sessions.Values)
            {
                session.Close();
                session.Dispose();
            }
            sessions.Clear();
        }
        public static bool IsRemoteSessionOpened(JCompDevice jd)
        {
            Session session = GetRemoteSession(jd);
            if (session != null)
            {
                return session.Opened;
            }
            else return false;
        }

        public static void ExecuteCommand(string command, JCompDevice jd, bool remote = true)
        {
            try
            {
                Session session = Reconnect(jd, remote);
                //SessionOptions sessionOptions = SessionOptionsFromJCompDeviceStorage(jo, remote);
                //using (Session session = new Session())
                //{
                // Connect
                //session.Open(sessionOptions);
                //session.OutputDataReceived += Session_OutputDataReceived;
                CommandExecutionResult result = session.ExecuteCommand(command);
                result.Check();
                if (result.Output != null) Log.ProcessDebug("Result of commmand: " + result.Output);
                //}

            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                throw sle;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                throw sre;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void Session_OutputDataReceived(object sender, OutputDataReceivedEventArgs e)
        {
            if (e.Error)
            {
                if (e.Data != null) Console.Write("\r Error: {0}", e.Data);
            }
            else
            {
                if (e.Data != null) Console.Write("\r Output: {0}", e.Data);
            }
        }

        private static string _lastFileName;
        private static void SessionFileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            // New line for every new file
            if ((_lastFileName != null) && (_lastFileName != e.FileName))
            {
                Console.WriteLine();
            }

            // Print transfer progress
            Console.Write("\r{0} ({1:P0})", e.FileName, e.FileProgress);

            // Remember a name of the last file reported
            _lastFileName = e.FileName;
        }


        private static void FileTransferred(object sender, TransferEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Upload of {0} succeeded", e.FileName);
            }
            else
            {
                Console.WriteLine("Upload of {0} failed: {1}", e.FileName, e.Error);
            }

            if (e.Chmod != null)
            {
                if (e.Chmod.Error == null)
                {
                    Console.WriteLine("Permisions of {0} set to {1}", e.Chmod.FileName, e.Chmod.FilePermissions);
                }
                else
                {
                    Console.WriteLine("Setting permissions of {0} failed: {1}", e.Chmod.FileName, e.Chmod.Error);
                }
            }
            else
            {
                Console.WriteLine("Permissions of {0} kept with their defaults", e.Destination);
            }

            if (e.Touch != null)
            {
                if (e.Touch.Error == null)
                {
                    Console.WriteLine("Timestamp of {0} set to {1}", e.Touch.FileName, e.Touch.LastWriteTime);
                }
                else
                {
                    Console.WriteLine("Setting timestamp of {0} failed: {1}", e.Touch.FileName, e.Touch.Error);
                }
            }
            else
            {
                // This should never happen during "local to remote" synchronization
                Console.WriteLine("Timestamp of {0} kept with its default (current time)", e.Destination);
            }
        }
        /// <summary>
        /// https://winscp.net/eng/docs/commandline
        /// https://winscp.net/eng/docs/scriptcommand_open
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static string MakePathForCmd(WebEntryInfo w)
        {
            if (w.Url == null) return null;

            StringBuilder str = new StringBuilder();
            str.Append("sftp://");
            if (w.Login != null)
            {
                str.Append(w.Login);
                if (w.Password != null)
                {
                    str.Append(":");
                    str.Append(w.Password);
                }
                str.Append("@");
            }
            if (w.IsInInternalNetwork)
            {
                if (string.IsNullOrEmpty(w.InternalAddress)) return null;
                str.Append(w.InternalAddress);
                if (string.IsNullOrEmpty(w.PortSSH) == false)
                {
                    str.Append(":");
                    str.Append(w.PortSSH);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(w.ExternalAddress)) return null;
                str.Append(w.ExternalAddress);
                if (string.IsNullOrEmpty(w.ExtPortSSH) == false)
                {
                    str.Append(":");
                    str.Append(w.ExtPortSSH);
                }
            }
            if (w.Path != null)
            {
                if (w.Path.StartsWith("/") == false) str.Append("/");
                str.Append(w.Path);
                if (w.Path.EndsWith("/") == false) str.Append("/");
            }
            else str.Append("/");
            return str.ToString();
        }

    }
}
