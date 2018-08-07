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
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CefSharp.Example.Proxy
{
    public class ProxyConfig
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetQueryOption(IntPtr hInternet, uint dwOption, IntPtr lpBuffer, ref int lpdwBufferLength);

        private const uint InternetOptionProxy = 38;

        public static InternetProxyInfo GetProxyInformation()
        {
            var bufferLength = 0;
            InternetQueryOption(IntPtr.Zero, InternetOptionProxy, IntPtr.Zero, ref bufferLength);
            var buffer = IntPtr.Zero;

            try
            {
                buffer = Marshal.AllocHGlobal(bufferLength);

                if (InternetQueryOption(IntPtr.Zero, InternetOptionProxy, buffer, ref bufferLength))
                {
                    var ipi = (InternetProxyInfo)Marshal.PtrToStructure(buffer, typeof(InternetProxyInfo));
                    return ipi;
                }
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }
    }
}