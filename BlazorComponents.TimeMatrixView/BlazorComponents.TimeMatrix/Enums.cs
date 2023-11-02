using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorComponents.TimeMatrix
{


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eTimeUnit
    {
        Day,//dateonly 数值
        Year,//公元年
        //Month,
        //Week,
        Custom,//自定义

    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum eEventMarkType
    {
        start,
        ongoing,
        end,
        instant,
        //simultaneous, //同一条时间线 在同一cell有多个事件发生
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eMatrixDisplayMode
    {
        num,
        week,
        month,
        decade,
        century,
    }


}
