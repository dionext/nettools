using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FrwSoftware.Kitty
{
    public class KittyUtils
    {
        /// <summary>
        /// http://www.9bis.net/kitty/?page=Command-line%20options
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static string MakeKittyPathForCmd(WebEntryInfo w)
        {
            if (w.Url == null) return null;
            StringBuilder str = new StringBuilder();
            if (w.Login != null)
            {
                str.Append(w.Login);
                str.Append("@");
            }
            if (w.IsInInternalNetwork)
            {
                if (string.IsNullOrEmpty(w.InternalAddress)) return null;
                str.Append(w.InternalAddress);
            }
            else
            {
                if (string.IsNullOrEmpty(w.ExternalAddress)) return null;
                str.Append(w.ExternalAddress);
            }
            /* not supported
             * todo script https://superuser.com/questions/1010031/auto-start-putty-from-command-line-with-a-specific-startup-path
            if (w.Path != null)
            {
                if (w.Path.StartsWith("/") == false) str.Append("/");
                str.Append(w.Path);
                if (w.Path.EndsWith("/") == false) str.Append("/");
            }
            */
            if (w.Password != null)
            {
                str.Append(" -pass ");
                str.Append(w.Password);
            }

            if (w.IsInInternalNetwork == true && string.IsNullOrEmpty( w.PortSSH) == false)
            {
                    str.Append(" -P ");
                    str.Append(w.PortSSH);
            }
            else if (w.IsInInternalNetwork == false && string.IsNullOrEmpty(w.ExtPortSSH) == false)
            {
                str.Append(" -P ");
                str.Append(w.ExtPortSSH);
            }
            return str.ToString();
        }
        /// <summary>
        /// http://www.9bis.net/kitty/?page=Command-line%20options
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static string MakePuttyPathForCmd(WebEntryInfo w)
        {
            if (w.Url == null) return null;
            StringBuilder str = new StringBuilder();
            if (w.Login != null)
            {
                str.Append(w.Login);
                str.Append("@");
            }
            if (w.IsInInternalNetwork)
            {
                if (string.IsNullOrEmpty(w.InternalAddress)) return null;
                str.Append(w.InternalAddress);
            }
            else
            {
                if (string.IsNullOrEmpty(w.ExternalAddress)) return null;
                str.Append(w.ExternalAddress);
            }
            /* not supported
             * todo script https://superuser.com/questions/1010031/auto-start-putty-from-command-line-with-a-specific-startup-path
            if (w.Path != null)
            {
                if (w.Path.StartsWith("/") == false) str.Append("/");
                str.Append(w.Path);
                if (w.Path.EndsWith("/") == false) str.Append("/");
            }
            */
            if (w.Password != null)
            {
                str.Append(" -pw ");
                str.Append(w.Password);
            }

            if (w.IsInInternalNetwork == true && string.IsNullOrEmpty(w.PortSSH) == false)
            {
                str.Append(" -P ");
                str.Append(w.PortSSH);
            }
            else if (w.IsInInternalNetwork == false && string.IsNullOrEmpty(w.ExtPortSSH) == false)
            {
                str.Append(" -P ");
                str.Append(w.ExtPortSSH);
            }
            return str.ToString();
        }
    }
}
