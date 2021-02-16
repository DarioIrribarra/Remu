namespace Labour
{
    partial class frmDatosEmpleado
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDatosEmpleado));
            this.txtTrabajador = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnConsultar = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.gridTrabajador = new DevExpress.XtraGrid.GridControl();
            this.viewTrabajador = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSalirAfp = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtTrabajador.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrabajador)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTrabajador)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtTrabajador
            // 
            this.txtTrabajador.EnterMoveNextControl = true;
            this.txtTrabajador.Location = new System.Drawing.Point(85, 112);
            this.txtTrabajador.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTrabajador.Name = "txtTrabajador";
            this.txtTrabajador.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.textEdit1_Properties_BeforeShowMenu);
            this.txtTrabajador.Size = new System.Drawing.Size(183, 22);
            this.txtTrabajador.TabIndex = 4;
            this.txtTrabajador.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrabajador_KeyPress_1);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(29, 31);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(48, 16);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Periodo:";
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(85, 54);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(215, 20);
            this.cbTodos.TabIndex = 1;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // txtConjunto
            // 
            this.txtConjunto.Enabled = false;
            this.txtConjunto.EnterMoveNextControl = true;
            this.txtConjunto.Location = new System.Drawing.Point(85, 85);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtConjunto_Properties_BeforeShowMenu);
            this.txtConjunto.Size = new System.Drawing.Size(69, 22);
            this.txtConjunto.TabIndex = 2;
            this.txtConjunto.DoubleClick += new System.EventHandler(this.AgregarConjunto_Click);
            this.txtConjunto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrabajador_KeyPress);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(34, 113);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(43, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Buscar:";
            // 
            // btnConsultar
            // 
            this.btnConsultar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConsultar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConsultar.ImageOptions.Image")));
            this.btnConsultar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnConsultar.Location = new System.Drawing.Point(85, 144);
            this.btnConsultar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(106, 37);
            this.btnConsultar.TabIndex = 5;
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.ToolTip = "Comenzar Busqueda";
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(19, 233);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(82, 21);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Resultados";
            // 
            // gridTrabajador
            // 
            this.gridTrabajador.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gridTrabajador.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridTrabajador.Location = new System.Drawing.Point(19, 257);
            this.gridTrabajador.MainView = this.viewTrabajador;
            this.gridTrabajador.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridTrabajador.Name = "gridTrabajador";
            this.gridTrabajador.Size = new System.Drawing.Size(617, 449);
            this.gridTrabajador.TabIndex = 2;
            this.gridTrabajador.TabStop = false;
            this.gridTrabajador.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewTrabajador});
            this.gridTrabajador.DoubleClick += new System.EventHandler(this.gridTrabajador_DoubleClick);
            // 
            // viewTrabajador
            // 
            this.viewTrabajador.DetailHeight = 431;
            this.viewTrabajador.GridControl = this.gridTrabajador;
            this.viewTrabajador.Name = "viewTrabajador";
            this.viewTrabajador.OptionsView.ShowGroupPanel = false;
            this.viewTrabajador.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.viewTrabajador_PopupMenuShowing);
            // 
            // btnSalirAfp
            // 
            this.btnSalirAfp.AllowFocus = false;
            this.btnSalirAfp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirAfp.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalirAfp.Location = new System.Drawing.Point(559, 21);
            this.btnSalirAfp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalirAfp.Name = "btnSalirAfp";
            this.btnSalirAfp.Size = new System.Drawing.Size(45, 37);
            this.btnSalirAfp.TabIndex = 6;
            this.btnSalirAfp.ToolTip = "Cerrar Formulario";
            this.btnSalirAfp.Click += new System.EventHandler(this.btnSalirAfp_Click);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(17, 89);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(60, 16);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "Condición:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnEditarReporte);
            this.groupBox1.Controls.Add(this.btnConjunto);
            this.groupBox1.Controls.Add(this.btnSalirAfp);
            this.groupBox1.Controls.Add(this.txtConjunto);
            this.groupBox1.Controls.Add(this.btnConsultar);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.txtTrabajador);
            this.groupBox1.Controls.Add(this.txtComboPeriodo);
            this.groupBox1.Controls.Add(this.cbTodos);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Location = new System.Drawing.Point(19, 17);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(616, 209);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formulario";
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.Enabled = false;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(161, 84);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 3;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(85, 25);
            this.txtComboPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Size = new System.Drawing.Size(202, 22);
            this.txtComboPeriodo.TabIndex = 0;
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(197, 144);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 25;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // frmDatosEmpleado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 716);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gridTrabajador);
            this.Controls.Add(this.labelControl2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmDatosEmpleado";
            this.Text = "Datos Trabajador";
            this.Load += new System.EventHandler(this.frmDatosEmpleado_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtTrabajador.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrabajador)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTrabajador)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnConsultar;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraGrid.GridControl gridTrabajador;
        private DevExpress.XtraGrid.Views.Grid.GridView viewTrabajador;
        private DevExpress.XtraEditors.SimpleButton btnSalirAfp;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtTrabajador;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}