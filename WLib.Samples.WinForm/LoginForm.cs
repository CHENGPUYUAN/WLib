using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WLib.Samples.WinForm
{
    public partial class LoginForm : Form
    {

        public event EventHandler Login_success;
        public event EventHandler Login_abort;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "admin" || textBox2.Text != "123456")
            {
                MessageBox.Show("用户名或密码错误");
            }
            else
            {
                Login_success?.Invoke(this, new EventArgs());
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttoExit_Click(object sender, EventArgs e)
        {
            Login_abort?.Invoke(this, new EventArgs());
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        public string msg
        {
            set
            {
                this.msg_label.Text = value;
            }
        }
    }
}
