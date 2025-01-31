using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTasks.VowelCounting
{
    public class StringProcessor
    {
        public (char symbol, int count)[] GetCharCount(string veryLongString, char[] countedChars)
        {
            var charCounts = new Dictionary<char,int>();

            foreach (char c in countedChars)
            {
                charCounts[c] = 0;
            }

            foreach(char c in veryLongString)
            {
                if (charCounts.ContainsKey(c))
                {
                    charCounts[c]++;
                }
            }
            return countedChars.Select(c => (c, charCounts[c])).ToArray();

            
        }
    }
    
}

