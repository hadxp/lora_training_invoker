using mtfx;

namespace DatasetGenerator;

internal class Program {
	public static void Main(string[] args) {
		var triggerWord = "mikaylahau";
		var inDir = MtfxUtil.AssemblyDir.GetSubDirectory("in");
		foreach(var arg in args) {
			var argSplit = arg.Split('=');
			try {
				if(argSplit[0] == "in") {
					inDir = argSplit[1].ToDirectoryInfo();
				} else if(argSplit[0] == "triggerword" || argSplit[0] == "tw") {
					triggerWord = argSplit[1];
				}
			} catch { /* if any error occurs -> ignore the argument */
			}
		}

		var captions = inDir.EnumerateFiles(".txt", SearchOption.TopDirectoryOnly);
		_ = new DatasetEditor(triggerWord, captions);
	}
}

public class DatasetEditor(string triggerWord, IEnumerable<FileInfo>? captions) : IDisposable {
	public void Dispose() {
		Generate();
	}

	public void Generate() {
		// load images from "in" folder
		foreach(var textFile in captions ?? []) {
			var lines = textFile.ReadAllLines().ToList();
			var linesCopy = lines.ToList();

			foreach(var line in linesCopy) {
				var newLine = line.Trim();

				//newLine = newLine.Replace("two", "one");
				//newLine = newLine.Replace("three", "two");

				newLine = newLine.Replace("woman", triggerWord);
				newLine = newLine.Replace("man", triggerWord);

				if(!newLine.Contains(triggerWord)) {
					newLine = $"{triggerWord} {newLine}";
				}

				lines.Replace(line, newLine);
				Console.WriteLine(@$"""{textFile.FileNameWithoutExtension()}"":{Environment.NewLine}""{newLine}""{Environment.NewLine}");
			}

			textFile.WriteAllLines(lines);
		}
	}
	
	public async Task GenerateAsync() {
		// load images from "in" folder
		foreach(var textFile in captions ?? []) {
			var lines = textFile.ReadAllLines().ToList();
			var linesCopy = lines.ToList();

			foreach(var line in linesCopy) {
				var newLine = line.Trim();

				//newLine = newLine.Replace("two", "one");
				//newLine = newLine.Replace("three", "two");

				newLine = newLine.Replace("woman", triggerWord);
				newLine = newLine.Replace("man", triggerWord);

				if(!newLine.Contains(triggerWord)) {
					newLine = $"{triggerWord} {newLine}";
				}

				lines.Replace(line, newLine);
				Console.WriteLine(@$"""{textFile.FileNameWithoutExtension()}"":{Environment.NewLine}""{newLine}""{Environment.NewLine}");
			}

			await File.WriteAllLinesAsync(textFile.FullName, lines);
		}
	}
}
