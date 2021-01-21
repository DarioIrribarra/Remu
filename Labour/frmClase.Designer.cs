namespace Labour
{
    partial class frmClase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClase));
            this.separador1 = new DevExpress.XtraEditors.SeparatorControl();
            this.btnEliminarClase = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevaClase = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardarClase = new DevExpress.XtraEditors.SimpleButton();
            this.lblclase = new DevExpress.XtraEditors.LabelControl();
            this.txtdesc = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtcod = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.lblerror = new DevExpress.XtraEditors.LabelControl();
            this.txtnumitem = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtformula = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtitem = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.gridClase = new DevExpress.XtraGrid.GridControl();
            this.viewClase = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnnuevoitem = new DevExpress.XtraEditors.SimpleButton();
            this.btnguardarItemClase = new DevExpress.XtraEditors.SimpleButton();
            this.btnEliminaritemClase = new DevExpress.XtraEditors.SimpleButton();
            this.griditemClase = new DevExpress.XtraGrid.GridControl();
            this.viewitemClase = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupItemClase = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.separador1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdesc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtnumitem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtformula.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtitem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridClase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewClase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.griditemClase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewitemClase)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupItemClase.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // separador1
            // 
            this.separador1.BackColor = System.Drawing.Color.Transparent;
            this.separador1.LineOrientation = System.Windows.Forms.Orientation.Vertical;
            this.separador1.Location = new System.Drawing.Point(402, 13);
            this.separador1.Name = "separador1";
            this.separador1.Size = new System.Drawing.Size(30, 465);
            this.separador1.TabIndex = 7;
            this.separador1.TabStop = false;
            // 
            // btnEliminarClase
            // 
            this.btnEliminarClase.AllowFocus = false;
            this.btnEliminarClase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarClase.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminarClase.ImageOptions.Image")));
            this.btnEliminarClase.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminarClase.Location = new System.Drawing.Point(239, 110);
            this.btnEliminarClase.Name = "btnEliminarClase";
            this.btnEliminarClase.Size = new System.Drawing.Size(36, 30);
            this.btnEliminarClase.TabIndex = 3;
            this.btnEliminarClase.ToolTip = "Eliminar Clase";
            this.btnEliminarClase.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnNuevaClase
            // 
            this.btnNuevaClase.AllowFocus = false;
            this.btnNuevaClase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevaClase.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevaClase.ImageOptions.Image")));
            this.btnNuevaClase.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevaClase.Location = new System.Drawing.Point(88, 110);
            this.btnNuevaClase.Name = "btnNuevaClase";
            this.btnNuevaClase.Size = new System.Drawing.Size(79, 30);
            this.btnNuevaClase.TabIndex = 1;
            this.btnNuevaClase.Text = "Nuevo";
            this.btnNuevaClase.ToolTip = "Nueva Clase";
            this.btnNuevaClase.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardarClase
            // 
            this.btnGuardarClase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardarClase.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardarClase.ImageOptions.Image")));
            this.btnGuardarClase.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardarClase.Location = new System.Drawing.Point(170, 110);
            this.btnGuardarClase.Name = "btnGuardarClase";
            this.btnGuardarClase.Size = new System.Drawing.Size(66, 30);
            this.btnGuardarClase.TabIndex = 2;
            this.btnGuardarClase.Text = "Guardar";
            this.btnGuardarClase.ToolTip = "Guardar Clase";
            this.btnGuardarClase.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // lblclase
            // 
            this.lblclase.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblclase.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblclase.Appearance.Options.UseFont = true;
            this.lblclase.Appearance.Options.UseForeColor = true;
            this.lblclase.Location = new System.Drawing.Point(88, 64);
            this.lblclase.Name = "lblclase";
            this.lblclase.Size = new System.Drawing.Size(53, 13);
            this.lblclase.TabIndex = 33;
            this.lblclase.Text = "message";
            this.lblclase.Visible = false;
            // 
            // txtdesc
            // 
            this.txtdesc.EnterMoveNextControl = true;
            this.txtdesc.Location = new System.Drawing.Point(88, 42);
            this.txtdesc.Name = "txtdesc";
            this.txtdesc.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtdesc.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtdesc_Properties_BeforeShowMenu);
            this.txtdesc.Size = new System.Drawing.Size(247, 20);
            this.txtdesc.TabIndex = 5;
            this.txtdesc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtdesc_KeyPress);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(21, 45);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(58, 13);
            this.labelControl2.TabIndex = 31;
            this.labelControl2.Text = "Descripcion:";
            // 
            // txtcod
            // 
            this.txtcod.EnterMoveNextControl = true;
            this.txtcod.Location = new System.Drawing.Point(88, 20);
            this.txtcod.Name = "txtcod";
            this.txtcod.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtcod_Properties_BeforeShowMenu);
            this.txtcod.Size = new System.Drawing.Size(55, 20);
            this.txtcod.TabIndex = 4;
            this.txtcod.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtcod_KeyDown);
            this.txtcod.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtcod_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Codigo:";
            // 
            // lblerror
            // 
            this.lblerror.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblerror.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblerror.Appearance.Options.UseFont = true;
            this.lblerror.Appearance.Options.UseForeColor = true;
            this.lblerror.Location = new System.Drawing.Point(82, 93);
            this.lblerror.Name = "lblerror";
            this.lblerror.Size = new System.Drawing.Size(53, 13);
            this.lblerror.TabIndex = 32;
            this.lblerror.Text = "message";
            this.lblerror.Visible = false;
            // 
            // txtnumitem
            // 
            this.txtnumitem.EnterMoveNextControl = true;
            this.txtnumitem.Location = new System.Drawing.Point(82, 71);
            this.txtnumitem.Name = "txtnumitem";
            this.txtnumitem.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtnumitem_Properties_BeforeShowMenu);
            this.txtnumitem.Size = new System.Drawing.Size(55, 20);
            this.txtnumitem.TabIndex = 14;
            this.txtnumitem.EditValueChanged += new System.EventHandler(this.txtnumitem_EditValueChanged);
            this.txtnumitem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtnumitem_KeyDown);
            this.txtnumitem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtnumitem_KeyPress);
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(32, 78);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(41, 13);
            this.labelControl5.TabIndex = 37;
            this.labelControl5.Text = "N° Item:";
            // 
            // txtformula
            // 
            this.txtformula.EnterMoveNextControl = true;
            this.txtformula.Location = new System.Drawing.Point(82, 50);
            this.txtformula.Name = "txtformula";
            this.txtformula.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtformula.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtformula_Properties_BeforeShowMenu);
            this.txtformula.Size = new System.Drawing.Size(241, 20);
            this.txtformula.TabIndex = 13;
            this.txtformula.EditValueChanged += new System.EventHandler(this.txtformula_EditValueChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(31, 56);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(42, 13);
            this.labelControl4.TabIndex = 36;
            this.labelControl4.Text = "Formula:";
            // 
            // txtitem
            // 
            this.txtitem.EnterMoveNextControl = true;
            this.txtitem.Location = new System.Drawing.Point(82, 29);
            this.txtitem.Name = "txtitem";
            this.txtitem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtitem.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtitem_Properties_BeforeShowMenu);
            this.txtitem.Size = new System.Drawing.Size(241, 20);
            this.txtitem.TabIndex = 12;
            this.txtitem.EditValueChanged += new System.EventHandler(this.txtitem_EditValueChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(46, 33);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(26, 13);
            this.labelControl3.TabIndex = 34;
            this.labelControl3.Text = "Item:";
            // 
            // gridClase
            // 
            this.gridClase.Location = new System.Drawing.Point(12, 20);
            this.gridClase.MainView = this.viewClase;
            this.gridClase.Name = "gridClase";
            this.gridClase.Size = new System.Drawing.Size(353, 247);
            this.gridClase.TabIndex = 6;
            this.gridClase.TabStop = false;
            this.gridClase.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewClase});
            this.gridClase.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridClase_ProcessGridKey);
            this.gridClase.Click += new System.EventHandler(this.gridClase_Click);
            this.gridClase.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridClase_KeyUp);
            // 
            // viewClase
            // 
            this.viewClase.GridControl = this.gridClase;
            this.viewClase.Name = "viewClase";
            this.viewClase.OptionsView.ShowGroupPanel = false;
            // 
            // btnnuevoitem
            // 
            this.btnnuevoitem.AllowFocus = false;
            this.btnnuevoitem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnnuevoitem.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnnuevoitem.ImageOptions.Image")));
            this.btnnuevoitem.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnnuevoitem.Location = new System.Drawing.Point(82, 109);
            this.btnnuevoitem.Name = "btnnuevoitem";
            this.btnnuevoitem.Size = new System.Drawing.Size(76, 30);
            this.btnnuevoitem.TabIndex = 9;
            this.btnnuevoitem.Text = "Nuevo";
            this.btnnuevoitem.ToolTip = "Nuevo item clase";
            this.btnnuevoitem.Click += new System.EventHandler(this.btnnuevoitem_Click);
            // 
            // btnguardarItemClase
            // 
            this.btnguardarItemClase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnguardarItemClase.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnguardarItemClase.ImageOptions.Image")));
            this.btnguardarItemClase.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnguardarItemClase.Location = new System.Drawing.Point(164, 109);
            this.btnguardarItemClase.Name = "btnguardarItemClase";
            this.btnguardarItemClase.Size = new System.Drawing.Size(75, 30);
            this.btnguardarItemClase.TabIndex = 10;
            this.btnguardarItemClase.Text = "Guardar";
            this.btnguardarItemClase.ToolTip = "Guardar item clase";
            this.btnguardarItemClase.Click += new System.EventHandler(this.btnguardarItemClase_Click);
            // 
            // btnEliminaritemClase
            // 
            this.btnEliminaritemClase.AllowFocus = false;
            this.btnEliminaritemClase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminaritemClase.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminaritemClase.ImageOptions.Image")));
            this.btnEliminaritemClase.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminaritemClase.Location = new System.Drawing.Point(243, 109);
            this.btnEliminaritemClase.Name = "btnEliminaritemClase";
            this.btnEliminaritemClase.Size = new System.Drawing.Size(35, 30);
            this.btnEliminaritemClase.TabIndex = 11;
            this.btnEliminaritemClase.ToolTip = "Eliminar item clase";
            this.btnEliminaritemClase.Click += new System.EventHandler(this.btnEliminaritemClase_Click);
            // 
            // griditemClase
            // 
            this.griditemClase.Location = new System.Drawing.Point(16, 26);
            this.griditemClase.MainView = this.viewitemClase;
            this.griditemClase.Name = "griditemClase";
            this.griditemClase.Size = new System.Drawing.Size(437, 242);
            this.griditemClase.TabIndex = 15;
            this.griditemClase.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewitemClase});
            this.griditemClase.Click += new System.EventHandler(this.griditemClase_Click);
            this.griditemClase.KeyUp += new System.Windows.Forms.KeyEventHandler(this.griditemClase_KeyUp);
            // 
            // viewitemClase
            // 
            this.viewitemClase.GridControl = this.griditemClase;
            this.viewitemClase.Name = "viewitemClase";
            this.viewitemClase.OptionsView.ShowGroupPanel = false;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(870, 5);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(34, 30);
            this.btnSalir.TabIndex = 41;
            this.btnSalir.TabStop = false;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnNuevaClase);
            this.groupBox1.Controls.Add(this.btnEliminarClase);
            this.groupBox1.Controls.Add(this.lblclase);
            this.groupBox1.Controls.Add(this.txtcod);
            this.groupBox1.Controls.Add(this.txtdesc);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnGuardarClase);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Location = new System.Drawing.Point(22, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 157);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clase";
            // 
            // groupItemClase
            // 
            this.groupItemClase.Controls.Add(this.txtitem);
            this.groupItemClase.Controls.Add(this.txtformula);
            this.groupItemClase.Controls.Add(this.lblerror);
            this.groupItemClase.Controls.Add(this.labelControl5);
            this.groupItemClase.Controls.Add(this.btnnuevoitem);
            this.groupItemClase.Controls.Add(this.btnguardarItemClase);
            this.groupItemClase.Controls.Add(this.txtnumitem);
            this.groupItemClase.Controls.Add(this.labelControl4);
            this.groupItemClase.Controls.Add(this.btnEliminaritemClase);
            this.groupItemClase.Controls.Add(this.labelControl3);
            this.groupItemClase.Location = new System.Drawing.Point(437, 43);
            this.groupItemClase.Name = "groupItemClase";
            this.groupItemClase.Size = new System.Drawing.Size(467, 156);
            this.groupItemClase.TabIndex = 43;
            this.groupItemClase.TabStop = false;
            this.groupItemClase.Text = "Item Clase:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.griditemClase);
            this.groupBox3.Location = new System.Drawing.Point(438, 205);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(467, 274);
            this.groupBox3.TabIndex = 44;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Registros Actuales";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.gridClase);
            this.groupBox4.Location = new System.Drawing.Point(23, 205);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(378, 273);
            this.groupBox4.TabIndex = 45;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Registros Actuales";
            // 
            // frmClase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 491);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupItemClase);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.separador1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmClase";
            this.Text = "Clase";
            this.Load += new System.EventHandler(this.frmClase_Load);
            ((System.ComponentModel.ISupportInitialize)(this.separador1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdesc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtnumitem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtformula.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtitem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridClase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewClase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.griditemClase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewitemClase)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupItemClase.ResumeLayout(false);
            this.groupItemClase.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SeparatorControl separador1;
        private DevExpress.XtraEditors.SimpleButton btnEliminarClase;
        private DevExpress.XtraEditors.SimpleButton btnNuevaClase;
        private DevExpress.XtraEditors.SimpleButton btnGuardarClase;
        private DevExpress.XtraEditors.TextEdit txtnumitem;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LookUpEdit txtformula;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LookUpEdit txtitem;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtdesc;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtcod;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraGrid.GridControl gridClase;
        private DevExpress.XtraGrid.Views.Grid.GridView viewClase;
        private DevExpress.XtraEditors.LabelControl lblerror;
        private DevExpress.XtraEditors.LabelControl lblclase;
        private DevExpress.XtraEditors.SimpleButton btnnuevoitem;
        private DevExpress.XtraEditors.SimpleButton btnguardarItemClase;
        private DevExpress.XtraEditors.SimpleButton btnEliminaritemClase;
        private DevExpress.XtraGrid.GridControl griditemClase;
        private DevExpress.XtraGrid.Views.Grid.GridView viewitemClase;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupItemClase;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}