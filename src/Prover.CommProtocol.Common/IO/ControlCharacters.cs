﻿using System;
using System.Collections.Generic;

namespace Prover.CommProtocol.Common.IO
{
    public static class ControlCharacters
    {
        public const char SOH = (char)1;
        public const char STX = (char)2;
        public const char ETX = (char)3;
        public const char EOT = (char)4;
        public const char ENQ = (char)5;
        public const char ACK = (char)6;
        public const char CR = (char)13;
        public const char NAK = (char)21;
        public const char RS = (char)30;
        public const char Comma = (char) 44;

        public static List<char> All => new List<char>
        {
            SOH, STX, ETX, EOT, ENQ, ACK, CR, NAK, RS
        };

        public static string Prettify(string msg)
        {
            msg = msg.Replace(SOH.FormatChar(), "[SOH]");
            msg = msg.Replace(STX.FormatChar(), "[STX]");
            msg = msg.Replace(ETX.FormatChar(), "[ETX]");
            msg = msg.Replace(EOT.FormatChar(), "[EOT]");
            msg = msg.Replace(ENQ.FormatChar(), "[ENQ]");
            msg = msg.Replace(ACK.FormatChar(), "[ACK]");
            msg = msg.Replace(CR.FormatChar(),  "[CR]");
            msg = msg.Replace(NAK.FormatChar(), "[NAK]");
            msg = msg.Replace(RS.FormatChar(),  "[RS]");
            return msg;
        }

        private static string FormatChar(this char c)
        {
            return new string (new[] {c});
        }
    }

    public static class ControlCharacterExtensions
    {
        public static bool IsEndOfTransmission(this char c)
        {
            return c == (char)ControlCharacters.EOT;
        }

        /// <summary>
        /// Checks is the char value is equal to SOH
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsStartOfHandshake(this char c)
        {
            return c == (char)ControlCharacters.SOH;
        }

        public static bool IsStartOfText(this char c)
        {
            return c == ControlCharacters.STX;
        }

        public static bool IsEndOfText(this char c)
        {
            return c == (char)ControlCharacters.ETX;
        }

        public static bool IsAcknowledgement(this char c)
        {
            return c == (char)ControlCharacters.ACK;
        }
    }
}
