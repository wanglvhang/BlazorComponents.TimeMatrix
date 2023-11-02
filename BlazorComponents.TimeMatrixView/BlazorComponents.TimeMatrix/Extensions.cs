using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorComponents.TimeMatrix
{
    public static class Extensions
    {

        public static int? GetPropertyInt(this JsonElement ele, string propertyName)
        {
            if (ele.TryGetProperty(propertyName, out JsonElement propertyEle))
            {
                if (propertyEle.TryGetInt32(out int value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public static Guid? GetPropertyGuid(this JsonElement ele, string propertyName)
        {
            if (ele.TryGetProperty(propertyName, out JsonElement propertyEle))
            {
                if (propertyEle.TryGetGuid(out Guid value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public static string GetPropertyString(this JsonElement ele, string propertyName)
        {
            if (ele.TryGetProperty(propertyName, out JsonElement propertyEle))
            {
                return propertyEle.GetString();
            }
            else
            {
                return null;
            }
        }

        public static bool? GetPropertyBoolean(this JsonElement ele, string propertyName)
        {
            if (ele.TryGetProperty(propertyName, out JsonElement propertyEle))
            {
                return propertyEle.GetBoolean();
            }
            else
            {
                return null;
            }
        }


        public static eTimeUnit ToTimeUnitEnum(this string str)
        {
            if (Enum.TryParse<eTimeUnit>(str, ignoreCase: true, out eTimeUnit eTimeUnit))
            {
                return eTimeUnit;
            }
            else
            {
                return eTimeUnit.Custom;
            }
        }


        internal static int GetCellValidSeq(this Cell cell)
        {
            var seq = 0;

            while (seq <= 2)
            {
                if (!cell.marks.Any(m => m.seq == seq))
                {
                    return seq;
                }

                seq++;

            }

            return -1;

        }


        internal static int GetCellValidSlot(this Cell cell)
        {
            var slot = 0;

            while (slot <= 2)
            {
                if (!cell.marks.Any(m => m.slot == slot))
                {
                    return slot;
                }

                slot++;

            }

            return -1;
        }


        internal static void SetDisplayTime(this CellEventData ced, eTimeUnit unit, string unit_name = "")
        {
            var cell_display_time = "";

            switch (unit)
            {
                //年
                case eTimeUnit.Year:
                    cell_display_time = ced.Cell.val > 0 ? $"公元 {ced.Cell.val} 年" : $"公元前 {ced.Cell.val * -1} 年";
                    break;
                case eTimeUnit.Day:
                    var date = DateOnly.FromDayNumber(ced.Cell.val);
                    cell_display_time = date.ToString();
                    break;
                case eTimeUnit.Custom:
                    cell_display_time = $"{ced.Cell.val} {unit_name}";
                    break;
                //自定义
                default:
                    cell_display_time = ced.Cell.val.ToString();
                    break;
            }

            ced.DisplayTime = cell_display_time;
        }


    }
}
