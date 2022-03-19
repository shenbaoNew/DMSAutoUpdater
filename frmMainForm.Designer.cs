namespace DMSAutoUpdater {
    partial class frmMainForm {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainForm));
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblUpgrade = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblInfo.ForeColor = System.Drawing.Color.Red;
            this.lblInfo.Location = new System.Drawing.Point(104, 47);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(835, 45);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "正在下载更新包，已完成 0 Byte......";
            // 
            // lblUpgrade
            // 
            this.lblUpgrade.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblUpgrade.ForeColor = System.Drawing.Color.Red;
            this.lblUpgrade.Location = new System.Drawing.Point(104, 92);
            this.lblUpgrade.Name = "lblUpgrade";
            this.lblUpgrade.Size = new System.Drawing.Size(835, 45);
            this.lblUpgrade.TabIndex = 1;
            this.lblUpgrade.Text = "正在更新......";
            this.lblUpgrade.Visible = false;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // frmMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 184);
            this.Controls.Add(this.lblUpgrade);
            this.Controls.Add(this.lblInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1039, 263);
            this.MinimumSize = new System.Drawing.Size(1039, 263);
            this.Name = "frmMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DMS更新程序...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMainForm_FormClosed);
            this.Load += new System.EventHandler(this.frmMainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblUpgrade;
        private System.Windows.Forms.Timer timer;
    }
}

