﻿/**********************************************************************************
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using FrwSoftware;

namespace Dionext
{
    public partial class JWebTreeWindow : SimpleTreeListWindow 
    {
        public JWebTreeWindow()
        {
            InitializeComponent();
            //showGroupsFolder = false;
            //notShowGroupsFolderTypes.Add(JWebCategory);//self
            notShowGroupsFolderTypes.Add(typeof(JWeb));
            notShowGroupsFolderTypes.Add(typeof(JWebAccount));
            rootEntites.Add(typeof(JWebCategory));
        }
        override public void CreateView()
        {
            base.CreateView();
            SetNewCaption(WebAccountLibRes.Web__tree_);
        }
    }
}
