namespace Labour
{
    partial class frmCalculoRemuneracion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCalculoRemuneracion));
            this.btnCalculo = new DevExpress.XtraEditors.SimpleButton();
            this.spwaitform = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.barraProgreso = new DevExpress.XtraEditors.ProgressBarControl();
            this.lblTermina = new DevExpress.XtraEditors.LabelControl();
            this.lblComienza = new DevExpress.XtraEditors.LabelControl();
            this.lblEvaluando = new DevExpress.XtraEditors.LabelControl();
            this.lbltranscurrido = new DevExpress.XtraEditors.LabelControl();
            this.txtRegistros = new DevExpress.XtraEditors.TextEdit();
            this.txtPeriodo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMesPalabra = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.barraProgreso.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
            this.groupControl3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMesPalabra.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCalculo
            // 
            this.btnCalculo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCalculo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCalculo.ImageOptions.Image")));
            this.btnCalculo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCalculo.Location = new System.Drawing.Point(17, 126);
            this.btnCalculo.Name = "btnCalculo";
            this.btnCalculo.Size = new System.Drawing.Size(87, 34);
            this.btnCalculo.TabIndex = 4;
            this.btnCalculo.Text = "Calcular";
            this.btnCalculo.ToolTip = "Comenzar Calculo";
            this.btnCalculo.Click += new System.EventHandler(this.btnCalculo_Click);
            // 
            // spwaitform
            // 
            this.spwaitform.ClosingDelay = 500;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(29, 33);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "# Registros:";
            // 
            // barraProgreso
            // 
            this.barraProgreso.Location = new System.Drawing.Point(20, 97);
            this.barraProgreso.Name = "barraProgreso";
            this.barraProgreso.Size = new System.Drawing.Size(390, 18);
            this.barraProgreso.TabIndex = 6;
            // 
            // lblTermina
            // 
            this.lblTermina.Location = new System.Drawing.Point(20, 58);
            this.lblTermina.Name = "lblTermina";
            this.lblTermina.Size = new System.Drawing.Size(42, 13);
            this.lblTermina.TabIndex = 4;
            this.lblTermina.Text = "Termina:";
            // 
            // lblComienza
            // 
            this.lblComienza.Location = new System.Drawing.Point(20, 39);
            this.lblComienza.Name = "lblComienza";
            this.lblComienza.Size = new System.Drawing.Size(50, 13);
            this.lblComienza.TabIndex = 4;
            this.lblComienza.Text = "Comienza:";
            // 
            // lblEvaluando
            // 
            this.lblEvaluando.Location = new System.Drawing.Point(20, 20);
            this.lblEvaluando.Name = "lblEvaluando";
            this.lblEvaluando.Size = new System.Drawing.Size(54, 13);
            this.lblEvaluando.TabIndex = 4;
            this.lblEvaluando.Text = "Evaluando:";
            // 
            // lbltranscurrido
            // 
            this.lbltranscurrido.Location = new System.Drawing.Point(20, 77);
            this.lbltranscurrido.Name = "lbltranscurrido";
            this.lbltranscurrido.Size = new System.Drawing.Size(64, 13);
            this.lbltranscurrido.TabIndex = 4;
            this.lbltranscurrido.Text = "Transcurrido:";
            // 
            // txtRegistros
            // 
            this.txtRegistros.EnterMoveNextControl = true;
            this.txtRegistros.Location = new System.Drawing.Point(102, 30);
            this.txtRegistros.Name = "txtRegistros";
            this.txtRegistros.Properties.ReadOnly = true;
            this.txtRegistros.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtRegistros_Properties_BeforeShowMenu);
            this.txtRegistros.Size = new System.Drawing.Size(50, 20);
            this.txtRegistros.TabIndex = 1;
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.EnterMoveNextControl = true;
            this.txtPeriodo.Location = new System.Drawing.Point(102, 53);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtPeriodo.Properties.Appearance.Options.UseFont = true;
            this.txtPeriodo.Properties.MaxLength = 6;
            this.txtPeriodo.Properties.ReadOnly = true;
            this.txtPeriodo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtPeriodo_Properties_BeforeShowMenu);
            this.txtPeriodo.Size = new System.Drawing.Size(50, 20);
            this.txtPeriodo.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(52, 56);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(40, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "Periodo:";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(480, 34);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(36, 34);
            this.btnSalir.TabIndex = 7;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // groupControl3
            // 
            this.groupControl3.Controls.Add(this.groupBox2);
            this.groupControl3.Controls.Add(this.groupBox1);
            this.groupControl3.Controls.Add(this.btnSalir);
            this.groupControl3.Controls.Add(this.btnCalculo);
            this.groupControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl3.Location = new System.Drawing.Point(0, 0);
            this.groupControl3.Name = "groupControl3";
            this.groupControl3.Size = new System.Drawing.Size(527, 312);
            this.groupControl3.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblName);
            this.groupBox2.Controls.Add(this.barraProgreso);
            this.groupBox2.Controls.Add(this.lblEvaluando);
            this.groupBox2.Controls.Add(this.lblComienza);
            this.groupBox2.Controls.Add(this.lblTermina);
            this.groupBox2.Controls.Add(this.lbltranscurrido);
            this.groupBox2.Location = new System.Drawing.Point(17, 168);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(457, 138);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // lblName
            // 
            this.lblName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblName.Appearance.Options.UseFont = true;
            this.lblName.Location = new System.Drawing.Point(21, 119);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(34, 13);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name";
            this.lblName.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMesPalabra);
            this.groupBox1.Controls.Add(this.txtRegistros);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.txtPeriodo);
            this.groupBox1.Location = new System.Drawing.Point(17, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 96);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Calculo";
            // 
            // txtMesPalabra
            // 
            this.txtMesPalabra.Location = new System.Drawing.Point(155, 53);
            this.txtMesPalabra.Name = "txtMesPalabra";
            this.txtMesPalabra.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtMesPalabra.Properties.Appearance.Options.UseFont = true;
            this.txtMesPalabra.Properties.ReadOnly = true;
            this.txtMesPalabra.Size = new System.Drawing.Size(114, 20);
            this.txtMesPalabra.TabIndex = 3;
            // 
            // frmCalculoRemuneracion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 312);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCalculoRemuneracion";
            this.Text = "Calculo remuneraciones";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCalculoRemuneracion_FormClosing);
            this.Load += new System.EventHandler(this.frmCalculoRemuneracion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barraProgreso.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
            this.groupControl3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMesPalabra.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnCalculo;
        private DevExpress.XtraSplashScreen.SplashScreenManager spwaitform;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtRegistros;
        private DevExpress.XtraEditors.ProgressBarControl barraProgreso;
        private DevExpress.XtraEditors.LabelControl lbltranscurrido;
        private DevExpress.XtraEditors.TextEdit txtPeriodo;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl lblEvaluando;
        private DevExpress.XtraEditors.LabelControl lblTermina;
        private DevExpress.XtraEditors.LabelControl lblComienza;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.GroupControl groupControl3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.TextEdit txtMesPalabra;
        private DevExpress.XtraEditors.LabelControl lblName;
    }
}