using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mike
{
    public static class DataCollection
    {
        public static void CollectJmeterIntoTxt()
        {
            string basic_addr = @"D:\done\210720 JSF網站正式區 Jmeter壓力測試\output_210726";
            string header = "目標筆數,主機,狀態,執行時間,#Sapmles,#Fails,失誤率,平均時間(ms),中位時間(ms),min(ms),max(ms),90% Line(ms),95% Line(ms),99% Line(ms),吞吐量(連線數/秒)";
            string[] remotes = { "56", "57", "58", "59", "60" };
            string[] scripts = { "FAVORITE", "SEARCH", "SHOPPING", "WANDER_AROUND" };
            string[] targetNumbers = { "20", "40", "60", "80", "100", "200" };

            foreach (string script in scripts)                
            {
                string destination = basic_addr + @"\" + script + ".txt";
                StringBuilder cont = new StringBuilder();

                foreach (string targetNumber in targetNumbers)
                {
                    foreach (string remote in remotes)
                    {
                        string source = basic_addr + @"\output" + remote + @"\JSF-" + script + "-" + targetNumber + @"\statistics.json";

                        Dictionary<string, string> contentDict = new Dictionary<string, string>();
                        string content = targetNumber + "," + remote + ",";
                        StringBuilder content2 = new StringBuilder();
                        bool flag = false;

                        if (!File.Exists(source))
                        {
                            content += "失敗" + Environment.NewLine;
                        }
                        else
                        {
                            using (StreamReader sr = new StreamReader(source))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.Contains("Total")) { flag = true; continue; }
                                    if (flag && line.Contains("receivedKBytesPerSec")) { flag = false; break; }
                                    if (flag)
                                    {
                                        if (line.Contains("errorPct")) {
                                            double errorPct = Convert.ToDouble(line.Split(':')[1].Split(',')[0]);
                                            content2.Append("," + errorPct / 100);
                                            continue;
                                        }
                                        content2.Append("," + line.Split(':')[1].Split(',')[0]);
                                        if (line.Contains("sampleCount")) { contentDict.Add("sampleCount", line.Split(':')[1].Split(',')[0]); continue; }
                                        if (line.Contains("meanResTime")) { contentDict.Add("meanResTime", line.Split(':')[1].Split(',')[0]); continue; }
                                    }
                                }
                            }

                            string time = StringExtenstions.multiplyTwoString(targetNumber, contentDict["meanResTime"], 2);
                            content += "成功," + TimeExtenstions.convertMsToStandardTime(Convert.ToDouble(time)) + Convert.ToString(content2) + Environment.NewLine;
                        }
                        cont.Append(content);
                    }
                }

                using (StreamWriter sw = new StreamWriter(destination))
                {
                    sw.WriteLine(header);
                    sw.WriteLine(cont);
                }
            }
        }
    }
}
