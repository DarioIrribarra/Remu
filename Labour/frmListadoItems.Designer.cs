namespace Labour
{
    partial class frmListadoItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmListadoItems));
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.btnGenerar = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.lueOrdenPor = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueOrdenPor.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(85, 26);
            this.txtComboPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Properties.PopupSizeable = false;
            this.txtComboPeriodo.Size = new System.Drawing.Size(159, 22);
            this.txtComboPeriodo.TabIndex = 16;
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(157, 78);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 17;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(31, 30);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 13;
            this.labelControl1.Text = "Periodo:";
            // 
            // txtConjunto
            // 
            this.txtConjunto.Location = new System.Drawing.Point(85, 79);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtConjunto_Properties_BeforeShowMenu);
            this.txtConjunto.Size = new System.Drawing.Size(65, 22);
            this.txtConjunto.TabIndex = 15;
            this.txtConjunto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtConjunto_KeyPress);
            // 
            // cbTodos
            // 
            this.cbTodos.Location = new System.Drawing.Point(85, 53);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(215, 20);
            this.cbTodos.TabIndex = 14;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // btnGenerar
            // 
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerar.ImageOptions.Image")));
            this.btnGenerar.Location = new System.Drawing.Point(85, 140);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(103, 43);
            this.btnGenerar.TabIndex = 18;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSalir.Location = new System.Drawing.Point(579, 14);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(44, 37);
            this.btnSalir.TabIndex = 115;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // lueOrdenPor
            // 
            this.lueOrdenPor.Location = new System.Drawing.Point(85, 111);
            this.lueOrdenPor.Name = "lueOrdenPor";
            this.lueOrdenPor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueOrdenPor.Size = new System.Drawing.Size(125, 22);
            this.lueOrdenPor.TabIndex = 116;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(16, 114);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(63, 16);
            this.labelControl2.TabIndex = 117;
            this.labelControl2.Text = "Orden por:";
            // 
            // frmListadoItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 210);
            this.ControlBox = false;
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.lueOrdenPor);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.txtComboPeriodo);
            this.Controls.Add(this.btnConjunto);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.txtConjunto);
            this.Controls.Add(this.cbTodos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmListadoItems";
            this.Text = "Reporte Items";
            this.Load += new System.EventHandler(this.frmListadoItems_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueOrdenPor.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.SimpleButton btnGenerar;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.LookUpEdit lueOrdenPor;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}