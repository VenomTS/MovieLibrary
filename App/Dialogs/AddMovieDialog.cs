namespace App.Dialogs;

public partial class AddMovieDialog : Form
{
    public string MovieName { get; private set; } = string.Empty;

    public DateOnly ReleaseDate { get; private set; }


    private TextBox nameBox;
    private DateTimePicker datePicker;
    private Button saveButton;
    private Button cancelButton;


    public AddMovieDialog()
    {
        SetupUI();
    }


    private void SetupUI()
    {
        Text = "Add New Movie";
        Width = 380;
        Height = 275;

        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        BackColor = Color.FromArgb(245, 246, 250);
        
        Label title = new Label()
        {
            Text = "Movie Information",
            Font = new Font(
                "Segoe UI",
                14,
                FontStyle.Bold),
            AutoSize = true,
            Left = 25,
            Top = 20
        };


        Label nameLabel = new Label()
        {
            Text = "Movie Name",
            AutoSize = true,
            Left = 25,
            Top = 65,
            Font = new Font("Segoe UI", 10)
        };


        nameBox = new TextBox()
        {
            Left = 25,
            Top = 90,
            Width = 300,
            Font = new Font("Segoe UI", 10)
        };


        Label dateLabel = new Label()
        {
            Text = "Release Date",
            AutoSize = true,
            Left = 25,
            Top = 125,
            Font = new Font("Segoe UI", 10)
        };


        datePicker = new DateTimePicker()
        {
            Left = 25,
            Top = 150,
            Width = 300,
            Format = DateTimePickerFormat.Short
        };

        saveButton = new Button()
        {
            Text = "Add Movie",
            Width = 120,
            Height = 35,
            Left = 205,
            Top = 195,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(52,152,219),
            ForeColor = Color.White,
            Font = new Font(
                "Segoe UI",
                9,
                FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        saveButton.FlatAppearance.BorderSize = 0;

        cancelButton = new Button()
        {
            Text = "Cancel",
            Width = 100,
            Height = 35,
            Left = 95,
            Top = 195,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.LightGray,
            Cursor = Cursors.Hand
        };


        saveButton.Click += SaveButton_Click;

        cancelButton.Click += (s,e)=>
        {
            DialogResult = DialogResult.Cancel;
        };


        Controls.Add(title);
        Controls.Add(nameLabel);
        Controls.Add(nameBox);
        Controls.Add(dateLabel);
        Controls.Add(datePicker);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);


        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if(string.IsNullOrWhiteSpace(nameBox.Text))
        {
            MessageBox.Show(
                "Movie name is required.",
                "Validation Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            nameBox.Focus();
            return;
        }


        MovieName = nameBox.Text.Trim();

        ReleaseDate = DateOnly.FromDateTime(
            datePicker.Value);


        DialogResult = DialogResult.OK;
        Close();
    }
}