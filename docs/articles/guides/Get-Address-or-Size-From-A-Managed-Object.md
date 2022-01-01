# Get Address
```cs
object obj = new object();
Pointer p = Intrinsic.GetObjectAddress(obj);
```

# Get Size
```cs
// An object has the following memory layout:
//   - Pointer TypeDef
//   - Pointer SyncBlock
//   - 0 .. n object data fields

class c 
{
    uint a;
    uint b;
    uint aa;
    uint bb;
}

uint size= (uint)((*((uint*)(typeof(c).TypeHandle.Value + (Pointer.Size * 3)))) + 2 * sizeof(Pointer));
Console.WriteLine($"SizeOf c:{size}");
```