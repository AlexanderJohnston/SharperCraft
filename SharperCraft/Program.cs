using SharperCraft.Runtime;

var runtimeUri = GenericRuntime.GetRuntimeUri();
var assembly = GenericRuntime.GetRuntimeAssembly();
Console.WriteLine("Runtime: {0},{1}Assembly: {2}", runtimeUri, Environment.NewLine, assembly);
var runtime = new GenericRuntime();
runtime.Entry();
//Console.ReadLine();