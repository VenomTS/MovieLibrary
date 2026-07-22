using App.Services.Interfaces;
using App.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class LoginView : UserControl
    {
        private TextBox txtUsername;
        private TextBox txtPassword;

        private CheckBox chkShowPassword;

        private Button btnLogin;
        private LinkLabel linkRegister;

        private ErrorProvider errorProvider;

        // Services
        private INavigationService _navigationService;
        private IAuthService _authService;

        public LoginView(INavigationService navigationService, IAuthService authService)
        {
            InitializeComponent();
            InitializeControls();

            _navigationService = navigationService;
            _authService = authService;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
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

            if (!valid)
                return;

            var success = await _authService.LoginAsync(txtUsername.Text, txtPassword.Text);

            if (success)
            {
                _navigationService.ShowView<RentMoviesView>();
            }

            else
            {
                MessageBox.Show(
                    "Invalid username or password.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InitializeControls()
        {
            Text = "Login";
            Size = new Size(420, 380);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);
            errorProvider = new ErrorProvider();

            Label lblTitle = new Label()
            {
                Text = "WELCOME BACK",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(90, 20)
            };

            Label lblSubtitle = new Label()
            {
                Text = "Sign in to continue",
                AutoSize = true,
                ForeColor = Color.Gray,
                Location = new Point(130, 60)
            };

            Controls.Add(lblTitle);
            Controls.Add(lblSubtitle);

            int y = 110;

            txtUsername = AddTextBox("Username", ref y);

            txtPassword = AddTextBox("Password", ref y);
            txtPassword.UseSystemPasswordChar = true;

            chkShowPassword = new CheckBox()
            {
                Text = "Show Password",
                AutoSize = true,
                Location = new Point(40, y)
            };

            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            };

            Controls.Add(chkShowPassword);

            y += 45;

            btnLogin = new Button()
            {
                Text = "LOGIN",
                Size = new Size(320, 45),
                Location = new Point(40, y),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            Controls.Add(btnLogin);

            y += 65;

            Label lblRegister = new Label()
            {
                Text = "Don't have an account?",
                AutoSize = true,
                Location = new Point(70, y)
            };

            linkRegister = new LinkLabel()
            {
                Text = "Register",
                AutoSize = true,
                Location = new Point(230, y)
            };

            linkRegister.Click += (s, e) =>
            {
                MessageBox.Show("Open Register Form");
            };

            Controls.Add(lblRegister);
            Controls.Add(linkRegister);
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
    }
}
