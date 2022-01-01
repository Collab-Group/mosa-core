## Use Mosa.Runtime.GC.AllocateObject(uint size) To Allocate Object.
## Sample
```CS
byte* buffer = (byte*)GC.AllocateObject(512);
```
## Free
### Use GC.Dispose to free a pointer
## Sample
```cs
GC.Dispose((uint)buffer, 512);
```

# Free a managed object
```cs
GC.Dispose(object obj)
```
OR
```cs
object.Dispose()
```