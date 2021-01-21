namespace Labour
{
    partial class frmCausalTermino
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCausalTermino));
            this.txtdesc = new DevExpress.XtraEditors.TextEdit();
            this.txtcod = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.gridcausal = new DevExpress.XtraGrid.GridControl();
            this.viewcausal = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblerror = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.txtJustificacion = new DevExpress.XtraEditors.MemoEdit();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtdesc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridcausal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewcausal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtJustificacion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtdesc
            // 
            this.txtdesc.EnterMoveNextControl = true;
            this.txtdesc.Location = new System.Drawing.Point(97, 53);
            this.txtdesc.Name = "txtdesc";
            this.txtdesc.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtdesc.Size = new System.Drawing.Size(265, 20);
            this.txtdesc.TabIndex = 2;
            this.txtdesc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtdesc_KeyDown);
            this.txtdesc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtdesc_KeyPress);
            // 
            // txtcod
            // 
            this.txtcod.EnterMoveNextControl = true;
            this.txtcod.Location = new System.Drawing.Point(97, 31);
            this.txtcod.Name = "txtcod";
            this.txtcod.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtcod.Size = new System.Drawing.Size(53, 20);
            this.txtcod.TabIndex = 1;
            this.txtcod.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtcod_KeyDown);
            this.txtcod.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtcod_KeyPress);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(25, 57);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(58, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Descripcion:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(46, 34);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(37, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Codigo:";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(173, 220);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(70, 30);
            this.btnEliminar.TabIndex = 6;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.ToolTip = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(21, 220);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(70, 30);
            this.btnNuevo.TabIndex = 4;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo registro";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(97, 220);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(70, 30);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.ToolTip = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // gridcausal
            // 
            this.gridcausal.Location = new System.Drawing.Point(21, 261);
            this.gridcausal.MainView = this.viewcausal;
            this.gridcausal.Name = "gridcausal";
            this.gridcausal.Size = new System.Drawing.Size(524, 281);
            this.gridcausal.TabIndex = 7;
            this.gridcausal.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewcausal});
            this.gridcausal.Click += new System.EventHandler(this.gridcausal_Click);
            this.gridcausal.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridcausal_KeyUp);
            // 
            // viewcausal
            // 
            this.viewcausal.GridControl = this.gridcausal;
            this.viewcausal.Name = "viewcausal";
            this.viewcausal.OptionsView.ShowGroupPanel = false;
            // 
            // lblerror
            // 
            this.lblerror.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblerror.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblerror.Appearance.Options.UseFont = true;
            this.lblerror.Appearance.Options.UseForeColor = true;
            this.lblerror.Location = new System.Drawing.Point(97, 201);
            this.lblerror.Name = "lblerror";
            this.lblerror.Size = new System.Drawing.Size(51, 13);
            this.lblerror.TabIndex = 24;
            this.lblerror.Text = "message";
            this.lblerror.Visible = false;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(502, 28);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 28);
            this.btnSalir.TabIndex = 30;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.txtJustificacion);
            this.groupControl1.Controls.Add(this.separatorControl1);
            this.groupControl1.Controls.Add(this.lblerror);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.btnSalir);
            this.groupControl1.Controls.Add(this.txtdesc);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.txtcod);
            this.groupControl1.Controls.Add(this.gridcausal);
            this.groupControl1.Controls.Add(this.btnNuevo);
            this.groupControl1.Controls.Add(this.btnEliminar);
            this.groupControl1.Controls.Add(this.btnGuardar);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(577, 591);
            this.groupControl1.TabIndex = 32;
            // 
            // txtJustificacion
            // 
            this.txtJustificacion.Location = new System.Drawing.Point(97, 75);
            this.txtJustificacion.Name = "txtJustificacion";
            this.txtJustificacion.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtJustificacion.Properties.MaxLength = 255;
            this.txtJustificacion.Size = new System.Drawing.Size(448, 103);
            this.txtJustificacion.TabIndex = 3;
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(21, 181);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Size = new System.Drawing.Size(524, 23);
            this.separatorControl1.TabIndex = 21;
            this.separatorControl1.TabStop = false;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(21, 77);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(62, 13);
            this.labelControl4.TabIndex = 1;
            this.labelControl4.Text = "Justificacion:";
            // 
            // frmCausalTermino
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 591);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCausalTermino";
            this.Text = "Causal de Termino";
            this.Load += new System.EventHandler(this.frmCausalTermino_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtdesc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridcausal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewcausal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtJustificacion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtdesc;
        private DevExpress.XtraEditors.TextEdit txtcod;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraGrid.GridControl gridcausal;
        private DevExpress.XtraGrid.Views.Grid.GridView viewcausal;
        private DevExpress.XtraEditors.LabelControl lblerror;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.MemoEdit txtJustificacion;
    }
}