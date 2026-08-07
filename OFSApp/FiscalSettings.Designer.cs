namespace OFSApp;

partial class FiscalSettings
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        ipInput = new System.Windows.Forms.TextBox();
        portInput = new System.Windows.Forms.TextBox();
        label3 = new System.Windows.Forms.Label();
        keyInput = new System.Windows.Forms.TextBox();
        label4 = new System.Windows.Forms.Label();
        pinInput = new System.Windows.Forms.TextBox();
        label5 = new System.Windows.Forms.Label();
        cancelButton = new System.Windows.Forms.Button();
        saveButton = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Font = new System.Drawing.Font("Segoe UI", 24F);
        label1.Location = new System.Drawing.Point(12, 9);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(268, 44);
        label1.TabIndex = 0;
        label1.Text = "Fiskalne Postavke";
        // 
        // label2
        // 
        label2.Font = new System.Drawing.Font("Segoe UI", 16F);
        label2.Location = new System.Drawing.Point(12, 73);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(108, 36);
        label2.TabIndex = 1;
        label2.Text = "IP Adresa";
        // 
        // ipInput
        // 
        ipInput.Font = new System.Drawing.Font("Segoe UI", 16F);
        ipInput.Location = new System.Drawing.Point(120, 73);
        ipInput.Name = "ipInput";
        ipInput.Size = new System.Drawing.Size(160, 36);
        ipInput.TabIndex = 2;
        // 
        // portInput
        // 
        portInput.Font = new System.Drawing.Font("Segoe UI", 16F);
        portInput.Location = new System.Drawing.Point(120, 115);
        portInput.Name = "portInput";
        portInput.Size = new System.Drawing.Size(160, 36);
        portInput.TabIndex = 4;
        // 
        // label3
        // 
        label3.Font = new System.Drawing.Font("Segoe UI", 16F);
        label3.Location = new System.Drawing.Point(12, 115);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(108, 36);
        label3.TabIndex = 3;
        label3.Text = "Port\r\n";
        // 
        // keyInput
        // 
        keyInput.Font = new System.Drawing.Font("Segoe UI", 16F);
        keyInput.Location = new System.Drawing.Point(120, 157);
        keyInput.Name = "keyInput";
        keyInput.Size = new System.Drawing.Size(160, 36);
        keyInput.TabIndex = 6;
        // 
        // label4
        // 
        label4.Font = new System.Drawing.Font("Segoe UI", 16F);
        label4.Location = new System.Drawing.Point(12, 157);
        label4.Name = "label4";
        label4.Size = new System.Drawing.Size(108, 36);
        label4.TabIndex = 5;
        label4.Text = "Kljuć";
        // 
        // pinInput
        // 
        pinInput.Font = new System.Drawing.Font("Segoe UI", 16F);
        pinInput.Location = new System.Drawing.Point(120, 199);
        pinInput.Name = "pinInput";
        pinInput.Size = new System.Drawing.Size(160, 36);
        pinInput.TabIndex = 8;
        // 
        // label5
        // 
        label5.Font = new System.Drawing.Font("Segoe UI", 16F);
        label5.Location = new System.Drawing.Point(12, 199);
        label5.Name = "label5";
        label5.Size = new System.Drawing.Size(108, 36);
        label5.TabIndex = 7;
        label5.Text = "Pin";
        // 
        // cancelButton
        // 
        cancelButton.Location = new System.Drawing.Point(12, 247);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new System.Drawing.Size(108, 42);
        cancelButton.TabIndex = 9;
        cancelButton.Text = "Otkaži";
        cancelButton.UseVisualStyleBackColor = true;
        // 
        // saveButton
        // 
        saveButton.Location = new System.Drawing.Point(172, 247);
        saveButton.Name = "saveButton";
        saveButton.Size = new System.Drawing.Size(108, 42);
        saveButton.TabIndex = 10;
        saveButton.Text = "Spasi";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += saveButton_Click;
        // 
        // FiscalSettings
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(296, 301);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
        Controls.Add(pinInput);
        Controls.Add(label5);
        Controls.Add(keyInput);
        Controls.Add(label4);
        Controls.Add(portInput);
        Controls.Add(label3);
        Controls.Add(ipInput);
        Controls.Add(label2);
        Controls.Add(label1);
        Text = "Fiskalne Postavke";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button saveButton;

    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox ipInput;
    private System.Windows.Forms.TextBox portInput;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox keyInput;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox pinInput;
    private System.Windows.Forms.Label label5;

    private System.Windows.Forms.Label label1;

    #endregion
}