namespace zipkakuyomu
{
    partial class FormMain
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            buttonOK = new Button();
            label1 = new Label();
            textBoxUrl = new TextBox();
            label2 = new Label();
            textBoxZipFilename = new TextBox();
            buttonBrowse = new Button();
            webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            saveFileDialogZipFile = new SaveFileDialog();
            buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)webView2).BeginInit();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(986, 415);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(75, 23);
            buttonOK.TabIndex = 7;
            buttonOK.Text = "開始";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += Button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(92, 15);
            label1.TabIndex = 0;
            label1.Text = "タイトルページ&URL";
            // 
            // textBoxUrl
            // 
            textBoxUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxUrl.Location = new Point(110, 6);
            textBoxUrl.Name = "textBoxUrl";
            textBoxUrl.Size = new Size(951, 23);
            textBoxUrl.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 45);
            label2.Name = "label2";
            label2.Size = new Size(91, 15);
            label2.TabIndex = 2;
            label2.Text = "保存先ファイル(&F)";
            // 
            // textBoxZipFilename
            // 
            textBoxZipFilename.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxZipFilename.Location = new Point(110, 42);
            textBoxZipFilename.Name = "textBoxZipFilename";
            textBoxZipFilename.Size = new Size(911, 23);
            textBoxZipFilename.TabIndex = 3;
            // 
            // buttonBrowse
            // 
            buttonBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonBrowse.Location = new Point(1027, 42);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new Size(34, 23);
            buttonBrowse.TabIndex = 4;
            buttonBrowse.Text = "...";
            buttonBrowse.UseVisualStyleBackColor = true;
            buttonBrowse.Click += ButtonBrowse_Click;
            // 
            // webView2
            // 
            webView2.AllowExternalDrop = true;
            webView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView2.CreationProperties = null;
            webView2.DefaultBackgroundColor = Color.White;
            webView2.Location = new Point(12, 71);
            webView2.Name = "webView2";
            webView2.Size = new Size(1049, 326);
            webView2.TabIndex = 5;
            webView2.ZoomFactor = 1D;
            // 
            // saveFileDialogZipFile
            // 
            saveFileDialogZipFile.DefaultExt = "zip";
            saveFileDialogZipFile.FileName = "noname.zip";
            saveFileDialogZipFile.Filter = "zip(*.zip)|*.zip|all(*.*)|*.*";
            saveFileDialogZipFile.Title = "ファイル名を付けて保存";
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(890, 415);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 6;
            buttonCancel.Text = "キャンセル";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // FormMain
            // 
            AcceptButton = buttonOK;
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(1083, 450);
            Controls.Add(buttonCancel);
            Controls.Add(webView2);
            Controls.Add(buttonBrowse);
            Controls.Add(textBoxZipFilename);
            Controls.Add(label2);
            Controls.Add(textBoxUrl);
            Controls.Add(label1);
            Controls.Add(buttonOK);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Text = "カクヨムを一括ダウンロード";
            Load += FormMain_Load;
            DragDrop += FormMain_DragDrop;
            DragEnter += FormMain_DragEnter;
            ((System.ComponentModel.ISupportInitialize)webView2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonOK;
        private Label label1;
        private TextBox textBoxUrl;
        private Label label2;
        private TextBox textBoxZipFilename;
        private Button buttonBrowse;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;
        private SaveFileDialog saveFileDialogZipFile;
        private Button buttonCancel;
    }
}
