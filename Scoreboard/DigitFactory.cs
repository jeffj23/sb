using System;
using Scoreboard.Elements.BaseElements;
using Scoreboard.Elements;

namespace Scoreboard.Factories
{
    public static class DigitFactory
    {
        /// <summary>
        /// Creates and returns an instance of an IDigit based on the provided digitType.
        /// </summary>
        /// <param name="digitType">The type of digit to create. 1 for Digit, 2 for SegmentDigit.</param>
        /// <param name="pixelManager">The SbPixelManager instance required for SegmentDigit.</param>
        /// <returns>An instance of IDigit.</returns>
        /// <exception cref="ArgumentException">Thrown if digitType is invalid.</exception>
        public static IDigit CreateDigit(int digitType, SbPixelManager pixelManager = null)
        {
            switch (digitType)
            {
                case 1:
                    return new Digit(); // Create and return a Digit instance

                case 2:
                    if (pixelManager == null)
                    {
                        throw new ArgumentException("pixelManager is required for creating a SegmentDigit.");
                    }
                    return new SegmentDigit(pixelManager); // Create and return a SegmentDigit instance

                default:
                    throw new ArgumentException("Invalid digitType. Use 1 for Digit or 2 for SegmentDigit.");
            }
        }
    }
}