using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FrwSoftware
{
    /// <summary>
    /// https://www.donkz.nl/overview-rdp-file-settings/
    /// </summary>
    public class RDPUtils
    {
        /// <summary>
        /// https://social.technet.microsoft.com/wiki/contents/articles/4487.access-remote-desktop-via-commandline.aspx
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static string MakePathForCmd(WebEntryInfo w)
        {
            if (w.IsRDPAllowed)
            {
                StringBuilder str = new StringBuilder();
                str.Append("/v:");
                if (w.IsInInternalNetwork)
                {
                    if (string.IsNullOrEmpty(w.InternalAddress)) return null;
                    str.Append(w.InternalAddress);
                    if (string.IsNullOrEmpty(w.PortRDP) == false)
                    {
                        str.Append(":");
                        str.Append(w.PortRDP);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(w.ExternalAddress)) return null;
                    str.Append(w.ExternalAddress);
                    if (string.IsNullOrEmpty(w.ExtPortRDP) == false)
                    {
                        str.Append(":");
                        str.Append(w.ExtPortRDP);
                    }
                }
                return str.ToString();
            }
            else return null;
        }
    }
}
