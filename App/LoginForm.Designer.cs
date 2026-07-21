namespace App
{
    partial class LoginForm
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
            loginButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 32F);
            label1.Location = new Point(313, 9);
            label1.Name = "label1";
            label1.Size = new Size(149, 59);
            label1.TabIndex = 0;
            label1.Text = "LOGIN";
            // 
            // usernameBox
            // 
            usernameBox.Location = new Point(284, 166);
            usernameBox.Name = "usernameBox";
            usernameBox.Size = new Size(218, 23);
            usernameBox.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F);
            label2.Location = new Point(284, 133);
            label2.Name = "label2";
            label2.Size = new Size(111, 30);
            label2.TabIndex = 2;
            label2.Text = "Username";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16F);
            label3.Location = new Point(284, 228);
            label3.Name = "label3";
            label3.Size = new Size(103, 30);
            label3.TabIndex = 4;
            label3.Text = "Password";
            // 
            // passwordBox
            // 
            passwordBox.Location = new Point(284, 261);
            passwordBox.Name = "passwordBox";
            passwordBox.Size = new Size(218, 23);
            passwordBox.TabIndex = 3;
            // 
            // loginButton
            // 
            loginButton.Location = new Point(284, 372);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(218, 23);
            loginButton.TabIndex = 5;
            loginButton.Text = "LOGIN";
            loginButton.UseVisualStyleBackColor = true;
            loginButton.Click += button1_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(loginButton);
            Controls.Add(label3);
            Controls.Add(passwordBox);
            Controls.Add(label2);
            Controls.Add(usernameBox);
            Controls.Add(label1);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox usernameBox;
        private Label label2;
        private Label label3;
        private TextBox passwordBox;
        private Button loginButton;
    }
}