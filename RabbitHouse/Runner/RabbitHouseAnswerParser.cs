using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    public class RabbitHouseAnswerParser
    {
        public RabbitHouseAnswerParser() { }

        public long[] Parse(string[] lines)
        {
            var result = new List<long>();

            foreach (string line in lines)
            {
                int lineIndex = line.IndexOf(":") + 1;
                long blocks = long.Parse(line.Substring(lineIndex));
                result.Add(blocks);

            }
            
            return result.ToArray();
        }
    }
}
