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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Example.Proxy;
using CefSharp.WinForms;
using FrwSoftware;

namespace EmbeddedBrowser
{
    public class EmbeddedBrowserHelper
    {

        public static bool IsInitilized { get; set; }


        // Use when debugging the actual SubProcess, to make breakpoints etc. inside that project work.
        private static readonly bool DebuggingSubProcess = Debugger.IsAttached;
        public static bool MultiThreadedMessageLoop { get; set; } = true;
        public static void InitIfNeed()
        {
            if (IsInitilized == false)
            {
                /*
                CachePath The location where cache data will be stored on disk.If empty an in-memory cache will be used for some features and a temporary disk cache will be used for others.HTML5 databases such as localStorage will only persist across sessions if a cache path is specified.
                Locale The locale string that will be passed to Blink.If empty the default locale of "en-US" will be used.Also configurable using the "lang" command - line switch.Change this to set the Context menu language as well.
                LogFile The directory and file name to use for the debug log.If empty, the default name of "debug.log" will be used and the file will be written to the application directory.Also configurable using the "log-file" command - line switch.
                LogSeverity The log severity. Only messages of this severity level or higher will be logged.Also configurable using the "log-severity" command - line switch with a value of "verbose", "info", "warning", "error", "error-report" or "disable".

                //CefCommandLineArgs example
                // Enable WebRTC                            
                settings.CefCommandLineArgs.Add("enable-media-stream", "1");
                //Disable GPU Acceleration
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                // Don't use a proxy server, always make direct connections. Overrides any other proxy server flags that are passed.
                // Slightly improves Cef initialize time as it won't attempt to resolve a proxy
                settings.CefCommandLineArgs.Add("no-proxy-server", "1"); 
                 //Chromium Command Line args
                //http://peter.sh/experiments/chromium-command-line-switches/
                */
                var settings = new CefSettings();
                
                JSetting setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
                {
                    Name = "MainApp.userAgent",
                    Description = "User Agent",
                    ValueType = typeof(JUserAgent),
                    IsUser = true
                });
                JUserAgent userAgent = (JUserAgent)FrwConfig.Instance.GetPropertyValue(setting.Name);
                if (userAgent != null)
                {
                    //JUserAgent userAgent = Dm.Instance.Find<JUserAgent>(userAgentId);
                    //if (userAgent == null) throw new Exception("User Agent not found by id = " + userAgentId);
                    settings.UserAgent = userAgent.Data;
                }
                else {
                    //    "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; Acoo Browser 1.98.744; .NET CLR 3.5.30729)",
                    //    "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; AS; rv:11.0) like Gecko",
                    //    "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 7.0; InfoPath.3; .NET CLR 3.1.40767; Trident/6.0; en-IN)",
                    //    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36"


                    //setting recomended to entrance to google account 
                    //https://www.magpcss.org/ceforum/viewtopic.php?f=10&t=16717
                    //to test use http://whatsmyuseragent.org/
                    settings.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:68.0) Gecko/20100101 Firefox/68.0";//  @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36";
                }
                //For Windows 7 and above, best to include relevant app.manifest entries as well
                //Cef.EnableHighDPISupport(); - todo test it 

            
                IBrowserProcessHandler browserProcessHandler;
                //if (MultiThreadedMessageLoop)
                //{
                    browserProcessHandler = new BrowserProcessHandler();
                //}
                // Use when debugging the actual SubProcess, to make breakpoints etc. inside that project work.
                //private static readonly 
                bool DebuggingSubProcess = Debugger.IsAttached;
                bool sperformDependencyCheck = !DebuggingSubProcess;

                if (!Cef.Initialize(settings, sperformDependencyCheck, browserProcessHandler: browserProcessHandler))
                {
                    throw new Exception("Unable to Initialize Cef");
                }
                IsInitilized = true;
            }
        }


        //from OSIRT
        public static bool HasJpegExtension(string path)
        {
            return Path.GetExtension(path).Equals(".jpg", StringComparison.InvariantCultureIgnoreCase)
                    || Path.GetExtension(path).Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase);
        }
        public static string StripQueryFromPath(string path)
        {
            Uri uri = new Uri(path);
            return uri.GetLeftPart(UriPartial.Path);
        }
        public static bool IsOnFacebook(string url)
        {

            string stripped = StripQueryFromPath(url);
            var rCaseInsensitive = new Regex(@"^(https?:\/\/)?((w{3}\.)?)facebook\.com\/(?:[^\s()\\\[\]{};:'"",<>?«»“”‘’]){5,}$", RegexOptions.IgnoreCase);
            return rCaseInsensitive.IsMatch(stripped);
        }
        public static bool IsOnTwitter(string url)
        {
            string pattern = @"http(?:s)?:\/\/(?:www.)?twitter\.com\/([a-zA-Z0-9_]+)";
            return Regex.Match(url, pattern).Success;
        }

        public static bool IsOnGoogle(string url)
        {
            return url.StartsWith("http://www.google") || url.StartsWith("https://www.google") || url.StartsWith("http://google") || url.StartsWith("https://google");
        }

        public static bool IsOnYouTube(string url)
        {
            string pattern = @"(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+";
            return Regex.Match(url, pattern).Success;
        }



    }
}
