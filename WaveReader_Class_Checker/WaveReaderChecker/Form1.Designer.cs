namespace WaveReaderChecker
{
    partial class FormOfWaveReaderCheker
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
            this.buttonOfStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonOfStart
            // 
            this.buttonOfStart.Location = new System.Drawing.Point(12, 12);
            this.buttonOfStart.Name = "buttonOfStart";
            this.buttonOfStart.Size = new System.Drawing.Size(260, 25);
            this.buttonOfStart.TabIndex = 0;
            this.buttonOfStart.Text = "Start";
            this.buttonOfStart.UseVisualStyleBackColor = true;
            this.buttonOfStart.Click += new System.EventHandler(this.buttonOfStart_Click);
            // 
            // FormOfWaveReaderCheker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 50);
            this.Controls.Add(this.buttonOfStart);
            this.Name = "FormOfWaveReaderCheker";
            this.Text = "WaveReader Checker";
            this.Load += new System.EventHandler(this.FormOfWaveReaderCheker_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOfStart;
    }
}

