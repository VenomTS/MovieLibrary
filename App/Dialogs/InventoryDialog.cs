namespace App.Dialogs;

public partial class InventoryDialog : Form
{
    public int Amount { get; private set; }

    public DateOnly ArrivalDate { get; private set; }
    
    NumericUpDown amountBox;
    DateTimePicker datePicker;

    public InventoryDialog()
    {
        Width = 300;
        Height = 200;


        amountBox = new NumericUpDown()
        {
            Minimum = 1,
            Maximum = 100000,
            Left = 20,
            Top = 20
        };


        datePicker = new DateTimePicker()
        {
            Left = 20,
            Top = 60
        };


        Button ok = new Button()
        {
            Text = "Save",
            Left = 20,
            Top = 100
        };


        ok.Click += (s,e)=>
        {
            Amount = (int)amountBox.Value;
            ArrivalDate = DateOnly.FromDateTime(datePicker.Value);

            DialogResult = DialogResult.OK;
        };


        Controls.Add(amountBox);
        Controls.Add(datePicker);
        Controls.Add(ok);
    }
}