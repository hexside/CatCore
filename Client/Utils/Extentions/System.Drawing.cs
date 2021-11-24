using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace System.Drawing;

public enum VTSType
{
	Foreground,
	Background,
	Both
}
public static partial class SystemDrawingExtentions
{
	public static string ToVTS(this Color color, VTSType type) => type switch
	{
		VTSType.Foreground => $"\x1b[38;2;{color.R};{color.G};{color.B}m",
		VTSType.Background => $"\x1b[48;2;{color.R};{color.G};{color.B}m",
		VTSType.Both => $"\x1b[38;2;{color.R};{color.G};{color.B}m\x1b[48;2;{color.R};{color.G};{color.B}m",
		_ => throw new NotImplementedException()
	};
}
