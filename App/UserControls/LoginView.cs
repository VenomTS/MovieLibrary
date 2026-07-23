using System.Net;
using App.Services.Interfaces;
using App.UserControls;

namespace App
{
    public partial class LoginView : UserControl
    {
        private TextBox txtMail;
        private TextBox txtPassword;

        private CheckBox chkShowPassword;

        private Button btnLogin;
        private Button btnRegister;

        private ErrorProvider errorProvider;

        private INavigationService _navigationService;
        private IAuthService _authService;


        public LoginView(
            INavigationService navigationService,
            IAuthService authService)
        {
            InitializeComponent();

            _navigationService = navigationService;
            _authService = authService;

            InitializeControls();
        }



        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();

            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtMail.Text))
            {
                errorProvider.SetError(
                    txtMail,
                    "Email is required.");

                valid = false;
            }


            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider.SetError(
                    txtPassword,
                    "Password is required.");

                valid = false;
            }


            if (!valid)
                return;



            var apiResponse = await _authService.LoginAsync(
                txtMail.Text,
                txtPassword.Text);
            
            if(apiResponse.Status == HttpStatusCode.OK)
                _navigationService.ShowView<RentMoviesView>();

            else
            {
                MessageBox.Show(
                    "Invalid mail or password.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }




        private void InitializeControls()
        {
            Dock = DockStyle.Fill;

            BackColor = Color.FromArgb(245,246,250);

            Font = new Font(
                "Segoe UI",
                10);


            errorProvider = new ErrorProvider();



            Panel card = new Panel()
            {
                Width = 370,
                Height = 420,
                BackColor = Color.White
            };



            void CenterCard()
            {
                card.Left = (Width - card.Width) / 2;
                card.Top = (Height - card.Height) / 2;
            }


            Resize += (s,e)=>
            {
                CenterCard();
            };


            CenterCard();



            card.Paint += (s,e)=>
            {
                ControlPaint.DrawBorder(
                    e.Graphics,
                    card.ClientRectangle,
                    Color.LightGray,
                    ButtonBorderStyle.Solid);
            };



            Label title = new Label()
            {
                Text = "WELCOME BACK",
                Font = new Font(
                    "Segoe UI",
                    18,
                    FontStyle.Bold),
                AutoSize = true,
                Left = 80,
                Top = 35
            };


            Label subtitle = new Label()
            {
                Text = "Sign in to continue",
                ForeColor = Color.Gray,
                AutoSize = true,
                Left = 105,
                Top = 75
            };


            card.Controls.Add(title);
            card.Controls.Add(subtitle);



            int y = 120;



            txtMail = AddTextBox(
                card,
                "Email",
                ref y);



            txtPassword = AddTextBox(
                card,
                "Password",
                ref y);


            txtPassword.UseSystemPasswordChar = true;



            chkShowPassword = new CheckBox()
            {
                Text = "Show Password",
                AutoSize = true,
                Left = 35,
                Top = y
            };


            chkShowPassword.CheckedChanged += (s,e)=>
            {
                txtPassword.UseSystemPasswordChar =
                    !chkShowPassword.Checked;
            };


            card.Controls.Add(chkShowPassword);



            y += 45;



            btnLogin = CreateButton(
                "LOGIN",
                y,
                Color.FromArgb(52,152,219));


            btnLogin.Click += BtnLogin_Click;


            card.Controls.Add(btnLogin);



            y += 60;



            btnRegister = CreateButton(
                "No Account? Register",
                y,
                Color.FromArgb(46,204,113));


            btnRegister.Click += (s,e)=>
            {
                _navigationService.ShowView<RegisterView>();
            };


            card.Controls.Add(btnRegister);



            Controls.Add(card);
        }




        private TextBox AddTextBox(
            Panel parent,
            string label,
            ref int y)
        {
            Label lbl = new Label()
            {
                Text = label,
                AutoSize = true,
                Left = 35,
                Top = y
            };


            parent.Controls.Add(lbl);



            TextBox txt = new TextBox()
            {
                Width = 300,
                Height = 30,
                Left = 35,
                Top = y + 25,
                Font = new Font(
                    "Segoe UI",
                    10)
            };


            parent.Controls.Add(txt);


            y += 70;


            return txt;
        }




        private Button CreateButton(
            string text,
            int y,
            Color color)
        {
            Button button = new Button()
            {
                Text = text,
                Width = 300,
                Height = 42,
                Left = 35,
                Top = y,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),
                Cursor = Cursors.Hand
            };


            button.FlatAppearance.BorderSize = 0;


            return button;
        }
    }
}