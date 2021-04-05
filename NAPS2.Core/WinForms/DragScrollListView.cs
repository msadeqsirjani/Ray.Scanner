using NAPS2.Util;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NAPS2.WinForms
{
    public class DragScrollListView : ListView
    {
        // From http://stackoverflow.com/questions/660663/c-sharp-implementing-auto-scroll-in-a-listview-while-drag-dropping

        private Timer tmrLvScroll;
        private System.ComponentModel.IContainer components;
        private int mintScrollDirection;

        private const int VERTICAL_SCROLL = 277; // Vertical scroll
        private const int SCROLL_LINE_UP = 0; // Scrolls one line up
        private const int SCROLL_LINE_DOWN = 1; // Scrolls one line down

        public DragScrollListView()
        {
            InitializeComponent();
        }

        private int EdgeSize => Font.Height;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tmrLvScroll = new Timer(components);
            SuspendLayout();
            // 
            // tmrLVScroll
            // 
            tmrLvScroll.Tick += TmrLVScroll_Tick;
            // 
            // ListViewBase
            // 
            DragOver += ListViewBase_DragOver;
            ResumeLayout(false);

        }

        private void ListViewBase_DragOver(object sender, DragEventArgs e)
        {
            var position = PointToClient(new Point(e.X, e.Y));

            if (position.Y <= EdgeSize)
            {
                // getting close to top, ensure previous item is visible
                mintScrollDirection = SCROLL_LINE_UP;
                tmrLvScroll.Enabled = true;
            }
            else if (position.Y >= ClientSize.Height - EdgeSize)
            {
                // getting close to bottom, ensure next item is visible
                mintScrollDirection = SCROLL_LINE_DOWN;
                tmrLvScroll.Enabled = true;
            }
            else
            {
                tmrLvScroll.Enabled = false;
            }
        }

        private void TmrLVScroll_Tick(object sender, EventArgs e)
        {
            Win32.SendMessage(Handle, VERTICAL_SCROLL, (IntPtr)mintScrollDirection, IntPtr.Zero);
        }
    }
}
