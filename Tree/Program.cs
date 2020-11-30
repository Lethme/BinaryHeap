using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a path to any text file: ");
            var Tree = BinaryHeap.Create(Reader.ReadWords(Console.ReadLine().Trim('"')));

            Console.WriteLine($"\n{Tree}\n");
            foreach (var word in Tree)
            {
                Console.WriteLine($"{word.ToString(WordOutputStyle.WordWithTotalCount)}: {word.LineNumbers}");
            }

            Console.WriteLine($"\nTree balance: {(Tree.IsBalanced ? "Balanced" : "Not balanced")}");

            Console.WriteLine("\nLess frequent words list: ");
            foreach (var word in Reader.GetWordList(FrequenceType.Less, WordListType.ByLineCount)) Console.WriteLine($"{word}");

            Console.WriteLine("\nMost frequent words list: ");
            foreach (var word in Reader.GetWordList(FrequenceType.Most, WordListType.ByLineCount)) Console.WriteLine($"{word}");
        }
    }
}
