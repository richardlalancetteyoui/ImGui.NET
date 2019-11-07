using System;
using System.IO;
using System.Numerics;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiNET
{
    class Program
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;
        private static MemoryEditor _memoryEditor;
        private static string _input = ""; 
        private static string _output = "";
        private static string _treeOutput = "";
        private static string _compilationOutput = "";

        // UI state
        private static readonly Vector3 ClearColor = new Vector3(0.45f, 0.55f, 0.6f);

        static void SetThing(out float i, float val)
        {
            i = val;
        }

        private static unsafe void SubmitUi()
        {
            ImGui.Begin("You.i Compact Language");
            
            if (ImGui.Button("Load"))
            {
                AntlrInputStream stream = new AntlrInputStream(File.OpenRead("main.youi"));
                _input = stream.ToString();
            }
            ImGui.SameLine(); 
            if (ImGui.Button("Run"))
            {
                var sw = new StringWriter();
                Console.SetOut(sw);
                Console.SetError(sw);
                
                AntlrInputStream inputStream = new AntlrInputStream(_input);
                ITokenSource lexer = new youiLexer(inputStream);
                ITokenStream tokens = new CommonTokenStream(lexer);
                youiParser parser = new youiParser(tokens);
                parser.BuildParseTree = true;

                IParseTree tree = parser.statements();
                ParseTreeWalker w = new ParseTreeWalker();
                KeyPrinter printer = new KeyPrinter();
                w.Walk(printer, tree);
//                IParseTreeListener listener = new youiBaseListener();
//                w.Walk(listener, tree);
                _output = tree.ToString();
                _treeOutput = tree.ToStringTree();
                _compilationOutput = sw.ToString();

            }

//            ImGui.SetNextWindowSize(new Vector2(320, 240));
//            ImGui.Begin("");
            ImGui.Separator();
            ImGui.NewLine();
            ImGui.Text("You.i layout code");
            ImGui.SameLine();
            ShowHelpMarker("Contents evaluated and appended to the window.");
            ImGui.PushItemWidth(-1);
            ImGui.InputTextMultiline("##source", ref _input, 800, Vector2.Zero);
            ImGui.PopItemWidth();

            ImGui.Separator();
            ImGui.NewLine();
            ImGui.Text("Compilation Output");
            ImGui.PushItemWidth(-1);
            ImGui.InputTextMultiline("##_compilationOutput", ref _compilationOutput, 800, Vector2.Zero);
            ImGui.PopItemWidth();

            ImGui.Separator();
            ImGui.NewLine();
            ImGui.Text("Tree Parsing Output");
            ImGui.PushItemWidth(-1);
            ImGui.InputTextMultiline("##_treeOutput", ref _treeOutput, 800, Vector2.Zero);
            ImGui.PopItemWidth();

            ImGui.Separator();
            ImGui.NewLine();
            ImGui.Text("Output");
            ImGui.PushItemWidth(-1);
            ImGui.InputTextMultiline("##_output", ref _output, 800, Vector2.Zero);
            ImGui.PopItemWidth();

            //            ImGui.End();
        }

        private static void ShowHelpMarker(string desc)
        {
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos((float) (ImGui.GetFontSize() * 35.0));
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        static void Main(string[] args)
        {
            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1920, 1080, WindowState.Normal, "ImGui.NET Sample Program"),
                new GraphicsDeviceOptions(true, null, true),
                out _window,
                out _gd);
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint) _window.Width, (uint) _window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };
            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width,
                _window.Height);
            _memoryEditor = new MemoryEditor();

            // Main application loop
            while (_window.Exists)
            {
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists)
                {
                    break;
                }

                _controller.Update(1f / 60f,
                    snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                SubmitUi();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(ClearColor.X, ClearColor.Y, ClearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
            }

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }
    }
}