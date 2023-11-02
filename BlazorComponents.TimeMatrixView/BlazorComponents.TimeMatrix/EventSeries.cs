using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorComponents.TimeMatrix
{
    public class EventSeries
    {

        public static EventSeries FromJson(string esjsonstr)
        {
            //在这里处理 end 值为 "now" 的情况
            var esdoc = JsonDocument.Parse(esjsonstr);

            var esId = esdoc.RootElement.GetPropertyGuid("id");
            var esTitle = esdoc.RootElement.GetPropertyString("title");
            var esDisplayTime = esdoc.RootElement.GetPropertyString("display_time");
            var esSummary = esdoc.RootElement.GetPropertyString("summary");
            var esThumb = esdoc.RootElement.GetPropertyString("thumb");
            var esUnit = esdoc.RootElement.GetPropertyString("time_unit");
            var esUnitName = esdoc.RootElement.GetPropertyString("unit_name");
            var esIsBG = esdoc.RootElement.GetPropertyBoolean("isbg");

            var time_unit = esUnit.ToTimeUnitEnum();


            var es = new EventSeries(esId, time_unit, esUnitName, esTitle, esDisplayTime, esIsBG.HasValue ? esIsBG.Value : false, esThumb, esSummary);

            var eleEvents = esdoc.RootElement.GetProperty("events").EnumerateArray();

            foreach (var ee in eleEvents)
            {

                var occur = ee.GetPropertyInt("occur");

                //获取end值
                int? end_val = null;
                if (ee.TryGetProperty("end", out JsonElement end_ele))
                {
                    var raw_str = end_ele.GetRawText();

                    raw_str = raw_str == null ? "" : raw_str.Replace("\"", "").ToLower();

                    if (int.TryParse(raw_str, out int end_int))
                    {
                        end_val = end_int;
                    }
                    else if (raw_str == "now")
                    {
                        end_val = time_unit == eTimeUnit.Day ? DateOnly.FromDateTime(DateTime.Now).DayNumber : DateTime.Now.Year;
                    }
                }


                var title = ee.GetPropertyString("title");
                var display_time = ee.GetPropertyString("display_time");
                var tags = ee.GetProperty("tags").Deserialize<string[]>();
                var summary = ee.GetPropertyString("summary");
                var content = ee.GetPropertyString("content");
                var color = ee.GetPropertyString("color");

                //检查参数
                if (!occur.HasValue)
                {
                    //occur 无值, 报错
                    throw new Exception("not valid event series json: event occur has null/empty value");
                }


                //开始添加event
                if (time_unit == eTimeUnit.Day) //将类似 19491001 的数值 转换为  dateonly 的 daynumber
                {
                    var occur_day = DateOnly.ParseExact(occur.ToString(), "yyyyMMdd");
                    DateOnly? end_day = end_val.HasValue ? DateOnly.ParseExact(end_val.ToString(), "yyyyMMdd") : null;

                    es.AddEvent(occur_day, end_day, title, display_time, summary, content, color);
                }
                else
                {
                    es.AddEvent(occur.Value, end_val, title, display_time, summary, content, color);
                }


            }


            return es;


        }


        public EventSeries(Guid? id , eTimeUnit unit,string unitName, string title, string displayTime,  bool isBG = false,
            string thumb = "", string summary = "", string description = "",string defaultColor = "")
        {
            //可自定义设置id
            if (!id.HasValue)
            {
                this.Id = Guid.NewGuid();
            }
            else
            {
                this.Id = id.Value;
            }

            this.TimeUnit = unit;
            this.Title = title;
            this.DisplayTime = displayTime;
            this.UnitName = unitName;
            this.Thumb = thumb;
            this.Summary = summary;
            this.Description = description;
            this.DefaultColor = defaultColor;
            this.Events = new List<Event>();
            this.isBG = isBG;
        }


        //用于内部管理，确定一个事件序列的编号
        public Guid Id { get; private set; }

        public eTimeUnit TimeUnit { get; private set; }

        public string Thumb { get; private set; }

        public string DisplayTime { get; private set; }

        //对于 custom 的时间单位， TimeScale 为单位值对应的时间单位字符，如： 百年，万年，一周，等。
        public string UnitName { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        //是否为背景事件
        public bool isBG { get; set; }

        public string DefaultColor { get; set; }

        public List<Event> Events { get; private set; }


        public int GetMinVal()
        {
            var minOccur = this.Events.Select(e => e.Occur).Min();
            //冗余逻辑，在确保 end>=occur的情况下
            var minEnd = this.Events.Select(e => e.End.HasValue ? e.End.Value : int.MaxValue).Min();

            return minOccur < minEnd ? minOccur : minEnd;
        }

        public int GetMaxVal()
        {
            var maxOccur = this.Events.Select(e=>e.Occur).Max();
            var maxEnd = this.Events.Select(e => e.End.HasValue ? e.End.Value : int.MinValue).Max();

            return maxOccur > maxEnd ? maxOccur : maxEnd;
        }


        //填充默认颜色，按照先后顺序
        public void SortAndFillDefaultColor(params string[] colors)
        {
            //首先对事件排序，颜色需要交错填充
            this.Events = this.Events.OrderBy(e => e.Occur).ToList();

            var cidx = 0;

            foreach (var e in this.Events)
            {
                if (string.IsNullOrEmpty(e.Color))
                {
                    e.Color = colors[cidx];
                    cidx++;
                    if (cidx >= colors.Length)
                    {
                        cidx = 0;
                    }
                }
            }

        }


        public void AddEvent(int occur, int? end, string title, string display_time, string summary, string content,string color)
        {
            //添加event时直接设置displaytime
            var ev = new Event(occur, end, this.TimeUnit, title, display_time, summary, color, content);
            this.Events.Add(ev);
        }

        public void AddEvent(DateOnly occur, DateOnly? end, string title, string display_time, string summary, string content, string color)
        {
            var occurnum = occur.DayNumber;
            var endnum = end?.DayNumber;
            this.AddEvent(occurnum,endnum,title,display_time,summary,content,color);
        }


    }


    public class Event
    {

        internal Event(int occur, int? end, eTimeUnit unit, string title, string display_time, string summary, string color,string content = "")
        {
            if (end.HasValue && end < occur) // 若 occur 等于 end 则视为 instant,因为在一个cell中
            {
                throw new ArgumentException("end can't smaller then occur ");
            }

            this.Id = Guid.NewGuid();
            this.Occur = occur;
            this.End = end;
            this.TimeUnit = unit;
            this.Title = title;
            this.DisplayTime = display_time;
            this.Summary = summary;
            this.Content = content;
            this.Color = color;
            this.Tags = new List<string>();
        }

        //内置id guid?
        public Guid Id { get; private set; }

        //时间值
        //整数？ 时间DateOnly？
        //happend time
        public int Occur { get; private set; }

        //endtime?
        public int? End { get; private set; }


        public eTimeUnit TimeUnit { get; private set; }


        //用于显示的时间
        public string DisplayTime { get; private set; }

        //title
        public string Title { get; private set; }

        public string Color { get; set; }

        //tags
        public List<string> Tags { get; private set; }

        //简要描述
        public string Summary { get; set; }

        //content md,html, 确定一种  md
        public string Content { get; set; }

    }

}
