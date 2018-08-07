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
using System.Text;
using System.Threading.Tasks;
using FrwSoftware;

namespace Dionext
{
    public class JIPAddressInfo
    {
        public string Ip { get; set; } //IP used for the query 173.194.67.94 string
        public string Country { get; set; } //country United States string
        public string CountryCode { get; set; } //country short US string
        public string Region { get; set; } //region/state short CA or 10 string
        public string RegionName { get; set; } //region/state California string
        public string City { get; set; } //city Mountain View string
        public string Zip { get; set; } //zip code 94043 string
        public string Lat { get; set; } //latitude 37.4192 float
        public string Lon { get; set; } //longitude -122.0574 float
        public string Timezone { get; set; } //city timezone America/Los_Angeles string
        public string Isp { get; set; } //ISP name Google string
        public string Org { get; set; } //Organization name Google string
        public string As { get; set; } //AS number and name, separated by space AS15169 Google Inc. string
        public string Reverse { get; set; } //Reverse DNS of the IP wi-in-f94.1e100.net string
        public string Mobile { get; set; } // mobile(cellular) connection true bool
        public string Proxy { get; set; } //proxy(anonymous) true bool

    }

}
