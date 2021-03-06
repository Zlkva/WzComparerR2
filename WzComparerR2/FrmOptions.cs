﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.Editors;
using WzComparerR2.Config;

namespace WzComparerR2
{
    public partial class FrmOptions : DevComponents.DotNetBar.Office2007Form
    {
        public FrmOptions()
        {
            InitializeComponent();

            cmbWzEncoding.Items.AddRange(new[]
            {
                new ComboItem("Default"){ Value = 0 },
                new ComboItem("CMS(gb2312)"){ Value = 936 },
                new ComboItem("KMS(euc-kr)"){ Value = 949 },
                new ComboItem("JMS(shift-jis)"){ Value = 932 },
                new ComboItem("TMS(big5)"){ Value = 950 },
                new ComboItem("GMS(iso-8859-1)"){ Value = 1252 },
                new ComboItem("Customize"){ Value = -1 },
            });
        }

        public bool SortWzOnOpened
        {
            get { return chkWzAutoSort.Checked; }
            set { chkWzAutoSort.Checked = value; }
        }

        public int DefaultWzCodePage
        {
            get
            {
                return ((cmbWzEncoding.SelectedItem as ComboItem)?.Value as int?) ?? 0;
            }
            set
            {
                var items = cmbWzEncoding.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as int? == value)
                    ?? items.Last();
                item.Value = value;
                cmbWzEncoding.SelectedItem = item;
            }
        }

        public bool AutoDetectExtFiles
        {
            get { return chkAutoCheckExtFiles.Checked; }
            set { chkAutoCheckExtFiles.Checked = value; }
        }

        public void Load(WcR2Config config)
        {
            this.SortWzOnOpened = config.SortWzOnOpened;
            this.DefaultWzCodePage = config.WzEncoding;
            this.AutoDetectExtFiles = config.AutoDetectExtFiles;
        }

        public void Save(WcR2Config config)
        {
            config.SortWzOnOpened = this.SortWzOnOpened;
            config.WzEncoding = this.DefaultWzCodePage;
            config.AutoDetectExtFiles = this.AutoDetectExtFiles;
        }
    }
}
