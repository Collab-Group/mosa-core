# GUI

Creating a Graphical User Interface (**GUI**) is relatively easy in MOSA-Core.

First, you'll need a ``Graphics`` driver. To automatically select one, use this:

``Graphics graphics = GraphicsSelector.GetGraphics(optionalWidth, optionalHeight);``

The default width and height are 640 and 480 respectively.

You can then call functions such as ``Clear()`` or ``DrawPoint()`` to clear and draw a point on the screen respectively. Here's an example:

```
graphics.Clear((uint)Color.Black.ToArgb());
graphics.DrawPoint((uint)Color.White.ToArgb(), 10, 20);
graphics.Update();
```

Note the ``Update()`` function. All graphics drivers in MOSA-Core are **double buffered**, which means that they write into a memory block separate from the video memory of the graphics device, then when ``Update()`` is call, the buffers are flipped (which means, everything in that memory block is written into the video memory of the graphics device).

# Mouse

To draw a mouse, you'll first need to initialize the mouse. Currently, only a PS/2 mouse driver is present, so we'll use that here. Here's how to initialize it:

``PS2Mouse.Initialize(screenWidth, screenHeight);``

Great! After that, you **will** need to add the interrupt of the mouse to your ``ProcessInterrupt`` function, or whatever you've called it (it's present by default if you choosed a MOSA-Core template). Here's an example:

```
case 0x2C:
    // PS/2 mouse interrupt is 0x2C IRQ 12
    PS2Mouse.OnInterrupt();
    break;
```

And finally, you can draw the X and Y of the mouse as a point on the screen:

``graphics.DrawPoint((uint)Color.Red.ToArgb(), PS2Mouse.X, PS2Mouse.Y);``