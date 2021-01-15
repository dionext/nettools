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

namespace FrwSoftware
{
    //public partial class JCompResourcesTreeWindow : BaseTreeListWindow //FrwBaseViewControl, IViewProcessor
    public partial class BaseCompResourcesTreeWindow : SimpleTreeListWindow
    {
        public BaseCompResourcesTreeWindow()
        {
            InitializeComponent();
            rootEntites.Add(typeof(JCompDeviceNetwork));
            //rootEntites.Add(typeof(JCompProvider));
            rootEntites.Add(typeof(JMailAccount));
            rootEntites.Add(typeof(JActor));
            rootEntites.Add(typeof(JDomain));
            rootEntites.Add(typeof(JSite));

            this.treeControl.SortingMethod += delegate (TreeNode parentNode)
            {
                if (parentNode == null) return TreeSortEnum.NONE;
                object x = (parentNode.Tag is TreeObjectWrap) ? (parentNode.Tag as TreeObjectWrap).Tag : parentNode.Tag;
                if (x == null) return TreeSortEnum.NONE;
                if (x is string) return TreeSortEnum.NONE;//folder 
                Type type = x.GetType();
                if (x is RootGroupTreeFolder)
                {
                    return TreeSortEnum.NONE;
                }
                else if (x is BranchGroupTreeFolder)
                {
                    BranchGroupTreeFolder bf = (x as BranchGroupTreeFolder);
                    if (bf.RefEntityInfo.ForeignEntity == typeof(JCompDeviceStorage)) return TreeSortEnum.ASC;
                    else return TreeSortEnum.NONE;
                }
                else
                {
                    //if (type == typeof(JCompDevice)) return TreeSortEnum.ASC;
                    return TreeSortEnum.NONE;
                }
            };
        }

        override public void CreateView()
        {
            base.CreateView();
            SetNewCaption("Инфраструктура (дерево)");
        }

        override protected void ComplateNodeFromObject(TreeNode node, object o)
        {
            base.ComplateNodeFromObject(node, o);
            object x = (node.Tag is TreeObjectWrap) ? (node.Tag as TreeObjectWrap).Tag : node.Tag;
            if (x == null) return;
            if (x is JCompDeviceStorage)
            {
                JCompDeviceStorage h = (x as JCompDeviceStorage);
                if (h.MasterStorage != null && h.MasterStorage.JCompDevice != h.JCompDevice)
                {
                    node.Text = h.JCompDevice.Name + " " + node.Text;
                    node.ToolTipText = h.JCompDevice.Name + " " + node.ToolTipText;
                }
            }
            else if (x is JSoftInstance)
            {
                JSoftInstance h = (x as JSoftInstance);
                if (h.IsDocker)
                {
                    SetImageForNode(node, NetworkAccountHelperLibRes.docker, "JSoftInstance_IsDocker");
                }
            }
            else if (x is JCompDevice)
            {
                JCompDevice h = (x as JCompDevice);
                if (CompDeviceTypeEnum.Virtual.ToString().Equals(h.Stage))
                {
                    SetImageForNode(node, NetworkAccountHelperLibRes.virtualbox, "JCompDevice_virtualbox");
                }
            }
        }
    }
}
