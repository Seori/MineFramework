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
    public partial class DataGridCls : UserControl
    {
        
        public event DataGridViewCellEventHandler GridCellClick;

        public DataGridCls()
        {
            InitializeComponent();

            // 왼쪽 선택라인삭제
            dgvList.RowHeadersVisible = false;

            // 맨 아래 자동으로 생성되는 행 삭제
            dgvList.AllowUserToAddRows = false;
            dgvList.AllowUserToDeleteRows = false;

            // 백그라운드 컬러 지정
            dgvList.BackgroundColor = Color.White;

            // 컬럼 채우기 확인(항상 전체채움)
            dgvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvList.CellClick += (s, e) =>
            {
                GridCellClick?.Invoke(this, e);
            };            
        }

        public DataGridView getMyGrid
        {
            get { return dgvList; }
        }

        [Category("Action"), Description("그리드 셀 클릭 이벤트(행/열/값 전달)")]
        public event EventHandler<DataGridViewCellEventArgs> DataGridViewCellClick;

        [Category("ColumnCountSet"), Description("컬럼갯수를 셋팅합니다.")]
        public int setColumn
        {
            get
            {
                return dgvList.ColumnCount;
            }
            set
            {
                dgvList.ColumnCount = value;
            }
        }

        [Category("ReadOnlySet"), Description("읽기여부를 셋팅합니다.")]
        public bool setReadOnly
        {
            get
            {
                return dgvList.ReadOnly;
            }
            set
            {
                dgvList.ReadOnly = value;
            }
        }

        [Category("HeaderAlign"), Description("헤더정렬을 선택합니다.")]
        public DataGridViewContentAlignment headerAlignment
        {
            get
            {
                return dgvList.ColumnHeadersDefaultCellStyle.Alignment;
            }
            set
            {
                dgvList.ColumnHeadersDefaultCellStyle.Alignment = value;
            }
        }

        [Category("RowSelectMode"), Description("Row선택모드를 선택합니다.")]
        public DataGridViewSelectionMode rowSelectMode
        {
            get
            {
                return dgvList.SelectionMode;
            }
            set
            {
                dgvList.SelectionMode = value;
            }
        }

        [Category("setNoEditGrid"), Description("그리드의 Height조정을 금지합니다.")]
        public void setNoEditGrid()
        {
            dgvList.RowTemplate.Resizable = DataGridViewTriState.False;
        }

        /// <summary>
        /// 그리드를 초기화합니다. (DataTable형태의 그리드 초기화값을 전달받습니다.)
        /// </summary>
        /// <param name="_gridStructure">DT형태의 그리드 초기화값</param>
        /// <returns></returns>
        public int setGrid(DataTable _gridStructure)
        {
            int totalwidth = 0;

            if (dgvList.ColumnCount < 1)
            {
                MessageBox.Show("컬럼갯수가 지정되지 않았습니다.");
                return -9;
            }

            if (dgvList.ColumnCount != _gridStructure.Rows.Count)
            {
                MessageBox.Show("컬럼갯수가 맞지않습니다. ColumnCount를 확인하세요.");
                return -9;
            }

            // Column 구성하기
            for (int i = 0; i < _gridStructure.Rows.Count; i++)
            {
                dgvList.Columns[i].Name = _gridStructure.Rows[i]["header_name"].ToString();
                if (_gridStructure.Rows[i]["resizable_yn"].ToString() == "Y")
                {
                    dgvList.Columns[i].Resizable = DataGridViewTriState.True;
                }
                else
                {
                    dgvList.Columns[i].Resizable = DataGridViewTriState.False;
                }
                
                if (_gridStructure.Rows[i]["text_align"].ToString() == "L")
                {
                    dgvList.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else if (_gridStructure.Rows[i]["text_align"].ToString() == "C")
                {
                    dgvList.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (_gridStructure.Rows[i]["text_align"].ToString() == "R")
                {
                    dgvList.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                dgvList.Columns[i].DataPropertyName = _gridStructure.Rows[i]["data_name"].ToString();
                dgvList.Columns[i].Width = int.Parse(_gridStructure.Rows[i]["width_size"].ToString());
                
                if (_gridStructure.Rows[i]["sort_mode"].ToString() == "A")
                {
                    dgvList.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                }
                else if (_gridStructure.Rows[i]["sort_mode"].ToString() == "N")
                {
                    dgvList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                else
                {
                    dgvList.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                }

                if (_gridStructure.Rows[i]["col_type"].ToString() == "string")
                {
                    dgvList.Columns[i].ValueType = typeof(string);
                }
                else if (_gridStructure.Rows[i]["col_type"].ToString() == "int")
                {
                    dgvList.Columns[i].ValueType = typeof(Int64);
                }
                else
                {
                    dgvList.Columns[i].ValueType = typeof(string);
                }

                if (_gridStructure.Rows[i]["display_yn"].ToString() == "Y")
                {
                    dgvList.Columns[i].Visible = true;
                }
                else
                {
                    dgvList.Columns[i].Visible = false;
                }

                totalwidth += int.Parse(_gridStructure.Rows[i]["width_size"].ToString());
            }

            // 컬림길이만큼 DataGridView의 Width를 수정합니다.
            dgvList.Refresh();

            // 컬럼길이를 Return합니다.
            return totalwidth;
        }

        /// <summary>
        /// DataTable 그리드뷰에 셋팅
        /// </summary>
        /// <param name="_gridDt"></param>
        public void setDataSource(DataTable _gridDt)
        {
            if (_gridDt != null && _gridDt.Rows.Count > 0) 
            {
                dgvList.DataSource = _gridDt;
            }
        }

        /// <summary>
        /// BindSource 그리드뷰에 셋팅
        /// </summary>
        /// <param name="_bind"></param>
        public void setBindSource(BindingSource _bind)
        {
            if (_bind != null && _bind.Count > 0)
            {
                dgvList.DataSource = _bind;                     
            }
        }

        public void columeSortSet(string _sortmode)
        {
            if (dgvList.Columns.Count < 1)
            {
                MessageBox.Show("컬럼열이 생성되지 않았습니다.");
                return;
            }

            if (_sortmode == "Y")
            {
                foreach (DataGridViewColumn i in dgvList.Columns)
                {
                    i.SortMode = DataGridViewColumnSortMode.Automatic;
                }
            }
            else
            {
                foreach (DataGridViewColumn i in dgvList.Columns)
                {
                    i.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }
    }
}
