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
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtVerifyPassword;

        private CheckBox chkShowPassword;

        private Button btnRegister;
        private LinkLabel linkLogin;

        private ErrorProvider errorProvider;

        public RegisterForm()
        {
            Text = "Register";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(420, 450);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            errorProvider = new ErrorProvider();

            Label lblTitle = new Label()
            {
                Text = "CREATE ACCOUNT",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(90, 20)
            };

            Label lblSubtitle = new Label()
            {
                Text = "Create your new account below",
                AutoSize = true,
                ForeColor = Color.Gray,
                Location = new Point(90, 60)
            };

            Controls.Add(lblTitle);
            Controls.Add(lblSubtitle);

            int y = 110;

            txtUsername = AddTextBox("Username", ref y);

            txtPassword = AddTextBox("Password", ref y);
            txtPassword.UseSystemPasswordChar = true;

            txtVerifyPassword = AddTextBox("Verify Password", ref y);
            txtVerifyPassword.UseSystemPasswordChar = true;

            chkShowPassword = new CheckBox()
            {
                Text = "Show Passwords",
                AutoSize = true,
                Location = new Point(40, y)
            };

            chkShowPassword.CheckedChanged += (s, e) =>
            {
                bool hide = !chkShowPassword.Checked;

                txtPassword.UseSystemPasswordChar = hide;
                txtVerifyPassword.UseSystemPasswordChar = hide;
            };

            Controls.Add(chkShowPassword);

            y += 45;

            btnRegister = new Button()
            {
                Text = "REGISTER",
                Size = new Size(320, 45),
                Location = new Point(40, y),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            Controls.Add(btnRegister);

            y += 65;

            Label lblLogin = new Label()
            {
                Text = "Already have an account?",
                AutoSize = true,
                Location = new Point(70, y)
            };

            linkLogin = new LinkLabel()
            {
                Text = "Login",
                AutoSize = true,
                Location = new Point(245, y)
            };

            Controls.Add(lblLogin);
            Controls.Add(linkLogin);
        }

        private TextBox AddTextBox(string label, ref int y)
        {
            Label lbl = new Label()
            {
                Text = label,
                AutoSize = true,
                Location = new Point(40, y)
            };

            Controls.Add(lbl);

            TextBox txt = new TextBox()
            {
                Width = 320,
                Location = new Point(40, y + 25)
            };

            Controls.Add(txt);

            y += 70;

            return txt;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();

            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider.SetError(txtUsername, "Username is required.");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider.SetError(txtPassword, "Password is required.");
                valid = false;
            }
            else if (txtPassword.Text.Length < 8)
            {
                errorProvider.SetError(txtPassword, "Password must be at least 8 characters.");
                valid = false;
            }

            if (txtPassword.Text != txtVerifyPassword.Text)
            {
                errorProvider.SetError(txtVerifyPassword, "Passwords do not match.");
                valid = false;
            }

            if (!valid)
                return;

            MessageBox.Show(
                "Registration Successful!",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
