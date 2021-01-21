using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labour
{
    class Data
    {
        public Data()
        {
            //CONSTRUCTOR
        }

        private string item;
        private string vo;
        private string haber;
        private string descuento;        

        public string Item
        {
            get { return this.item; }
            set { this.item = value; }
        }

        public string Vo
        {
            get { return this.vo; }
            set { this.vo = value; }
        }

        public string Haber
        {
            get { return this.haber; }
            set { this.haber = value; }
        }
        public string Descuento
        {
            get { return this.descuento; }
            set { this.descuento = value; }
        }
    }

}
