#### Add ResourceAttribute Above KMain
```cs
[Resource("YourFileName")] //You should copy your file to your projects bin folder
[Plug("Mosa.Runtime.StartUp::KMain")]
[UnmanagedCallersOnly(EntryPoint = "KMain", CallingConvention = CallingConvention.StdCall)]
public static void KMain()
...
```
#### Load It
```cs
byte[] Data = ResourceManager.GetObject("YourFileName");
```