namespace App
{
    partial class RegisterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            label1 = new Label();
            usernameBox = new TextBox();
            label2 = new Label();
            label3 = new Label();
            passwordBox = new TextBox();
            label4 = new Label();
            repeatPasswordBox = new TextBox();
            registerButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 32F);
            label1.Location = new Point(292, 21);
            label1.Name = "label1";
            label1.Size = new Size(208, 59);
            label1.TabIndex = 0;
            label1.Text = "REGISTER";
            // 
            // usernameBox
            // 
            usernameBox.Location = new Point(292, 177);
            usernameBox.Name = "usernameBox";
            usernameBox.Size = new Size(208, 23);
            usernameBox.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 18F);
            label2.Location = new Point(292, 142);
            label2.Name = "label2";
            label2.Size = new Size(121, 32);
            label2.TabIndex = 2;
            label2.Text = "Username";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 18F);
            label3.Location = new Point(292, 234);
            label3.Name = "label3";
            label3.Size = new Size(111, 32);
            label3.TabIndex = 4;
            label3.Text = "Password";
            // 
            // passwordBox
            // 
            passwordBox.Location = new Point(292, 269);
            passwordBox.Name = "passwordBox";
            passwordBox.Size = new Size(208, 23);
            passwordBox.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 18F);
            label4.Location = new Point(292, 295);
            label4.Name = "label4";
            label4.Size = new Size(191, 32);
            label4.TabIndex = 6;
            label4.Text = "Repeat Password";
            // 
            // repeatPasswordBox
            // 
            repeatPasswordBox.Location = new Point(292, 330);
            repeatPasswordBox.Name = "repeatPasswordBox";
            repeatPasswordBox.Size = new Size(208, 23);
            repeatPasswordBox.TabIndex = 5;
            // 
            // registerButton
            // 
            registerButton.Location = new Point(292, 388);
            registerButton.Name = "registerButton";
            registerButton.Size = new Size(208, 23);
            registerButton.TabIndex = 7;
            registerButton.Text = "Register";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += registerButton_Click;
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(registerButton);
            Controls.Add(label4);
            Controls.Add(repeatPasswordBox);
            Controls.Add(label3);
            Controls.Add(passwordBox);
            Controls.Add(label2);
            Controls.Add(usernameBox);
            Controls.Add(label1);
            Name = "RegisterForm";
            Text = "RegisterForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox usernameBox;
        private Label label2;
        private Label label3;
        private TextBox passwordBox;
        private Label label4;
        private TextBox repeatPasswordBox;
        private Button registerButton;
    }
}