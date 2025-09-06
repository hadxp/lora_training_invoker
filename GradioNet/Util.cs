using mtfx;

namespace GradioNet.util;

public static class Util {
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
