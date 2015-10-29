﻿/*
 * Kons 2012-12-03 Version .1
 * 
 * Supported features:
 * - DXT1
 * - DXT5
 * - LinearImage
 * 
 * http://code.google.com/p/kprojects/
 * Send me any change/improvement at kons.snok<at>gmail.com
 *
 * License: MIT
 */

using System;
using System.Drawing;
using System.IO;

namespace ValveResourceFormat.ThirdParty
{
    internal static class DDSImage
    {
        #region DXT1
        public static Bitmap UncompressDXT1(BinaryReader r, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);
            int blockCountX = (w + 3) / 4;
            int blockCountY = (h + 3) / 4;

            for (int j = 0; j < blockCountY; j++)
            {
                for (int i = 0; i < blockCountX; i++)
                {
                    byte[] blockStorage = r.ReadBytes(8);
                    DecompressBlockDXT1(i * 4, j * 4, w, blockStorage, res);
                }
            }

            return res;
        }

        private static void DecompressBlockDXT1(int x, int y, int width, byte[] blockStorage, Bitmap image)
        {
            ushort color0 = (ushort)(blockStorage[0] | blockStorage[1] << 8);
            ushort color1 = (ushort)(blockStorage[2] | blockStorage[3] << 8);

            int temp;

            temp = (color0 >> 11) * 255 + 16;
            byte r0 = (byte)((temp / 32 + temp) / 32);
            temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
            byte g0 = (byte)((temp / 64 + temp) / 64);
            temp = (color0 & 0x001F) * 255 + 16;
            byte b0 = (byte)((temp / 32 + temp) / 32);

            temp = (color1 >> 11) * 255 + 16;
            byte r1 = (byte)((temp / 32 + temp) / 32);
            temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
            byte g1 = (byte)((temp / 64 + temp) / 64);
            temp = (color1 & 0x001F) * 255 + 16;
            byte b1 = (byte)((temp / 32 + temp) / 32);

            uint code = (uint)(blockStorage[4] | blockStorage[5] << 8 | blockStorage[6] << 16 | blockStorage[7] << 24);

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    Color finalColor = Color.FromArgb(0);
                    byte positionCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

                    if (color0 > color1)
                    {
                        switch (positionCode)
                        {
                            case 0:
                                finalColor = Color.FromArgb(255, r0, g0, b0);
                                break;
                            case 1:
                                finalColor = Color.FromArgb(255, r1, g1, b1);
                                break;
                            case 2:
                                finalColor = Color.FromArgb(255, (2 * r0 + r1) / 3, (2 * g0 + g1) / 3, (2 * b0 + b1) / 3);
                                break;
                            case 3:
                                finalColor = Color.FromArgb(255, (r0 + 2 * r1) / 3, (g0 + 2 * g1) / 3, (b0 + 2 * b1) / 3);
                                break;
                        }
                    }
                    else
                    {
                        switch (positionCode)
                        {
                            case 0:
                                finalColor = Color.FromArgb(255, r0, g0, b0);
                                break;
                            case 1:
                                finalColor = Color.FromArgb(255, r1, g1, b1);
                                break;
                            case 2:
                                finalColor = Color.FromArgb(255, (r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2);
                                break;
                            case 3:
                                finalColor = Color.FromArgb(255, 0, 0, 0);
                                break;
                        }
                    }

                    if (x + i < width)
                    {
                        image.SetPixel(x + i, y + j, finalColor);
                    }
                }
            }
        }
        #endregion

        #region DXT5
        public static Bitmap UncompressDXT5(BinaryReader r, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);

            int blockCountX = (w + 3) / 4;
            int blockCountY = (h + 3) / 4;

            for (int j = 0; j < blockCountY; j++)
            {
                for (int i = 0; i < blockCountX; i++)
                {
                    byte[] blockStorage = r.ReadBytes(16);
                    DecompressBlockDXT5(i * 4, j * 4, w, blockStorage, res);
                }
            }

            return res;
        }

        private static void DecompressBlockDXT5(int x, int y, int width, byte[] blockStorage, Bitmap image)
        {
            byte alpha0 = blockStorage[0];
            byte alpha1 = blockStorage[1];

            int bitOffset = 2;
            uint alphaCode1 = (uint)(blockStorage[bitOffset + 2] | (blockStorage[bitOffset + 3] << 8) | (blockStorage[bitOffset + 4] << 16) | (blockStorage[bitOffset + 5] << 24));
            ushort alphaCode2 = (ushort)(blockStorage[bitOffset + 0] | (blockStorage[bitOffset + 1] << 8));

            ushort color0 = (ushort)(blockStorage[8] | blockStorage[9] << 8);
            ushort color1 = (ushort)(blockStorage[10] | blockStorage[11] << 8);

            int temp;

            temp = (color0 >> 11) * 255 + 16;
            byte r0 = (byte)((temp / 32 + temp) / 32);
            temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
            byte g0 = (byte)((temp / 64 + temp) / 64);
            temp = (color0 & 0x001F) * 255 + 16;
            byte b0 = (byte)((temp / 32 + temp) / 32);

            temp = (color1 >> 11) * 255 + 16;
            byte r1 = (byte)((temp / 32 + temp) / 32);
            temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
            byte g1 = (byte)((temp / 64 + temp) / 64);
            temp = (color1 & 0x001F) * 255 + 16;
            byte b1 = (byte)((temp / 32 + temp) / 32);

            uint code = (uint)(blockStorage[12] | blockStorage[13] << 8 | blockStorage[14] << 16 | blockStorage[15] << 24);

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int alphaCodeIndex = 3 * (4 * j + i);
                    int alphaCode;

                    if (alphaCodeIndex <= 12)
                    {
                        alphaCode = (alphaCode2 >> alphaCodeIndex) & 0x07;
                    }
                    else if (alphaCodeIndex == 15)
                    {
                        alphaCode = (int)(((uint)alphaCode2 >> 15) | ((alphaCode1 << 1) & 0x06));
                    }
                    else
                    {
                        alphaCode = (int)((alphaCode1 >> (alphaCodeIndex - 16)) & 0x07);
                    }

                    byte finalAlpha;
                    if (alphaCode == 0)
                    {
                        finalAlpha = alpha0;
                    }
                    else if (alphaCode == 1)
                    {
                        finalAlpha = alpha1;
                    }
                    else
                    {
                        if (alpha0 > alpha1)
                        {
                            finalAlpha = (byte)(((8 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 7);
                        }
                        else
                        {
                            if (alphaCode == 6)
                            {
                                finalAlpha = 0;
                            }
                            else if (alphaCode == 7)
                            {
                                finalAlpha = 255;
                            }
                            else
                            {
                                finalAlpha = (byte)(((6 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 5);
                            }
                        }
                    }

                    byte colorCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

                    Color finalColor = new Color();
                    switch (colorCode)
                    {
                        case 0:
                            finalColor = Color.FromArgb(finalAlpha, r0, g0, b0);
                            break;
                        case 1:
                            finalColor = Color.FromArgb(finalAlpha, r1, g1, b1);
                            break;
                        case 2:
                            finalColor = Color.FromArgb(finalAlpha, (2 * r0 + r1) / 3, (2 * g0 + g1) / 3, (2 * b0 + b1) / 3);
                            break;
                        case 3:
                            finalColor = Color.FromArgb(finalAlpha, (r0 + 2 * r1) / 3, (g0 + 2 * g1) / 3, (b0 + 2 * b1) / 3);
                            break;
                    }

                    if (x + i < width)
                    {
                        image.SetPixel(x + i, y + j, finalColor);
                    }
                }
            }
        }
        #endregion

        public static Bitmap ReadLinearImage(BinaryReader r, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    res.SetPixel(x, y, Color.FromArgb(r.ReadInt32()));
                }
            }

            return res;
        }
    }
}