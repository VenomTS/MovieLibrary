using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            var username = usernameBox.Text;
            var password = passwordBox.Text;
            var repeatPassword = repeatPasswordBox.Text;

            if (password != repeatPassword)
                this.ShowDialog();
        }
    }
}
