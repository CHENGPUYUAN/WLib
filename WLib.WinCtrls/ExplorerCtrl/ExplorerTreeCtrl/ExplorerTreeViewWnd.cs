/*---------------------------------------------------------------- 
// auth�� Source by WilsonProgramming
// date�� None
// desc�� ��չTreeView,����Ŀ¼���ؼ������ڷ�����봰������ʾ��������ʹ��FolderBrowserDialog������ʾ
// mdfy:  Windragon
//----------------------------------------------------------------*/

using System.Collections;
using System.Windows.Forms;

namespace WLib.WinCtrls.ExplorerCtrl.ExplorerTreeCtrl
{
    /// <summary>
    /// ��TreeView������չ��Ŀ¼����ͼ�ؼ�
    /// </summary>
    public class ExplorerTreeViewWnd : TreeView
    {
        /// <summary>
        /// ��TreeView������չ��Ŀ¼����ͼ�ؼ�
        /// </summary>
        public ExplorerTreeViewWnd() { }

        /// <summary>
        /// չ��ĳ���ļ��нڵ�ǰ����ȡ���ļ�����Ϣ��Ϊ�ӽڵ�
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            // Remove the placeholder node.
            // ɾ��ռλ���ڵ�
            e.Node.Nodes.Clear();

            // We stored the ShellItem object in the node's Tag property - hah!
            // ShellItem����洢�ڽڵ��Tag������
            ShellItem shellItem = (ShellItem)e.Node.Tag;
            ArrayList subShellItems = shellItem.GetSubFolders();
            foreach (ShellItem subShellItem in subShellItems)
            {
                var treeNode = new TreeNode
                {
                    Text = subShellItem.DisplayName,
                    ImageIndex = subShellItem.IconIndex,
                    SelectedImageIndex = subShellItem.IconIndex,
                    Tag = subShellItem
                };

                // If this is a folder item and has children then add a place holder node.
                // ������һ���ļ����������������һ��ռλ���ڵ�
                if (subShellItem.IsFolder && subShellItem.HasSubFolder)
                    treeNode.Nodes.Add("PH");
                e.Node.Nodes.Add(treeNode);
            }

            base.OnBeforeExpand(e);
        }
    }
}
