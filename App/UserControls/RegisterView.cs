using App.Services.Interfaces;

namespace App.UserControls;

public partial class RegisterView : UserControl
{
    private TextBox txtMail;
    private TextBox txtPassword;
    private TextBox txtVerifyPassword;

    private CheckBox chkShowPassword;

    private Button btnRegister;
    private Button btnLogin;

    private ErrorProvider errorProvider;

    private INavigationService _navigationService;
    private IAuthService _authService;


    public RegisterView(INavigationService navigationService, IAuthService authService)
    {
        InitializeComponent();

        _navigationService = navigationService;
        _authService = authService;

        InitializeControls();
    }



    private void InitializeControls()
    {
        Dock = DockStyle.Fill;

        BackColor = Color.FromArgb(245, 246, 250);

        Font = new Font(
            "Segoe UI",
            10);


        errorProvider = new ErrorProvider();



        Panel card = new Panel()
        {
            Width = 370,
            Height = 500,
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
            Text = "CREATE ACCOUNT",
            Font = new Font(
                "Segoe UI",
                18,
                FontStyle.Bold),
            AutoSize = true,
            Left = 70,
            Top = 35
        };


        Label subtitle = new Label()
        {
            Text = "Create your new account below",
            ForeColor = Color.Gray,
            AutoSize = true,
            Left = 80,
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

        txtVerifyPassword = AddTextBox(
            card,
            "Verify Password",
            ref y);

        txtVerifyPassword.UseSystemPasswordChar = true;



        chkShowPassword = new CheckBox()
        {
            Text = "Show Passwords",
            AutoSize = true,
            Left = 35,
            Top = y
        };


        chkShowPassword.CheckedChanged += (s,e)=>
        {
            bool hide = !chkShowPassword.Checked;

            txtPassword.UseSystemPasswordChar = hide;
            txtVerifyPassword.UseSystemPasswordChar = hide;
        };


        card.Controls.Add(chkShowPassword);
        
        y += 45;



        btnRegister = CreateButton(
            "REGISTER",
            y,
            Color.FromArgb(52,152,219));


        btnRegister.Click += BtnRegister_Click;


        card.Controls.Add(btnRegister);



        y += 60;



        btnLogin = CreateButton(
            "Already have an account? Login",
            y,
            Color.FromArgb(46,204,113));


        btnLogin.Click += (s,e)=>
        {
            _navigationService.ShowView<LoginView>();
        };


        card.Controls.Add(btnLogin);



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




    private async void BtnRegister_Click(
        object sender,
        EventArgs e)
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
        else if(txtPassword.Text.Length < 8)
        {
            errorProvider.SetError(
                txtPassword,
                "Password must be at least 8 characters.");

            valid = false;
        }



        if(txtPassword.Text != txtVerifyPassword.Text)
        {
            errorProvider.SetError(
                txtVerifyPassword,
                "Passwords do not match.");

            valid = false;
        }



        if(!valid)
            return;
        
        var result = await _authService.RegisterAsync(txtMail.Text, txtPassword.Text);
        if (!result)
        {
            MessageBox.Show("Registration Failed - Reason: ", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _navigationService.ShowView<LoginView>();
    }
}