namespace Labour
{
    partial class frmCargarTrabajador
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCargarTrabajador));
            this.BtnBuscar = new DevExpress.XtraEditors.SimpleButton();
            this.txtBusqueda = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gridTrabajador = new DevExpress.XtraGrid.GridControl();
            this.viewTrabajador = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.txtContrato = new DevExpress.XtraEditors.TextEdit();
            this.txtRut = new DevExpress.XtraEditors.TextEdit();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtApeMaterno = new DevExpress.XtraEditors.TextEdit();
            this.txtApepaterno = new DevExpress.XtraEditors.TextEdit();
            this.txtNombre = new DevExpress.XtraEditors.TextEdit();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSalirArea = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusqueda.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrabajador)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTrabajador)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContrato.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRut.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApeMaterno.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApepaterno.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnBuscar
            // 
            this.BtnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnBuscar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("BtnBuscar.ImageOptions.Image")));
            this.BtnBuscar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.BtnBuscar.Location = new System.Drawing.Point(12, 77);
            this.BtnBuscar.Name = "BtnBuscar";
            this.BtnBuscar.Size = new System.Drawing.Size(86, 39);
            this.BtnBuscar.TabIndex = 7;
            this.BtnBuscar.Text = "Buscar";
            this.BtnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Location = new System.Drawing.Point(12, 51);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBusqueda.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtBusqueda_Properties_BeforeShowMenu);
            this.txtBusqueda.Size = new System.Drawing.Size(245, 20);
            this.txtBusqueda.TabIndex = 6;
            this.txtBusqueda.EditValueChanged += new System.EventHandler(this.txtRut_EditValueChanged);
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBusqueda_KeyDown);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8F);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(12, 32);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(112, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Ingrese una búsqueda:";
            // 
            // gridTrabajador
            // 
            this.gridTrabajador.Location = new System.Drawing.Point(12, 125);
            this.gridTrabajador.MainView = this.viewTrabajador;
            this.gridTrabajador.Name = "gridTrabajador";
            this.gridTrabajador.Size = new System.Drawing.Size(614, 348);
            this.gridTrabajador.TabIndex = 8;
            this.gridTrabajador.TabStop = false;
            this.gridTrabajador.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewTrabajador});
            this.gridTrabajador.DoubleClick += new System.EventHandler(this.gridTrabajador_DoubleClick);
            this.gridTrabajador.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridTrabajador_KeyDown);
            // 
            // viewTrabajador
            // 
            this.viewTrabajador.GridControl = this.gridTrabajador;
            this.viewTrabajador.Name = "viewTrabajador";
            this.viewTrabajador.OptionsView.ShowGroupPanel = false;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(428, 100);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(116, 13);
            this.labelControl3.TabIndex = 10;
            this.labelControl3.Text = "(* Campos Obligatorios)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(528, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "(*)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(258, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "(*)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(528, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "(*)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(528, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "(*)";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblError.Location = new System.Drawing.Point(108, 92);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(57, 13);
            this.lblError.TabIndex = 6;
            this.lblError.Text = "Message";
            this.lblError.Visible = false;
            // 
            // txtContrato
            // 
            this.txtContrato.EnterMoveNextControl = true;
            this.txtContrato.Location = new System.Drawing.Point(356, 68);
            this.txtContrato.Name = "txtContrato";
            this.txtContrato.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtContrato.Size = new System.Drawing.Size(167, 20);
            this.txtContrato.TabIndex = 5;
            this.txtContrato.EnabledChanged += new System.EventHandler(this.txtContrato_EnabledChanged);
            this.txtContrato.Enter += new System.EventHandler(this.txtContrato_Enter);
            this.txtContrato.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContrato_KeyDown);
            this.txtContrato.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContrato_KeyPress);
            this.txtContrato.Leave += new System.EventHandler(this.txtContrato_Leave);
            // 
            // txtRut
            // 
            this.txtRut.EnterMoveNextControl = true;
            this.txtRut.Location = new System.Drawing.Point(111, 69);
            this.txtRut.Name = "txtRut";
            this.txtRut.Size = new System.Drawing.Size(140, 20);
            this.txtRut.TabIndex = 4;
            this.txtRut.EditValueChanged += new System.EventHandler(this.txtRut_EditValueChanged);
            this.txtRut.Enter += new System.EventHandler(this.txtRut_Enter);
            this.txtRut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRut_KeyDown);
            this.txtRut.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRut_KeyPress);
            this.txtRut.Leave += new System.EventHandler(this.txtRut_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(280, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "N° Contrato:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Rut:";
            // 
            // txtApeMaterno
            // 
            this.txtApeMaterno.EnterMoveNextControl = true;
            this.txtApeMaterno.Location = new System.Drawing.Point(356, 48);
            this.txtApeMaterno.Name = "txtApeMaterno";
            this.txtApeMaterno.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtApeMaterno.Size = new System.Drawing.Size(167, 20);
            this.txtApeMaterno.TabIndex = 3;
            this.txtApeMaterno.Enter += new System.EventHandler(this.txtNombre_Enter);
            this.txtApeMaterno.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtApeMaterno_KeyPress);
            // 
            // txtApepaterno
            // 
            this.txtApepaterno.EnterMoveNextControl = true;
            this.txtApepaterno.Location = new System.Drawing.Point(111, 48);
            this.txtApepaterno.Name = "txtApepaterno";
            this.txtApepaterno.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtApepaterno.Size = new System.Drawing.Size(140, 20);
            this.txtApepaterno.TabIndex = 2;
            this.txtApepaterno.Enter += new System.EventHandler(this.txtNombre_Enter);
            this.txtApepaterno.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtApepaterno_KeyPress);
            // 
            // txtNombre
            // 
            this.txtNombre.EnterMoveNextControl = true;
            this.txtNombre.Location = new System.Drawing.Point(111, 27);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombre.Size = new System.Drawing.Size(412, 20);
            this.txtNombre.TabIndex = 1;
            this.txtNombre.Enter += new System.EventHandler(this.txtNombre_Enter);
            this.txtNombre.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNombre_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(258, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Apellido Materno:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Apellido Paterno:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombres:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSalirArea);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtApeMaterno);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtNombre);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.lblError);
            this.groupBox1.Controls.Add(this.txtApepaterno);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtRut);
            this.groupBox1.Controls.Add(this.txtContrato);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(14, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(644, 128);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Datos Personales";
            // 
            // btnSalirArea
            // 
            this.btnSalirArea.AllowFocus = false;
            this.btnSalirArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirArea.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalirArea.Location = new System.Drawing.Point(586, 18);
            this.btnSalirArea.Name = "btnSalirArea";
            this.btnSalirArea.Size = new System.Drawing.Size(38, 30);
            this.btnSalirArea.TabIndex = 43;
            this.btnSalirArea.ToolTip = "Cerrar Formulario";
            this.btnSalirArea.Click += new System.EventHandler(this.btnSalirArea_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gridTrabajador);
            this.groupBox2.Controls.Add(this.BtnBuscar);
            this.groupBox2.Controls.Add(this.labelControl1);
            this.groupBox2.Controls.Add(this.txtBusqueda);
            this.groupBox2.Location = new System.Drawing.Point(12, 143);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(646, 495);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Búsqueda";
            // 
            // frmCargarTrabajador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 649);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCargarTrabajador";
            this.Text = "Trabajador";
            this.Load += new System.EventHandler(this.frmCargarTrabajador_Load);
            this.Shown += new System.EventHandler(this.frmCargarTrabajador_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtBusqueda.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrabajador)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTrabajador)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContrato.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRut.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApeMaterno.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApepaterno.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton BtnBuscar;
        private DevExpress.XtraEditors.TextEdit txtBusqueda;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.GridControl gridTrabajador;
        private DevExpress.XtraGrid.Views.Grid.GridView viewTrabajador;
        private DevExpress.XtraEditors.TextEdit txtNombre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblError;
        private DevExpress.XtraEditors.TextEdit txtContrato;
        private DevExpress.XtraEditors.TextEdit txtRut;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtApeMaterno;
        private DevExpress.XtraEditors.TextEdit txtApepaterno;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevExpress.XtraEditors.SimpleButton btnSalirArea;
    }
}