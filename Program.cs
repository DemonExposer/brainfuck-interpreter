namespace Brainfuck; 

using System.IO;
using System.Text.RegularExpressions;

public class Program {
	private static int[] memory = new int[100];
	private static int pointer = 0;

	private static void doubleMem() {
		int[] newMem = new int[memory.Length * 2];
		memory.CopyTo(newMem, 0);
		memory = newMem;
	}

	private static void runCommand(LineChar c, int index) {
		switch (c.C) {
			case '+':
				memory[pointer]++;
				break;
			case '-':
				memory[pointer]--;
				break;
			case '<':
				if (pointer > 0)
					pointer--;
				else
					throw new IndexOutOfRangeException("Pointer below 0 at instruction " + index + " (Line: " + c.Line + ")");
					
				break;
			case '>':
				if (pointer >= memory.Length)
					doubleMem();
				pointer++;
				
				break;
			case ',':
				char readChar;
				do {
					readChar = (char) Console.Read();
				} while (readChar == '\r' || readChar == '\n');
				memory[pointer] = readChar;
				break;
			case '.':
				Console.Write((char) memory[pointer]);
				break;
		}
	}
	
	private static int loop(LineChar[] code, int index) {
		for (int i = index+1; ; i++) {
			if (i >= code.Length)
				throw new IndexOutOfRangeException("Loop at " + index + " (Line: " + code[i].Line + ") has no ending bracket");
			
			switch (code[i].C) {
				case '[':
					i = loop(code, i);
					break;
				case ']':
					if (memory[pointer] != 0)
						i = index;
					else
						return i;

					break;
				default:
					runCommand(code[i], index);
					break;
			}
		}
	}
	
	private static void interpret(LineChar[] code) {
		for (int i = 0; i < code.Length; i++) {
			if (code[i].C == '[')
				i = loop(code, i);
			else
				runCommand(code[i], i);
		}
	}
	
	public static void Main(string[] args) {
		string[] data;
		try {
			data = File.ReadAllLines(args[0]);
		} catch (IndexOutOfRangeException) {
			Console.WriteLine("No input file specified");
			return;
		} catch (FileNotFoundException) {
			Console.WriteLine("Input file does not exist");
			return;
		}

		List<LineChar> formattedData = new List<LineChar>();
		for (int i = 0; i < data.Length; i++)
			Regex.Matches(data[i], "[\\[\\]+-<>.,]").ToList().Select(match => new LineChar {C = match.Value[0], Line = i}).ToList().ForEach(formattedData.Add);

		interpret(formattedData.ToArray());
	}
}