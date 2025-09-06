using DatasetGenerator;
using Gradio.Net;
using GradioNet.util;
using LoraTrainingInvoker;
using mtfx;

// ReSharper disable UnusedMember.Local
// ReSharper disable ReturnOfUsingVariable

namespace GradioNet.UI;

internal class Ui {
	private static async Task<Textbox> GetReadmeContentTxt() {
		var readmeContent = await Util.GetReadmeContent() ?? [];
		var readmeContentStr = string.Join(Environment.NewLine, readmeContent);
		return gr.Textbox(readmeContentStr, interactive: false, lines: readmeContent.Count());
	}

	private static async Task<Output> Reload(Input? input) {
		return gr.Output(await GetReadmeContentTxt());
	}

	private static async Task<IEnumerable<FileInfo>?> CreateCaptions(DirectoryInfo imagesDirPath) {
		#if DEBUG
		var csprojDirectory = Util.FindCsproj(MtfxUtil.AssemblyDirectory)!.ToDirectoryInfo().Parent!;
		var workflow = csprojDirectory.GetSubDirectory("comfyui-workflows").GetFile("GenTextForImages.json");
		var inputDirectory = csprojDirectory.GetSubDirectory("input");
		var runComfyFile = csprojDirectory.GetFile("run_comfy.sh");
		#else
	    var workflow = MtfxUtil.AssemblyDir.GetSubDirectory("comfyui-workflows").GetFile("GenTextForImages.json");
	    var inputDirectory = MtfxUtil.AssemblyDir.GetSubDirectory("input");
		
		var runComfyFile = MtfxUtil.AssemblyDir.GetFile("run_comfy.sh");
		#endif

		// edit the dataset
		var de = new DatasetEdit(workflow, imagesDirPath, inputDirectory);
		de.Edit();
		await de.SaveAsync();

		// invoke comfy-ui
		MtfxUtil.ExecuteProcess($"bash {runComfyFile}", csprojDirectory, out var comfyError);
		if(comfyError.IsNotNullOrEmpty()) {
			Console.WriteLine(comfyError);
			return null;
		}

		var captions = new List<FileInfo>();
		var allCaptionsExist = false;
		foreach(var fileInfo in imagesDirPath.GetAllFiles()) {
			var filename = fileInfo.FileNameWithoutExtension();
			var captionFile = imagesDirPath.GetFile(filename + ".txt");
			captions.Add(captionFile);
			allCaptionsExist |= captionFile.Exists;
		}

		if(!allCaptionsExist) {
			return null;
		}

		return captions;
	}

	private static async Task<Output> CreateDataset(Input input) {
		var imagesDirPath = Textbox.Payload(input.Data[0]).ToDirectoryInfo();

		var generatedCaptions = await CreateCaptions(imagesDirPath);

		return gr.Output(gr.Textbox(visible: true));
	}

	private static async Task<Output> AddTriggerword(Input input) {
		var triggerWord = Textbox.Payload(input.Data[0]);
		var imagesDirPath = Textbox.Payload(input.Data[1]).ToDirectoryInfo();
		
		var captions = imagesDirPath.EnumerateFiles(".txt", SearchOption.TopDirectoryOnly);
		await new DatasetEditor(triggerWord, captions).GenerateAsync();
		
		return gr.Output(gr.Textbox(visible: true));
	}

	internal static async Task<Blocks> CreateBlocks() {
		using(var blocks = gr.Blocks(css: "footer { display: none !important; }")) {
			var csprojDirectory = Util.FindCsproj(MtfxUtil.AssemblyDirectory)?.ToDirectoryInfo().Parent;
			var rawDatasetDirectory = csprojDirectory?.GetSubDirectory("dataset");
			
			using(gr.Tab("Generate dataset")) {
				var imagesDirPathTxt = gr.Textbox(label: "Path to image directory", value: rawDatasetDirectory!.FullName);
				var processBtn = gr.Button("Process");
				var doneLbl = gr.Textbox("Done", interactive: false, visible: false);
				await processBtn.Click(CreateDataset, [imagesDirPathTxt], [doneLbl]);
			}

			using(gr.Tab("Add Triggerword")) {
				var triggerwordTxt = gr.Textbox(label: "Triggerword", value: "caro");
				var imagesDirPathTxt = gr.Textbox(label: "Path to image directory", value: rawDatasetDirectory.FullName);
				var processBtn = gr.Button("Process");
				var doneLbl = gr.Textbox("Done", interactive: false, visible: false);
				await processBtn.Click(fn:AddTriggerword, inputs:[triggerwordTxt, imagesDirPathTxt], outputs:[doneLbl]);
			}

			using(gr.Tab("About")) {
				var readmeContentTxt = await GetReadmeContentTxt();
				var reloadBtn = gr.Button("Reload");
				await reloadBtn.Click(Reload, [], [readmeContentTxt]);
			}

			return blocks;
		}
	}

	#region gradio examples

	private static Task<Output> Greet(Input input) {
		var txt = Textbox.Payload(input.Data[0]);
		return Task.FromResult(gr.Output(gr.Textbox($"Hello, {txt}!", visible: true)));
	}

	#endregion gradio examples
}
