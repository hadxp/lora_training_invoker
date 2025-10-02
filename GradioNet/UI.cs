using System.Diagnostics;
using Gradio.Net;
using GradioNet.util;
using mtfx;

// ReSharper disable ReturnOfUsingVariable
// ReSharper disable UnusedParameter.Local

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

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

	private static async Task<Output> CreateDataset(Input input) {
		var triggerword = Textbox.Payload(input.Data[0]);
		var jsonlFile = await GenerateCaptions(triggerword);
		return gr.Output(gr.Textbox(visible: true));
	}
	
	private static async Task<Output> Train(Input input) {
		var datasetPath = Textbox.Payload(input.Data[0]);
		var checkpointPath = Textbox.Payload(input.Data[1]);
		var textEncoder1Path = Textbox.Payload(input.Data[2]);
		var textEncoder2Path = Textbox.Payload(input.Data[3]);
		var imageEncoderPath = Textbox.Payload(input.Data[4]);
		var vaePath = Textbox.Payload(input.Data[5]);

		await Train(datasetPath, checkpointPath, textEncoder1Path, textEncoder2Path, imageEncoderPath, vaePath);
		
		return gr.Output(gr.Textbox(visible: true));
	}

	private static async Task<Output> GenerateAndTrain(Input input) {
		var triggerword = Textbox.Payload(input.Data[0]);
		var checkpointPath = Textbox.Payload(input.Data[1]);
		var textEncoder1Path = Textbox.Payload(input.Data[2]);
		var textEncoder2Path = Textbox.Payload(input.Data[3]);
		var imageEncoderPath = Textbox.Payload(input.Data[4]);
		var vaePath = Textbox.Payload(input.Data[5]);

		var datasetFile = await GenerateCaptions(triggerword);
		
		await Train(datasetFile?.FullName ?? string.Empty, checkpointPath, textEncoder1Path, textEncoder2Path, imageEncoderPath, vaePath);
		
		return gr.Output(gr.Textbox(visible: true));
	}
	
	internal static async Task<Blocks> CreateBlocks() {
		using(var blocks = gr.Blocks(css: "footer { display: none !important; }")) {
			var csprojDirectory = Util.FindCsproj(MtfxUtil.AssemblyDirectory)?.ToDirectoryInfo().Parent;
			var rawDatasetDirectory = csprojDirectory?.GetSubDirectory("dataset");
			
			using(gr.Tab("Generate dataset")) {
				var triggerwordTxt = gr.Textbox(label: "Triggerword", value: "julia_forstner");
				var processBtn = gr.Button("Process");
				var doneLbl = gr.Textbox("Done", interactive: false, visible: false);
				await processBtn.Click(CreateDataset, [triggerwordTxt], [doneLbl]);
			}

			using(gr.Tab("Train")) {
				var datasetPathTxt = gr.Textbox(label: "Dataset Path", value: "./output/0_dataset.jsonl");
				var checkpointPathTxt = gr.Textbox(label: "Checkpoint(DIT)", value: "/home/philipp-haderer/Dokumente/FramePack-Studio/hf_download/hub/models--lllyasviel--FramePack_F1_I2V_HY_20250503/snapshots/ab239828e0b384fed75580f186f078717d4020f7");
				var textEncoder1Txt = gr.Textbox(label: "Text Encoder 1", value: "/home/philipp-haderer/Dokumente/FramePack/hf_download/hub/models--hunyuanvideo-community--HunyuanVideo/snapshots/e8c2aaa66fe3742a32c11a6766aecbf07c56e773");
				var textEncoder2Txt = gr.Textbox(label: "Text Encoder 2", value: "/home/philipp-haderer/Dokumente/FramePack/hf_download/hub/models--hunyuanvideo-community--HunyuanVideo/snapshots/e8c2aaa66fe3742a32c11a6766aecbf07c56e773");
				var imageEncoderTxt = gr.Textbox(label: "Image Encoder", value: "/home/philipp-haderer/Dokumente/FramePack-Studio/hf_download/hub/models--lllyasviel--flux_redux_bfl/snapshots/45b801affc54ff2af4e5daf1b282e0921901db87");
				var vaePathTxt = gr.Textbox(label: "VAE Path", value: "/home/philipp-haderer/Dokumente/FramePack-Studio/hf_download/hub/models--hunyuanvideo-community--HunyuanVideo/snapshots/e8c2aaa66fe3742a32c11a6766aecbf07c56e773");
				var processBtn = gr.Button("Process");
				var doneLbl = gr.Textbox("Done", interactive: false, visible: false);
				await processBtn.Click(Train, [datasetPathTxt, checkpointPathTxt, textEncoder1Txt, textEncoder2Txt, imageEncoderTxt, vaePathTxt], [doneLbl]);
			}
			
			using(gr.Tab("Generate and train dataset")) {
				var triggerwordTxt = gr.Textbox(label: "Triggerword", value: "julia_forstner");
				var checkpointPathTxt = gr.Textbox(label: "Checkpoint(DIT)", value: "/home/philipp-haderer/Dokumente/FramePack-Studio/hf_download/hub/models--lllyasviel--FramePack_F1_I2V_HY_20250503/snapshots/ab239828e0b384fed75580f186f078717d4020f7");
				var textEncoder1Txt = gr.Textbox(label: "Text Encoder 1", value: "/home/philipp-haderer/Dokumente/FramePack/hf_download/hub/models--hunyuanvideo-community--HunyuanVideo/snapshots/e8c2aaa66fe3742a32c11a6766aecbf07c56e773");
				var textEncoder2Txt = gr.Textbox(label: "Text Encoder 2", value: "/home/philipp-haderer/Dokumente/FramePack/hf_download/hub/models--hunyuanvideo-community--HunyuanVideo/snapshots/e8c2aaa66fe3742a32c11a6766aecbf07c56e773");
				var imageEncoderTxt = gr.Textbox(label: "Image Encoder", value: "/home/philipp-haderer/Dokumente/FramePack-Studio/hf_download/hub/models--lllyasviel--flux_redux_bfl/snapshots/45b801affc54ff2af4e5daf1b282e0921901db87");
				var vaePathTxt = gr.Textbox(label: "VAE Path", value: "/home/philipp-haderer/Dokumente/FramePack-Studio/hf_download/hub/models--hunyuanvideo-community--HunyuanVideo/snapshots/e8c2aaa66fe3742a32c11a6766aecbf07c56e773");
				var processBtn = gr.Button("Process");
				var doneLbl = gr.Textbox("Done", interactive: false, visible: false);
				await processBtn.Click(GenerateAndTrain, [triggerwordTxt, checkpointPathTxt, textEncoder1Txt, textEncoder2Txt, imageEncoderTxt, vaePathTxt], [doneLbl]);
			}

			using(gr.Tab("About")) {
				var readmeContentTxt = await GetReadmeContentTxt();
				var reloadBtn = gr.Button("Reload");
				await reloadBtn.Click(Reload, [], [readmeContentTxt]);
			}

			return blocks;
		}
	}

	private static async Task<FileInfo?> GenerateCaptions(string triggerword) {
		var projectRootDir = Util.GetRootDir();

		var datasetGeneratorDir = projectRootDir?.GetSubDirectory("DatasetGenerator");
		var runPythonFile = datasetGeneratorDir?.GetFile("main.py");

		var venvPython = Util.GetVenvPython(datasetGeneratorDir);

		var inputDir = projectRootDir?.GetSubDirectory("input");
		var outputDir = projectRootDir?.GetSubDirectory("output");

		var pythonOutputDirectory = inputDir;
		var pythonInputDirectory = outputDir;

		#if !DEBUG
		static void OutDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
			Debug.WriteLine(@"[out] " + e.Data);
		}

		static void ErrorDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
			Debug.WriteLine(@"[err] " + e.Data);
		}

		var runPythonCmd = $"-c {venvPython?.FullName ?? "python"} {runPythonFile?.FullName ?? "main.py"} {pythonInputDirectory?.FullName ?? "input"} {pythonOutputDirectory?.FullName ?? "output"} {triggerword}";
		var output = MtfxUtil.ExecuteProcess("cmd.exe", datasetGeneratorDir, out var err, [runPythonCmd], OutDataReceivedEventHandler, ErrorDataReceivedEventHandler);
		if(output.IsNullOrEmpty()) {
			return null;
		}
		#endif

		var jsonlFiles = outputDir?.GetFiles("*.jsonl", SearchOption.TopDirectoryOnly);
		if(!jsonlFiles?.Any() is true && jsonlFiles.Length == 1) {
			var jsonlFile = jsonlFiles[0];
			return jsonlFile;
		}

		return null;
	}
	
	private static async Task Train(string datasetPath, string checkpointPath, string textEncoder1Path, string textEncoder2Path, string imageEncoderPath, string vaePath) {
		var projectRootDir = Util.GetRootDir();
		var musibitunerDir = projectRootDir?.GetSubDirectory("musubi-tuner");
		var venvPython = Util.GetVenvPython(musibitunerDir);

		var scriptsDir = projectRootDir?.GetSubDirectory("scripts");

		var datasetImgConfig = scriptsDir?.GetFile("dataset_img.toml");
		var datasetImgConfigLines = datasetImgConfig?.ReadAllLines();

		var musubiTunerConfig = scriptsDir?.GetFile("config.toml");
		var musubiTunerConfigLines = musubiTunerConfig?.ReadAllLines();

		var cacheLatentCommandLines = scriptsDir?.GetFile("fpack_cache_latents.txt").ReadAllLines();
		var cacheTextEncoderOutputsCommandLines = scriptsDir?.GetFile("fpack_cache_text_encoder_outputs.txt").ReadAllLines();
		var trainNetworkCommandLines = scriptsDir?.GetFile("fpack_train_network.txt").ReadAllLines();

		#region dataset and musubituner config handling

		var datasetImgConfigNewLines = Util.ReplaceInList(datasetImgConfigLines, line => {
			if(line.Contains("image_jsonl_file")) {
				return @$"image_jsonl_file = ""{datasetPath}""";
			}

			return line;
		});

		if(datasetImgConfigNewLines != null && datasetImgConfig != null) {
			datasetImgConfig.WriteAllLines(datasetImgConfigNewLines);
		}

		var musubiTunerConfigNewLines = Util.ReplaceInList(musubiTunerConfigLines, line => {
			if(line.Contains("dit")) {
				return @$"dit = ""{checkpointPath}""";
			}

			if(line.Contains("text_encoder1")) {
				return @$"text_encoder1 = ""{textEncoder1Path}""";
			}

			if(line.Contains("text_encoder2")) {
				return @$"text_encoder2 = ""{textEncoder2Path}""";
			}

			if(line.Contains("image_encoder")) {
				return @$"image_encoder = ""{imageEncoderPath}""";
			}

			if(line.Contains("vae")) {
				return @$"vae = ""{vaePath}""";
			}

			return line;
		});

		if(musubiTunerConfigNewLines != null && musubiTunerConfig != null) {
			musubiTunerConfig.WriteAllLines(musubiTunerConfigNewLines);
		}

		#endregion dataset and musubituner config handling

		#region commandline file editing

		var cacheLatentCommand = Util.ReplaceInList(cacheLatentCommandLines, line => {
			if(line.Contains("dataset_config")) {
				return $"--dataset_config {datasetImgConfig?.FullName}";
			}

			if(line.Contains("vae")) {
				return $"--vae {vaePath}";
			}

			if(line.Contains("image_encoder")) {
				return $"--image_encoder {imageEncoderPath}";
			}

			return line;
		});

		var cacheTextEncoderOutputsCommand = Util.ReplaceInList(cacheTextEncoderOutputsCommandLines, line => {
			if(line.Contains("dataset_config")) {
				return $"--dataset_config {datasetImgConfig?.FullName}";
			}

			if(line.Contains("text_encoder1")) {
				return $"--text_encoder1 {textEncoder1Path}";
			}

			if(line.Contains("text_encoder2")) {
				return $"--text_encoder2 {textEncoder2Path}";
			}

			return line;
		});

		var trainNetworkCommand = Util.ReplaceInList(trainNetworkCommandLines, line => {
			if(line.Contains("dataset_config")) {
				return $"--dataset_config {datasetImgConfig?.FullName}";
			}

			if(line.Contains("image_encoder")) {
				return $"--image_encoder {imageEncoderPath}";
			}

			if(line.Contains("config_file")) {
				return $"--config_file {musubiTunerConfig?.FullName}";
			}

			return line;
		});

		#endregion commandline file editing

		{
			#region cache latents

			var arg = $"-c {venvPython?.FullName ?? "python"} {cacheLatentCommand}";

			static void OutDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
				Debug.WriteLine(@"[out] " + e.Data);
			}

			static void ErrorDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
				Debug.WriteLine(@"[err] " + e.Data);
			}

			var output = MtfxUtil.ExecuteProcess("bash", musibitunerDir, out var err, [arg], OutDataReceivedEventHandler, ErrorDataReceivedEventHandler);

			#endregion cache latents
		}

		{
			#region cache text encoder outputs

			var arg = $"-c {venvPython?.FullName ?? "python"} {cacheTextEncoderOutputsCommand}";

			static void OutDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
				Debug.WriteLine(@"[out] " + e.Data);
			}

			static void ErrorDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
				Debug.WriteLine(@"[err] " + e.Data);
			}

			var output = MtfxUtil.ExecuteProcess("bash", musibitunerDir, out var err, [arg], OutDataReceivedEventHandler, ErrorDataReceivedEventHandler);

			#endregion cache text encoder outputs
		}

		{
			#region train

			var arg = $"-c {venvPython?.FullName ?? "python"} {trainNetworkCommand}";

			static void OutDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
				Debug.WriteLine(@"[out] " + e.Data);
			}

			static void ErrorDataReceivedEventHandler(object sender, DataReceivedEventArgs e) {
				Debug.WriteLine(@"[err] " + e.Data);
			}

			var output = MtfxUtil.ExecuteProcess("bash", musibitunerDir, out var err, [arg], OutDataReceivedEventHandler, ErrorDataReceivedEventHandler);

			#endregion train
		}
	}
}
