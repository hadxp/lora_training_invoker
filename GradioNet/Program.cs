using Gradio.Net;
using GradioNet.UI;

namespace GradioNet;

internal class Program {
	private static async Task Main(string[] args) {
		App.Launch(await Ui.CreateBlocks());
	}
}
