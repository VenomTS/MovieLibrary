namespace App.Dialogs;

public partial class InventoryDialog : Form
{
    public int Amount { get; private set; }

    public DateOnly ArrivalDate { get; private set; }

    private NumericUpDown amountBox;
    private DateTimePicker datePicker;


    public InventoryDialog()
    {
        InitializeComponent();

        SetupUI();
    }


    private void SetupUI()
    {
        Text = "Add Inventory";
        Width = 380;
        Height = 250;

        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        BackColor = Color.FromArgb(245,246,250);


        Label amountLabel = new Label
        {
            Text = "Quantity",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(30,25)
        };


        amountBox = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 100000,
            Value = 1,
            Width = 280,
            Height = 30,
            Location = new Point(30,50),
            Font = new Font("Segoe UI",10)
        };


        Label dateLabel = new Label
        {
            Text = "Arrival Date",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(30,90)
        };


        datePicker = new DateTimePicker
        {
            Width = 280,
            Height = 30,
            Location = new Point(30,115),
            Format = DateTimePickerFormat.Short,
            Font = new Font("Segoe UI",10),
            Value = DateTime.Today
        };


        Button saveButton = new Button
        {
            Text = "Save",
            Width = 120,
            Height = 35,
            Location = new Point(190,160),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(46,204,113),
            ForeColor = Color.White,
            Font = new Font("Segoe UI",10,FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        saveButton.FlatAppearance.BorderSize = 0;


        Button cancelButton = new Button
        {
            Text = "Cancel",
            Width = 120,
            Height = 35,
            Location = new Point(50,160),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(220,220,220),
            ForeColor = Color.Black,
            Font = new Font("Segoe UI",10),
            Cursor = Cursors.Hand
        };

        cancelButton.FlatAppearance.BorderSize = 0;


        saveButton.Click += (s,e)=>
        {
            Amount = (int)amountBox.Value;
            ArrivalDate = DateOnly.FromDateTime(datePicker.Value);

            DialogResult = DialogResult.OK;
            Close();
        };


        cancelButton.Click += (s,e)=>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };


        Controls.Add(amountLabel);
        Controls.Add(amountBox);
        Controls.Add(dateLabel);
        Controls.Add(datePicker);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
    }
}