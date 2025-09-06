import gradio as gr

with gr.Blocks() as indexDemo:
    with gr.Tab("Greet"):
        name_input = gr.Textbox(label="Your Name")
        greet_btn = gr.Button("Say Hello")
        greet_output = gr.Textbox()
        greet_btn.click(lambda name: f"Hello, {name}!", inputs=name_input, outputs=greet_output)

    with gr.Tab("Farewell"):
        name_input2 = gr.Textbox(label="Your Name")
        bye_btn = gr.Button("Say Goodbye")
        bye_output = gr.Textbox()
        bye_btn.click(lambda name: f"Goodbye, {name}!", inputs=name_input2, outputs=bye_output)

indexDemo.launch()