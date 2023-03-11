using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineFramework.Control
{
    public partial class NumCls : UserControl
    {
        public NumCls()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TextBox의 Text정렬위치를 조정합니다.
        /// </summary>
        /// <param name="_align">L, R, C</param>
        public void alignSet(string _align)
        {
            if (_align.Equals("L"))
            {
                tbNum.TextAlign = HorizontalAlignment.Left;
            }
            else if (_align.Equals("R"))
            {
                tbNum.TextAlign = HorizontalAlignment.Right;
            }
            else if (_align.Equals("C"))
            {
                tbNum.TextAlign = HorizontalAlignment.Center;
            }            
        }

        /// <summary>
        /// 기본값을 셋팅하는 속성영역입니다.
        /// </summary>
        [Category("BasicValSet"), Description("기본값 셋팅")]
        public string BasicValSet
        {
            get
            {
                return tbNum.Text;
            }
            set
            {
                tbNum.Text = value;
            }
        }

        /// <summary>
        /// 필드를 ReadOnly로 변경합니다.
        /// </summary>
        public void readOnly()
        {
            tbNum.ReadOnly = true;
        }

        /// <summary>
        /// 필드를 ReadOnly에서 해제합니다.
        /// </summary>
        public void notReadOnly()
        {
            tbNum.ReadOnly = false;
        }

        /// <summary>
        /// 글자 입력시마다 숫자가아닌 문자열이 입력되는지 확인합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbNum_TextChanged(object sender, EventArgs e)
        {
            tbNum.Text = StringCls.AmountComma(StringCls.onlyNumber(tbNum.Text.Replace(",", "")));
            tbNum.SelectionStart = tbNum.Text.Length;
            tbNum.ScrollToCaret();            
        }
    }
}
