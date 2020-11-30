using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Heap
{
    public enum FrequenceType
    {
        Most,
        Less
    }
    public enum WordListType
    {
        ByLineCount,
        ByTotalWordCount
    }
    public enum WordOutputStyle
    {
        WordOnly,
        WordWithTotalCount
    }
    public static class Reader
    {
        public static List<Word> Words { get; private set; }
        public static List<Word> GetWordList(FrequenceType frequenceType, WordListType wordListType = WordListType.ByLineCount)
        {
            if (Words.Count != 0)
            {
                switch (frequenceType, wordListType)
                {
                    case (FrequenceType.Most, WordListType.ByLineCount):
                        {
                            var maxCount = Words.Select(x => x.LineInfoList.Count).Aggregate((x, y) => x.CompareTo(y) > 0 ? x : y);
                            var List = Words.Where(x => x.LineInfoList.Count == maxCount).ToList();
                            List.Sort((x, y) => x.CompareTo(y));
                            return List;
                        }
                    case (FrequenceType.Most, WordListType.ByTotalWordCount):
                        {
                            var maxCount = Words.Select(x => x.Count).Aggregate((x, y) => x.CompareTo(y) > 0 ? x : y);
                            var List = Words.Where(x => x.Count == maxCount).ToList();
                            List.Sort((x, y) => x.CompareTo(y));
                            return List;
                        }
                    case (FrequenceType.Less, WordListType.ByLineCount):
                        {
                            var minCount = Words.Select(x => x.LineInfoList.Count).Aggregate((x, y) => x.CompareTo(y) < 0 ? x : y);
                            var List = Words.Where(x => x.LineInfoList.Count == minCount).ToList();
                            List.Sort((x, y) => x.CompareTo(y));
                            return List;
                        }
                    case (FrequenceType.Less, WordListType.ByTotalWordCount):
                        {
                            var minCount = Words.Select(x => x.Count).Aggregate((x, y) => x.CompareTo(y) < 0 ? x : y);
                            var List = Words.Where(x => x.Count == minCount).ToList();
                            List.Sort((x, y) => x.CompareTo(y));
                            return List;
                        }
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            return new List<Word>();
        }
        private static List<String> ReadLines(string FileName)
        {
            try
            {
                using var reader = new StreamReader(FileName);
                var Lines = new List<String>();
                while (reader.Peek() >= 0)
                {
                    Lines.Add(reader.ReadLine());
                }
                return Lines;
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }
        public static List<Word> ReadWords(string FileName)
        {
            var LineList = ReadLines(FileName);
            var WordList = new List<Word>();
            for (var i = 0; i < LineList.Count; i++)
            {
                var CurrentLineWords = Regex.Split(LineList[i], " ").Where(x => x != String.Empty);
                foreach (var word in CurrentLineWords)
                {
                    if (WordList.Any(x => x.Text.ToLower() == word.ToLower()))
                    {
                        var Word = WordList.First(x => x.Text.ToLower() == word.ToLower());
                        if (Word.LineInfoList.Any(x => x.LineNumber == i)) 
                            Word.LineInfoList[Word.LineInfoList.Count - 1].WordCountInc();
                        else
                            Word.LineInfoList.Add(new LineInfo(i));
                    }
                    else
                    {
                        WordList.Add(new Word(word, i));
                    }
                }
            }
            return Words = WordList;
        }
    }
    public class LineInfo
    {
        public int LineNumber { get; }
        public int WordAppearanceCount { get; private set; } = 1;
        public LineInfo(int lineNumber) => LineNumber = lineNumber;
        public void WordCountInc() => WordAppearanceCount++;
    }
    public class Word : IComparable<Word>
    {
        public string Text { get; private set; }
        public int Count => LineInfoList.Select(x => x.WordAppearanceCount).Aggregate((x, y) => x + y);
        public string LineNumbers => LineInfoList.Select(x => $"{x.LineNumber + 1}").Aggregate((x, y) => $"{x} {y}");
        public string LineNumbersWithCount => LineInfoList.Select(x => $"{x.LineNumber + 1}:{x.WordAppearanceCount}").Aggregate((x, y) => $"{x} {y}");
        public List<LineInfo> LineInfoList { get; private set; }
        public Word()
        {
            Text = String.Empty;
            LineInfoList = new List<LineInfo>();
        }
        public Word(string text)
        {
            Text = text;
            LineInfoList = new List<LineInfo>();
        }
        public Word(string text, int lineNumber)
        {
            Text = text;
            LineInfoList = new List<LineInfo>();
            LineInfoList.Add(new LineInfo(lineNumber));
        }
        public int CompareTo(Word other)
        {
            return Text.ToLower().CompareTo(other.Text.ToLower());
        }
        public override string ToString() => ToString(WordOutputStyle.WordOnly);
        public string ToString(WordOutputStyle wordOutputStyle = WordOutputStyle.WordOnly) 
        {
            switch (wordOutputStyle)
            {
                case WordOutputStyle.WordOnly: return $"{Text}";
                case WordOutputStyle.WordWithTotalCount: return $"{Text}({Count})";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        public static Word Create() => new Word();
        public static Word Create(string text) => new Word(text);
        public static Word Create(string text, int lineNumber) => new Word(text, lineNumber);
    }
}
