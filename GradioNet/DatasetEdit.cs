using System.Text.Json;
using System.Text.Json.Nodes;
using mtfx;

namespace LoraTrainingInvoker;

internal record DatasetEdit(FileInfo Workflow, DirectoryInfo RawDatasetDirectory, DirectoryInfo InputDirectory) : IDisposable {
	private JsonNode? _json;

	public void Dispose() {
		Save();
	}

	public void Edit() {
		var jsonStr = File.ReadAllText(Workflow.FullName);
		var json = JsonNode.Parse(jsonStr);
		if(json is not null) {
			var nodes = json["nodes"]?.AsArray();
			foreach(var node in nodes ?? []) {
				var type = node?["type"]?.ToString();
				// set the dataset path
				if(type == "LoadImageListFromDir //Inspire") {
					var widgetsValues = node?["widgets_values"]?.AsArray();
					if(widgetsValues is { Count: > 0 }) {
						widgetsValues[0] = RawDatasetDirectory.FullName;
					}
				} else if(type == "Image Save" || type == "Save Text File") {
					var widgetsValues = node?["widgets_values"]?.AsArray();
					if(widgetsValues is { Count: > 0 }) {
						widgetsValues[0] = InputDirectory.FullName;
					}
				}
			}

			_json = json;
		}
	}

	internal void Save() {
		if(_json is null) { return; }

		// Convert to JSON string
		var updatedJson = _json.ToJsonString(new JsonSerializerOptions {
			WriteIndented = true // Optional: makes the output pretty
		});

		// Save to file
		Workflow.WriteAllText(updatedJson);
	}
	
	internal async Task SaveAsync() {
		if(_json is null) { return; }

		// Convert to JSON string
		var updatedJson = _json.ToJsonString(new JsonSerializerOptions {
			WriteIndented = true // Optional: makes the output pretty
		});

		// Save to file
		await File.WriteAllTextAsync(Workflow.FullName, updatedJson);
	}
}
