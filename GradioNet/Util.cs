using System.Runtime.InteropServices;
using mtfx;
using mtfx.function;

namespace GradioNet.util;

public static class Util {
	public static string GetProgramToExecute() {
		if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			return "cmd.exe";
		} else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
			return "/bin/bash";
		}

		throw new Exception("No executable found");
	}
	public static DirectoryInfo? GetRootDir() {
		var projectRootDir = MtfxUtil.AssemblyDir;

		#if DEBUG
		projectRootDir = FindCsproj(MtfxUtil.AssemblyDirectory)!.ToDirectoryInfo().Parent!.Parent;
		#endif

		return projectRootDir;
	}

	public static FileInfo? GetVenvPython(DirectoryInfo? directoryInfo) {
		var pythonVenvDir = directoryInfo?.GetSubDirectory("venv");

		DirectoryInfo? venvScriptsPath = null;
		if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			venvScriptsPath = pythonVenvDir?.GetSubDirectory("Scripts");
		} else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
			venvScriptsPath = pythonVenvDir?.GetSubDirectory("bin");
		}

		var venvPythonPath = venvScriptsPath?.GetFile("python.exe");
		return venvPythonPath;
	}

	internal static async Task<IEnumerable<string>?> GetReadmeContent() {
		#if DEBUG
		var csprojDirectory = FindCsproj(MtfxUtil.AssemblyDirectory)?.ToDirectoryInfo().Parent;
		var readmeFile = csprojDirectory?.GetSubDirectory("..").GetFile("README.MD");
		#else
	    var readmeFile = MtfxUtil.AssemblyDir.GetFile("README.MD");
		#endif
		if(readmeFile?.Exists is false) { return null; }

		return await File.ReadAllLinesAsync(readmeFile!.FullName);
		//return readmeFile?.ReadAllLines(); // <-- cannot be made async
	}

	public static IEnumerable<T>? ReplaceInList<T>(IEnumerable<T>? lines, Provider<T, T> func) {
		var newLines = lines?.ToList();
		for(var i = 0; i < (lines?.Count() ?? 0); i++) {
			var line = newLines![i];
			var newLine = func.Invoke(line);
			newLines!.Replace(line, newLine);
		}

		return newLines;
	}

	internal static string? FindCsproj(string startDir) {
		var dir = new DirectoryInfo(startDir);
		while(dir != null) {
			var files = dir.GetFiles("*.csproj");
			if(files.Length > 0) {
				return files[0].FullName;
			}

			dir = dir.Parent;
		}

		return null;
	}
}
