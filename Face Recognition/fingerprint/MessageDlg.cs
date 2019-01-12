using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ScanAPIDemo
{
    public partial class MessageDlg : Form
    {
        public delegate void OnCancelHandler();
        public delegate void OnSetParameterHandler( Object value );

        static private MessageDlg m_dlg = null;
        static private AutoResetEvent m_initEvent;

        String m_Message;
        String m_Title;
        IntPtr m_hParent;
        public event OnCancelHandler CancelPressed;

        public MessageDlg()
        {
            InitializeComponent();
            m_Message = String.Empty;
            m_Title = String.Empty;
        }

        private String Message
        {
            set
            {
                if (value != null)
                {
                    m_Message = value;
                }
            }
        }

        private String Title
        {
            set
            {
                if (value != null)
                {
                    m_Title = value;
                }
            }
        }

        private IntPtr ParentWindow
        {
            set
            {
                if (value != null)
                {
                    m_hParent = value;
                }
            }
        }

        private void SetMessage(Object value)
        {
            this.Text = (String)value;
        }

        private void CloseDlg()
        {
            this.Close();
            Application.ExitThread();
            m_initEvent.Set();
        }

        class ThreadParameters
        {
            public IntPtr hParent;
            public String message;
            public String title;
            public OnCancelHandler handler;
        }

        static private void ShowDialogThread(Object param)
        {
            ThreadParameters dlgParam = (ThreadParameters)param;
            m_dlg = new MessageDlg();
            m_dlg.Message = dlgParam.message;
            m_dlg.Title = dlgParam.title;
            m_dlg.ParentWindow = dlgParam.hParent;
            if (dlgParam.handler != null)
                m_dlg.CancelPressed += dlgParam.handler;
            Application.Run(m_dlg);
        }

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text and caption.
        /// </summary>
        /// <param name="hParent">An implementation of IWin32Window that will parent the modelles dialog box.</param>
        /// <param name="message">The text to display in the message box.</param>
        /// <param name="title">The text to display in the title bar of the message box.</param>
        /// <param name="handler">"cancel button pressed" event handler</param>
        static public bool ShowMessageDlg( IWin32Window hParent, String message, String title, OnCancelHandler handler )
        {
            if (m_dlg != null)
            {
                return false;
            }

            m_initEvent = new AutoResetEvent(false);
            ThreadParameters dlgParam = new ThreadParameters();
            dlgParam.message = message;
            dlgParam.title = title;
            dlgParam.hParent = hParent.Handle;
            dlgParam.handler = handler;
            Thread thread = new Thread( ShowDialogThread);
            thread.IsBackground = true;
            thread.Start( dlgParam );
            m_initEvent.WaitOne();
            m_initEvent.Close();
            m_initEvent = null;

            return true;
        }

        static public void SetMessageDlg( String szMessage )
        {
            if( m_dlg != null )
            {
                m_dlg.Invoke((OnSetParameterHandler)m_dlg.SetMessage, new Object[] { szMessage });
            }
        }

        static public bool HideMessageDlg()
        {
            if (m_dlg != null)
            {
                m_initEvent = new AutoResetEvent(false);
                m_dlg.Invoke((OnCancelHandler)m_dlg.CloseDlg);
                m_initEvent.WaitOne();
                m_initEvent.Close();
                m_initEvent = null;
                m_dlg = null;
            }

            return true;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Control parent = Control.FromHandle(m_hParent);
            Point parentPosition = parent.Location;
            int x = parentPosition.X + ((parent.Width - this.Width)/2);
            int y = parentPosition.Y + ((parent.Height - this.Height)/2);
            this.Location = new Point( x, y );
            this.Text = m_Title;
            this.m_lblMessage.Text = m_Message;
            m_initEvent.Set();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            if(CancelPressed != null)
            {
                CancelPressed();
            }
        }

        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            if (CancelPressed != null)
            {
                CancelPressed();
            }
        }
    }
}