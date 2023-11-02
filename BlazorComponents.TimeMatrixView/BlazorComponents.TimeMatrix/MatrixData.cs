using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorComponents.TimeMatrix
{
    internal class MatrixData
    {

        public MatrixData()
        {
            this.cells = new List<Cell>();
        }

        public eTimeUnit time_unit { get; set; }

        public string unit_name { get; set; }

        public int matrix_width { get; set; }

        public int matrix_lineCount { get; set; }

        public eMatrixDisplayMode display_mode { get; set; } // unit为day的情况每一行为一个月， year的情况 每一行为一个年代，或一个世纪

        public List<Cell> cells { get; set; }
    }


    internal class Cell
    {
        internal Cell()
        {
            this.marks = new List<EventMark>();
            this.bg_events = new List<Event>();
        }

        public int val { get; set; }

        public int c { get; set; } //列号

        public int l { get; set; } //行号

        public string display_time { get; set; }

        public string[] tags { get; set; }

        //背景事件不能超过两个
        public List<Event> bg_events { get; set; }

        public List<EventMark> marks { get; set; }
    }


    internal class EventMark
    {

        public int val { get; set; }

        public int c { get; set; }

        public int l { get; set; }


        public string color { get; set; }
        public Guid esid { get; set; }

        public Guid eid { get; set; }

        //public Guid eid1 { get; set; }

        public int seq { get; set; }

        public int slot { get; set; } // 对应instant mark 的 x轴位置

        public string display_time { get; set; }

        public eEventMarkType type { get; set; }
    }


}
