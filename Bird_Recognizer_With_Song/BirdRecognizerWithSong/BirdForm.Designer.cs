namespace BirdRecognizerWithSong
{
    partial class BirdForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BirdForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.ProgressBar4ReadingPoint = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDiscriminatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FeatureSetStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.FeatureGeneratorSelector = new System.Windows.Forms.ToolStripComboBox();
            this.thresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TBforThreshold = new System.Windows.Forms.ToolStripTextBox();
            this.soundDetectorLoggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CmboxSoundDetectorLog = new System.Windows.Forms.ToolStripComboBox();
            this.detectionMoceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CmboxDetectionMode = new System.Windows.Forms.ToolStripComboBox();
            this.operationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.learningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discriminatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discriminationWithWAVEFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discriminationWithWAVEFilesInDirectryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crossValidationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage4Graph = new System.Windows.Forms.TabPage();
            this.pictureBox4Graph = new System.Windows.Forms.PictureBox();
            this.tabPage4Setting = new System.Windows.Forms.TabPage();
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage4Graph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4Graph)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar4ReadingPoint});
            this.statusStrip.Location = new System.Drawing.Point(0, 176);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(403, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // ProgressBar4ReadingPoint
            // 
            this.ProgressBar4ReadingPoint.Name = "ProgressBar4ReadingPoint";
            this.ProgressBar4ReadingPoint.Size = new System.Drawing.Size(100, 16);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingToolStripMenuItem,
            this.operationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(403, 26);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDiscriminatorToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openDiscriminatorToolStripMenuItem
            // 
            this.openDiscriminatorToolStripMenuItem.Name = "openDiscriminatorToolStripMenuItem";
            this.openDiscriminatorToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.openDiscriminatorToolStripMenuItem.Text = "Open discriminator";
            this.openDiscriminatorToolStripMenuItem.ToolTipText = "識別器の設定を読み込みます";
            this.openDiscriminatorToolStripMenuItem.Click += new System.EventHandler(this.openDiscriminatorToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.ToolTipText = "本ソフトを終了します";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FeatureSetStrip,
            this.thresholdToolStripMenuItem,
            this.soundDetectorLoggingToolStripMenuItem,
            this.detectionMoceToolStripMenuItem});
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(62, 22);
            this.settingToolStripMenuItem.Text = "Setting";
            this.settingToolStripMenuItem.ToolTipText = "設定してください";
            // 
            // FeatureSetStrip
            // 
            this.FeatureSetStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FeatureGeneratorSelector});
            this.FeatureSetStrip.Name = "FeatureSetStrip";
            this.FeatureSetStrip.Size = new System.Drawing.Size(216, 22);
            this.FeatureSetStrip.Text = "FeatureSet";
            this.FeatureSetStrip.ToolTipText = "特徴ベクトル生成に使用するユニットを選択します";
            // 
            // FeatureGeneratorSelector
            // 
            this.FeatureGeneratorSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FeatureGeneratorSelector.Name = "FeatureGeneratorSelector";
            this.FeatureGeneratorSelector.Size = new System.Drawing.Size(150, 26);
            this.FeatureGeneratorSelector.Sorted = true;
            // 
            // thresholdToolStripMenuItem
            // 
            this.thresholdToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TBforThreshold});
            this.thresholdToolStripMenuItem.Name = "thresholdToolStripMenuItem";
            this.thresholdToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.thresholdToolStripMenuItem.Text = "Threshold";
            this.thresholdToolStripMenuItem.ToolTipText = "識別時の判別閾値を決定します。\r\n例えば0.9と設定した場合、識別器の出力尤度が0.9未満であるとUnknown扱いとなります。";
            // 
            // TBforThreshold
            // 
            this.TBforThreshold.Name = "TBforThreshold";
            this.TBforThreshold.Size = new System.Drawing.Size(100, 25);
            this.TBforThreshold.ToolTipText = "ここに識別時の判別閾値を入力します";
            // 
            // soundDetectorLoggingToolStripMenuItem
            // 
            this.soundDetectorLoggingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmboxSoundDetectorLog});
            this.soundDetectorLoggingToolStripMenuItem.Name = "soundDetectorLoggingToolStripMenuItem";
            this.soundDetectorLoggingToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.soundDetectorLoggingToolStripMenuItem.Text = "Sound Detector Logging";
            this.soundDetectorLoggingToolStripMenuItem.ToolTipText = "鳴き声検出器の動作ログを残すかどうかを選択します。\r\nTrueを選択した場合、非常に動作が遅くなり、かつ音源が長いと非常に大きなファイルを作成され得ます。";
            // 
            // CmboxSoundDetectorLog
            // 
            this.CmboxSoundDetectorLog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmboxSoundDetectorLog.Items.AddRange(new object[] {
            "True",
            "False"});
            this.CmboxSoundDetectorLog.Name = "CmboxSoundDetectorLog";
            this.CmboxSoundDetectorLog.Size = new System.Drawing.Size(121, 26);
            // 
            // detectionMoceToolStripMenuItem
            // 
            this.detectionMoceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmboxDetectionMode});
            this.detectionMoceToolStripMenuItem.Name = "detectionMoceToolStripMenuItem";
            this.detectionMoceToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.detectionMoceToolStripMenuItem.Text = "Detection Moce";
            this.detectionMoceToolStripMenuItem.ToolTipText = "音源ファイルを使った識別時の動作\r\nフィルタ名と識別結果が同一であった場合にのみ識別結果を保存するかどうか選択します。";
            // 
            // CmboxDetectionMode
            // 
            this.CmboxDetectionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmboxDetectionMode.Items.AddRange(new object[] {
            "Default",
            "Match case only"});
            this.CmboxDetectionMode.Name = "CmboxDetectionMode";
            this.CmboxDetectionMode.Size = new System.Drawing.Size(121, 26);
            // 
            // operationToolStripMenuItem
            // 
            this.operationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFeatureToolStripMenuItem,
            this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem,
            this.learningToolStripMenuItem,
            this.discriminatorToolStripMenuItem,
            this.discriminationWithWAVEFileToolStripMenuItem,
            this.discriminationWithWAVEFilesInDirectryToolStripMenuItem,
            this.crossValidationToolStripMenuItem});
            this.operationToolStripMenuItem.Name = "operationToolStripMenuItem";
            this.operationToolStripMenuItem.Size = new System.Drawing.Size(77, 22);
            this.operationToolStripMenuItem.Text = "Operation";
            this.operationToolStripMenuItem.ToolTipText = "動作を選択します";
            // 
            // createFeatureToolStripMenuItem
            // 
            this.createFeatureToolStripMenuItem.Name = "createFeatureToolStripMenuItem";
            this.createFeatureToolStripMenuItem.Size = new System.Drawing.Size(361, 22);
            this.createFeatureToolStripMenuItem.Text = "Create feature file from a WAVE file";
            this.createFeatureToolStripMenuItem.ToolTipText = "一つの音源ファイルから特徴ベクトルを生成します";
            this.createFeatureToolStripMenuItem.Click += new System.EventHandler(this.createFeatureToolStripMenuItem_Click);
            // 
            // createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem
            // 
            this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem.Name = "createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem";
            this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem.Size = new System.Drawing.Size(361, 22);
            this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem.Text = "Create feature file from WAVE files in a directory";
            this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem.ToolTipText = "多数の音源ファイルから特徴ベクトルを生成します";
            this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem.Click += new System.EventHandler(this.createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem_Click);
            // 
            // learningToolStripMenuItem
            // 
            this.learningToolStripMenuItem.Name = "learningToolStripMenuItem";
            this.learningToolStripMenuItem.Size = new System.Drawing.Size(361, 22);
            this.learningToolStripMenuItem.Text = "Learning";
            this.learningToolStripMenuItem.ToolTipText = "ファイルを指定して学習を行います";
            this.learningToolStripMenuItem.Click += new System.EventHandler(this.learningToolStripMenuItem_Click);
            // 
            // discriminatorToolStripMenuItem
            // 
            this.discriminatorToolStripMenuItem.Name = "discriminatorToolStripMenuItem";
            this.discriminatorToolStripMenuItem.Size = new System.Drawing.Size(361, 22);
            this.discriminatorToolStripMenuItem.Text = "Discrimination created features in a file";
            this.discriminatorToolStripMenuItem.ToolTipText = "ファイルを指定して、ファイル内に書かれた特徴ベクトルを識別します";
            this.discriminatorToolStripMenuItem.Click += new System.EventHandler(this.discriminatorToolStripMenuItem_Click);
            // 
            // discriminationWithWAVEFileToolStripMenuItem
            // 
            this.discriminationWithWAVEFileToolStripMenuItem.Name = "discriminationWithWAVEFileToolStripMenuItem";
            this.discriminationWithWAVEFileToolStripMenuItem.Size = new System.Drawing.Size(361, 22);
            this.discriminationWithWAVEFileToolStripMenuItem.Text = "Discrimination with a WAVE file";
            this.discriminationWithWAVEFileToolStripMenuItem.ToolTipText = "一つの音源を指定して、識別を実施します";
            this.discriminationWithWAVEFileToolStripMenuItem.Click += new System.EventHandler(this.discriminationWithWAVEFileToolStripMenuItem_Click);
            // 
            // discriminationWithWAVEFilesInDirectryToolStripMenuItem
            // 
            this.discriminationWithWAVEFilesInDirectryToolStripMenuItem.Name = "discriminationWithWAVEFilesInDirectryToolStripMenuItem";
            this.discriminationWithWAVEFilesInDirectryToolStripMenuItem.Size = new System.Drawing.Size(361, 22);
            this.discriminationWithWAVEFilesInDirectryToolStripMenuItem.Text = "Discrimination with WAVE files in a directory";
            this.discriminationWithWAVEFilesInDirectryToolStripMenuItem.ToolTipText = "複数の音源を指定して、識別を実施します";
            this.discriminationWithWAVEFilesInDirectryToolStripMenuItem.Click += new System.EventHandler(this.discriminationWithWAVEFilesInDirectryToolStripMenuItem_Click);
            // 
            // crossValidationToolStripMenuItem
            // 
            this.crossValidationToolStripMenuItem.Name = "crossValidationToolStripMenuItem";
            this.crossValidationToolStripMenuItem.Size = new System.Drawing.Size(361, 22);
            this.crossValidationToolStripMenuItem.Text = "Cross Validation";
            this.crossValidationToolStripMenuItem.ToolTipText = "学習用教師データ群を選択して交差確認法を実施します。\r\n生成した特徴ベクトルの識別能力の評価に利用可能です。";
            this.crossValidationToolStripMenuItem.Click += new System.EventHandler(this.crossValidationToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage4Graph);
            this.tabControl.Controls.Add(this.tabPage4Setting);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 26);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(403, 150);
            this.tabControl.TabIndex = 2;
            // 
            // tabPage4Graph
            // 
            this.tabPage4Graph.Controls.Add(this.pictureBox4Graph);
            this.tabPage4Graph.Location = new System.Drawing.Point(4, 22);
            this.tabPage4Graph.Name = "tabPage4Graph";
            this.tabPage4Graph.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4Graph.Size = new System.Drawing.Size(395, 124);
            this.tabPage4Graph.TabIndex = 0;
            this.tabPage4Graph.Text = "Graph";
            this.tabPage4Graph.UseVisualStyleBackColor = true;
            // 
            // pictureBox4Graph
            // 
            this.pictureBox4Graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox4Graph.Location = new System.Drawing.Point(3, 3);
            this.pictureBox4Graph.Name = "pictureBox4Graph";
            this.pictureBox4Graph.Size = new System.Drawing.Size(389, 118);
            this.pictureBox4Graph.TabIndex = 0;
            this.pictureBox4Graph.TabStop = false;
            // 
            // tabPage4Setting
            // 
            this.tabPage4Setting.Location = new System.Drawing.Point(4, 22);
            this.tabPage4Setting.Name = "tabPage4Setting";
            this.tabPage4Setting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4Setting.Size = new System.Drawing.Size(395, 124);
            this.tabPage4Setting.TabIndex = 1;
            this.tabPage4Setting.Text = "Setting";
            this.tabPage4Setting.UseVisualStyleBackColor = true;
            // 
            // BirdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 198);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BirdForm";
            this.Text = "Bird Recognizer With Song";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BirdForm_FormClosing);
            this.Load += new System.EventHandler(this.BirdForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage4Graph.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4Graph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar4ReadingPoint;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage4Graph;
        private System.Windows.Forms.PictureBox pictureBox4Graph;
        private System.Windows.Forms.TabPage tabPage4Setting;
        private System.Windows.Forms.ToolStripMenuItem operationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createFeatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem learningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem discriminatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem discriminationWithWAVEFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem discriminationWithWAVEFilesInDirectryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDiscriminatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createFeatureFileFromWAVEFilesInDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FeatureSetStrip;
        private System.Windows.Forms.ToolStripComboBox FeatureGeneratorSelector;
        private System.Windows.Forms.ToolStripMenuItem crossValidationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox TBforThreshold;
        private System.Windows.Forms.ToolStripMenuItem soundDetectorLoggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox CmboxSoundDetectorLog;
        private System.Windows.Forms.ToolStripMenuItem detectionMoceToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox CmboxDetectionMode;
    }
}

