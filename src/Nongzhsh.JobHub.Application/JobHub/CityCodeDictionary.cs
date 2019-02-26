using System.Collections.Generic;
using System.Linq;

namespace Nongzhsh.JobHub.JobHub
{
    //TODO:可否重构为从配置文件添加城市代码？
    public class CityCodeDictionary
    {
        private static readonly Dictionary<string, string[]> _cityCodeDictionary = new Dictionary<string, string[]>();

        /// <summary> 
        /// 获取区域编码
        /// </summary>
        /// <param name="city">区域（中文）</param>
        /// <param name="source">类型（0:【智联】1:【前程】2:【猎聘】）3:【BOSS】</param>
        /// <returns></returns>
        public static string Get(JobSource source, string city = "深圳")
        {
            var cityCode = city = city?.Trim() ?? "";
            if(!_cityCodeDictionary.Any())
            {
                InitialData();
            }

            if(_cityCodeDictionary.Keys.Contains(city))
            {
                switch(source)
                {
                    case JobSource.智联招聘:
                        cityCode = _cityCodeDictionary[city][0];
                        break;
                    case JobSource.前程无忧:
                        cityCode = _cityCodeDictionary[city][1];
                        break;
                    case JobSource.猎聘招聘:
                        cityCode = _cityCodeDictionary[city][2];
                        break;
                    case JobSource.Boss招聘:
                        cityCode = _cityCodeDictionary[city][3];
                        break;
                    case JobSource.拉钩招聘:
                        break;
                }
            }
            return cityCode;
        }

        private static void InitialData()
        {
            _cityCodeDictionary.Add("北京", new[] { "北京", "010000", "010", "101010100" });
            _cityCodeDictionary.Add("上海", new[] { "上海", "020000", "020", "101020100" });
            _cityCodeDictionary.Add("深圳", new[] { "深圳", "040000", "050090", "101280600" });
            _cityCodeDictionary.Add("广州", new[] { "广州", "030200", "050020", "101280100" });
            _cityCodeDictionary.Add("杭州", new[] { "杭州", "080200", "070020", "101210100" });
            _cityCodeDictionary.Add("成都", new[] { "成都", "090200", "280020", "101270100" });
            _cityCodeDictionary.Add("南京", new[] { "南京", "070200", "060020", "101190100" });
            _cityCodeDictionary.Add("武汉", new[] { "武汉", "180200", "170020", "101200100" });
            _cityCodeDictionary.Add("西安", new[] { "西安", "200200", "270020", "101110100" });
            _cityCodeDictionary.Add("厦门", new[] { "厦门", "110300", "090040", "101230200" });
            _cityCodeDictionary.Add("长沙", new[] { "长沙", "190200", "180020", "101250100" });
            _cityCodeDictionary.Add("苏州", new[] { "苏州", "070300", "060080", "101190400" });
            _cityCodeDictionary.Add("天津", new[] { "天津", "050000", "030", "101030100" });
            _cityCodeDictionary.Add("重庆", new[] { "重庆", "060000", "040", "101040100" });
            _cityCodeDictionary.Add("郑州", new[] { "郑州", "170200", "150020", "101180100" });
            _cityCodeDictionary.Add("青岛", new[] { "青岛", "120300", "250070", "101120200" });
            _cityCodeDictionary.Add("合肥", new[] { "合肥", "150200", "080020", "101220100" });
            _cityCodeDictionary.Add("福州", new[] { "福州", "110200", "090020", "101230100" });
            _cityCodeDictionary.Add("济南", new[] { "济南", "120200", "250020", "101120100" });
            _cityCodeDictionary.Add("大连", new[] { "大连", "230300", "210040", "101070200" });
            _cityCodeDictionary.Add("珠海", new[] { "珠海", "030500", "050140", "101280700" });
            _cityCodeDictionary.Add("无锡", new[] { "无锡", "070400", "060100", "101190200" });
            _cityCodeDictionary.Add("佛山", new[] { "佛山", "030600", "050050", "101280800" });
            _cityCodeDictionary.Add("东莞", new[] { "东莞", "030800", "050040", "101281600" });
            _cityCodeDictionary.Add("宁波", new[] { "宁波", "080300", "070030", "101210400" });
            _cityCodeDictionary.Add("常州", new[] { "常州", "070500", "060040", "101191100" });
            _cityCodeDictionary.Add("沈阳", new[] { "沈阳", "230200", "210020", "101070100" });
            _cityCodeDictionary.Add("石家庄", new[] { "石家庄", "160200", "140020", "101090100" });
            _cityCodeDictionary.Add("昆明", new[] { "昆明", "250200", "310020", "101290100" });
            _cityCodeDictionary.Add("南昌", new[] { "南昌", "130200", "200020", "101240100" });
            _cityCodeDictionary.Add("南宁", new[] { "南宁", "140200", "110020", "101300100" });
            _cityCodeDictionary.Add("哈尔滨", new[] { "哈尔滨", "220200", "160020", "101050100" });
            _cityCodeDictionary.Add("海口", new[] { "海口", "100200", "130020", "101310100" });
            _cityCodeDictionary.Add("中山", new[] { "中山", "030700", "050130", "101281700" });
            _cityCodeDictionary.Add("惠州", new[] { "惠州", "030300", "050060", "101280300" });
            _cityCodeDictionary.Add("贵阳", new[] { "贵阳", "260200", "120020", "101260100" });
            _cityCodeDictionary.Add("长春", new[] { "长春", "240200", "190020", "101060100" });
            _cityCodeDictionary.Add("太原", new[] { "太原", "210200", "260020", "101100100" });
            _cityCodeDictionary.Add("嘉兴", new[] { "嘉兴", "080700", "070090", "101210300" });
            _cityCodeDictionary.Add("泰安", new[] { "泰安", "121100", "250090", "101120800" });
            _cityCodeDictionary.Add("昆山", new[] { "昆山", "070600", "060050", "101191400" });
            _cityCodeDictionary.Add("烟台", new[] { "烟台", "120400", "250120", "101120500" });
            _cityCodeDictionary.Add("兰州", new[] { "兰州", "270200", "100020", "101160100" });
            _cityCodeDictionary.Add("泉州", new[] { "泉州", "110400", "090030", "101230500" });
        }
    }
}
