using ImageMagick;
using Polo.Abstractions.Enums;
using Polo.Abstractions.Exceptions;

namespace Polo.Extensions
{
    public static class StringExtension
    {
        public static Gravity ParsePosition(this string position)
        {// TODO LA - Move this extension to MagickImage extensions
            var split = position.Split(CommandParser.ShortCommandPrefix);
            if (split.Length != 2)
            {
                throw new ParameterParseException($"ERROR: Invalid parameter value '{position}'. Should be only single delimiter '{CommandParser.ShortCommandPrefix}'.");
            }

            var screenPosition = ScreenPosition.None;
            screenPosition |= ConvertStringToScreenPosition(split[0], screenPosition);
            screenPosition |= ConvertStringToScreenPosition(split[1], screenPosition);

            switch (screenPosition)
            {
                case ScreenPosition.None:
                    throw new ParameterParseException($"ERROR: Invalid parameter value '{position}'. Unsupported position name.");
                case ScreenPosition.Top | ScreenPosition.Left:
                    return Gravity.Northwest;
                case ScreenPosition.Top | ScreenPosition.Right:
                    return Gravity.Northeast;
                case ScreenPosition.Top | ScreenPosition.Center:
                    return Gravity.North;
                case ScreenPosition.Center | ScreenPosition.Left:
                    return Gravity.West;
                case ScreenPosition.Center | ScreenPosition.Right:
                    return Gravity.East;
                case ScreenPosition.Center | ScreenPosition.Center:
                    return Gravity.Center;
                case ScreenPosition.Bottom | ScreenPosition.Left:
                    return Gravity.Southwest;
                case ScreenPosition.Bottom | ScreenPosition.Right:
                    return Gravity.Southeast;
                case ScreenPosition.Bottom | ScreenPosition.Center:
                    return Gravity.South;
                default:
                    throw new ParameterParseException($"ERROR: Invalid parameter value '{position}'. Incorrect position.");
            }
        }

        private static ScreenPosition ConvertStringToScreenPosition(string position, ScreenPosition screenPosition)
        {
            const string left = "left";
            const string right = "right";
            const string center = "center";
            const string top = "top";
            const string bottom = "bottom";

            if (position == top)
            {
                screenPosition |= ScreenPosition.Top;
            }
            else if (position == center)
            {
                screenPosition |= ScreenPosition.Center;
            }
            else if (position == bottom)
            {
                screenPosition |= ScreenPosition.Bottom;
            }
            else if (position == left)
            {
                screenPosition |= ScreenPosition.Left;
            }
            else if (position == right)
            {
                screenPosition |= ScreenPosition.Right;
            }

            return screenPosition;
        }
    }
}
