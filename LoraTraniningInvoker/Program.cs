using DatasetGenerator;
using mtfx;

namespace LoraTrainingInvoker;

internal class Program {
	private static void Main(string[] args) {
		#if DEBUG
		var csprojDirectory = FindCsproj(MtfxUtil.AssemblyDirectory)?.ToDirectoryInfo().Parent;

		var workflow = csprojDirectory?.GetSubDirectory("comfyui-workflows").GetFile("GenTextForImages.json");
		var rawDatasetDirectory = csprojDirectory?.GetSubDirectory("..").GetSubDirectory("dataset");
		var inputDirectory = csprojDirectory?.GetSubDirectory("..").GetSubDirectory("input");
		var outputDirectory = csprojDirectory?.GetSubDirectory("..").GetSubDirectory("output");

		var runComfyFile = csprojDirectory?.GetFile("run_comfy.sh");
		var runDiffusionPipe = csprojDirectory?.GetFile("run_diffusion_pipe.sh");
		var runDiffusionPipeFramepack = csprojDirectory?.GetFile("run_diffusion_pipe_framepack.sh");
		#else
	    var workflow = MtfxUtil.AssemblyDir.GetSubDirectory("comfyui-workflows").GetFile("GenTextForImages.json");
		var rawDatasetDirectory = MtfxUtil.AssemblyDir.GetSubDirectory("dataset");
	    var inputDirectory = MtfxUtil.AssemblyDir.GetSubDirectory("input");
	    var outputDirectory = MtfxUtil.AssemblyDir.GetSubDirectory("output");
		
		var runComfyFile = MtfxUtil.AssemblyDir.GetFile("run_comfy.sh");
		var runDiffusionPipeFile = MtfxUtil.AssemblyDir.GetFile("run_comfy.sh");
		var runDiffusionPipeFramepackFile = MtfxUtil.AssemblyDir.GetFile("run_diffusion_pipe_framepack.sh");
		#endif

		if(workflow?.Exists is true &&
		   rawDatasetDirectory?.Exists is true &&
		   inputDirectory?.Exists is true &&
		   outputDirectory?.Exists is true &&
		   runComfyFile?.Exists is true &&
		   runDiffusionPipe?.Exists is true &&
		   runDiffusionPipeFramepack?.Exists is true) {
			// prepare the comfyui/florence2 to describe an image
			using(var de = new DatasetEdit(workflow, rawDatasetDirectory, inputDirectory)) {
				de.Edit();
			}

			// run comfyui
			MtfxUtil.ExecuteProcess($"bash {runComfyFile}", csprojDirectory, out var comfyError);
			Console.WriteLine(comfyError);

			// add a triggerword to every text description of an image
			using(new DatasetEditor("mikahlaau", inputDirectory)) { }

			// run diffusion-pipe
			MtfxUtil.ExecuteProcess($"bash {runDiffusionPipe}", csprojDirectory, out var diffusionPipeError);

			// run diffusion-pipe
			// MtfxUtil.ExecuteProcess($"bash {runDiffusionPipeFramepack}", csprojDirectory, out var diffusionPipeFramepackError);
		}
	}

	private static string? FindCsproj(string startDir) {
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
